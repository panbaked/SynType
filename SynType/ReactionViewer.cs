using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SynType.Chemical_Classes;
using System.Drawing.Drawing2D;
using SynType.Math_Classes;

namespace SynType
{
  
    public partial class ReactionViewer : UserControl
    {
        private bool debug = false;
        public int regionCount = 3;
        private Synthesis synthesis;
        private List<Compound> reactants;
        private List<Molecule> visualReactants;
        private List<Compound> reagents;
        private List<Molecule> visualReagents;
        private List<Compound> products;
        private List<Molecule> visualProducts;
        Compound solvent;

        private List<RectangleF> allRegions;
        private List<RectangleF> reactantRegions;
        private List<RectangleF> reagentRegions;
        private List<RectangleF> productRegions;

        private List<CompoundRegion> reactRegions;
        private List<CompoundRegion> reagRegions;
        private List<CompoundRegion> prodRegions;

        public Synthesis Synth { get { return synthesis; } set { synthesis = value; } }

        private Font font;
        public ReactionViewer()
        {
            InitializeComponent();
            reactants = new List<Compound>();
            reagents = new List<Compound>();
            products = new List<Compound>();
            visualReactants = new List<Molecule>();
            visualReagents = new List<Molecule>();
            visualProducts = new List<Molecule>();
            allRegions = new List<RectangleF>();
            reactantRegions = new List<RectangleF>();
            reagentRegions = new List<RectangleF>();
            productRegions = new List<RectangleF>();

            reactRegions = new List<CompoundRegion>();
            reagRegions = new List<CompoundRegion>();
            prodRegions = new List<CompoundRegion>();
            font = new Font("Times New Roman", 15);
        }
       

      
        private void ReactionViewer_Load(object sender, EventArgs e)
        {
            DividRegions();
            ContextMenuStrip contextMenuStrip = new ContextMenuStrip();
            contextMenuStrip.Items.Add("Toggle Display Type");
            contextMenuStrip.ItemClicked += new ToolStripItemClickedEventHandler(contextMenuStrip_ItemClicked);
            this.ContextMenuStrip = contextMenuStrip;
        }

        void contextMenuStrip_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            if (e.ClickedItem.Text == "Toggle Display Type")
            {
                //Check which region we clicked in
                Point p = this.PointToClient(new Point(e.ClickedItem.Owner.Bounds.X, e.ClickedItem.Owner.Bounds.Y));
                Compound comp = GetCompoundFromPositionOnControl(p);
                if (comp.DisplayRefNameWhenDrawn)
                    comp.DisplayRefNameWhenDrawn = false;
                else
                    comp.DisplayRefNameWhenDrawn = true;

                Invalidate();
            }
        }
        public void UpdateControl()
        {
            reactants = new List<Compound>();
            reagents = new List<Compound>();
            products = new List<Compound>();
            foreach (Compound comp in synthesis.Reactants)
            {
                reactants.Add(comp);
               
            }
            foreach (Compound comp in synthesis.Reagents)
            {
                reagents.Add(comp);
              
            }
            foreach (Compound comp in synthesis.Products)
            {
                products.Add(comp);
               
            }
            DividRegions();
            GetVisualMolecules();

            Invalidate();
        }

