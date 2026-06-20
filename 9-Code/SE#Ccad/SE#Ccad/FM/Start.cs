using BASS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Collections.Specialized.BitVector32;

namespace FM
{
    // 负责计算流程控制的类
    public class Start
    {
        public static void caculate(string firstname)
        {
            // 1. 查找起始计算站
            var Station = gv2.AllStations.FirstOrDefault(s => s != null && s.SelfName == firstname);
            if (Station == null)
            {
                gv.Message.Enqueue($"错误：未找到起始计算站 [{firstname}]");
                return;
            }

            // 2. 处理当前计算站（值/函数）
            var form = Application.OpenForms.OfType<Form2>().FirstOrDefault();
            bool processSuccess = ProcessStation(Station, form);
            if (!processSuccess)
            {
                gv.Message.Enqueue($"流程终止：计算站 [{firstname}] 处理失败");
                return;
            }

            // 3. 嵌套调用NextName（递归执行下一个计算站）
            if (!string.IsNullOrWhiteSpace(Station.NextName))
            {
                gv.Message.Enqueue($"→ 开始执行下一个计算站：[{Station.NextName}]");
                caculate(Station.NextName); // 递归嵌套
            }
            else
            {
                gv.Message.Enqueue($"流程结束：计算站 [{firstname}] 无后续链接");
            }
        }
        public static bool ProcessStation(calculate Station, Form2 form)
        {
            string funcName = Station.SelfName;
            if (Station.Value != null)
            {
                gv.Message.Enqueue($"计算站 [{Station.SelfName}] 已有值，跳过计算，值：{Station.Value}");
                return true; // 已有值，跳过计算
            }
            else if (gv2.allFunction.TryGetValue(funcName, out var funcCache))
            {
                // 1. 初始化参数值列表
                List<object> paramValues = new List<object>();
                // 2. 遍历函数参数名，查找对应计算站的值
                foreach (string paramName in funcCache.ParamNames)
                { 
                    // 2.1 查找SelfName与参数名匹配的计算站
                    var paramStation = gv2.AllStations.FirstOrDefault(s =>
                        s != null && string.Equals(s.SelfName, paramName, StringComparison.Ordinal)
                    );
                    if (paramStation == null)
                    {
                        gv.Message.Enqueue($"错误：函数 [{funcName}] 缺少参数 [{paramName}] 对应的计算站");
                        return false;
                    }
                    // 2.2 若参数计算站无值，递归调用caculate获取值
                    if (paramStation.Value == null)
                    {
                        gv.Message.Enqueue($"参数 [{paramName}] 无值，开始计算...");
                        caculate(paramName); // 递归计算该参数值

                        // 计算后仍无值，终止流程
                        if (paramStation.Value == null)
                        {
                            gv.Message.Enqueue($"错误：参数 [{paramName}] 计算后仍无值");
                            return false;
                        }
                    }
                    paramValues.Add(paramStation.Value);
                    gv.Message.Enqueue($"参数 [{paramName}] 取值：{paramStation.Value}");
                }
                // 3. 调用函数并获取结果
                Type calculatorType = funcCache.Assembly.GetType("UserFunctions.MyCalculator");
                if (calculatorType == null)
                {
                    gv.Message.Enqueue($"错误：未找到函数类 UserFunctions.MyCalculator");
                    return false;
                }
                MethodInfo method = calculatorType.GetMethod(funcName,
                BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);
                if (method == null)
                {
                    gv.Message.Enqueue($"错误：未找到函数 [{funcName}]");
                    return false;
                }

                object instance = method.IsStatic ? null : Activator.CreateInstance(calculatorType);
                object result = method.Invoke(instance, paramValues.ToArray());

                Station.Value = result; // 存结果
                StationSync.SyncStationToTextBox(form);
                gv.Message.Enqueue($"计算站 [{Station.SelfName}] 函数执行成功，结果：{result}");
                return true;
            }
            else
            {
                gv.Message.Enqueue($"错误：计算站 [{Station.SelfName}] 未找到对应函数");
                return false;
            }
        }
    }
}
