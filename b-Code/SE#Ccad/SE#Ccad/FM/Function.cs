using BASS;
using Microsoft.CSharp;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FM
{
    // 负责处理用户自定义函数的类
    internal class Function
    {
        public static Assembly newfunction(string get)
        {
            string[] commandParts = get.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            string functionName = commandParts[3];
            if (gv2.allFunction.ContainsKey(functionName))
            {
                gv.Message.Enqueue($"复用已编译函数：[{functionName}]");
                return gv2.allFunction[functionName].Assembly;
            }
            string fullCode = $@"
            using System;  // 引入系统库，支持数学运算等
            namespace UserFunctions  // 定义一个命名空间
            {{
                public class MyCalculator  // 定义一个类，用于存放函数
                {{
                    {get}  // 这里放入用户输入的函数
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
                    gv.Message.Enqueue("函数代码有错误：");
                    foreach (CompilerError error in results.Errors)
                    {
                        gv.Message.Enqueue($"错误：{error.ErrorText}");
                    }
                    return null;  // 有错误就退出
                }
                else
                {
                    gv.Message.Enqueue("函数编译成功！");
                    // 7. 从编译结果中找到用户定义的函数
                    Assembly assembly = results.CompiledAssembly;  // 编译后的程序集
                    List<string> paramNames = ExtractParamNames(assembly, functionName);
                    gv2.allFunction[functionName] = new gv2.FunctionCache
                    {
                        Assembly = assembly,
                        ParamNames = paramNames
                    };
                    return assembly;
                }
            }
        }
        private static List<string> ExtractParamNames(Assembly assembly, string functionName)
        {
            Type calculatorType = assembly.GetType("UserFunctions.MyCalculator");
            if (calculatorType == null) return new List<string>();

            MethodInfo method = calculatorType.GetMethod(
                functionName,
                BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance
            );
            if (method == null) return new List<string>();
            return method.GetParameters().Select(p => p.Name).ToList();
        }

        public static void name(string get, DraggableTextBox textBox)
        {
            string[] commandParts = get.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            StationSync.changename(textBox, commandParts[3]);
            gv.Message.Enqueue($"函数名称已设置为：{commandParts[3]}");
        }

        public static double? usingfunction(Assembly assembly, string functionName, object[] paramValue)
        {
            if (assembly == null || string.IsNullOrWhiteSpace(functionName))
            {
                gv.Message.Enqueue("程序集或函数名为空！");
                return null;
            }

            try
            {
                // 1. 获取用户定义的类（固定命名空间和类名）
                Type calculatorType = assembly.GetType("UserFunctions.MyCalculator");
                if (calculatorType == null)
                {
                    gv.Message.Enqueue("未找到类 UserFunctions.MyCalculator");
                    return null;
                }

                // 2. 筛选目标方法：
                // - 静态方法（IsStatic = true）
                // - 方法名匹配（Name == functionName）
                // - 返回值为 double（ReturnType == typeof(double)）
                // - 参数为1个且类型为 double（GetParameters() 长度1且类型为double）
                MethodInfo targetMethod = calculatorType.GetMethods(BindingFlags.Public | BindingFlags.Static)
                    .FirstOrDefault(m =>
                        m.Name == functionName &&
                        m.ReturnType == typeof(double) &&
                        m.GetParameters().Length == 1 &&
                        m.GetParameters()[0].ParameterType == typeof(double)
                    );

                if (targetMethod == null)
                {
                    gv.Message.Enqueue($"未找到匹配的函数：public static double {functionName}(double)");
                    return null;
                }

                // 3. 传递参数并调用（静态方法实例参数为null）
                object result = targetMethod.Invoke(null, paramValue);

                // 4. 转换返回值为double
                return (double)result;
            }
            catch (Exception ex)
            {
                gv.Message.Enqueue($"调用函数出错：{ex.InnerException?.Message ?? ex.Message}");
                return null;
            }
        }
        public static void set(string name,double x, DraggableTextBox textBox)
        {
            StationSync.changename(textBox, name);
            StationSync.changevalue(textBox, x);
        }
    }
}
