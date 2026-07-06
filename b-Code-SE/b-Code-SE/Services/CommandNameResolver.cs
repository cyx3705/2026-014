using System.Collections.Concurrent;
using SolidEdgeFramework;

namespace b_Code_SE.Services;

/// <summary>
/// 将 ApplicationEvents 中的 CommandID 解析为可读名称。
/// 优先查 SolidEdgeCommandConstants 枚举，再查各 Environment 的 CommandCategories。
/// </summary>
internal sealed class CommandNameResolver
{
    private readonly ConcurrentDictionary<int, string> _cache = new();

    public void BuildCache(Application application)
    {
        _cache.Clear();

        foreach (SolidEdgeFramework.SolidEdgeCommandConstants value in
                 Enum.GetValues(typeof(SolidEdgeFramework.SolidEdgeCommandConstants)))
        {
            _cache.TryAdd((int)value, value.ToString());
        }

        foreach (SolidEdgeConstants.SolidEdgeCommandConstants value in
                 Enum.GetValues(typeof(SolidEdgeConstants.SolidEdgeCommandConstants)))
        {
            _cache.TryAdd((int)value, value.ToString());
        }

        try
        {
            Environments environments = application.Environments;
            for (int i = 1; i <= environments.Count; i++)
            {
                SolidEdgeFramework.Environment environment = environments.Item(i);
                IndexEnvironmentCommands(environment);
            }
        }
        catch
        {
            // 部分环境在特定状态下可能不可访问，忽略即可。
        }
    }

    public string Resolve(int commandId)
    {
        if (_cache.TryGetValue(commandId, out string? name))
        {
            return name;
        }

        return $"UnknownCommand(0x{commandId:X})";
    }

    private void IndexEnvironmentCommands(SolidEdgeFramework.Environment environment)
    {
        CommandCategories categories = environment.CommandCategories;
        for (int c = 1; c <= categories.Count; c++)
        {
            CommandCategory category = categories.Item(c);
            for (int i = 1; i <= category.Count; i++)
            {
                CommandInfo info = category.Item(i);
                _cache.TryAdd(info.Id, $"{category.Caption} / {info.Caption}");
            }
        }
    }
}