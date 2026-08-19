# 2026-014-Csharp学习

> C# 学习资料与实践代码，包含课程练习、Solid Edge 二次开发监视器、桌面与算法实验工程，以及 Codex MCP 接入示例

## 项目内容

1. **a1-C#课程代码**

- K6：控制台练习 K6-1（类型与变量）、K8-2（圆锥体积计算）、K9-1（构造函数与方法重载）；K8-1 为 C++ `student` 类示例（`student.h` / `student.cpp` / `源.cpp`）；
- K10：数组、字典、运算符、`typeof` 与 `default`；
- K11：`var`、匿名类型、溢出检查、`sizeof` 与指针；K11-2 为 WPF，按钮将文本框设为 “Hello WPF”；
- K12：显式类型转换（`stone`→`Monkey`）与数值提升；K12-2 为 WPF 两数相加；另有 `显示类型转换.png`、`隐式类型转换.png`。

2. **b-Code-SE**
WPF 程序「Solid Edge 操作监视器」（`net8.0-windows`），可连接或启动 Solid Edge、将窗口嵌入 SE 右侧，并记录命令与零件特征参数（如拉伸 Depth）。
3. **b-Code**

- `Desktop-core`：Windows 服务 `WallpaperChangerService`，按时刻更换「图片」目录中的壁纸；
- `music/music-one`：NAudio 实时音频超分（44100 Hz → 96000 Hz）；
- `nano学习`：输出 `Hello, World!` 的控制台程序；
- `SE#Ccad`：WinForms 计算站流程（`FM`）、动态编译用户函数（`CS`）、DeepSeek API 调用（`BASS/Askdeepseek.cs`），以及空的 `测试` 工程；
- `text/destkop text`：RSS 非协作无线定位 SDP 估计与 CRLB 复现；
- `zhuanhua`：OpenAPI 3.0 JSON 转 C# API 常量类生成器；
- `SE#C`、`SE#F` 为空目录。

4. **b-Codex-use**
ASP.NET Core 8 工程，在 `/mcp` 提供 MCP 服务，含 `hello_world`、`show_message`（Win32 消息框）工具；同目录另有 `concept_car_blender.py`、`concept_car.blend` 与 `concept_car_preview.png`。

## 保留内容
- 本模板项目介绍：此为最初的准备的项目模板
 每个分支项目都会由他去继承
- 作者：Pinavia - 2025
