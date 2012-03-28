using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SynType.Chemical_Classes;
using SynType.Math_Classes;
using System.Drawing.Drawing2D;

namespace SynType
{
   
    public partial class MolViewer : UserControl
    {
       
        private Molecule mol;
        private Molecule molRawCopy;

        public Molecule Mol { get { return mol; } 
            set 
            { 
                mol = value;
                //We make a raw copy of the molecule for use in resizing molecules
                if(value != null)
                    molRawCopy = (Molecule)value.Clone();
                //Then we call normalize coords so everything looks right
                NormalizeCoords();
            } }
     
        private BoundingBox bounds;
        private bool structureSelected = false;

        private PointF center = new PointF(130, 130);
        public MolViewer()
        {
            InitializeComponent();
            this.MouseWheel +=new MouseEventHandler(MolViewer_MouseWheel);
 
        }
        
        private void MolViewer_Load(object sender, EventArgs e)
        {

        }
        public void paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.Clear(Color.White);
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            if (mol == null) return;
           
            

            //First we draw bonds
            foreach (Bond bond in mol.Bonds)
            {
                PointF p1 = new PointF((float)bond.Node1.Position.X,(float) bond.Node1.Position.Y);
                PointF p2 = new PointF((float)bond.Node2.Position.X, (float) bond.Node2.Position.Y);
               
                DrawBonds(g, p1, p2, bond.BondOrder, bond.WhichSide);
         
              
            }
            //Then we draw symbols
            foreach (Node atom in mol.Atoms.Values)
            {
                if (atom.Symbol != "C")
                {
                    //First we draw a white circle with radius 10 at the atom location
                    g.FillEllipse(Brushes.White, (float)atom.Position.X - 10, (float)atom.Position.Y - 10, 20, 20);
                    //Then we draw the symbol
                    string symbol = atom.Symbol;
                    if (symbol == "S" || symbol == "O" || symbol == "N")
                    {
                        if (atom.ImplicitHydrogens == 0)
                        {
                            g.DrawString(symbol, new Font("Times New Roman", 15), Brushes.Black, (float)atom.Position.X - 9, (float)atom.Position.Y - 12);
                            continue;
                        }
                        else if (atom.ImplicitHydrogens == 1)
                        {
                            symbol += "H";
                            g.DrawString(symbol, new Font("Times New Roman", 15), Brushes.Black, (float)atom.Position.X - 9, (float)atom.Position.Y - 12);
                            continue;
                        }
                        else
                        {
                            //If there are more than one hydrogen we have to make a sub script
                            Font normalFont = new Font("Times New Roman", 15);
                            Font subscriptFont = new Font("Times New Roman", 10);
                            symbol += "H";
                            g.DrawString(symbol, normalFont, Brushes.Black, (float)atom.Position.X - 9, (float)atom.Position.Y - 12);
                            g.DrawString(atom.ImplicitHydrogens.ToString(), subscriptFont, Brushes.Black, (float)atom.Position.X + 20, (float)(atom.Position.Y + 5));
                            continue;
                        }
                    }
                    g.DrawString(symbol, new Font("Times New Roman", 15), Brushes.Black, (float)atom.Position.X - 9, (float)atom.Position.Y - 12);
                    
                    
                }
            }
            //Draw the bounding box
            if (structureSelected)
            {
                g.DrawRectangle(new Pen(Brushes.GreenYellow), bounds.UpperLeftCorner.X - 10, bounds.UpperLeftCorner.Y - 10, bounds.Width + 20, bounds.Height + 20);
                
                
                
                 
            }
        }
        private List<PointF> FindIntersection(PointF p1, PointF p2, PointF circ, double r)
        {


            //line
            //first check if the nom/denom is zero, then we have a line at x = whatever
            double m;
            if (p2.X - p1.X != 0 && p2.Y - p1.Y != 0)
            {
                m = (p2.Y - p1.Y) / (p2.X - p1.X);

                double h = p1.Y - m * p1.X;
                //We have for line-circle intersection (x-c)^2+(y-d)^2=r^2 and y=m*x+h circle center = (c,d)
                //ax^2+bx+c = 0 is the form
                //a = m^2+1
                double a = Square(m) + 1;
                //
                //intermediate
                double BD = h - circ.Y;
                double b = 2 * (m * BD - circ.X);
                //c
                double c = Square(circ.X) + Square(BD) - Square(r);

                //Solve quadratic equation, two solutions (-b+sqrt(b^2-4*a*c))/2*a and (-b-sqrt(b^2-4*a*c))/2*a
                double d = Math.Sqrt(Square(b) - 4 * a * c);
                double x1 = (-b + d) / (2 * a);
                double x2 = (-b - d) / (2 * a);

                double y1 = m * x1 + h;
                double y2 = m * x2 + h;


                //We then return the two points as a list
                List<PointF> points = new List<PointF>();
                points.Add(new PointF((float)x1, (float)y1));
                points.Add(new PointF((float)x2, (float)y2));

                return points;
            }
            else if (p2.X - p1.X == 0 && p2.Y - p1.Y != 0)
            {
                //If we have a zero denom that means we have a line x = #, so we have a vector
                double x = p2.X;
                //From circle equation we get y = sqrt(r^2-(x-c)^2)+-d
                double y1 = Math.Sqrt(Square(r) - Square(x - circ.X)) + circ.Y;
                double y2 = -Math.Sqrt(Square(r) - Square(x - circ.X)) + circ.Y;

                List<PointF> points = new List<PointF>();
                points.Add(new PointF((float)x, (float)y1));
                points.Add(new PointF((float)x, (float)y2));
                return points;

            }
            else if (p2.X - p1.X != 0 && p2.Y - p1.Y == 0)
            {
                //same y coords so we simply put the value in
                double y = p2.Y;

                double x1 = Math.Sqrt(Square(r) - Square(y - circ.Y)) + circ.X;
                double x2 = -Math.Sqrt(Square(r) - Square(y - circ.Y)) + circ.X;
                List<PointF> points = new List<PointF>();
                points.Add(new PointF((float)x1, (float)y));
                points.Add(new PointF((float)x2, (float)y));
                return points;
            }
            else
            {
                //If we get here the points 1 and 2 are the same
                List<PointF> points = new List<PointF>();
                points.Add(new PointF(p1.X, p1.Y));
                points.Add(new PointF(p1.X, p1.Y));
                return points;
            }

        }
        private PointF[] GetSingleCircleInersectionPoints(PointF point1, PointF point2, double r, int centerPoint)
        {
            if (centerPoint == 1)
            {
                List<PointF> set = FindIntersection(point1, point2, point1, r);
                double shortest = 0;
                //We find the shortest distance
                foreach (PointF p in set)
                {
                    PointF l = new PointF(point2.X - p.X, point2.Y - p.Y);
                    double distance = Math.Sqrt(Square(l.X) + Square(l.Y));
                    if (shortest == 0) shortest = distance;
                    if (distance < shortest && distance != 0) shortest = distance;
                }
                //Then we find it again and return those points;
                foreach (PointF p in set)
                {
                    PointF l = new PointF(point2.X - p.X, point2.Y - p.Y);
                    double distance = Math.Sqrt(Square(l.X) + Square(l.Y));
                    if (distance == shortest)
                    {
                        PointF[] returnList = new PointF[2];
                        returnList[0] = p; returnList[1] = point2;
                        return returnList;
                    }
                }
            }
            else if (centerPoint == 2)
            {
                List<PointF> set = FindIntersection(point1, point2, point2, r);
                double shortest = 0;
                //We find the shortest distance
                foreach (PointF p in set)
                {
                    PointF l = new PointF(point1.X - p.X, point1.Y - p.Y);
                    double distance = Math.Sqrt(Square(l.X) + Square(l.Y));
                    if (shortest == 0) shortest = distance;
                    if (distance < shortest && distance != 0) shortest = distance;
                }
                //Then we find it again and return those points;
                foreach (PointF p in set)
                {
                    PointF l = new PointF(point1.X - p.X, point1.Y - p.Y);
                    double distance = Math.Sqrt(Square(l.X) + Square(l.Y));
                    if (distance == shortest)
                    {
                        PointF[] returnList = new PointF[2];
                        returnList[0] = p; returnList[1] = point1;
                        return returnList;
                    }
                }
            }
            return null;

        }
        private PointF[] GetTwoCircleIntersectionPoints(PointF point1, PointF point2, double r)
        {
            //We get to lists, we then compare all the distances and find the line with the shortest distance, and return those two points
            List<PointF> set1 = FindIntersection(point1, point2, point1, r);
            List<PointF> set2 = FindIntersection(point1, point2, point2, r);

            double shortest = 0;
            foreach (PointF p in set1)
            {
                foreach (PointF q in set2)
                {
                    PointF l = new PointF(q.X - p.X, q.Y - p.Y);
                    double distance = Math.Sqrt(Square(l.X) + Square(l.Y));
                    if (shortest == 0) shortest = distance;
                    if (distance < shortest && distance != 0) shortest = distance;
                }
            }
            //then we go through and return the two points when we find this distance again
            foreach (PointF p in set1)
            {
                foreach (PointF q in set2)
                {
                    PointF l = new PointF(q.X - p.X, q.Y - p.Y);
                    double distance = Math.Sqrt(Square(l.X) + Square(l.Y));
                    if (distance == shortest)
                    {
                        PointF[] returnList = new PointF[2];
                        returnList[0] = p; returnList[1] = q;
                        return returnList;
                    }
                }
            }
            return null;
        }
     
