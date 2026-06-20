using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace K8_2

{
    class Program 
    { static void Main(string[] args) 
        { 
            double result = Calculator.GetConeVolume(100, 100); 
        } 
    }
    class Calculator
    {
        public static double GetCircleArea(double r) 
        { 
            return Math.PI * r * r; 
        }
        public static double GetCylinderVolume(double r, double h)
        { 
            double a = GetCircleArea(r); 
            return a * h; 
        }
        public static double GetConeVolume(double r, double h) 
        { 
            double cv = GetCylinderVolume(r, h); 
            return cv / 3; 
        }
    }
}
