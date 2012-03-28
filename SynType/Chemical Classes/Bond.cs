using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;


namespace SynType.Chemical_Classes
{
    public class Bond : ICloneable
    {
        //The atom at the pointed end of a wedge is node1
        Node node1, node2;
        //Bond type is 1,2,3 for sing, doub, trip, and 4 for aromatic, 5 for single or double
        //6 for single or aromatic, 7 for double or aromatic and 8 for any
        int bondOrder;
        //0 not stereo, 1 up, 3 either cis or trans DB, 5 any, 6 down, 0 use xyz coords to determine cis/trans
        int bondStereo;
        int bondTopology; //0 either, 1 ring and 2 chain
        int reactingCenterStatus; //To be added
        bool whichSide; //which side a double bond is on
        GraphicsPath boundPath;

        public Bond()
        {
            node1 = new Node();
            node2 = new Node();
        }
        public Bond(Node node1, Node node2)
        {
            this.node1 = node1;
            this.node2 = node2;
        }
        public Bond(Node node1, Node node2, int order)
        {
            this.node1 = node1;
            this.node2 = node2;
            this.bondOrder = order;
            this.whichSide = false;
           
        }

        public Node Node1 { get { return node1; } set { node1 = value; } }
        public Node Node2 { get { return node2; } set { node2 = value; } }
        public int BondOrder { get { return bondOrder; } set { bondOrder = value; } }
        public int BondStereo { get { return bondStereo; } set { bondStereo = value; } }
        public bool WhichSide { get { return whichSide; } set { whichSide = value; } }
        public GraphicsPath BoundPath { get { return boundPath; } }
        //This function calculates the box that encases the bond
        public void CalculateBoundPath()
        {
            //We need to have four points calculated for each of the four corners of the box
            //First we need the two vectors between the nodes, one for each direction
            PointF v1 = new PointF((float)(node2.Position.X - node1.Position.X), (float)(node2.Position.Y - node1.Position.Y));
            PointF v2 = new PointF((float)(node1.Position.X - node2.Position.X), (float)(node1.Position.Y - node2.Position.Y));
            //Now we need to get four normals, two from each position pointing in opposite directions, these are the four corners
            //we multiply by 5 so we get a box like structure
            //The matrix for rotating 90 degress is A = cos(90) -sin(90) => 0 -1 * x => (-y, x)
            //                                          sin(90) cos(90)     1  0 * y
            //and for the opposite direction B = cos(270) -sin(270) => 0 1 * x => (y, -x) 
            //                                   sin(270) cos(270 =>  -1 0 * y
            PointF n1_90 = new PointF(-v1.Y, v1.X);
            float l1_90 = (float)Math.Sqrt(Square(n1_90.X) + Square(n1_90.Y));
            n1_90 = new PointF(n1_90.X / l1_90 * 5 + (float) node1.Position.X, n1_90.Y / l1_90 * 5 + (float) node1.Position.Y);

            PointF n1_270 = new PointF(v1.Y, -v1.X);
            float l1_270 = (float)Math.Sqrt(Square(n1_270.X) + Square(n1_270.Y));
            n1_270 = new PointF(n1_270.X / l1_270 * 5 + (float) node1.Position.X, n1_270.Y / l1_270 * 5 + (float) node1.Position.Y);

            PointF n2_90 = new PointF(-v2.Y, v2.X);
            float l2_90 = (float)Math.Sqrt(Square(n2_90.X) + Square(n2_90.Y));
            n2_90 = new PointF(n2_90.X / l2_90 * 5 + (float) node2.Position.X, n2_90.Y / l2_90*5 + (float) node2.Position.Y );

            PointF n2_270 = new PointF(v2.Y, -v2.X);
            float l2_270 = (float)Math.Sqrt(Square(n2_270.X) + Square(n2_270.Y));
            n2_270 = new PointF(n2_270.X / l2_270 * 5 + (float) node2.Position.X, n2_270.Y / l2_270 * 5 + (float) node2.Position.Y);

            boundPath = new GraphicsPath();
            boundPath.StartFigure();
            boundPath.AddLine(n1_90, n2_270);
            boundPath.AddLine(n2_270, n2_90);
            boundPath.AddLine(n2_90, n1_270);
            boundPath.AddLine(n1_270, n1_90);
            boundPath.CloseFigure();
        }
        private double Square(double num) { return num * num; }
        public override string ToString()
        {
            return "Bond between atoms #" + node1.ID + " and #" + node2.ID + " of order " + bondOrder;
        }
        public object Clone()
        {
            Bond bondCopy = new Bond((Node)node1.Clone(), (Node)node2.Clone(), this.bondOrder);
            return bondCopy;
        }
    }
}
