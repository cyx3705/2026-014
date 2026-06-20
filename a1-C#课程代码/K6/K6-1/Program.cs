using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace K6_1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            short s;
            s = -1000;
            string str = Convert.ToString(s, 2);
            Console.WriteLine(str);
            student stu;
            stu = new student();
            int a;
            //Console.WriteLine(a);不允许使用没有赋值的本地变量
            //const int x = 1;
            //x = 200;
            //Console.WriteLine(x);
        }
    }

    class student
    {
        public uint ID;
        public ushort Score;

    }
}
