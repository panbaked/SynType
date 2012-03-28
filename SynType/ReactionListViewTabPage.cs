using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SynType.File_ReadWrite;
using SynType.Program_Classes;
using SynType.Chemical_Classes;
using System.IO;

namespace SynType
{
    public partial class ReactionListViewTabPage : UserControl
    {
        Project currentProject; //the project that contains all the syntheses
        List<Image> reactionImages;
        public ReactionListViewTabPage(Project project)
        {
            InitializeComponent();
            currentProject = project;

            BuildList();
            //this.Paint +=new PaintEventHandler(this.paint);
        }
        private void BuildList()
        {
            ReactionViewer reactionViewer = new ReactionViewer();
            reactionImages = new List<Image>();
            
            int i = 0;
            foreach (Synthesis synth in currentProject.AllSyntheses)
            {
                reactionViewer.Synth = synth;
                reactionViewer.UpdateControl();
                Bitmap bitmap = reactionViewer.GenerateReactionBitmap();
                Image image = Image.FromHbitmap(bitmap.GetHbitmap(Color.White));
                image.Tag = i;
                Console.WriteLine("Adding image " + image.Size.ToString());
                //add it to our list of objects
                reactionImages.Add(image);
                //bitmap.Save(@"C:\SynType\test" + i + ".bmp");
                bitmap.Dispose(); //make sure we free memory
                
                i++;
            }
            reactionViewer.Dispose();
        }
        
        private void paint(Object sender, PaintEventArgs e)
        {
            Graphics g = panel.CreateGraphics();
            int i = 0;

            foreach (Image image in reactionImages)
            {
                g.DrawImage(image, new Point(0, image.Height * i));
                i++;
            }
            panel.Invalidate();
        }
      
        private string GetImageKey(object row)
        {
            ReactionListObject reaction = row as ReactionListObject;
            if (reaction != null)
            {
                Console.WriteLine("Getting image key for " + reaction.Name);
                return reaction.Name;
            }
            else
            {
                Console.WriteLine("This object isn't a reaction!");
            }
            return string.Empty;
        }
        private Image GetReactionImage(List<ReactionListObject> objects, string key)
        {
            foreach (ReactionListObject reaction in objects)
            {
                if (reaction.Name == key)
                    return reaction.ReactionImage;
            }
            return null;
        }
        private class ReactionListObject
        {
            public Image ReactionImage { get; set; }
            public string Name { get; set; }

            public ReactionListObject(string name, Image reactionImage)
            {
                ReactionImage = reactionImage;
                Name = name;
            }
        }

        private void panel_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            int i = 0;

            foreach (Image image in reactionImages)
            {

                g.DrawImage(image, new Point(0, image.Height * i));
                g.DrawRectangle(new Pen(Brushes.Green), new Rectangle(new Point(0, image.Height * i), image.Size));
                i++;
            }
            panel.Height = reactionImages[0].Height * i;
        }
    }
}
