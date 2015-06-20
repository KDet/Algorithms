using System;

namespace GameAlgorithms
{
    public class Path : IEquatable<Path>
    {
        public int X { get; set; }
        public int Y { get; set; }
        public double Weight { get; set; }
        public Path Previous { get; set; }
        public double FromStart { get; set; }
        public double ToEnd { get; set; }
        public bool Checked { get; set; }

        public Path(int x, int y)
        {
            X = x;
            Y = y;
        }

        public static bool IsNotZero(Path direction)
        {
            return direction.X != 0 && direction.Y != 0;
        }

        bool IEquatable<Path>.Equals(Path other)
        {
            return Equals(other);
        }

        public bool Equals(Path other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Y == other.Y && X == other.X;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((Path) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Y*397) ^ X;
            }
        }

        public static bool operator ==(Path left, Path right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Path left, Path right)
        {
            return !Equals(left, right);
        }

        public static Path GetDirection(Path from, Path to)
        {
            int dx = (to.X - from.X) / Math.Max(Math.Abs(to.X - from.X), 1);
            int dy = (to.Y - from.Y) / Math.Max(Math.Abs(to.Y - from.Y), 1);
            return new Path(dx, dy);
        }

        public override string ToString()
        {
            return string.Format("X: {1}, Y: {0}", Y, X);
        }
    }
}