        public static double Square(double num) { return num * num; }
        private void DrawBonds(Graphics g, PointF point1, PointF point2, int order, bool whichSide)
        {
            switch (order)
            {
                case (int)BondOrder.Single:
                  
                    PointF[] points = new PointF[2] { point1, point2 };
   
                    g.DrawLines(new Pen(Brushes.Black), points);
             
                    break;
                case (int) BondOrder.Double:
                    DrawDoubleBond(g, point1, point2, whichSide);
                    break;
                case (int) BondOrder.Triple:
                    DrawTripleBond(g, point1, point2);
                    break;
            }
        }
        private void DrawDoubleBond(Graphics g, PointF point1, PointF point2, bool whichSide)
        {
            
            //First we get the vector between the two points
            PointF v;
            if (whichSide)
            {
                v = new PointF(point2.X - point1.X, point2.Y - point1.Y); ;
            }
            else
            {
                v = new PointF(point1.X - point2.X, point1.Y - point2.Y);
            }
            //The matrix for rotating 90 degress is A = cos(90) -sin(90) => 0 -1 * x
            //                                          sin(90) cos(90)     1  0 * y
            PointF n = new PointF(-v.Y, v.X);
            float l = (float) Math.Sqrt(Square(n.X) + Square(n.Y));
            n = new PointF((float)n.X / l*5, (float) n.Y / l*5); //normalize vector
            //Now add the normalized vector the start and end points
            PointF dbStart;
            PointF dbEnd;
            if (whichSide)
            {
                dbStart = new PointF(v.X * .1f + n.X + point1.X, v.Y * .1f + n.Y + point1.Y);
                dbEnd = new PointF(v.X * -.1f + n.X + point2.X, v.Y * -.1f + n.Y + point2.Y);
            }
            else
            {
                dbStart = new PointF(v.X * -.1f + n.X + point1.X, v.Y * -.1f + n.Y + point1.Y);
                dbEnd = new PointF(v.X * .1f + n.X + point2.X, v.Y * .1f + n.Y + point2.Y);
            }

           
            g.DrawLine(new Pen(Brushes.Black), point1, point2);


            g.DrawLine(new Pen(Brushes.Black), dbStart, dbEnd);
            
        }
        private void DrawTripleBond(Graphics g, PointF point1, PointF point2)
        {
            //A triple bond is just a double bond drawn from each direction
            PointF v1, v2;
            v1 = new PointF(point2.X - point1.X, point2.Y - point1.Y);
            v2 = new PointF(point1.X - point2.X, point1.Y - point2.Y);

            //Now we get the rotated vector, the normal of the bond
            PointF n1 = new PointF(-v1.Y, v1.X);
            PointF n2 = new PointF(-v2.Y, v2.X);
            //Now we normalize the vectors and extend by 5
            float l1 = (float) Math.Sqrt(Square(n1.X)+Square(n1.Y));
            float l2 = (float) Math.Sqrt(Square(n2.X)+Square(n2.Y));
            n1 = new PointF((float)n1.X / l1 * 5, (float)n1.Y / l1 * 5);
            n2 = new PointF((float)n2.X / l2 * 5, (float)n2.Y / l2 * 5);
            //Then we get the points for each double bond to be drawn
            PointF dbStart1 = new PointF(v1.X * .1f + n1.X + point1.X, v1.Y * .1f + n1.Y + point1.Y);
            PointF dbEnd1 = new PointF(v1.X * -.1f + n1.X + point2.X, v1.Y * -.1f + n1.Y + point2.Y);

            PointF dbStart2 = new PointF(v2.X * -.1f + n2.X + point1.X, v2.Y * -.1f + n2.Y + point1.Y);
            PointF dbEnd2 = new PointF(v2.X * .1f + n2.X + point2.X, v2.Y * .1f + n2.Y + point2.Y);

            g.DrawLine(new Pen(Brushes.Black), point1, point2);
            g.DrawLine(new Pen(Brushes.Black), dbStart1, dbEnd1);
            g.DrawLine(new Pen(Brushes.Black), dbStart2, dbEnd2);
        }
        private BoundingBox GetBoundingBox(Molecule molecule)
        {
 
            //First we find the min/max in x and y for all the points
            double minX = 0; double maxX = 0;
            double minY = 0; double maxY = 0;

            foreach (Node atom in molecule.Atoms.Values)
            {
                if (minX == 0) minX = atom.Position.X;

                if (maxX == 0) maxX = atom.Position.X;

                if (minY == 0) minY = atom.Position.Y;

                if (maxY == 0) maxY = atom.Position.Y;

                double currentX = atom.Position.X; double currentY = atom.Position.Y;
                if (currentX < minX) minX = currentX;

                if (currentX > maxX) maxX = currentX;

                if (currentY < minY) minY = currentY;

                if (currentY > maxY) maxY = currentY;
            }
            //So now we get the distance between min and max
            double distX = maxX - minX;
            double distY = maxY - minY;
            //The center of this box is the dist/2
            double midX = distX / 2;
            double midY = distY / 2;
            return new BoundingBox((float)distX, (float) distY, new PointF((float)midX,(float) midY), new PointF((float)minX, (float)minY), 0);
        }
        private void ResizeCoords(int wheelDelta)
        {
            if (mol == null) return;

            //first we get the limit for scaling in x and y
            BoundingBox rawBounds = GetBoundingBox(molRawCopy);
            double scaleLimitX = 275 / rawBounds.Width - 30;
            double scaleLimitY = 275 / rawBounds.Height - 30;
            //The minimum scale distance is 30
            double minimumScaleXY = 30;
            int scaleAmount = 10;
            //Now we get our current scaling
            double currentScaleX =  bounds.Width / rawBounds.Width;
            double currentScaleY =  bounds.Height / rawBounds.Height;

            double sx = 0, sy = 0;
            //Now based on the value of wheelDelta (+-) we determine which direction we scale to
            if (wheelDelta > 0)
            {
                //If its greater we scale up
                //But first we make sure we are not above the scale limit
                if (currentScaleX >= scaleLimitX || currentScaleY >= scaleLimitY)
                    return;

                //Now we want to increase the scale by 10 in both x and y, but we don't want it going beyond the limit so we get the min of the two
                sx = Math.Min(currentScaleX + scaleAmount, scaleLimitX);
                sy = Math.Min(currentScaleY + scaleAmount, scaleLimitY);
            }
            else if (wheelDelta < 0)
            {
                //Now we try and scale down, doing the same as for scaling up 
                if (currentScaleX <= minimumScaleXY || currentScaleY <= minimumScaleXY)
                    return;

                sx = Math.Max(currentScaleX - scaleAmount, minimumScaleXY);
                sy = Math.Max(currentScaleY - scaleAmount, minimumScaleXY);
            }
            foreach (Node rawAtom in molRawCopy.Atoms.Values)
            {
                foreach (Node atom in mol.Atoms.Values)
                {
                    if (rawAtom.ID == atom.ID)
                    {
                        atom.Position = rawAtom.Position;
                        break;
                    }
                }
            }
            //Now we go through and scale the points base off the raw copy
            foreach (Node atom in mol.Atoms.Values)
            {
                double ox = atom.Position.X;
                double oy = atom.Position.Y;



                double x = (ox * sx + center.X);
                double y = (oy * sy + center.Y);


                atom.Position = new Vector3(x, y, 0);
             
            }
            bounds = GetBoundingBox(mol);
            foreach (Bond bond in mol.Bonds)
            {
                bond.CalculateBoundPath();
            }
        }
        private void NormalizeCoords()
        {
            if (mol == null) return;
            //we get the bounds of the raw copy for initial sizing
            bounds = GetBoundingBox(molRawCopy);

            //The scale values for x and y are then calculated
            double sx = 30;//275 / bounds.Width-30;
            double sy = 40;//275 / bounds.Height-30;

            foreach (Node atom in mol.Atoms.Values)
            {
                double ox = atom.Position.X; 
                double oy = atom.Position.Y;
                
                

                double x = (ox*sx + center.X);
                double y = (oy*sy + center.Y);
                
             
                atom.Position = new Vector3(x, y, 0);
          
                //Finally we go through the bonds and update their nodes, due to some error in copying
                foreach (Bond bond in mol.Bonds)
                {
                    if (bond.Node1.ID == atom.ID)
                        bond.Node1 = atom;
                    else if (bond.Node2.ID == atom.ID)
                        bond.Node2 = atom;
                }
                //and again go through one more time to calculate the paths
                foreach (Bond bond in mol.Bonds)
                {
                    bond.CalculateBoundPath();
                }
            }
            //We also need to get the new size of the bounding box and get the bounds for each bond
            bounds = GetBoundingBox(mol);
           // bondBoundList = GetBondBoundList();
        }
        
        
        private void MolViewer_MouseClick(object sender, MouseEventArgs e)
        {
            //When the mouse is clicked we check if it was in the bounds, if so we select the molecule, making the bounds visible
            if (e.X > bounds.UpperLeftCorner.X && e.X < bounds.UpperLeftCorner.X + bounds.Width &&
                e.Y > bounds.UpperLeftCorner.Y && e.Y < bounds.UpperLeftCorner.Y + bounds.Height)
            {
                //the click is inside the bounds
                structureSelected = true;
           
                foreach (Bond bond in mol.Bonds)
                {
                    Region region = new Region(bond.BoundPath);
                    if (region.IsVisible(e.Location))
                    {
                        if (bond.BondOrder == 2)
                        {
                            Console.WriteLine("Clicked a double bond");
                            //If it is a double bond switch the side of the double bond
                            if (bond.WhichSide) bond.WhichSide = false;
                            else bond.WhichSide = true;
                            Console.WriteLine("Double bond is now {0}", bond.WhichSide.ToString());
                        }
                        break;
                    }
           
                }
            }
            else
            {
                structureSelected = false;
            }
            //redraw
            this.Invalidate();
        }
        private void MolViewer_MouseWheel(object sender, MouseEventArgs e)
        {
            //if the structure is selected we scale it
            if (structureSelected)
            {
                Console.WriteLine("E delta " + e.Delta);
                ResizeCoords(e.Delta);
            }
            this.Invalidate();
        }

   
    }
    public enum BondOrder
    {
        Single = 1,
        Double,
        Triple
    }
 
    
}
