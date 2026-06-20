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
    // 负责创建可拖动文本框的类
    internal class Birth
    {
        public void AddDraggableTextBox(Form parentForm)
        {
            string stationName = "DefaultStation"; // 默认计算站名称
            string nextName = string.Empty; // 指向的下一个计算站名称（默认为空）
            Point initialPos = new Point(0, 0); // 初始位置
            object initialValue = null; // 初始值（默认为null）

            // 2. 创建计算站实例并存入全局数组
            var station = new calculate(stationName, nextName, initialPos, initialValue);
            if (gv2.StationIndex < gv2.AllStations.Length)
            {
                gv2.AllStations[gv2.StationIndex] = station;
                gv2.StationIndex++; // 索引自增
            }
            else
            {
                gv.Message.Enqueue("警告：计算站数量已达上限！");
                return;
            }

            DraggableTextBox draggableTB = new DraggableTextBox(station);
            draggableTB.Text = "蛋生蛋！";
            draggableTB.Location = initialPos; // 位置与计算站一致
            draggableTB.Size = new Size(200, 50); // 恢复为原高度（适配上下显示框）
            draggableTB.KeyDown += Event.Form2_Load;

            parentForm.Controls.Add(draggableTB);
        }
        public void AddNewDraggableTextBox(Form parentForm, string name = null)
        {
            // 处理默认名称
            string actualName = string.IsNullOrWhiteSpace(name) ? "newbox"+(gv.TextBoxCount - 1).ToString(): name;
            // 创建文本框实例（传入父窗体）
            CreateTextBoxInstance(actualName, parentForm);
            gv.Message.Enqueue("已创建新的蛋！");
        }
        public void AddNewDraggableTextBox(string name, Form parentForm)
        {
            CreateTextBoxInstance(name, parentForm);
            gv.Message.Enqueue("已创建新的蛋！"); // 消息提示
        }

        private void CreateTextBoxInstance(string name, Form parentForm)
        {
            // 1. 计算初始位置（避免重叠）
            int initialY = 50 + (gv.TextBoxCount - 1) * 60;
            Point initialPos = new Point(50, initialY);

            // 2. 定义计算站参数（name为文本框名称，nextName默认空，value默认null）
            string nextName = string.Empty;
            object initialValue = null;


            // 3. 创建计算站实例并存入全局数组
            var station = new calculate(name, nextName, initialPos, initialValue);
            if (gv2.StationIndex < gv2.AllStations.Length)
            {
                gv2.AllStations[gv2.StationIndex] = station;
                gv2.StationIndex++;
            }
            else
            {
                gv.Message.Enqueue("警告：计算站数量已达上限！");
                return;
            }

            // 4. 用带参构造创建文本框，绑定计算站
            DraggableTextBox newTB = new DraggableTextBox(station);
            newTB.Text = "蛋生蛋！";
            newTB.Location = initialPos; // 位置与计算站同步
            newTB.Size = new Size(200, 50); // 高度改为50（适配上下显示框）
            newTB.BoxName = name; // 文本框上方名称=计算站名称
            newTB.KeyDown += Event.Form2_Load;

            parentForm.Controls.Add(newTB);
            gv.TextBoxCount++;
        }
    }
}
