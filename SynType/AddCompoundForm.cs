using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using SynType.Chemical_Classes;
using SynType.File_ReadWrite;
using SynType.Math_Classes;

namespace SynType
{
    public partial class AddCompoundForm : Form
    {
        //We keep two references, one to the compound and one to the molecule
        Compound compound = null;
        Synthesis currentSynthesis = null;
        Molecule molecule;

        private bool structurePasted = false;

        Form1 parent = null;

        public AddCompoundForm(Form1 form1, Synthesis synthesis)
        {
            currentSynthesis = synthesis;
            
            InitializeComponent();
            parent = form1;
            
            
        }

        private void AddCompoundForm_Load(object sender, EventArgs e)
        {
            //Build the type selection drop down box
            typeSelection.Items.Add("Reactant"); 
            typeSelection.Items.Add("Reagent"); 
            typeSelection.Items.Add("Product");
            typeSelection.SelectedIndex = 0;

            //Build the state selection drop down
            stateSelection.Items.Add("Solid");
            stateSelection.Items.Add("Liquid");
            stateSelection.Items.Add("Gas");
            stateSelection.Items.Add("Solution");
            stateSelection.SelectedIndex = 0;

            //The following unit selection boxes have to follow the UNIT_POWERS Enum, that is none = 0, u = 1, m = 2, k = 3
            //Build the mols unit selection drop down
            molUnitSelection.Items.Add("mol");
            molUnitSelection.Items.Add("umol");
            molUnitSelection.Items.Add("mmol");
            molUnitSelection.SelectedIndex = 0;

            //Build the mass unit selection drop down
            massUnitSelection.Items.Add("g");
            massUnitSelection.Items.Add("ug");
            massUnitSelection.Items.Add("mg");
            massUnitSelection.Items.Add("kg");
            massUnitSelection.SelectedIndex = 0;

            //Build the volume unit selection drop down
            volumeUnitSelection.Items.Add("l");
            volumeUnitSelection.Items.Add("ul");
            volumeUnitSelection.Items.Add("ml");
            volumeUnitSelection.SelectedIndex = 2;

            molecule = new Molecule();
            //setup event handler for the formula textbox
            this.formulaTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(CheckFormulaKeys);
            this.molsTextBox.KeyPress += new KeyPressEventHandler(molsTextBox_KeyPress);

            //Next we need to create the molViewer 
            molViewer.Paint +=new PaintEventHandler(molViewer.paint);
            //If the synthesis already has a limiting reactant we remove the islimitingcheckbox
            if (currentSynthesis.LimitingReactant != null)
            {
                isLimitingCheckBox.Hide();
            }
        }
        private void molsTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13)
            {
                //We check to see if a structure is pasted or formula is set then we set mass
                if (structurePasted || !string.IsNullOrEmpty(molecule.Formula))
                {
                    float mols = 0;
                    try
                    {
                        mols = float.Parse(molsTextBox.Text);
                    }
                    catch (FormatException)
                    {
                        MessageBox.Show("Mols input was not a number, please check your input.");
                    }
                    float mass = molecule.MolecularWeight * mols;

                    massTextBox.Text = mass.ToString();
                }
                else
                    Console.WriteLine("Either a structure was not pasted or no formula was entered.");
            }
        }
        private void CheckNameKeys(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13)
            {
                molecule.Name = nameTextBox.Text;


            }
        }
        private void CheckFormulaKeys(object sender, System.Windows.Forms.KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13) //if enter has been pressed we try and build a molecule from the formula
            {
                molecule = new Molecule(this);
                molecule.Formula = formulaTextBox.Text;
                molecule.OnlyHasFormula = true;
                molWeightLabel.Text = molecule.MolecularWeight.ToString();
            }
        }
        private void pasteMDL_Click(object sender, EventArgs e)
        {
            molecule = new Molecule();
            GetMolFromClipboard();
            //Now fill in the data fields, first is formula
            formulaTextBox.Text = molecule.Formula.ToString();
            //Next is mol weight
            molWeightLabel.Text = molecule.MolecularWeight.ToString();
            structurePasted = true;
            //and finally we set the molviewer's molecule
            molViewer.Mol = (Molecule) molecule.Clone();
            molViewer.Invalidate(); //redraw the viewer
        }
        private void GetMolFromClipboard()
        {
            //get data from clipboard
            IDataObject retrievedObject = Clipboard.GetDataObject();
            //Get a memory stream and then read in the molfile string
            MemoryStream ms = (MemoryStream)retrievedObject.GetData("MDLCT");
            if (ms == null) return;
            byte[] b = new byte[ms.Length];
            ms.Read(b, 0, (int)ms.Length);
            string result = ConvertMDLCTToMol(b);
            //Then parse the mol file into the molecule
            MolFileReader.ReadMolData(result, ref molecule);
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

        private void addCompoundButton_Click(object sender, EventArgs e)
        {
            //Before anything else we check that the molecule is initialized either by formula or structure
            if (molecule == null)
            {
                MessageBox.Show("There is no molecular data, please enter a formula or paste a structure.", "Error");
                return;
            }
            //First we get all of the values in the text boxes
            double density = 0, massVal = 0, molsVal = 0, volVal = 0, concVal = 0;
            string refname = string.Empty;
            try
            {
                massVal = double.Parse(massTextBox.Text);
                molsVal = double.Parse(molsTextBox.Text);
                
                if(densityTextBox.Enabled)
                    density = double.Parse(densityTextBox.Text);
                if(volumeTextBox.Enabled)
                    volVal = double.Parse(volumeTextBox.Text);
                if(concentrationTextBox.Enabled)
                    concVal = double.Parse(concentrationTextBox.Text);
            }
            catch (FormatException)
            {
                MessageBox.Show("Some input was not a number, please check your input and add the compound again");
                return;
            }
            if (!string.IsNullOrEmpty(nameTextBox.Text))
                refname = nameTextBox.Text;
            //Build the new compound
            bool isLimiting = false;
            bool isTarget = false;
            if (typeSelection.SelectedIndex == (int)COMPOUND_TYPES.Product)
            {
                //if it is the product we get the checkbox value and set limiting to false
                isLimiting = false;
                isTarget = isLimitingCheckBox.Checked;
            }
            else
            {
                isLimiting = isLimitingCheckBox.Checked;
                isTarget = false;
            }
            //Finally build the dictionaries
            Unit mols, mass, volume;
        
            mols = new Unit(molsVal, "mol", molUnitSelection.SelectedIndex);
            
            mass = new Unit(massVal, "g", massUnitSelection.SelectedIndex);
            volume = new Unit(volVal, "l", volumeUnitSelection.SelectedIndex);
            Solution solution = new Solution(solventTextBox.Text, new Unit(concVal, "mol/l",(int)UNIT_POWERS.none));
            

            
            
            Compound comp = new Compound(molecule, refname, typeSelection.SelectedIndex, stateSelection.SelectedIndex, isLimiting, isTarget, density, mols, mass, volume, solution);
            if (comp.State == PHASE_STATE.Liquid)
                Console.WriteLine("Liquid, my mass is " + comp.Mass.ToString());
            parent.AddCompound(comp);
           
            this.Close();
        }

        private void stateSelection_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (stateSelection.SelectedIndex)
            {
                case 0:
                    //Solid case, we grey out volume, density, solution concentration
                    massTextBox.Enabled = true;
                    volumeTextBox.Enabled = false;
                    densityTextBox.Enabled = false;
                    concentrationTextBox.Enabled = false;
                    solventTextBox.Enabled = false;
                    break;

                case 1:
                    //Liquid case, we grey out mass and sol conc.
                    volumeTextBox.Enabled = true;
                    densityTextBox.Enabled = true;
                    massTextBox.Enabled = false;
                    concentrationTextBox.Enabled = false;
                    solventTextBox.Enabled = false;
                    break;
                case 2:
                    //Gas case NOT IMPLEMENTED
                    break;
                case 3:
                    //Solution case, grey out everything except volume, solution conc. and the solvent text box
                    volumeTextBox.Enabled = true;
                    concentrationTextBox.Enabled = true;
                    massTextBox.Enabled = false;
                    densityTextBox.Enabled = false;
                    solventTextBox.Enabled = true;
                    break;
                    
            }
        }

        private void typeSelection_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (typeSelection.SelectedIndex)
            {
                case (int)COMPOUND_TYPES.Reactant:
                    //When its a reactant the checkbox is limiting
                    isLimitingCheckBox.Text = "Is limiting reactant?";
                    break;
                case (int)COMPOUND_TYPES.Reagent:
                    //When its a reagent same as reactant
                    isLimitingCheckBox.Text = "Is limiting reagent?";
                    break;
                case (int)COMPOUND_TYPES.Product:
                    //And for products it is target, we also check the box
                    isLimitingCheckBox.Text = "Is target product?";
                    if(currentSynthesis.TargetProduct == null)
                        isLimitingCheckBox.Show();
                    isLimitingCheckBox.Checked = true;
                    break;
            }
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

       

        public void ShowError(string errorMessage)
        {
            MessageBox.Show(errorMessage, "Error");
        }

        private void molsTextBox_TextChanged(object sender, EventArgs e)
        {
            errorLabel.Text = string.Empty;
            //When the mol value changes we can compute the mass of the compound, so we do that and place the value in the mass box
            try
            {
                float mols = float.Parse(molsTextBox.Text);
                //m = n * M;
                float mass = (float)(mols * molecule.MolecularWeight);
                massTextBox.Text = mass.ToString();
              
            }
            catch (FormatException)
            {
                if (!String.IsNullOrEmpty(molsTextBox.Text))
                {
                    MessageBox.Show("The mol value entered was not a number, please retype the number and try again");
                    molsTextBox.Text = string.Empty;
                }
            }
        }

        private void massTextBox_TextChanged(object sender, EventArgs e)
        {
            errorLabel.Text = string.Empty; // changed later if there was a problem
            //When the mass value changes we can compute the mols of the compound used
            try
            {
                float mass = float.Parse(massTextBox.Text);
                //n = m / M
                float mols = (float)(mass / molecule.MolecularWeight);
                molsTextBox.Text = mols.ToString();
            }
            catch (FormatException)
            {
                if (String.IsNullOrEmpty(massTextBox.Text))
                {
                    MessageBox.Show("The mol or mass value entered was not a number, please retype the number and try again");
                    massTextBox.Text = string.Empty;
                }
            }
        }

        private void densityTextBox_TextChanged(object sender, EventArgs e)
        {
            try
            {
                double density = double.Parse(densityTextBox.Text);
                double mols = double.Parse(molsTextBox.Text);

                //v = n * M / density, density is in g / ml, so our result is in ml
                double volume = mols * molecule.MolecularWeight / density;

                volumeTextBox.Text = volume.ToString();
            }
            catch (FormatException)
            {
                MessageBox.Show("The density or mol value entered was not a number, please retype the number and try again");
                densityTextBox.Text = string.Empty;
            }
        }

        private void concentrationTextBox_TextChanged(object sender, EventArgs e)
        {
            try
            {
                double mols = double.Parse(molsTextBox.Text);
                double conc = double.Parse(concentrationTextBox.Text) * Math.Pow(10, -3); //convert to mol/ml

                double volume = mols / conc;
                volumeTextBox.Text = volume.ToString();
            }
            catch (FormatException)
            {
                MessageBox.Show("The mol or conc value entered was not a number, please retype and try again");
            }
        }

        private void molViewer_Load(object sender, EventArgs e)
        {

        }

     

        

        
    }
}