        void ReactionViewer_DataChanged(object sender, DataChangedEventArgs e)
        {
            Console.WriteLine("Updating control since the data changed");
        
        }
        public void paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.Clear(Color.White);
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            if (debug)
            {
                foreach (RectangleF rect in reactantRegions)
                {
                    GraphicsPath reactantBox = new GraphicsPath();
                    reactantBox.AddRectangle(rect);
                    g.DrawPath(Pens.Orange, reactantBox);

                }

                foreach (RectangleF rect in reagentRegions)
                {
                    GraphicsPath reagentBox = new GraphicsPath();
                    reagentBox.AddRectangle(rect);
                    g.DrawPath(Pens.Green, reagentBox);
                }
                foreach (RectangleF rect in productRegions)
                {
                    GraphicsPath productBox = new GraphicsPath();
                    productBox.AddRectangle(rect);
                    g.DrawPath(Pens.Red, productBox);
                }
            }
            foreach (Molecule mol in visualReactants)
            {
                
                DrawMolecule(mol, g);
              
            }
            int count = 0;
            foreach (Compound comp in reactants)
            {
                if (comp.MoleculeData == null)
                    continue;

                if (comp.MoleculeData.OnlyHasFormula)
                    DrawCompoundText(comp, reactantRegions[count], g, comp.DisplayRefNameWhenDrawn);
                 
                count++;
            }
            foreach (Molecule mol in visualProducts)
            {
                DrawMolecule(mol, g);
               
            }
            count = 0;
            
            foreach (Compound comp in products)
            {
                if (comp.MoleculeData == null)
                    continue;

                if (comp.MoleculeData.OnlyHasFormula)
                    DrawCompoundText(comp, productRegions[count], g, comp.DisplayRefNameWhenDrawn);
                count++;
            }

