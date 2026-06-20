using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace K11
{
    internal class Program
    {
        static void Main(string[] args)
        {

            //隐式声明变量
            int x = 100;//显式声明变量x
            var y = 100D;//隐式声明变量x
            var z = "Hello";//隐式声明变量z
            Console.WriteLine(z.GetType().Name);

            //对form属性赋值
            Form myForm = new Form()
            {
                Text = "Hello",
                FormBorderStyle = FormBorderStyle.SizableToolWindow
            };//在新建的时候直接赋值标题属性
            //myForm.Text = "使用var关键字声明变量";
            //myForm.ShowDialog();


            //可以直接使用.进行访问。但是不能不赋值会被垃圾收集器回收,需要使用初始化器
            //new Form() { }.ShowDialog();
            //new Form() {Text = "heollo" }.ShowDialog();


            //为了统一基本类型使用的体验，可以使用语法糖影藏new关键字
            int xz = 100;
            string names = "Tim";
            int[] myarr = { 1, 2, 3, 4, 5 };//隐式声明数组可以省略new关键字


            //体现了var的优势，可以隐式声明匿名类型，隐式类型只能使用var来声明
            Form myForm1 = new Form() { Text = "Hello" };
            var person = new { Name = "Mr.oka", Age = 34 };
            Console.WriteLine($"Name:{person.Name},Age:{person.Age}");


            //不能滥用var
            //var n;//错误，隐式声明变量必须初始化
            //var m = null;//错误，隐式声明变量必须有确定的类型


            ///new关键字的多种用法
            student stu = new student();
            //stu.Report();
            CsStudent csstu = new CsStudent();
            csstu.Report();


            ///uint的用法
            uint a = uint.MaxValue;
            Console.WriteLine($"uint的最大值是{a}");
            string str = Convert.ToString(a);
            Console.WriteLine($"uint的最大值是{str}");

            ///checked操作符和unchecked操作符
            //溢出检查处理
            //try
            //{
            //    uint b = checked(a + 1);
            //    Console.WriteLine(y);
            //}
            //catch (OverflowException ex)
            //{
            //    Console.WriteLine("发生了溢出异常");
            //}


            //checked
            //{
            //    //在checked代码块中的所有算术运算都会进行溢出检查
            //    uint c = a + 1;
            //}
            //unchecked
            //{
            //    //在unchecked代码块中的所有算术运算都不会进行溢出检查
            //    uint d = a + 1;
            //}

            ///delegate的用法
            //详见K11-2项目

            ///sizeof操作符的用法
            int p = sizeof(int);
            Console.WriteLine($"int类型的大小是{p}字节");
            unsafe
            {
                int q = sizeof(stu);
                Console.WriteLine($"stu结构体的大小是{q}字节");
            }

            ///->的用法
            unsafe
            {
                student Stu1;
                stu.id = 1001;
                stu.score = 98;
                student* pStu1 = &Stu1;
                pStu1->score = 1002;
                (*pStu1).score = 99;
                Console.WriteLine(stu.score);
            }



            ///一元运算符的用法
            ///+-!~++--*&

            //&是取地址符号变量到达地址
            //*是指针运算符地址到达的变量
            //+的用法
            int num1 = 100;
            int num2 = +num1;
            Console.WriteLine($"num2={num2}");
            //-的用法
            int num3 = -num1;
            Console.WriteLine($"num3={num3}");
            //不能连续使用两个一元运算符
            //int num4 = --num1;//错误，不能连续使用两个一元运算符
            //如果要使用负负得正，可以使用括号
            int num4 = -(-num1);
            Console.WriteLine($"num4={num4}");
            //-的用法可能会导致溢出
            Console.WriteLine(int.MaxValue);
            Console.WriteLine(int.MinValue);//溢出

            //所以可以使用checked来检查溢出
            //可以使用求反运算符~来得到负数的补码表示
            int x3 = int.MinValue;
            int y3 = ~x3;

            Console.WriteLine($"num5={y3}");
            string xstr= Convert.ToString(x3, 2).PadLeft(32, '0');
            string ystr= Convert.ToString(y3, 2).PadLeft(32, '0');
            Console.WriteLine($"x3的二进制表示是{xstr}");
            Console.WriteLine($"y3的二进制表示是{ystr}");

            //取非运算符!
            bool b1 = true;
            bool b2 = !b1;
            Console.WriteLine(b2);

            //自增运算符++
            int a1 = 10;
            int a2 = ++a1;//先自增后赋值
            int a3 = a1++;//先赋值后自增
            Console.WriteLine($"a1={a1},a2={a2},a3={a3}");//有赋值的地方才会有区别
            a1++;
            ++a1;
            Console.WriteLine($"a1={a1}");//单独使用自增运算符没有区别
            //--自减运算符同理
            int n1= 10;
            int n2= --n1;//先自减后赋值
            int n3= n1--;//先赋值后自减
            Console.WriteLine($"n1={n1},n2={n2},n3={n3}");//有赋值的地方才会有区别
        }
        struct student
        {
        public int id;
        public long score;
        }
        struct stu
        {
            int id;
            long score;
        }
    }


    //new关键字的用法的类示例
    class student
        {
            public void Report()
            {
                Console.WriteLine("I am a student");
            }
            public string Name { get; set; }
            public int Age { get; set; }
        }
        class CsStudent : student
        {
            new public void Report()
            {
                Console.WriteLine("I am a computer student");
            }
        }

}

