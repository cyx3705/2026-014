using BASS;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FM
{
    // 负责处理消息显示的类
    public class Message
    {
        public void MessageMode()  // 去掉 static，以便访问实例控件 messageBox
        {
            while (gv.IsMessageRunning)
            {
                if (gv.Message.TryDequeue(out string message))
                {
                    // 关键：跨线程安全更新 UI 控件（使用 Invoke 确保在 UI 线程执行）
                    gv.MessageBox.Invoke(new Action(() =>
                    {
                        gv.MessageBox.Text += $"{message}{Environment.NewLine}";
                        // 自动滚动到最新内容
                        gv.MessageBox.SelectionStart = gv.MessageBox.TextLength;
                        gv.MessageBox.ScrollToCaret();
                    }));
                }
                else
                {
                    Thread.Sleep(50); // 队列空时休眠，降低 CPU 占用
                }
            }
        }

        public static void Form2_Resize(object sender, EventArgs e)
        {
            Form form = sender as Form;
            // 实时获取新的完整大小
            Size newSize = form.Size;
            // 实时获取新的客户区大小
            Size newClientSize = form.ClientSize;
            gv.Form2_Width = newSize.Width;
            gv.Form2_Height = newSize.Height;
            gv.MessageBox.Location = new System.Drawing.Point(0, gv.Form2_Height / 3 * 2);  // 初始位置
            gv.MessageBox.Size = new System.Drawing.Size(gv.Form2_Width, gv.Form2_Height / 3);  // 大小
            // 示例：显示到消息窗口
            gv.Message.Enqueue($"窗体大小变化：完整尺寸 {newSize.Width}x{newSize.Height}，客户区 {newClientSize.Width}x{newClientSize.Height}");
        }
        public static void MessageBox(Form parentForm)
        {
            // 检查传入的窗体是否为空
            if (parentForm == null)
                throw new ArgumentNullException(nameof(parentForm), "父窗体不能为空");

            // 创建消息文本框
            gv.MessageBox = new TextBox
            {
                Location = new Point(0, parentForm.Height / 3 * 2), // 基于父窗体高度定位
                Size = new Size(parentForm.Width, parentForm.Height / 3), // 基于父窗体大小设置
                Multiline = true,
                ReadOnly = true,
                ScrollBars = ScrollBars.Vertical
            };

            gv.MessageBox.Text = "这是消息窗口";

            // 通过传入的父窗体添加控件（替代 this.Controls.Add）
            parentForm.Controls.Add(gv.MessageBox);
        }
    

        public static void clear(object sender)
        {
            DraggableTextBox textBox = sender as DraggableTextBox;
            gv.MessageBox.Invoke(new Action(() =>
            {
                gv.MessageBox.Clear();
            }));
            gv.Message.Enqueue("消息窗口已清空！");
            textBox.Clear();
        }
    } 
}

