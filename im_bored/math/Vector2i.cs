using SFML.System;

namespace im_bored.math{
    public struct Vector2i(int x = 0, int y = 0)
    {
        public int X { get; set; } = x;
        public int Y { get; set; } = y;
        public readonly float Magnitude{get{return MathF.Sqrt(X*X + Y*Y);}}
        public readonly Vector2f Normalized{get{return new(X/Magnitude,Y/Magnitude);}}
        public static Vector2i operator +(Vector2i a, Vector2i b){
            return new(a.X + b.X, a.Y + b.Y);
        }
        public static Vector2i operator -(Vector2i a, Vector2i b){
            return new(a.X - b.X, a.Y - b.Y);
        }
        public static Vector2i operator -(Vector2i a){
            return new(-a.X, -a.Y);
        }
        public static bool operator ==(Vector2i a, Vector2i b){
            return a.X == b.X && a.Y == b.Y;
        }
        public static bool operator !=(Vector2i a, Vector2i b){
            return !(a==b);
        }

        public static explicit operator SFML.System.Vector2f(Vector2i v)
        {
            return new(v.X,v.Y);
        }

        public static explicit operator Vector2i(SFML.System.Vector2f v)
        {
            return new((int)v.X,(int)v.Y);
        }

        public override readonly bool Equals(object? obj)
        {
            if (obj is Vector2i v)
            {
                return this == v;
            }
            return false;
        }
        public override readonly int GetHashCode()
        {
            return X.GetHashCode() ^ Y.GetHashCode();
        }
        public override readonly string ToString()
        {
            return $"{X};{Y}";
        }
    }
}