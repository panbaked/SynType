using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using SynType.File_ReadWrite;
using SynType.File_ReadWrite.SynType;
using SynType.Chemical_Classes;
using SynType.Math_Classes;
using SynType.Program_Classes;
using System.Runtime.Serialization.Formatters.Binary;
using System.Diagnostics;

namespace SynType
{
    public partial class Form1 : Form
    {
        Synthesis currentSynthesis;
        Project currentProject;
        List<Synthesis> currentOpenSyntheses;
        string username = "rap";
        User user;
        public Form1()
        {
            InitializeComponent();
            FFManager.FirstRunSetup("rap");
            currentOpenSyntheses = new List<Synthesis>();
            currentSynthesis = new Synthesis();
            

            tabControl.Selecting += new TabControlCancelEventHandler(tabControl_Selecting);

            user = new User("rap","Richard Albert Peck","rap");
           
            OpenLastOpenedProject();


        }

        void tabControl_Selecting(object sender, TabControlCancelEventArgs e)
        {
            ReactionListViewTabPage reactionListViewTab = e.TabPage.GetChildAtPoint(new Point(0, 0)) as ReactionListViewTabPage;
            //if we are looking at a synthesis tab we want to change our current synthesis to that
            TabPageControl synthesisTab = e.TabPage.GetChildAtPoint(new Point(0, 0)) as TabPageControl;
            if(synthesisTab != null)
                currentSynthesis = synthesisTab.currentSynthesis;
        }

    
       /* private void GetMolFromClipboard()
        {
            //get data from clipboard
            IDataObject retrievedObject = Clipboard.GetDataObject();

            MemoryStream ms = (MemoryStream)retrievedObject.GetData("MDLCT");
            if (ms == null) return;
            byte[] b = new byte[ms.Length];
            ms.Read(b, 0, (int)ms.Length);
            string result = ConvertMDLCTToMol(b);


            Molecule mol = new Molecule();
            MolFileReader.ReadMolData(result, ref mol);
            Console.WriteLine(mol.ToString());

            //TEST
            //save the mol file and read it in again
            MolFileWriter.WriteMolFile("test1.mol", mol);
            string molString = MolFileWriter.WriteMolString(mol);
            Molecule mol2 = new Molecule();
            MolFileReader.ReadMolData(molString, ref mol2);
            Console.WriteLine(mol2.ToString());

        }
        private string ConvertMDLCTToMol(byte[] mdlct)
        {
            string result = string.Empty;
            int byteindex = 0;
            System.Text.Encoding encoding = new System.Text.ASCIIEncoding();
            while (byteindex < mdlct.Length)
            {
                int lineLength = Convert.ToInt32(mdlct[byteindex]);
                byte[] linebytes = new byte[lineLength];
                Buffer.BlockCopy(mdlct, byteindex + 1, linebytes, 0, lineLength);

                string line = encoding.GetString(linebytes);
                result += line + "\r\n";

                byteindex += lineLength + 1;
            }
            return result;
        }
        */
      
        private void button3_Click(object sender, EventArgs e)
        {
            /*
            AddCompoundForm f = new AddCompoundForm(this, currentSynthesis);
            f.Show(this);*/
            AddReactionListViewTabPage(currentProject);
        }
        
        public void AddCompound(Compound compound)
        {
            switch (compound.Type)
            {
                case COMPOUND_TYPES.Reactant:
                    currentSynthesis.Reactants.Add(compound);
                    break;
                case COMPOUND_TYPES.Reagent:
                    currentSynthesis.Reagents.Add(compound);
                    break;
                case COMPOUND_TYPES.Product:
                    currentSynthesis.Products.Add(compound);
                    break;

            }
            currentSynthesis.UpdateSynthesis();
          
            Console.WriteLine("AddCompound reac {0}, reag {1}, prod {2}", currentSynthesis.Reactants.Count, currentSynthesis.Reagents.Count, currentSynthesis.Products.Count);
        }
        private void UpdateTreeView()
        {
            projectView.Nodes.Clear();

            //Configure context menus
            //For synth directories we can add next step and delete reaction
            ContextMenuStrip synthAddNextDeleteMenu = new ContextMenuStrip();
            synthAddNextDeleteMenu.Items.Add("Add Next Step");
            synthAddNextDeleteMenu.Items.Add("Delete synthesis");
            synthAddNextDeleteMenu.ItemClicked += new ToolStripItemClickedEventHandler(synthAddNextDeleteMenu_ItemClicked);

            TreeNode baseNode = new TreeNode(currentProject.ProjectName);
            
            FFManager.BuildTreeView(baseNode, currentProject.ProjectPath, currentProject, synthAddNextDeleteMenu);

            projectView.Nodes.Add(baseNode);


        }

