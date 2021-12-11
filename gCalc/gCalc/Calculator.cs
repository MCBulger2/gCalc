using NCalc2;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace gCalc
{
    internal class Calculator
    {
        public delegate void ComputationEvent(int id);
        public event ComputationEvent ComputationCompleted;
        public event ComputationEvent InvalidFunction;

        Function Function;
        Expression Expression;
        public Semaphore ResultInvalid;
        public Semaphore FunctionInvalid;
        int Id;
        static int count = 0;
        public bool IsCalculating = false;
        bool IsWaitingForValidFunction = true;

        int MinResolution = 10;
        int MaxResolution = 1000;
        int Resolution = 1000;


        public Calculator(Expression expression, int id)
        {
            Function = Functions.CustomFunction;
            Expression = expression;
            ResultInvalid = new Semaphore(1, 1);
            FunctionInvalid = new Semaphore(1, 1);
            Id = id;
        }

        public void Run()
        {
            lock (Buffer.Functions)
            {
                Buffer.Functions.Add(new FunctionPoints());
            }

            while (true)
            {
                //bool valid = false;
                //lock(Buffer.Functions)
                //{
                //    valid = Buffer.Functions[Id].IsValid;
                //}

                //if (!valid)
                //{
                //    Resolution = MinResolution;
                //}
                
                IsCalculating = false;
                ResultInvalid.WaitOne();
                IsCalculating = true;
                //valid.WaitOne();


                if (true)
                {
                    Window window;
                    lock (Buffer.Window)
                    {
                        window = Buffer.Window;
                    }
                    double[] scale;
                    lock (Buffer.Scale)
                    {
                        scale = Buffer.Scale;
                    }

                    CalculateFunction(window, scale);
                }
            }
        }

        public void CalculateFunction(Window window, double[] scale)
        {
            double domainSize = window.Domain.Difference * 2;
            int steps = Resolution;
            // Resolution *= 2;
            // idea - start with steps being small, and the more times we recalculate with the same window and scale, recalculate with higher resolution
            double stepSize = domainSize / steps;

            List<FPoint> newPoints = new List<FPoint>();
            double x = window.Domain.Start - (domainSize / 2);
            while (x < (window.Domain.End + (domainSize / 2)))
            {
                if (Function(Expression, x, out double y))
                {
                    if (!double.IsNaN(y) && y < window.Range.End + 500 && y > window.Range.Start - 500)
                    {
                        newPoints.Add(new FPoint(x, y));
                    }
                }
                else
                {
                    InvalidFunction?.Invoke(Id);
                    IsWaitingForValidFunction = true;
                    FunctionInvalid.WaitOne();
                    IsWaitingForValidFunction = false;
                    CalculateFunction(window, scale);
                    return;
                }

                x += stepSize;
            }

            ComputationCompleted?.Invoke(Id);
            lock (Buffer.Functions)
            {
                Buffer.Functions[Id].Points = newPoints.ToArray();
                Buffer.Functions[Id].IsValid = true;
            }
            // Potentially have a callback that passes the points back or at least validation that the points are ready to draw to the GUI, because right now the canvas is getting drawn before the points are completed
            count++;

//            FunctionInvalid.Release();
        }

        public double CalculatePoint(double x)
        {
            if (!IsWaitingForValidFunction && Function(Expression, x, out double y))
            {
                return y;
            }

            return double.NaN;
        }

        public void SetExpression(Expression e)
        {
            Expression = e;
            if (IsWaitingForValidFunction)
            {
                FunctionInvalid.Release();
            }
        }
    }
}
