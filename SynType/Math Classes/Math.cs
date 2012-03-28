using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace SynType.Math_Classes
{
    public struct Vector3
    {
        private double x, y, z;
        public double X { get { return x; } set { x = value; } }
        public double Y { get { return y; } set { y = value; } }
        public double Z { get { return z; } set { z = value; } }

        //Generates a new vector for 2 points a-b
        public Vector3(PointF a, PointF b)
        {
            this.x = b.X - a.X;
            this.y = b.Y - a.Y;
            this.z = 0;

        }
        public Vector3(double x, double y, double z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }
        public override string ToString()
        {
            if (z == 0)
                return "x=" + x + " y=" + y;
            else
                return "x=" + x + " y=" + y + " z=" + z;
        }
        public double[] Array
        {
            get { return new double[] { x, y, z }; }
            set
            {
                if (value.Length == 3) //do we have 3 parts?
                {
                    x = value[0];
                    y = value[1];
                    z = value[2];
                }
                else
                {
                    throw new ArgumentException(THREE_COMPONENTS);
                }
            }
        }
        public double this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0: { return X; }
                    case 1: { return Y; }
                    case 2: { return Z; }
                    default: throw new ArgumentException(THREE_COMPONENTS, "index");
                }
            }
            set
            {
                switch (index)
                {
                    case 0: { X = value; break; }
                    case 1: { Y = value; break; }
                    case 2: { Z = value; break; }
                    default: throw new ArgumentException(THREE_COMPONENTS, "index");
                }
            }
        }
        public double Length
        {
            get
            {
                return Math.Sqrt(x * x + y * y + z * z);
            }
        }
        public void Normalize()
        {
            x /= Length;
            y /= Length;
            z /= Length;
        }
        //Calculate the dot product between this vector and another vector v
        public double DotProduct(Vector3 v)
        {
            return (this.x * v.X + this.y * v.Y + this.z * v.Z);
        }
        public double AngleBetween(Vector3 v)
        {
            return Math.Acos(DotProduct(v));
        }
        //Gives the angle telling us is v is ahead or behind this vector
        public double AngleAhead(Vector3 v)
        {
            return (Math.Atan2(v.Y, v.X) - Math.Atan2(this.y, this.x));
        }
        public static double DotProduct(Vector3 v1, Vector3 v2)
        {
            return (v1.X * v2.X + v1.Y * v2.Y + v1.Z * v2.Z);
        }
        public static double AngleBetween(Vector3 v1, Vector3 v2)
        {
            return Math.Acos(DotProduct(v1, v2));
        }
        public static double AngleAhead(Vector3 v1, Vector3 v2)
        {
            return (Math.Atan2(v2.Y, v2.X) - Math.Atan2(v1.Y, v1.X));
        }
        public static Vector3 operator *(Vector3 v, float scale)
        {
            return new Vector3(v.X * scale, v.Y * scale, v.Z * scale);
        }
        public static Vector3 operator +(Vector3 v1, Vector3 v2)
        {
            return new Vector3(v1.X + v2.X, v1.Y + v2.Y, v1.Z + v2.Z);
        }
        public static Vector3 operator -(Vector3 v1, Vector3 v2)
        {
            return new Vector3(v2.X - v1.X, v2.Y - v1.Y, v2.Z - v1.Z);
        }
        private const string THREE_COMPONENTS = "Array must contain exactly 3 components, (x,y,z)";
    }
}
