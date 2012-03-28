using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace SynType.Math_Classes
{
    public struct BoundingBox
    {
        private float width;
        private float height;
        private PointF center;
        private PointF upperLeftCorner;
        private float rotation;

        public float Width { get { return width; } set { width = value; } }
        public float Height { get { return height; } set { height = value; } }
        public PointF Center { get { return center; } set { center = value; } }
        public PointF UpperLeftCorner { get { return upperLeftCorner; } }
        public float Rotation { get { return rotation; } set { rotation = value; } }
        public BoundingBox(float width, float height, PointF center, PointF upperLeft, float rotation)
        {
            this.width = width;
            this.height = height;
            this.center = center;

            this.upperLeftCorner = upperLeft;
            this.rotation = rotation;
        }
    }
}
