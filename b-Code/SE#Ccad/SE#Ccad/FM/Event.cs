using BASS;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FM
{
    // 负责处理事件的类
    internal class Event
    {
        public static string DraggableTB_KeyDown(object sender, KeyEventArgs e)
        {
            // 将 sender 转换为 DraggableTextBox 类型，获取其 Text 属性
            DraggableTextBox textBox = sender as DraggableTextBox;
            if (textBox != null && e.KeyCode == Keys.Enter)
            {
                string inputText = textBox.Text;

                // 入队消息（可选项：按下 Enter 后清空文本框，模拟输入完成）
                gv.Message.Enqueue("当前输入：" + inputText);
                //textBox.Clear(); // 清空输入框，方便下次输入
                e.SuppressKeyPress = true; // 阻止 Enter 键的默认行为（如换行）
                return inputText;
            }
            else
            {
                return null;
            }
        }

        // 定义一个委托，用于跨类调用窗体的方法（创建新TextBox）
        public static Action CreateNewTextBox { get; set; }
        public static void Form2_Load(object sender, KeyEventArgs e)
        {
            DraggableTextBox textBox = sender as DraggableTextBox;
            string get=DraggableTB_KeyDown(sender,e);
            if (get != null)
            {
                string[] commandParts = get.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                switch (commandParts[0])
                {
                    case "test":
                        gv.Message.Enqueue("你输入了 test 命令！");
                        break;
                    case "birth":
                        CreateNewTextBox?.Invoke();
                        textBox.Clear();
                        break;
                    case "clear":
                        Message.clear(sender);
                        break;
                    case "public":
                        Function.newfunction(get);
                        Function.name(get, textBox);
                        break;
                    case "link":
                        Link.link(commandParts[1], commandParts[2]);
                        break;
                    case "set":
                        Function.set(commandParts[1], Double.Parse(commandParts[2]), textBox);
                        break;
                    case "start":
                        Start.caculate(commandParts[1]);
                        break;
                    default:
                        break;
                }
            }
        }


    }
}
