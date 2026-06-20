using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace K10
{
    internal class Program
    {
       
        static void Main(string[] args)
        {
            //int x = 5;
            //int y = 10;
            //int z = x + y;
            //Console.WriteLine(z);

            //double a = 5.5;
            //double b = 10.2;
            //Console.WriteLine(a + b);


            //int x = 100;
            //int y= 200;
            //int z = 300;
            //x += y += z;
            //Console.WriteLine(x);
            //Console.WriteLine(y);
            //Console.WriteLine(z);


            //System.IO.File.Create("C:\\HelloWorld.text");
            //Person p1 = new Person();
            //Person p2 = new Person();
            //p1.Name = "Tom";
            //p2.Name = "Mary";
            //List<Person> nation = p1+p2;
            //foreach(var p in nation)
            //{
            //    Console.WriteLine(p.Name);
            //}
            //calculator c = new calculator();
            //Action myaction = new Action(c.printhello);
            //myaction();


            //元素访问操作符[]的基本用法：数组Array
            int[] myintArray = new int[5] { 1, 2, 3, 4, 5 };//初始化器（长度严格）
            Console.WriteLine(myintArray[0]);
            Console.WriteLine(myintArray[myintArray.Length - 1]);
            foreach (var i in myintArray)
            {
                Console.WriteLine(i);
            }


            //元素访问操作符[]的另一种用法：字典Dictionary
            Dictionary<string,Student>stuDIC= new Dictionary<string, Student>();
            for(int i=0;i<5;i++)
            {
                Student stu = new Student();
                stu.Name = "s_"+i.ToString();
                stu.score = 20 + i;
                stuDIC.Add(stu.Name, stu);
            }
            Student number6 = stuDIC["s_3"];
            Console.WriteLine(number6.score);


            //x自加会改变x的值，而y只是赋值了x自加前的值
            int x = 100;
            int y = x++;
            Console.WriteLine(y);
            x = x + 1;
            Console.WriteLine(x);


            //前置++会先自加再赋值，后置++会先赋值再自加

            //Metadata
            //可以查看类型的完整名称、命名空间、方法等信息
            Type t = typeof(int);
            Console.WriteLine(t.FullName);
            Console.WriteLine(t.Namespace);
            Console.WriteLine(t.Name);
            int c = t.GetMethods().Length;
            foreach (var m in t.GetMethods())
            {
                Console.WriteLine(m.Name);
            }
            Console.WriteLine(c);

            //default关键字的用法是返回类型的默认值
            double u = default(double);
            Console.WriteLine(u);
            //引用类型的默认值是null
            level level = default(level);
            Console.WriteLine(level);
        }
    }

    enum level
    {
        low = 1,
        mid = 3,
        high = 2
    }

    class Student
    {
        public string Name;
        public int score;
    }
    class calculator
    {
        public void printhello()
        {
            Console.WriteLine("hello from calculator");
        }
        public calculator()
        {
            Console.WriteLine("calculator created");
        }
    }
    class Person
    {
        public string Name;
        public static List<Person> operator +(Person p1, Person p2)
        {
            List<Person> people = new List<Person>();
            people.Add(p1);
            people.Add(p2);
            for(int i=0;i<11;i++)
            {
                Person child = new Person();
                child.Name = p1.Name +"&"+ p2.Name + "s child"+i;
                people.Add(child);
            }
            return people;
        }
    }
}
