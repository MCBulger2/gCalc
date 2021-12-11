using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gCalc
{
    public class PointRange
    {
        public double Start { get; set; }
        public double End { get; set; }

        public double Difference
        {
            get { return End - Start; }
        }


        public PointRange(double start, double end)
        {
            Start = start;
            End = end;
        }
    }
}
