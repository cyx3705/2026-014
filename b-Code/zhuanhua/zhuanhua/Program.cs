using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

/// <summary>
/// OpenAPI 3.0 转 C# API常量类生成器
/// </summary>
public class OpenApiToCSharpGenerator
{
    // OpenAPI数据模型（简化版，仅包含需要的字段）
    private class OpenApiDocument
    {
        public List<Tag> tags { get; set; }
        public Dictionary<string, PathItem> paths { get; set; }
    }

    private class Tag
    {
        public string name { get; set; }
        public string description { get; set; }
    }

    private class PathItem
    {
        public Operation get { get; set; }
        public Operation post { get; set; }
        public Operation put { get; set; }
        public Operation delete { get; set; }
    }

    private class Operation
    {
        public List<string> tags { get; set; }
        public string summary { get; set; }
        public string description { get; set; }
    }

    /// <summary>
    /// 从JSON字符串生成C# API常量类
    /// </summary>
    /// <param name="openApiJson">OpenAPI 3.0 JSON字符串</param>
    /// <param name="className">生成的类名</param>
    /// <returns>C#类代码字符串</returns>
    public string GenerateCSharpClass(string openApiJson, string className = "ApiName")
    {
        // 反序列化JSON
        var openApiDoc = JsonConvert.DeserializeObject<OpenApiDocument>(openApiJson);
        if (openApiDoc == null || openApiDoc.paths == null || openApiDoc.tags == null)
        {
            throw new ArgumentException("无效的OpenAPI JSON数据");
        }

        // 构建标签描述映射
        var tagDescMap = openApiDoc.tags.ToDictionary(t => t.name, t => t.description);

        // 按标签分组整理API信息
        var apiByTag = new Dictionary<string, List<(string constantName, string path, string comment)>>();

        // 初始化所有标签的分组
        foreach (var tag in openApiDoc.tags)
        {
            apiByTag[tag.name] = new List<(string, string, string)>();
        }

        // 解析每个API路径
        foreach (var pathKvp in openApiDoc.paths)
        {
            string apiPath = pathKvp.Key;
            var pathItem = pathKvp.Value;

            // 处理所有HTTP方法（get/post/put/delete）
            var operations = new List<Operation>();
            if (pathItem.get != null) operations.Add(pathItem.get);
            if (pathItem.post != null) operations.Add(pathItem.post);
            if (pathItem.put != null) operations.Add(pathItem.put);
            if (pathItem.delete != null) operations.Add(pathItem.delete);

            foreach (var operation in operations)
            {
                if (operation.tags == null || !operation.tags.Any()) continue;

                // 取第一个标签作为分组依据
                string mainTag = operation.tags.First();
                if (!apiByTag.ContainsKey(mainTag)) continue;

                // 生成常量名称（例如：/api/core/system/v1/power/status → power_status）
                string constantName = GenerateConstantName(apiPath);

                // 优先使用description，没有则用summary
                string comment = string.IsNullOrEmpty(operation.description)
                    ? operation.summary
                    : operation.description;

                // 添加到对应分组
                apiByTag[mainTag].Add((constantName, apiPath, comment));
            }
        }

        // 生成C#代码
        var codeBuilder = new StringBuilder();
        codeBuilder.AppendLine($"public class {className}");
        codeBuilder.AppendLine("{");

        // 为每个标签生成#region
        foreach (var tagKvp in apiByTag)
        {
            string tagName = tagKvp.Key;
            string tagDesc = tagDescMap.TryGetValue(tagName, out var desc) ? desc : tagName;
            var apiList = tagKvp.Value;

            // 生成region
            codeBuilder.AppendLine($"    #region {tagDesc}");
            codeBuilder.AppendLine();

            // 生成该标签下的所有API常量
            foreach (var api in apiList)
            {
                // 添加注释
                codeBuilder.AppendLine($"    /// <summary>");
                // 处理多行注释
                foreach (var line in api.comment.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    codeBuilder.AppendLine($"    /// {line.Trim()}");
                }
                codeBuilder.AppendLine($"    /// </summary>");

                // 生成常量定义
                codeBuilder.AppendLine($"    public const string {api.constantName} = \"{api.path}\";");
                codeBuilder.AppendLine();
            }

            codeBuilder.AppendLine($"    #endregion");
            codeBuilder.AppendLine();
        }

        codeBuilder.AppendLine("}");

        return codeBuilder.ToString();
    }

    /// <summary>
    /// 从API路径生成符合C#规范的常量名称
    /// </summary>
    /// <param name="path">API路径</param>
    /// <returns>常量名称</returns>
    private string GenerateConstantName(string path)
    {
        // 移除前缀 /api/core/xxx/v1/
        string cleanPath = Regex.Replace(path, @"^/api/core/[^/]+/v\d+/", "");

        // 替换特殊字符为下划线
        cleanPath = cleanPath.Replace("/", "_")
                             .Replace(":", "_")
                             .Replace("-", "_")
                             .ToLower();

        // 移除重复的下划线
        cleanPath = Regex.Replace(cleanPath, @"_+", "_");

        // 移除首尾下划线
        cleanPath = cleanPath.Trim('_');

        // 如果为空则用默认名称
        return string.IsNullOrEmpty(cleanPath) ? "unknown_api" : cleanPath;
    }

    // 辅助方法：从文件读取JSON
    public string ReadOpenApiJsonFromFile(string filePath)
    {
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException("文件不存在", filePath);
        }
        return File.ReadAllText(filePath, Encoding.UTF8);
    }
}

// 使用示例
class Program
{
    static void Main(string[] args)
    {
        try
        {
            // 1. 初始化生成器
            var generator = new OpenApiToCSharpGenerator();

            // 2. 读取OpenAPI JSON（可以从文件读取，也可以直接传入JSON字符串）
            // 方式1：从文件读取
            string openApiJson = generator.ReadOpenApiJsonFromFile(
                @"C:\Users\202509007D\Desktop\swagger-conf.json");

            // 方式2：直接传入JSON字符串（替换为你的完整JSON）
            //string openApiJson = @"你的完整OpenAPI JSON字符串";

            // 3. 生成C#类代码
            string csharpCode = generator.GenerateCSharpClass(openApiJson, "ApiName");

            // 4. 输出结果（可以保存到文件）
            Console.WriteLine(csharpCode);

            // 保存到文件
            File.WriteAllText("ApiName.cs", csharpCode, Encoding.UTF8);
            Console.WriteLine("C#类文件已生成：ApiName.cs");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"生成失败：{ex.Message}");
        }
    }
}
