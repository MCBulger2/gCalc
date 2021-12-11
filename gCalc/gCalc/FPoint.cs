using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gCalc
{
    public class FPoint : IEquatable<FPoint>
    {
        public double X { get; set; }
        public double Y { get; set; }

        public FPoint(double x, double y)
        {
            X = x;
            Y = y;
        }

        public override string ToString()
        {
            return $"({X}, {Y})";
        }

        public bool Equals(FPoint other)
        {
            if (ReferenceEquals(this, other))
            {
                return true;
            }
            if (other is null)
            {
                return false;
            }
            return X == other.X && Y == other.Y;
        }

        public static bool operator ==(FPoint lhs, FPoint rhs)
        {
            if (lhs is null)
            {
                if (rhs is null)
                {
                    return true;
                }

                // Only the left side is null.
                return false;
            }
            // Equals handles case of null on right side.
            return lhs.Equals(rhs);
        }

        public static bool operator !=(FPoint lhs, FPoint rhs) => !(lhs == rhs);
    }
}