        void synthAddNextDeleteMenu_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            TreeNode selectedNode = projectView.SelectedNode;
            if (selectedNode == null)
            {
                Console.WriteLine("Tag is somehow null on an item");
                return;
            }
            Synthesis synthesisToAlter;
            //first check if its a folder
            SynthesisDirectory synthDir = selectedNode.Tag as SynthesisDirectory;
            if (synthDir != null)
                synthesisToAlter = synthDir.Synth;
            else
                synthesisToAlter = selectedNode.Tag as Synthesis;

            if (synthesisToAlter == null)
                return;

            if (e.ClickedItem.Text == "Add Next Step")
            {
                InputDialog inputDialog = new InputDialog("Enter a name for the new synthesis");
                if (inputDialog.ShowDialog() == DialogResult.OK)
                {
                    while (string.IsNullOrEmpty(inputDialog.Input))
                    {
                        if (inputDialog.ShowDialog() == DialogResult.Cancel)
                        {
                            return;
                        }
                    }
                    //unregister our last event handler
                    //currentSynthesis.SynthesisChanged -= new SynthesisChangedEventHandler(currentSynthesis_SynthesisChanged);
                    
                    Synthesis newSynth = new Synthesis(inputDialog.Input, currentProject.GetNewProjectID());
                    //set the next step to be our new synthesis, and the new synthesis' previous step to be the old synthesis
                    currentSynthesis.NextStep = new SynthesisInfo(newSynth.Name, newSynth.ProjectID);
                    newSynth.PreviousStep = new SynthesisInfo(currentSynthesis.Name, currentSynthesis.ProjectID);
                    //and before we change our synthesis we save the current synth to update its nextstep property
                    FFManager.SaveSynFile(currentSynthesis, currentProject);
                    //Add a new tab with this synthesis
                    AddNewTabPage(newSynth);
                    //Save the synthesis
                    FFManager.SaveSynFile(currentSynthesis, currentProject);

                }
                //build the tree view
                UpdateTreeView();
            }
            else
            {
                PromptUserToDeleteSynthesis(synthesisToAlter);
            }
        }

