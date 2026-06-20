using BASS;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FM
{
    // 负责存储全局计算站和函数缓存的类
    public class gv2
    {
        public static calculate[] AllStations = new calculate[128];
        public static int StationIndex = 0;
        public static Dictionary<string, FunctionCache> allFunction = new Dictionary<string, FunctionCache>();
        public class FunctionCache
        {
            public Assembly Assembly { get; set; } // 编译后的程序集
            public List<string> ParamNames { get; set; } // 函数参数名列表
        }
    }
    // 负责存储计算站信息的类
    public class calculate
    {
        // 必选字段（通过构造函数强制初始化，避免null）
        public string SelfName { get; set; }
        public string NextName { get; set; }
        // 可读写的值
        public object Value { get; set; }
        public Point Position { get; set; }      // 新增：文本框的位置（X,Y）

        // 构造函数：强制传入必要参数，保证数据完整性

        public calculate(string selfName, string targetStackName,
                   Point initialPosition = default, object value = null)
        {
            if (string.IsNullOrWhiteSpace(selfName))
                throw new ArgumentException("计算站名称不能为空", nameof(selfName));

            SelfName = selfName;
            NextName = targetStackName ?? string.Empty;
            Value = value;
            Position = initialPosition; // 初始化位置（关键修复）
        }
        // 可选：添加自定义方法（扩展能力）
        public T GetValue<T>() where T : class
        {
            // 安全转换值类型，避免强制转换异常
            return Value as T;
        }

        // 可选：重写ToString（调试/日志更友好）
        public override string ToString()
        {
            return $"[{SelfName}] → [{NextName}] = {Value ?? "null"}";
        }

        // 使用示例
        //public static CalculationStation[] Stations = new CalculationStation[128];
        //Stations[0] = new CalculationStation("Station2", "StackB", "自定义字符串值");
        //    // 调用自定义方法
        //    double? numValue = Stations[0].GetValue<double>(); // 安全转换，失败返回null
        //    gv.Message.Enqueue(Stations[0].ToString()); // 输出格式化日志
    }
    // 负责创建可拖动文本框的类
    public class DraggableTextBox : TextBox
    {
        // 通过私有字段+属性访问器，在设置Station时同步位置
        private calculate _station;
        public calculate Station
        {
            get => _station;
            set
            {
                _station = value;
                // 新增：设置Station时同步当前文本框位置到计算站
                if (_station != null)
                {
                    _station.Position = this.Location;
                }
            }
        }


        // 附属控件
        private TextBox _nameLabel;     // 上方名称显示框
        private TextBox _outputLabel;    // 下方输出显示框       
        private int _offsetX;        // 拖动相关变量
        private int _offsetY;
        private bool _isDragging = false;



        //以下是复用的初始化和拖动逻辑
        // 同步上下显示框的位置（跟随主文本框）
        private void UpdateLabelPositions()
        {
            if (_nameLabel == null || _outputLabel == null) return;

            // 上方名称框：位于主文本框正上方，左对齐
            _nameLabel.Location = new Point(this.Left, this.Top - _nameLabel.Height - 2);
            _nameLabel.Width = this.Width;  // 宽度与主文本框一致

            // 下方输出框：位于主文本框正下方，左对齐
            _outputLabel.Location = new Point(this.Left, this.Bottom + 2);
            _outputLabel.Width = this.Width;  // 宽度与主文本框一致
        }
        [Obsolete("建议使用带 calculate 参数的构造函数，确保位置同步")]
        public DraggableTextBox()
        {
            InitializeTextBoxStyle();  // 复用初始化逻辑
        }
        private void InitializeTextBoxStyle()
        {
            this.Multiline = true;
            this.Height = 50;  // 主文本框高度
            this.BorderStyle = BorderStyle.FixedSingle;

            // 初始化上方名称显示框（仅输出）
            _nameLabel = new TextBox
            {
                ReadOnly = true,
                BackColor = SystemColors.Control,
                BorderStyle = BorderStyle.None,
                Text = "TextBoxName",  // 默认名称
                Height = 20
            };

            // 初始化下方输出显示框（仅输出）
            _outputLabel = new TextBox
            {
                ReadOnly = true,
                BackColor = SystemColors.Control,
                BorderStyle = BorderStyle.None,
                Text = "Output: ",  // 默认输出提示
                Height = 20
            };

            // 当主文本框所在容器变化时，同步添加附属控件
            this.ParentChanged += (s, e) =>
            {
                if (this.Parent != null)
                {
                    this.Parent.Controls.Add(_nameLabel);
                    this.Parent.Controls.Add(_outputLabel);
                    this.BringToFront();
                }
            };

            // 主文本框内容变化时，更新下方输出
            this.TextChanged += (s, e) =>
            {
                _outputLabel.Text = $"Output: {this.Text}";
            };

            UpdateLabelPositions();  // 初始位置同步
        }
        // 位置变更事件，确保附属框跟随移动
        protected override void OnLocationChanged(EventArgs e)
        {
            base.OnLocationChanged(e);
            UpdateLabelPositions(); // 同步上下显示框位置
            // 同步位置到计算站（判断Station不为空）
            if (this.Station != null)
            {
                this.Station.Position = this.Location; // 实时更新X,Y坐标
                Link.TriggerRefresh();
                // 可选：打印日志验证
                //gv.Message.Enqueue($"计算站 [{Station.SelfName}] 位置更新：({Location.X}, {Location.Y})");
            }
        }
        // 大小变更事件，确保附属框宽度同步
        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            UpdateLabelPositions();  // 大小变化时同步
        }
        // 拖动逻辑（保持原有功能）
        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                _isDragging = true;
                _offsetX = e.X;
                _offsetY = e.Y;
                this.Cursor = Cursors.Hand;
            }
            base.OnMouseDown(e);
        }
        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (_isDragging && this.Parent != null)
            {
                Point parentPos = this.Parent.PointToClient(Cursor.Position);
                int newX = parentPos.X - _offsetX;
                int newY = parentPos.Y - _offsetY;

                // 限制在父容器内
                newX = Math.Max(0, Math.Min(newX, Parent.ClientSize.Width - Width));
                newY = Math.Max(
                    _nameLabel.Height + 2,  // 确保上方名称框不超出父容器顶部
                    Math.Min(newY, Parent.ClientSize.Height - Height - _outputLabel.Height - 2)  // 确保下方输出框不超出底部
                );

                this.Location = new Point(newX, newY);
            }
            base.OnMouseMove(e);
        }
        protected override void OnMouseUp(MouseEventArgs e)
        {
            _isDragging = false;
            this.Cursor = Cursors.IBeam;
            base.OnMouseUp(e);
        }



        //以下对外提供访问方法和属性
        // 添加清空方法（仅清空主文本框和输出框）
        public void Clear()
        {
            this.Text = "";
            _outputLabel.Text = "Output: ";
        }

        // 提供名称文本的访问属性（可外部修改名称）
        public string BoxName
        {
            get => _nameLabel.Text;
            set => _nameLabel.Text = value;
        }
        public void SetOutputLabelText(double text)
        {
            if (_outputLabel != null)
            {
                _outputLabel.Text = text.ToString();
            }
        }
        // 接收计算站的构造函数，确保创建时即绑定并同步位置
        public DraggableTextBox(calculate station)
        {
            InitializeTextBoxStyle();
            this.Station = station;             // 绑定计算站
            this.Location = station.Position;  // 初始位置从计算站同步
            this.BoxName = station.SelfName;   // 名称从计算站同步
        }
    }
}

