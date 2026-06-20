using BASS;
using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace FM
{
    internal static class Program
    {

        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            // 启动消息处理线程
            Message Message = new Message();
            Thread messageThread = new Thread(Message.MessageMode);
            messageThread.IsBackground = true; // 设为后台线程，随程序退出而结束
            messageThread.Start();

            //启动绘制链接线线程
            Link link = new Link();
            Thread linkThread = new Thread(Link.RefreshLinksLoop);
            linkThread.IsBackground = true; // 设为后台线程，随程序退出而结束
            linkThread.Priority = ThreadPriority.BelowNormal; // 主函数中设置优先级
            linkThread.Start();

            // 启动主窗体
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form2());
        }
    }

    partial class Form1 : System.Windows.Forms.Form
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Text = "Form1";
        }

        #endregion
    }
    public class Form2 : System.Windows.Forms.Form
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        public Form2()
        {
            Birth birth = new Birth();
            InitializeComponent();
            birth.AddDraggableTextBox(this);
            Message.MessageBox(this);
            Event.CreateNewTextBox = () =>
            {
                // 调用创建方法，传入父窗体和默认名称
                birth.AddNewDraggableTextBox(this);
            };
            this.Paint += (sender, e) => Link.DrawLinks(e);
            this.Resize += Message.Form2_Resize;
        }
        // 向窗口添加可拖动的TextBox
       
        #region Windows Form Designer generated code
       
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(gv.Form2_Width, gv.Form2_Height);
            this.Text = "主窗口";
        }
        #endregion
    }
}