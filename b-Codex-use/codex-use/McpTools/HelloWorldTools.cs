using System.ComponentModel;
using ModelContextProtocol.Server;

namespace codex_use.McpTools;

[McpServerToolType]
public static class HelloWorldTools
{
    [McpServerTool(Name = "hello_world", Title = "Hello World", ReadOnly = true)]
    [Description("Returns a friendly message that can be displayed by a local desktop MCP client.")]
    public static string HelloWorld(
        [Description("Optional text to include in the greeting.")] string? message = null)
    {
        return string.IsNullOrWhiteSpace(message)
            ? "Hello, world! Codex is speaking through the local MCP service."
            : $"Hello, world! {message}";
    }
}