            DrawArrow(g);
            DrawReagentNames(g);
            DrawPlusSigns(g);
        }
        //We need to generate images for listviews so we just paint as usual to a bitmap
        public Bitmap GenerateReactionBitmap()
        {
            Bitmap bmp = new Bitmap(1024, 256);
            Graphics g = Graphics.FromImage(bmp);
            foreach (Molecule mol in visualReactants)
            {

                DrawMolecule(mol, g);

            }
            int count = 0;
            foreach (Compound comp in reactants)
            {
                if (comp.MoleculeData == null)
                    continue;

                if (comp.MoleculeData.OnlyHasFormula)
                    DrawCompoundText(comp, reactantRegions[count], g, comp.DisplayRefNameWhenDrawn);

                count++;
            }
            foreach (Molecule mol in visualProducts)
            {
                DrawMolecule(mol, g);

            }
            count = 0;

            foreach (Compound comp in products)
            {
                if (comp.MoleculeData == null)
                    continue;

                if (comp.MoleculeData.OnlyHasFormula)
                    DrawCompoundText(comp, productRegions[count], g, comp.DisplayRefNameWhenDrawn);
                count++;
            }

            DrawArrow(g);
            DrawReagentNames(g);
            DrawPlusSigns(g);

            return bmp;
        }
        private void DividRegions()
        {
            //First we clear the region lists
            reactantRegions = new List<RectangleF>();
            reagentRegions = new List<RectangleF>();
            productRegions = new List<RectangleF>();
            int boardWidth = this.Width-10;
            int boardHeight = this.Height-10;

            //We count up all the regions needed and divid the width by that
            int reactantCount = reactants.Count >= 1 ? reactants.Count : 1;
            int productCount = products.Count >= 1 ? products.Count : 1;
            int regionCount = reactantCount + 1 + productCount;
            if (regionCount < 3) regionCount = 3; //we always have at least three regions one for each compound type
           
            float defaultRegionWidth = boardWidth / regionCount;
            
            float widthOffset = 0;
            //add all the reactant regions
            if (reactants.Count == 0)
            {
                reactants.Add(new Compound(null)); //if there are zero reactants we add at least one
            }
            foreach (Compound comp in reactants)
            {
                reactantRegions.Add(new RectangleF(widthOffset, 0, defaultRegionWidth, boardHeight));
                allRegions.Add(new RectangleF(widthOffset, 0, defaultRegionWidth, boardHeight));
                widthOffset += defaultRegionWidth;
            }
            
            //add a single region for reagents
            reagentRegions.Add(new RectangleF(widthOffset, 0, defaultRegionWidth, boardHeight));
            widthOffset += defaultRegionWidth;
            //We alsso add at least a single product region
            if (products.Count == 0)
            {
                products.Add(new Compound(null));
            }
            //And finally the product regions
            foreach (Compound comp in products)
            {
                productRegions.Add(new RectangleF(widthOffset, 0, defaultRegionWidth, boardHeight));
                allRegions.Add(new RectangleF(widthOffset, 0, defaultRegionWidth, boardHeight));
                widthOffset += defaultRegionWidth;
            }
        }
       
        private void GetVisualMolecules()
        {
            int count = 0;
            //First reset the lists
            visualReactants = new List<Molecule>();
            visualReagents = new List<Molecule>();
            visualProducts = new List<Molecule>();
            foreach (Compound comp in reactants)
            {
                if (comp.MoleculeData == null) break;
                else
                {
                    if (comp.VisualMolecule == null)
                        comp.VisualMolecule = (Molecule)comp.MoleculeData.Clone();
                    else
                    {
                        //otherwise we clear the molecule and get a new copy, otherwise the coords won't normalize correctly
                        comp.VisualMolecule = null;
                        comp.VisualMolecule = (Molecule)comp.MoleculeData.Clone();
                    }
                    //first we normalize the coordinates and then add the molecule to the visual list
                    NormalizeCoords(comp.VisualMolecule, COMPOUND_TYPES.Reactant, count);
                    visualReactants.Add(comp.VisualMolecule);
                }
                count++;
            }
            count = 0;
            foreach (Compound comp in reagents)
            {
                if (comp.MoleculeData == null) break;
                else
                {
                    if (comp.VisualMolecule == null)
                        comp.VisualMolecule = (Molecule)comp.MoleculeData.Clone();
                    else
                    {
                        //otherwise we clear the molecule and get a new copy, otherwise the coords won't normalize correctly
                        comp.VisualMolecule = null;
                        comp.VisualMolecule = (Molecule)comp.MoleculeData.Clone();
                    }
                    NormalizeCoords(comp.VisualMolecule, COMPOUND_TYPES.Reagent, count);
                    visualReagents.Add(comp.VisualMolecule);
                }
                count++;
            }
            count = 0;
            foreach (Compound comp in products)
            {
                if (comp.MoleculeData == null) break;
                else
                {
                    if (comp.VisualMolecule == null)
                        comp.VisualMolecule = (Molecule)comp.MoleculeData.Clone();
                    else
                    {
                        //otherwise we clear the molecule and get a new copy, otherwise the coords won't normalize correctly
                        comp.VisualMolecule = null;
                        comp.VisualMolecule = (Molecule)comp.MoleculeData.Clone();
                    }
                    NormalizeCoords(comp.VisualMolecule, COMPOUND_TYPES.Product, count);
                    visualProducts.Add(comp.VisualMolecule);
                }
                count++;
            }
            
        }
        private void NormalizeCoords(Molecule mol, COMPOUND_TYPES type, int id)
        {
            if (mol == null) return;

           
            //First we get the region we are working in
            RectangleF region;
            if (type == COMPOUND_TYPES.Reactant) region = reactantRegions[id];
            else if (type == COMPOUND_TYPES.Reagent) region = reagentRegions[0]; //we only ever have one reagent region
            else region = productRegions[id];

            //we get the bounds of the raw copy for initial sizing for the given region
            mol.Bounds = GetBoundingBox(mol, region);

            PointF center = new PointF(region.X + region.Width / 2, region.Y + region.Height / 2);
            
            //The scale values for x and y are then calculated
            //We want to keep the same ratio so we get the ratio, find the max sx can be and then calculte sy from that
            float ratio = mol.Bounds.Width/mol.Bounds.Height;

            double sx = 40; //region.Width / mol.Bounds.Width-30;
            double sy = 30; //ratio / sx;

            foreach (Node atom in mol.Atoms.Values)
            {
                double ox = atom.Position.X;
                double oy = atom.Position.Y;



                double x = (ox * sx + center.X);
                double y = (oy * sy + center.Y);


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
            mol.Bounds = GetBoundingBox(mol, region);
        }
        private BoundingBox GetBoundingBox(Molecule molecule, RectangleF region)
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
            double distX = maxX - minX + region.X;
            double distY = maxY - minY + region.Y;
            //The center of this box is the dist/2
            double midX = distX / 2 + region.X;
            double midY = distY / 2 + region.Y;
          
            return new BoundingBox((float)distX, (float)distY, new PointF((float)midX, (float)midY), new PointF((float)minX, (float)minY), 0);
        }
        private void DrawReagentNames(Graphics g)
        {
            //Get the center of the reagent region
            RectangleF region = reagentRegions[0];
            PointF center = new PointF(region.Location.X + region.Width * 0.5f, region.Location.Y + region.Height * 0.5f);
            //We need to build the string for reagents which goes Reagent1, Reagent2, \nReagent3, Reagent4 , if there are more than 4 we jump to a new line again
            string names = string.Empty;
            int count = 0;
            foreach (Compound c in reagents)
            {
                if(count % 2 == 0 && count != 0)
                    names += "\n";

                names += c.RefName;
                //add a comma if we there are more reagents to follow
                if (count + 1 < reagents.Count)
                    names += ", ";

                count++;
            }
           
            //Now we get the string size
            SizeF stringSize = g.MeasureString(names, font);
            PointF pos = new PointF(center.X - stringSize.Width * 0.5f, center.Y - stringSize.Height);
            g.DrawString(names, font, Brushes.Black, pos);
        }
        private void DrawCompoundText(Compound comp, RectangleF region, Graphics g, bool displayRefName)
        {
            //Find the center of the region
            PointF center = new PointF(region.Location.X + region.Width * 0.5f, region.Location.Y + region.Height * 0.5f);
            string name = string.Empty;
            //if we display refname the name is that otherwise we just write the formula
            if (displayRefName)
                name = comp.RefName;
            else
                name = comp.Formula;

            SizeF stringSize = g.MeasureString(name, font);
            PointF drawPos = new PointF(center.X - stringSize.Width * 0.5f, center.Y);
            g.DrawString(name, font, Brushes.Black, drawPos);
        }
        private void DrawMolecule(Molecule mol, Graphics g)
        {
       
        
            //First we draw bonds
            foreach (Bond bond in mol.Bonds)
            {
                PointF p1 = new PointF((float)bond.Node1.Position.X, (float)bond.Node1.Position.Y);
                PointF p2 = new PointF((float)bond.Node2.Position.X, (float)bond.Node2.Position.Y);

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
                            g.DrawString(symbol, font, Brushes.Black, (float)atom.Position.X - 9, (float)atom.Position.Y - 12);
                            continue;
                        }
                        else if (atom.ImplicitHydrogens == 1)
                        {
                            symbol += "H";
                            g.DrawString(symbol, font, Brushes.Black, (float)atom.Position.X - 9, (float)atom.Position.Y - 12);
                            continue;
                        }
                        else
                        {
                            //If there are more than one hydrogen we have to make a sub script
                            Font normalFont = font;
                            Font subscriptFont = new Font("Times New Roman", 10);
                            symbol += "H";
                            g.DrawString(symbol, normalFont, Brushes.Black, (float)atom.Position.X - 9, (float)atom.Position.Y - 12);
                            g.DrawString(atom.ImplicitHydrogens.ToString(), subscriptFont, Brushes.Black, (float)atom.Position.X + 20, (float)(atom.Position.Y + 5));
                            continue;
                        }
                    }
                    g.DrawString(symbol, font, Brushes.Black, (float)atom.Position.X - 9, (float)atom.Position.Y - 12);


                }
            }
        }
        private void DrawBonds(Graphics g, PointF point1, PointF point2, int order, bool whichSide)
        {
            switch (order)
            {
                case (int)BondOrder.Single:

                    PointF[] points = new PointF[2] { point1, point2 };

                    g.DrawLines(new Pen(Brushes.Black), points);

                    break;
                case (int)BondOrder.Double:
                    DrawDoubleBond(g, point1, point2, whichSide);
                    break;
                case (int)BondOrder.Triple:
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
            float l = (float)Math.Sqrt(Square(n.X) + Square(n.Y));
            n = new PointF((float)n.X / l * 5, (float)n.Y / l * 5); //normalize vector
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
            float l1 = (float)Math.Sqrt(Square(n1.X) + Square(n1.Y));
            float l2 = (float)Math.Sqrt(Square(n2.X) + Square(n2.Y));
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
        private void DrawArrow(Graphics g)
        {
            //First we get the center of the reagent region
            RectangleF rect = reagentRegions[0];
            PointF center = new PointF(rect.X + rect.Width / 2, rect.Y + rect.Height / 2);
            //Now we draw the arrow from 1/4 width to 3/4 width at the center height
            PointF start = new PointF(rect.X + rect.Width / 4, center.Y);
            PointF end = new PointF(rect.X + rect.Width * 3 / 4, center.Y);
            

            //Now we draw the arrow head, we have a line going from the end point to -30,5 from end
            //The next line is from top to bottom (-30, -5) and then finally from bottom to end
            PointF arrowTop = new PointF(end.X - 30, center.Y + 5);
            PointF arrowBottom = new PointF(end.X - 30, center.Y - 5);
            GraphicsPath arrowHead = new GraphicsPath();
            arrowHead.AddLine(end, arrowTop);
            arrowHead.AddLine(arrowTop, arrowBottom);
            arrowHead.AddLine(arrowBottom, end);
            //Finally we draw the line and fill the path
            g.DrawLine(Pens.Black, start, end);
            g.FillPath(Brushes.Black, arrowHead);
        }
        private void DrawPlusSigns(Graphics g)
        {
            //first do the reactants, if we have more than 1 then we add a plus sign where the two reactant regions meet
            if (reactants.Count >= 2)
            {
                int count = 0; //start after the first region
                float width = 0;
                float height = reactantRegions[0].Height * 0.5f;
                while (count + 1 < reactants.Count)
                {
                    width += reactantRegions[count].Width;
                    g.DrawLine(new Pen(Brushes.Black), new PointF(width - 15, height), new PointF(width + 15, height));
                    g.DrawLine(new Pen(Brushes.Black), new PointF(width, height - 15), new PointF(width, height + 15));
                    count++;
                }
            }
            //and then the same for products
            if (products.Count >= 2)
            {
                int count = 0; //start after the first region
                float width = 0;
                float height = productRegions[0].Height * 0.5f;
                while (count + 1 < reactants.Count)
                {
                    width += productRegions[count].Width;
                    g.DrawLine(new Pen(Brushes.Black), new PointF(width - 15, height), new PointF(width + 15, height));
                    g.DrawLine(new Pen(Brushes.Black), new PointF(width, height - 15), new PointF(width, height + 15));
                    count++;
                }
            }
        }
        private Compound GetCompoundFromPositionOnControl(Point p)
        {
            int count = 0;
            foreach (RectangleF rect in reactantRegions)
            {
                if (rect.Contains(p))
                    return reactants[count];
                count++;
            }
            count = 0;
            foreach (RectangleF rect in reagentRegions)
            {
                if (rect.Contains(p) && reagents.Count > count)
                    return reagents[count];
                count++;
            }
            count = 0;
            foreach (RectangleF rect in productRegions)
            {
                if (rect.Contains(p))
                    return products[count];
                count++;
            }
            return null;
        }
        private double Square(float n) { return n * n; }
    }

    public class CompoundRegion
    {
        public Compound compound;
       
        public Molecule visualMol;
        public RectangleF rect;

        public CompoundRegion(Compound comp, Molecule visualComp, RectangleF rect)
        {
            this.compound = comp;
            this.rect = rect;
            this.visualMol = visualComp;
        }
    }
}
