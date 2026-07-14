using System.ComponentModel;
using System.Runtime.InteropServices;
using ModelContextProtocol.Server;

namespace codex_use.McpTools;

[McpServerToolType]
public static class ShowMessageTools
{
    [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    private static extern int MessageBoxW(nint hWnd, string text, string caption, uint type);

    private const uint MB_OK = 0x00000000;
    private const uint MB_ICONINFORMATION = 0x00000040;

    [McpServerTool(Name = "show_message", Title = "Show Desktop Message")]
    [Description("Opens a native Windows message box on the desktop. Use this to verify the MCP service is alive and can interact with the local machine.")]
    public static string ShowMessage(
        [Description("The text to display in the message box. Defaults to '你好'.")] string? text = null,
        [Description("The title of the message box window. Defaults to 'MCP Test'.")] string? title = null)
    {
        var displayText = string.IsNullOrWhiteSpace(text) ? "你好" : text;
        var displayTitle = string.IsNullOrWhiteSpace(title) ? "MCP Test" : title;

        MessageBoxW(nint.Zero, displayText, displayTitle, MB_OK | MB_ICONINFORMATION);

        return $"Message box displayed successfully with text: {displayText}";
    }
}
