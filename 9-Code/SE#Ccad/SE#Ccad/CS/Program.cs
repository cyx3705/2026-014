using Microsoft.CSharp;
using System;
using System.CodeDom.Compiler;
using System.Reflection;

class DirectFunctionHandler
{
    static void Main()
    {
        // 1. 提示用户输入完整的函数代码
        string userFunction = Console.ReadLine();  // 接收用户输入的函数

        // 2. 把用户输入的函数包装成可编译的完整代码
        // 必须放在类和命名空间中，C#才能编译
        string fullCode = $@"
            using System;  // 引入系统库，支持数学运算等
            namespace UserFunctions  // 定义一个命名空间
            {{
                public class MyCalculator  // 定义一个类，用于存放函数
                {{
                    {userFunction}  // 这里放入用户输入的函数
                }}
            }}";

        // 3. 创建C#编译器（需要提前安装NuGet包）
        using (CSharpCodeProvider compiler = new CSharpCodeProvider())
        {
            // 4. 设置编译参数
            CompilerParameters parameters = new CompilerParameters
            {
                GenerateInMemory = true,  // 编译结果存在内存中，不生成文件
                GenerateExecutable = false,  // 生成类库（不是可执行程序）
                ReferencedAssemblies = { "System.dll" }  // 引用系统核心库
            };

            // 5. 编译用户输入的代码
            CompilerResults results = compiler.CompileAssemblyFromSource(parameters, fullCode);

            // 6. 检查编译是否有错误（比如用户输入的函数语法错误）
            if (results.Errors.HasErrors)
            {
                Console.WriteLine("函数代码有错误：");
                foreach (CompilerError error in results.Errors)
                {
                    Console.WriteLine($"错误：{error.ErrorText}");
                }
                return;  // 有错误就退出
            }

            // 7. 从编译结果中找到用户定义的函数
            Assembly assembly = results.CompiledAssembly;  // 编译后的程序集
            Type calculatorType = assembly.GetType("UserFunctions.MyCalculator");  // 找到存放函数的类
            MethodInfo function = calculatorType.GetMethod("Calculate");  // 找到名为Calculate的函数

            if (function == null)
            {
                Console.WriteLine("未找到名为Calculate的函数，请确保函数名正确");
                return;
            }

            // 8. 获取函数需要的参数（根据用户定义的参数动态询问）
            ParameterInfo[] parametersInfo = function.GetParameters();  // 获取函数的参数信息（如x、y）
            object[] parameterValues = new object[parametersInfo.Length];  // 存储用户输入的参数值

            for (int i = 0; i < parametersInfo.Length; i++)
            {
                ParameterInfo param = parametersInfo[i];
                Console.WriteLine($"请输入参数{param.Name}的值（类型：{param.ParameterType.Name}）：");

                // 尝试转换用户输入为参数所需的类型（这里假设是数字类型）
                while (true)
                {
                    if (double.TryParse(Console.ReadLine(), out double value))
                    {
                        parameterValues[i] = value;
                        break;
                    }
                    else
                    {
                        Console.WriteLine($"输入错误，请重新输入{param.Name}的值：");
                    }
                }
            }

            // 9. 调用函数并输出结果
            object result = function.Invoke(null, parameterValues);  // 调用函数，传入参数值
            Console.WriteLine($"函数执行结果：{result}");
        }
    }
}

