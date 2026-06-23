using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using System.Windows.Forms;

namespace BASS
{
    public class gv
    {
        public static ConcurrentQueue<string> Message = new ConcurrentQueue<string>();
        public static bool IsMessageRunning=true;
        public static System.Windows.Forms.TextBox MessageBox { get; set; }
        public static int Form2_Width=800;
        public static int Form2_Height=700;
        public static int TextBoxCount = 1;
    }
}
