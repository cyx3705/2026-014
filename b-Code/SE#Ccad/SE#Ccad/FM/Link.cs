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
    // 负责处理计算站链接的类
    public class Link
    {
        
        private static volatile bool _needRefresh = false;
        public static void link(string box1, string box2)
        {
            gv.Message.Enqueue($"连接命令已执行，box1：{box1}，box2：{box2}");
            // 校验输入参数（避免空值导致的逻辑错误）
            if (string.IsNullOrWhiteSpace(box1))
            {
                gv.Message.Enqueue("错误：要链接的第一个计算站名称不能为空！");
                return;
            }
            if (string.IsNullOrWhiteSpace(box2))
            {
                gv.Message.Enqueue("错误：要链接的目标计算站名称不能为空！");
                return;
            }
            // 在这里添加实际的链接逻辑
            // 关键修复：先将 calculate 类的 NextName 改为可读写（原代码是只读 get;，无法修改）
            // 需在 calculate 类中修改：public string NextName { get; set; }

            // 1. 遍历全局计算站数组，查找 SelfName = box1 的计算站
            calculate sourceStation = null;
            foreach (var station in gv2.AllStations)
            {
                // 跳过数组中未初始化的 null 元素，精准匹配名称
                if (station != null && string.Equals(station.SelfName, box1, StringComparison.Ordinal))
                {
                    sourceStation = station;
                    break; // 找到后立即退出循环，提升效率
                }
            }

            // 2. 处理查找结果
            if (sourceStation == null)
            {
                gv.Message.Enqueue($"错误：未找到名称为 [{box1}] 的计算站！");
                return;
            }

            // 3. 验证目标计算站 box2 是否存在（可选，增强合理性）
            bool targetExists = false;
            foreach (var station in gv2.AllStations)
            {
                if (station != null && string.Equals(station.SelfName, box2, StringComparison.Ordinal))
                {
                    targetExists = true;
                    break;
                }
            }
            if (!targetExists)
            {
                gv.Message.Enqueue($"警告：目标计算站 [{box2}] 不存在，仍执行链接！");
            }

            // 4. 执行链接：将 box1 的 NextName 指向 box2
            sourceStation.NextName = box2;
            gv.Message.Enqueue($"成功链接：[{box1}] → [{box2}]");
            Link.TriggerRefresh();
        }

        public static void TriggerRefresh()
        {
            _needRefresh = true;
        }
        public static void RefreshLinksLoop()
        {
            while (true)
            {
                Thread.Sleep(50); // 降低检查频率，减少资源占用
                if (_needRefresh)
                {
                    // 获取Form2实例，触发Paint事件（间接重绘）
                    Form2 form = Application.OpenForms.OfType<Form2>().FirstOrDefault();
                    form?.Invalidate(); // 触发窗体重绘（会调用Paint事件）
                    _needRefresh = false;
                }
            }
        }
        public static void DrawLinks(PaintEventArgs e)
        {
            if (e == null) return;

            Graphics g = e.Graphics; // 用窗体刷新时的绘图上下文（不会被覆盖）
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias; // 抗锯齿，连线更清晰

            foreach (var sourceStation in gv2.AllStations)
            {
                if (sourceStation == null || string.IsNullOrWhiteSpace(sourceStation.NextName))
                    continue;

                // 查找目标计算站
                var targetStation = gv2.AllStations.FirstOrDefault(
                    s => s != null && string.Equals(s.SelfName, sourceStation.NextName, StringComparison.Ordinal)
                );

                if (targetStation == null) continue;

                // 计算连线起点（源文本框中心）
                PointF start = new PointF(
                    sourceStation.Position.X + 100, // 文本框宽200，中心X=左+100
                    sourceStation.Position.Y + 25   // 文本框高50，中心Y=上+25
                );

                // 计算连线终点（目标文本框中心）
                PointF end = new PointF(
                    targetStation.Position.X + 100,
                    targetStation.Position.Y + 25
                );

                // 绘制带箭头的连线
                using (Pen pen = new Pen(Color.Blue, 2))
                {
                    pen.EndCap = System.Drawing.Drawing2D.LineCap.ArrowAnchor;
                    g.DrawLine(pen, start, end);
                }
            }
        }

        // 可选：添加清除连线的方法（切换场景时使用）
        public static void ClearLinks(Form2 form)
        {
            if (form == null) return;
            // 重绘Form2画布，清除所有连线
            form.Invalidate();
            //gv.Message.Enqueue("已清除所有关联线！");
        }
    }
}

