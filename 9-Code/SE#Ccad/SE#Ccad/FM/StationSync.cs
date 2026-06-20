using BASS;
using FM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FM
{
    // 负责计算站与文本框同步的静态类
    public static class StationSync
    {
        /// <summary>
        /// 核心同步方法：以 gv2.AllStations 为基准，同步到对应的文本框
        /// （计算站的 SelfName、Value 变化后，更新文本框的 BoxName 和 Text）
        /// </summary>
        /// <param name="form">主窗体（用于查找所有文本框）</param>
        public static void SyncStationToTextBox(Form2 form)
        {
            if (form == null) return;

            // 遍历所有计算站（以计算站为基准）
            foreach (var station in gv2.AllStations)
            {
                if (station == null) continue; // 跳过空元素

                // 查找窗体中与当前计算站 SelfName 匹配的 DraggableTextBox
                var targetTextBox = form.Controls.OfType<DraggableTextBox>()
                    .FirstOrDefault(tb => tb.Station != null &&
                                         string.Equals(tb.Station.SelfName, station.SelfName, StringComparison.Ordinal));

                if (targetTextBox == null)
                {
                    gv.Message.Enqueue($"警告：未找到与计算站 [{station.SelfName}] 匹配的文本框，跳过同步！");
                    continue;
                }

                // 同步：计算站 → 文本框（覆盖文本框的名称和内容）
                targetTextBox.BoxName = station.SelfName; // 同步名称（上方显示框）
                double outputText = station.Value != null ? Convert.ToDouble(station.Value) : 0;
                targetTextBox.SetOutputLabelText(outputText); // 调用新增的访问方法
                targetTextBox.Location = station.Position; // 同步位置（确保文本框位置与计算站一致）

                //gv.Message.Enqueue($"已同步计算站 [{station.SelfName}] 到文本框");
            }
        }

        /// <summary>
        /// 辅助同步方法：文本框 → 计算站（文本框修改后，更新计算站）
        /// （比如手动修改文本框名称/内容后，同步到 gv2.AllStations）
        /// </summary>
        /// <param name="textBox">要同步的文本框</param>
        public static void SyncTextBoxToStation(DraggableTextBox textBox)
        {
            if (textBox == null || textBox.Station == null) return;

            // 遍历计算站，找到与文本框 Station.SelfName 匹配的元素
            foreach (var station in gv2.AllStations)
            {
                if (station == null) continue;

                // 匹配条件：计算站 SelfName == 文本框绑定的 Station.SelfName
                if (string.Equals(station.SelfName, textBox.Station.SelfName, StringComparison.Ordinal))
                {
                    // 同步：文本框 → 计算站（更新计算站的属性）
                    station.Value = textBox.Text; // 文本框内容 → 计算站 Value
                    station.Position = textBox.Location; // 文本框位置 → 计算站 Position
                                                         // 注意：SelfName 是只读属性（构造时确定），若要修改需先把 SelfName 改为 get; set;
                                                         // station.SelfName = textBox.BoxName; 

                    gv.Message.Enqueue($"已同步文本框 [{textBox.BoxName}] 到计算站");
                    Link.TriggerRefresh(); // 同步后刷新连线
                    break;
                }
            }
        }

        /// <summary>
        /// 批量同步：遍历所有文本框，统一执行 文本框→计算站 同步
        /// </summary>
        /// <param name="form">主窗体</param>
        public static void SyncAllTextBoxToStation(Form2 form)
        {
            if (form == null) return;

            // 遍历窗体中所有 DraggableTextBox，逐个同步
            foreach (var textBox in form.Controls.OfType<DraggableTextBox>())
            {
                SyncTextBoxToStation(textBox);
            }
        }

        public static void changename(DraggableTextBox textBox, string newName)
        {
            if (textBox == null || textBox.Station == null) return;
            // 更新文本框名称
            textBox.BoxName = newName;
            // 同步到计算站（需先把 SelfName 改为 get; set;）
            foreach (var station in gv2.AllStations)
            {
                if (station == null) continue;
                if (string.Equals(station.SelfName, textBox.Station.SelfName, StringComparison.Ordinal))
                {
                    station.SelfName = newName; // 更新计算站名称
                    gv.Message.Enqueue($"已将文本框名称同步到计算站，新的计算站名称为 [{newName}]");
                    Link.TriggerRefresh(); // 刷新连线
                    break;
                }
            }
        }
        public static void changevalue(DraggableTextBox textBox, double newValue)
        {
            if (textBox == null || textBox.Station == null) return;
            // 更新文本框内容
            textBox.SetOutputLabelText(newValue);
            // 同步到计算站
            foreach (var station in gv2.AllStations)
            {
                if (station == null) continue;
                if (string.Equals(station.SelfName, textBox.Station.SelfName, StringComparison.Ordinal))
                {
                    station.Value = newValue; // 更新计算站值
                    gv.Message.Enqueue($"已将文本框内容同步到计算站，新的计算站值为 [{newValue}]");
                    Link.TriggerRefresh(); // 刷新连线
                    break;
                }
            }
        }
    } 
}