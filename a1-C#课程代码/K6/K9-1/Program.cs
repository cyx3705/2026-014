using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Text;
using System.Threading.Tasks;

namespace K9_1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //Student stu=new Student();
            //Console.WriteLine(stu.Name == null);
            //Console.WriteLine(stu.ID);
            //Console.WriteLine(stu.Name);
            //Console.WriteLine();
            js c = new js();
            double x = c.add(100D,100D);
            Console.WriteLine(x);
        }
    }
    class Student
    {
        public Student()
        {
            this.ID = 1;
            this.Name ="name";

        }
        //public Student(int id,string name)
        //{
        //    this.ID = id;
        //    this.Name = name;
        //}
        public int ID;
        public string Name;
        
    }
    class js
    {
        public int add (int a, int b)
        {
            return a + b;
        }
        public double add (double x,double b)
        {
            return b + x;

        }
        public double add(int a,int b, int c)
        {
            return a + b + c;
        }
        public int add<T>(int a,int b)
        {
            T t;
            return a + b;
        }
        public int add<T>(ref int a, int b)
        {
            T t;
            return a + b;
        }
        //public int add<T>(out int a, int b)
        //{
        //    T t;
        //    return a + b;
        //}


    }
}
