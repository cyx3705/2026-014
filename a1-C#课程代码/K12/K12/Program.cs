using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace K12
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //类型转换
            //string str1=Console.ReadLine();
            //string str2=Console.ReadLine();
            //int x=Convert.ToInt32(str1);
            //int y=Convert.ToInt32(str2);
            //Console.WriteLine(x+y);

            //隐式转换
            //int x1 = int.MaxValue;
            //long y1 = x;
            //Console.WriteLine(y1);

            //显式转换
            //Convert.ToInt32(x1);//几乎可以把其他所有类型转换为int类型
            stone stone=new stone();
            stone.age = 5000;
            Monkey m=(Monkey)stone;//引用类型之间的转换
            Console.WriteLine(m.age);


            //类型推断
            var x = 3.0 * 4.0;
            Console.WriteLine(x.GetType().FullName);//System.Double
            Console.WriteLine(x);

            //数值提升（不丢失精度）
            var x2 = 3.0 * 4;
            Console.WriteLine(x2.GetType().FullName);//System.Double
            Console.WriteLine(x2);


            //除法操作符号
            int x3=5;
            int x4=0;
            //int z = x3 / x4; 不能除以0
            //Console.WriteLine(z);//0

            double z2 = x3 / x4;//可以除以0
            Console.WriteLine(z2);//0

            double a = (double)(5 / 4);//括号内先算5/4，结果是int类型1，再转换为double类型1.0
            Console.WriteLine(a);//1.25




        }


    }
    class stone
    {
        public int age;

        public static explicit operator Monkey(stone s)
        {
            Monkey m=new Monkey();
            m.age=s.age/500;
            return m;
        }

    }
    class Monkey
    {
        public int age;
    }
}

