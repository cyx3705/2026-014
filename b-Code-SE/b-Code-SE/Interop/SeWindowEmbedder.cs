using System.Runtime.InteropServices;
using System.Windows.Interop;
using SeApplication = SolidEdgeFramework.Application;
using WpfWindow = System.Windows.Window;
using WpfWindowStyle = System.Windows.WindowStyle;
using WpfResizeMode = System.Windows.ResizeMode;

namespace b_Code_SE.Interop;

/// <summary>
/// 通过 Win32 SetParent 将 WPF 窗口停靠到 Solid Edge 主窗口右侧。
/// 正式 EdgeBar 嵌入需 AddIn 注册；此方案适合外部自动化工具快速集成。
/// </summary>
internal sealed class SeWindowEmbedder : IDisposable
{
    private const int GwlStyle = -16;
    private const int WsChild = 0x40000000;
    private const int WsCaption = 0x00C00000;
    private const int WsThickFrame = 0x00040000;
    private const int WsPopup = unchecked((int)0x80000000);
    private const int SwpNoZOrder = 0x0004;
    private const int SwpShowWindow = 0x0040;

    private readonly WpfWindow _window;
    private readonly System.Windows.Threading.DispatcherTimer _layoutTimer;

    private IntPtr _originalParent;
    private WpfWindowStyle _originalWindowStyle;
    private WpfResizeMode _originalResizeMode;
    private bool _isEmbedded;

    public SeWindowEmbedder(WpfWindow window)
    {
        _window = window;
        _layoutTimer = new System.Windows.Threading.DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(400)
        };
        _layoutTimer.Tick += (_, _) => UpdateLayout();
    }

    public bool IsEmbedded => _isEmbedded;

    public void Embed(SeApplication application, int panelWidth = 380)
    {
        if (_isEmbedded)
        {
            return;
        }

        IntPtr childHwnd = new WindowInteropHelper(_window).EnsureHandle();
        IntPtr parentHwnd = new IntPtr(application.hWnd);

        _originalParent = GetParent(childHwnd);
        _originalWindowStyle = _window.WindowStyle;
        _originalResizeMode = _window.ResizeMode;

        int style = GetWindowLong(childHwnd, GwlStyle);
        style = (style & ~WsPopup & ~WsCaption & ~WsThickFrame) | WsChild;
        SetWindowLong(childHwnd, GwlStyle, style);

        _window.WindowStyle = WpfWindowStyle.None;
        _window.ResizeMode = WpfResizeMode.NoResize;
        _window.ShowInTaskbar = false;
        _window.Width = panelWidth;

        SetParent(childHwnd, parentHwnd);
        _isEmbedded = true;
        UpdateLayout();
        _layoutTimer.Start();
    }

    public void Detach()
    {
        if (!_isEmbedded)
        {
            return;
        }

        _layoutTimer.Stop();
        IntPtr childHwnd = new WindowInteropHelper(_window).Handle;
        SetParent(childHwnd, _originalParent == IntPtr.Zero ? GetDesktopWindow() : _originalParent);

        _window.WindowStyle = _originalWindowStyle;
        _window.ResizeMode = _originalResizeMode;
        _window.ShowInTaskbar = true;
        _window.Topmost = false;
        _isEmbedded = false;
    }

    public void Dispose()
    {
        Detach();
    }

    private void UpdateLayout()
    {
        if (!_isEmbedded)
        {
            return;
        }

        IntPtr parentHwnd = GetParent(new WindowInteropHelper(_window).Handle);
        if (parentHwnd == IntPtr.Zero || !GetClientRect(parentHwnd, out Rect client))
        {
            return;
        }

        int width = (int)Math.Max(280, _window.Width);
        int height = client.Bottom - client.Top;
        int x = client.Right - width;
        int y = client.Top;

        SetWindowPos(
            new WindowInteropHelper(_window).Handle,
            IntPtr.Zero,
            x, y, width, height,
            SwpNoZOrder | SwpShowWindow);
    }

    [DllImport("user32.dll")]
    private static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

    [DllImport("user32.dll")]
    private static extern IntPtr GetParent(IntPtr hWnd);

    [DllImport("user32.dll")]
    private static extern IntPtr GetDesktopWindow();

    [DllImport("user32.dll", EntryPoint = "GetWindowLong")]
    private static extern int GetWindowLong32(IntPtr hWnd, int nIndex);

    [DllImport("user32.dll", EntryPoint = "GetWindowLongPtr")]
    private static extern IntPtr GetWindowLongPtr64(IntPtr hWnd, int nIndex);

    private static int GetWindowLong(IntPtr hWnd, int nIndex)
        => IntPtr.Size == 8
            ? (int)GetWindowLongPtr64(hWnd, nIndex)
            : GetWindowLong32(hWnd, nIndex);

    [DllImport("user32.dll", EntryPoint = "SetWindowLong")]
    private static extern int SetWindowLong32(IntPtr hWnd, int nIndex, int dwNewLong);

    [DllImport("user32.dll", EntryPoint = "SetWindowLongPtr")]
    private static extern IntPtr SetWindowLongPtr64(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

    private static void SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong)
    {
        if (IntPtr.Size == 8)
        {
            SetWindowLongPtr64(hWnd, nIndex, new IntPtr(dwNewLong));
        }
        else
        {
            SetWindowLong32(hWnd, nIndex, dwNewLong);
        }
    }

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool GetClientRect(IntPtr hWnd, out Rect lpRect);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool SetWindowPos(
        IntPtr hWnd,
        IntPtr hWndInsertAfter,
        int x,
        int y,
        int cx,
        int cy,
        int uFlags);

    [StructLayout(LayoutKind.Sequential)]
    private struct Rect
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;
    }
}