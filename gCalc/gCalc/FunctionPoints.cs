using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gCalc
{
    public class FunctionPoints
    {
        public FPoint[] Points { get; set; }
        public bool IsValid { get; set; }
        public bool IsPainted { get; set; }

        public FunctionPoints()
        {
            IsValid = false;
            IsPainted = false;
        }
    }
}