        private void PromptUserToDeleteSynthesis(Synthesis syn)
        {
            if (MessageBox.Show("Are you sure you wish to delete the selected synthesis? This action can not be undone", "Delete synthesis?", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                //user selected yes so we delete the synthesis
                FFManager.DeleteSynthesisFolder(syn, currentProject);
                currentProject.AllSyntheses.Remove(syn);
                UpdateTreeView();
            }
        }

       
        private void DisableCell(DataGridViewCellStyle cellStyle)
        {
            cellStyle.BackColor = Color.LightGray;
            cellStyle.ForeColor = Color.DarkGray;
        }
       
        //FILE toolstrip click events
        private void newProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            InputDialog inputDialog = new InputDialog("Please enter the name of the project you wish to create.");
            if (inputDialog.ShowDialog() == DialogResult.OK)
            {
                string projectName = inputDialog.Input;

                if (!string.IsNullOrEmpty(projectName))
                {
                    ProjectInfo projectInfo = new ProjectInfo(projectName, "C:\\SynType\\Data\\rap\\"+ projectName, username);
                    FFManager.BuildNewProjectFolder("rap", projectInfo);
                }
                
            }
        }
        //Opens a project of the users choosing
        private void openProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.DefaultExt = "*.spf";
            fileDialog.InitialDirectory = "C:\\SynType\\Data";
            string projectFileName = string.Empty;
            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                projectFileName = fileDialog.FileName;
            }
            ProjectInfo projectInfo = null;
            if (string.IsNullOrEmpty(projectFileName)) return;
            using (Stream s = File.OpenRead(projectFileName))
            {
                BinaryFormatter bf = new BinaryFormatter();
                projectInfo = bf.Deserialize(s) as ProjectInfo;
            }
            if (projectInfo == null)
                return;
            OpenProject(this.user, projectInfo);
        }
        //Saves a single syn type file, the current synthesis
        private void saveSynTypeFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (currentProject == null)
            {
                MessageBox.Show("You must open a project before saving any files!");
            }
            FFManager.SaveSynFile(currentSynthesis, currentProject);
        }
        //Saves all open syntheses (open tabs)
        private void saveAllOpenSynthesesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (Synthesis synth in currentOpenSyntheses)
            {
                FFManager.SaveSynFile(synth, currentProject);
            }
        }
        //Creates a new synthesis, opening it in a new tab
        private void newSynthesisToolStripMenuItem_Click(object sender, EventArgs e)
        {
            InputDialog inputDialog = new InputDialog("Enter a name for the new synthesis");
            if (inputDialog.ShowDialog() == DialogResult.OK)
            {
                while (string.IsNullOrEmpty(inputDialog.Input))
                {
                    if (inputDialog.ShowDialog() == DialogResult.Cancel)
                    {
                        return;
                    }
                }
               
                Synthesis newSynth = new Synthesis(inputDialog.Input, currentProject.GetNewProjectID());
                AddNewTabPage(newSynth);
                //currentSynthesis.UpdateSynthesis();
                currentProject.AllSyntheses.Add(currentSynthesis);
                //Save the synthesis
                FFManager.SaveSynFile(currentSynthesis, currentProject);

            }
            //build the tree view
            UpdateTreeView();
        }
        //Tree view events
        private void projectView_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            TreeNode node = projectView.SelectedNode;
            if (node == null) return;
            //check if the node's name's last 4 letters are .syn
            string name = node.Name;
            Console.WriteLine("User double-click on " + Path.GetFileName(name));
            string extension = Path.GetExtension(name);
            
            if (extension == ".syn")
            {
                AddNewTabPage(currentProject.FindSynthesis(Path.GetFileNameWithoutExtension(name)));
                return;
            }

            //When a user double clicks the molecules or spectra folder we open those in explorer
            if (Path.GetFileName(name) == "molecules" || Path.GetFileName(name) == "spectra")
            {
                Console.WriteLine("Opening molecules folder " + name);
                Process.Start(name);
            }
        }
        
        private void OpenProject(User user, ProjectInfo projectInfo)
        {
            this.user = user;
            this.Text = "SynType " + this.user.UserInitials + " loaded project: " + projectInfo.ProjectName;
            currentProject = new Project(projectInfo.ProjectName, projectInfo.ProjectPath, user);
            if (currentProject.AllSyntheses.Count == 0)
            {
                //IF there are no syntheses then  we prompt the user to create a new one
                if (MessageBox.Show("The project is empty, would you like to create a new synthesis now?", "Create a new synthesis?", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    InputDialog inputDialog = new InputDialog("Enter a name for the new synthesis");
                    if (inputDialog.ShowDialog() == DialogResult.OK)
                    {
                        while(string.IsNullOrEmpty(inputDialog.Input))
                        {
                            if (inputDialog.ShowDialog() == DialogResult.Cancel)
                                return;

                        }
                       
                        Synthesis newSynth = new Synthesis(inputDialog.Input, currentProject.GetNewProjectID());
                        currentSynthesis = newSynth;
                        currentProject.AllSyntheses.Add(currentSynthesis);
                        //Save the synthesis
                        FFManager.SaveSynFile(currentSynthesis, currentProject);

                    }
                }
            }
            UpdateTreeView();
        }
        //opens the last project that was open when the user closed
        private void OpenLastOpenedProject()
        {
            if (!File.Exists("lastrun.dat"))
                return;
            StartupInfo startupInfo = null;
            using (Stream s = File.OpenRead("lastrun.dat"))
            {
                BinaryFormatter bf = new BinaryFormatter();
                startupInfo = bf.Deserialize(s) as StartupInfo;

            }
            if (startupInfo == null)
                return;

            OpenProject(startupInfo.LastUser, startupInfo.LastProject);
        }

        //When the user closes the program we save our last run info
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            StartupInfo startupInfo = new StartupInfo(user, new ProjectInfo(currentProject.ProjectName, currentProject.ProjectPath, user.UserID));

            using (Stream s = File.Create("lastrun.dat"))
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(s, startupInfo);
            }
        }

        private void AddNewTabPage(Synthesis synthesis)
        {
            currentOpenSyntheses.Add(synthesis);
            tabControl.TabPages.Add(new TabPage(synthesis.Name));
            TabPageControl control = new TabPageControl(this, synthesis);
            control.Anchor = (AnchorStyles.Bottom | AnchorStyles.Top);
            control.Dock = DockStyle.Fill;
            tabControl.TabPages[tabControl.TabPages.Count - 1].Controls.Add(control);
            
            //switch to the new tab
            tabControl.SelectTab(tabControl.TabPages[tabControl.TabPages.Count - 1]);
            currentSynthesis = currentOpenSyntheses.Last();

        }

        private void AddReactionListViewTabPage(Project project)
        {
            tabControl.TabPages.Add("All Reactions");
            
            ReactionListViewTabPage control = new ReactionListViewTabPage(project);
            control.Anchor = (AnchorStyles.Bottom | AnchorStyles.Top);
            control.Dock = DockStyle.Fill;
            tabControl.TabPages[tabControl.TabPages.Count - 1].Controls.Add(control);

            //switch to the new tab
            tabControl.SelectTab(tabControl.TabPages[tabControl.TabPages.Count - 1]);
        }
       
    }
}
