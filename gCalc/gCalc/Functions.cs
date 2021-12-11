using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NCalc2;

namespace gCalc
{
    //delegate double Function(Expression e, double x);
    delegate bool Function(Expression e, double x, out double y);

    internal class Functions
    {
        //static Expression e = new Expression("[x]*[x]*[x]");
        //public static double ConstantFunction(double x)
        //{
        //    return 0.5;
        //}

        //public static double LinearFunction(double x)
        //{
        //    return x;
        //}

        //public static double QuadraticFunction(double x)
        //{
        //    return Math.Pow(x, 2);
        //}

        //public static double Sin(double x)
        //{
        //    return Math.Sin(x) ;
        //}

        public static bool CustomFunction(Expression expression, double x, out double y)
        {
            try
            {
                if (expression != null)
                {
                    expression.Parameters["x"] = x;
                    var val = expression.Evaluate(); // this is super super slow, maybe try the async version??
                    if (double.TryParse(val.ToString(), out double result))
                    {
                        y = result;
                        return true;
                    }
                }
            }
            catch (ArgumentException)
            {
                y = double.NaN;
                return false;
            }

            y = double.NaN;
            return false;
        }
    }
}
