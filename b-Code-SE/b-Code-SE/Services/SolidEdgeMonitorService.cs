using System.Runtime.InteropServices;
using b_Code_SE.Interop;
using SolidEdgeFramework;

namespace b_Code_SE.Services;

internal sealed record SeLogEntry(DateTime Time, string Category, string Message);

/// <summary>
/// 连接 Solid Edge 并订阅 ApplicationEvents，记录命令与特征参数。
/// </summary>
internal sealed class SolidEdgeMonitorService : IDisposable
{
    private readonly CommandNameResolver _commandNames = new();
    private readonly FeatureParameterInspector _featureInspector = new();

    private Application? _application;
    private ISEApplicationEvents_Event? _applicationEvents;
    private bool _isConnected;

    public event Action<SeLogEntry>? LogReceived;
    public event Action<string>? StatusChanged;

    public bool IsConnected => _isConnected;

    public Application? Application => _application;

    public string? Version => _application?.Version;

    public void ConnectToRunningInstance()
    {
        Disconnect();

        OleMessageFilter.Register();
        try
        {
            _application = (Application)ComHelper.GetActiveObject("SolidEdge.Application");
            SubscribeEvents();
            _commandNames.BuildCache(_application);
            _isConnected = true;
            StatusChanged?.Invoke($"已连接 Solid Edge {_application.Version}");
            Emit("系统", "成功连接到正在运行的 Solid Edge 实例");
        }
        catch (COMException ex) when (ex.HResult == unchecked((int)0x800401E3))
        {
            throw new InvalidOperationException("未检测到正在运行的 Solid Edge，请先启动 Solid Edge。", ex);
        }
    }

    public void StartAndConnect()
    {
        Disconnect();

        OleMessageFilter.Register();
        Type? type = Type.GetTypeFromProgID("SolidEdge.Application")
            ?? throw new InvalidOperationException("未找到 Solid Edge 安装（ProgID: SolidEdge.Application）。");

        _application = (Application)Activator.CreateInstance(type)!;
        _application.Visible = true;
        SubscribeEvents();
        _commandNames.BuildCache(_application);
        _isConnected = true;
        StatusChanged?.Invoke($"已启动并连接 Solid Edge {_application.Version}");
        Emit("系统", "已启动新的 Solid Edge 实例并完成连接");
    }

    public void Disconnect()
    {
        UnsubscribeEvents();

        if (_application != null)
        {
            Marshal.ReleaseComObject(_application);
            _application = null;
        }

        _isConnected = false;
        StatusChanged?.Invoke("未连接");
    }

    public void Dispose()
    {
        Disconnect();
        OleMessageFilter.Revoke();
    }

    private void SubscribeEvents()
    {
        if (_application == null)
        {
            return;
        }

        _applicationEvents = (ISEApplicationEvents_Event)_application.ApplicationEvents;
        _applicationEvents.BeforeCommandRun += OnBeforeCommandRun;
        _applicationEvents.AfterCommandRun += OnAfterCommandRun;
        _applicationEvents.AfterEnvironmentActivate += OnAfterEnvironmentActivate;
        _applicationEvents.AfterActiveDocumentChange += OnAfterActiveDocumentChange;
        _applicationEvents.AfterDocumentOpen += OnAfterDocumentOpen;
        _applicationEvents.AfterDocumentSave += OnAfterDocumentSave;
        _applicationEvents.BeforeDocumentClose += OnBeforeDocumentClose;
        _applicationEvents.BeforeQuit += OnBeforeQuit;
    }

    private void UnsubscribeEvents()
    {
        if (_applicationEvents == null)
        {
            return;
        }

        _applicationEvents.BeforeCommandRun -= OnBeforeCommandRun;
        _applicationEvents.AfterCommandRun -= OnAfterCommandRun;
        _applicationEvents.AfterEnvironmentActivate -= OnAfterEnvironmentActivate;
        _applicationEvents.AfterActiveDocumentChange -= OnAfterActiveDocumentChange;
        _applicationEvents.AfterDocumentOpen -= OnAfterDocumentOpen;
        _applicationEvents.AfterDocumentSave -= OnAfterDocumentSave;
        _applicationEvents.BeforeDocumentClose -= OnBeforeDocumentClose;
        _applicationEvents.BeforeQuit -= OnBeforeQuit;
        _applicationEvents = null;
    }

    private void OnBeforeCommandRun(int theCommandID)
    {
        if (_application != null)
        {
            _featureInspector.Snapshot(_application);
        }

        Emit("命令·执行前", FormatCommand(theCommandID));
    }

    private void OnAfterCommandRun(int theCommandID)
    {
        Emit("命令·执行后", FormatCommand(theCommandID));
        _ = InspectParametersAsync(theCommandID);
    }

    private async Task InspectParametersAsync(int commandId)
    {
        if (_application == null)
        {
            return;
        }

        string commandName = _commandNames.Resolve(commandId);
        int[] delays = [200, 400, 800];

        for (int attempt = 0; attempt < delays.Length; attempt++)
        {
            await Task.Delay(delays[attempt]);
            try
            {
                bool isLastAttempt = attempt == delays.Length - 1;
                IReadOnlyList<string> lines = _featureInspector.InspectChanges(_application, commitSnapshot: isLastAttempt);
                bool hasUsefulData = lines.Any(line =>
                    !line.StartsWith("未检测到特征变化", StringComparison.Ordinal));

                if (hasUsefulData || isLastAttempt)
                {
                    foreach (string line in lines)
                    {
                        Emit("参数", $"{commandName} → {line}");
                    }

                    return;
                }
            }
            catch (Exception ex)
            {
                if (attempt == delays.Length - 1)
                {
                    Emit("参数", $"解析失败: {ex.Message}");
                }
            }
        }
    }

    private void OnAfterEnvironmentActivate(object theEnvironment)
        => Emit("环境", GetComName(theEnvironment));

    private void OnAfterActiveDocumentChange(object theDocument)
        => Emit("文档·激活", GetDocumentLabel(theDocument));

    private void OnAfterDocumentOpen(object theDocument)
        => Emit("文档·打开", GetDocumentLabel(theDocument));

    private void OnAfterDocumentSave(object theDocument)
        => Emit("文档·保存", GetDocumentLabel(theDocument));

    private void OnBeforeDocumentClose(object theDocument)
        => Emit("文档·关闭前", GetDocumentLabel(theDocument));

    private void OnBeforeQuit()
    {
        Emit("系统", "Solid Edge 即将退出");
        _isConnected = false;
        StatusChanged?.Invoke("Solid Edge 正在退出");
    }

    private string FormatCommand(int commandId)
        => $"ID={commandId} (0x{commandId:X})  {_commandNames.Resolve(commandId)}";

    private static string GetComName(object? comObject)
    {
        if (comObject == null)
        {
            return "(null)";
        }

        try
        {
            dynamic item = comObject;
            string? name = item.Name;
            return string.IsNullOrWhiteSpace(name) ? comObject.GetType().Name : name;
        }
        catch
        {
            return comObject.GetType().Name;
        }
    }

    private static string GetDocumentLabel(object? document)
    {
        if (document == null)
        {
            return "(无文档)";
        }

        try
        {
            dynamic doc = document;
            string name = doc.Name ?? "(未命名)";
            string path = doc.FullName;
            return string.IsNullOrWhiteSpace(path) ? name : $"{name}  [{path}]";
        }
        catch
        {
            return GetComName(document);
        }
    }

    private void Emit(string category, string message)
        => LogReceived?.Invoke(new SeLogEntry(DateTime.Now, category, message));
}