using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gCalc
{
    static class Buffer
    {
        public static List<FunctionPoints> Functions = new List<FunctionPoints>();
        public static Window Window = new Window(-2, 2, -2, 2);
        public static double[] Scale = { 50,50 };

        public static Window GetWindow()
        {
            Window window;
            lock (Window)
            {
                window = Window;
            }
            return window;
        }

        public static double[] GetScale()
        {
            double[] scale;
            lock (Scale)
            {
                scale = Scale;
            }
            return scale;
        }

        public static List<FunctionPoints> GetFunctions()
        {
            List<FunctionPoints> points;
            lock (Functions)
            {
                points = Functions;
            }
            return points;
        }
    }
}
