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

### logo与作者

- 作者：pinavia
- ![logo](./Logo.png)

### 学习许可与权利保留

- 本项目允许个人、学校和研究者进行非商业性的学习、研究和引用。
- 未经著作权人书面许可，不得将项目内容用于商业开发、产品制造、商业服务、商业宣传、投标交付、毕业设计成品交付或再授权。
- 不得删除作者、来源、版本历史和第三方许可证声明，不得将本项目或其衍生成果宣称为与作者无关的原创项目。
- 软件源码适用 [PolyForm Noncommercial 1.0.0](https://polyformproject.org/licenses/noncommercial/1.0.0/)；机械模型、PCB、原理图、图片、文档、Unity 工程和其他非软件资料适用 [CC BY-NC-ND 4.0](https://creativecommons.org/licenses/by-nc-nd/4.0/)。
- SE、SW、STEP/STP 及其他三维模型仅允许个人或学校进行非商业学习、评审和研究；未经著作权人书面许可，不得用于加工、打样、制造、销售、量产、投标、产品交付、商业宣传、改作或再授权。
- 第三方组件、模板、元件库、外购件模型和工具仍受其原始许可证约束；本声明不扩大作者对第三方内容的权利。

商业使用、产品化、公司内部长期使用、转让或排他性授权需另行取得著作权人的书面许可。
