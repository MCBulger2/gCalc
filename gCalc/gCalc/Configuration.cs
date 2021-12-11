using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gCalc
{
    internal class Configuration
    {
        public Dictionary<string, Pen> Colors { get; set; }
        public Font Font { get; set; }

        public Configuration()
        {
            Colors = new Dictionary<string, Pen>
            {
                { "Axes", Pens.Green },
                { "Grid", Pens.Black },
                { "Subgrid", Pens.Gray },
                { "Function0", new Pen(Brushes.Red, 3) },
                { "Function1", new Pen(Brushes.Blue, 3) },
                { "Function2", new Pen(Brushes.Green, 3) },
                { "Function3", new Pen(Brushes.Purple, 3) },
                { "Function4", new Pen(Brushes.LightBlue, 3) },
                { "Function5", new Pen(Brushes.Pink, 3) }
            };
            Font = new Font("Calibri", 8,
                FontStyle.Regular, GraphicsUnit.Point);
        }
    }
}
