using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SynType.Chemical_Classes;

namespace SynType
{
    //A tab page control displays the data corresponding to a single synthesis
    public partial class TabPageControl : UserControl
    {
        public Synthesis currentSynthesis { get; set; }
        Form1 baseForm = null;
        public TabPageControl(Form1 form1, Synthesis synthesis)
        {
            InitializeComponent();
            baseForm = form1;
            //init the synthesis we are showing
            currentSynthesis = synthesis;
            currentSynthesis.SynthesisChanged += new SynthesisChangedEventHandler(currentSynthesis_SynthesisChanged);
            //next the data grid
            SetupDataGridView();
            //and then the reaction viewer
            reactionViewer1.Synth = currentSynthesis;
            reactionViewer1.Paint +=new PaintEventHandler(reactionViewer1.paint);
            //and finally the textboxes
            procedureInputBox.TextChanged += new EventHandler(procedureInputBox_TextChanged);
            
            UpdateControl();
        }

       
        //--------------------Setup Methods-----------------------------------------
        private void SetupDataGridView()
        {
            gridView.AutoGenerateColumns = false;

            //resize columns based on the info displayed

            //gridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells;


            DataGridViewTextBoxColumn formulaColumn = new DataGridViewTextBoxColumn();
            formulaColumn.DataPropertyName = "Formula";
            formulaColumn.HeaderText = "Formula";
            gridView.Columns.Add(formulaColumn);

            DataGridViewTextBoxColumn nameColumn = new DataGridViewTextBoxColumn();
            nameColumn.DataPropertyName = "RefName";
            nameColumn.HeaderText = "Name";

            gridView.Columns.Add(nameColumn);

            DataGridViewTextBoxColumn idColumn = new DataGridViewTextBoxColumn();
            idColumn.DataPropertyName = "LocalID";
            idColumn.HeaderText = "ID";
            gridView.Columns.Add(idColumn);

            DataGridViewTextBoxColumn mwColumn = new DataGridViewTextBoxColumn();
            mwColumn.DataPropertyName = "MolWeight";
            mwColumn.HeaderText = "MW (g/mol)";
            gridView.Columns.Add(mwColumn);

            DataGridViewTextBoxColumn typeColumn = new DataGridViewTextBoxColumn();
            typeColumn.DataPropertyName = "Type";
            typeColumn.HeaderText = "Type";
            gridView.Columns.Add(typeColumn);

            DataGridViewTextBoxColumn molColumn = new DataGridViewTextBoxColumn();
            molColumn.DataPropertyName = "Mols";
            molColumn.HeaderText = "n";
            gridView.Columns.Add(molColumn);

            DataGridViewTextBoxColumn equivColumn = new DataGridViewTextBoxColumn();
            equivColumn.DataPropertyName = "Equivalency";
            equivColumn.HeaderText = "equiv.";
            gridView.Columns.Add(equivColumn);

            DataGridViewTextBoxColumn stateColumn = new DataGridViewTextBoxColumn();
            stateColumn.DataPropertyName = "State";
            stateColumn.HeaderText = "State";
            gridView.Columns.Add(stateColumn);

            DataGridViewTextBoxColumn massColumn = new DataGridViewTextBoxColumn();
            massColumn.DataPropertyName = "Mass";

            massColumn.HeaderText = "m";
            gridView.Columns.Add(massColumn);

            DataGridViewTextBoxColumn densityColumn = new DataGridViewTextBoxColumn();
            densityColumn.DataPropertyName = "Density";
            densityColumn.HeaderText = "d (g/ml)";
            gridView.Columns.Add(densityColumn);

            DataGridViewTextBoxColumn volumeColumn = new DataGridViewTextBoxColumn();
            volumeColumn.DataPropertyName = "Volume";
            volumeColumn.HeaderText = "vol (ml)";
            gridView.Columns.Add(volumeColumn);

            DataGridViewTextBoxColumn solutionColumn = new DataGridViewTextBoxColumn();
            solutionColumn.DataPropertyName = "SolutionConc";
            solutionColumn.HeaderText = "Sol. Conc.";
            gridView.Columns.Add(solutionColumn);

            //make the columns fit the control size
            float width = (gridView.Width - 30) / 12;
            foreach (DataGridViewColumn col in gridView.Columns)
            {
                col.Width = (int)width;
            }
            // gridView.DataSource = currentSynthesis.GetCompoundTable();
            gridView.EditMode = DataGridViewEditMode.EditOnEnter;

            gridView.CellValueChanged += new DataGridViewCellEventHandler(gridView_CellValueChanged);
            gridView.UserDeletingRow += new DataGridViewRowCancelEventHandler(gridView_UserDeletingRow);
            gridView.CellFormatting += new DataGridViewCellFormattingEventHandler(gridView_CellFormatting);
            gridView.AllowUserToDeleteRows = true;
        }

       
        //-------------------Other Methods------------------------------------------
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
            gridView.DataSource = currentSynthesis.AllCompounds;
            reactionViewer1.Synth = currentSynthesis;
            reactionViewer1.UpdateControl();

            Console.WriteLine("AddCompound reac {0}, reag {1}, prod {2}", currentSynthesis.Reactants.Count, currentSynthesis.Reagents.Count, currentSynthesis.Products.Count);
        }
        public void LoadSynthesis(Synthesis synthesis)
        {
            currentSynthesis = synthesis;
            currentSynthesis_SynthesisChanged(this, EventArgs.Empty);
        }

        private void UpdateControl()
        {
            //update the gridview, the reaction viewer and the text boxes
            gridView.DataSource = typeof(List<Compound>);
            gridView.DataSource = currentSynthesis.AllCompounds;
            reactionViewer1.Synth = currentSynthesis;
            reactionViewer1.UpdateControl();
            procedureInputBox.Text = currentSynthesis.Proc.RawText;
            procedureOutputBox.Text = currentSynthesis.Proc.ParsedText;
            Invalidate();
        }
        
        //-------------------Event Handlers-----------------------------------------
        //handles the change of a synthesis by updating all the components on this component
        void currentSynthesis_SynthesisChanged(object sender, EventArgs e)
        {
            Console.WriteLine("TabPageControl Current synthesis changed being called!");
            UpdateControl();
        }

        //handles the coloring of the data grid view rows, called every paint
        void gridView_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            //This is called every paint, and we make sure the isLimiting reactant is painted yellow
            if (e.RowIndex >= currentSynthesis.AllCompounds.Count)
                return;
            if (currentSynthesis.AllCompounds[e.RowIndex].IsLimiting)
            {
                e.CellStyle.BackColor = Color.LightYellow;

            }
            //other reactants are painted light blue
            if (!currentSynthesis.AllCompounds[e.RowIndex].IsLimiting && currentSynthesis.AllCompounds[e.RowIndex].Type == COMPOUND_TYPES.Reactant)
            {
                e.CellStyle.BackColor = Color.LightBlue;
            }
            //reagents are painted light salmon
            if (currentSynthesis.AllCompounds[e.RowIndex].Type == COMPOUND_TYPES.Reagent)
            {
                e.CellStyle.BackColor = Color.LightSalmon;
            }
            //the target product is colored light green
            if (currentSynthesis.AllCompounds[e.RowIndex].IsTarget)
            {
                e.CellStyle.BackColor = Color.LightGreen;
            }
            //other products are gray
            if (!currentSynthesis.AllCompounds[e.RowIndex].IsTarget && currentSynthesis.AllCompounds[e.RowIndex].Type == COMPOUND_TYPES.Product)
            {
                e.CellStyle.BackColor = Color.LightGray;
            }
        }
        //handles when a user changes a cell's value
        void gridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            Console.WriteLine("Cell value changed " + e.RowIndex + " " + e.ColumnIndex + " " + e.ToString());
            //print the compound that has changed
            Console.WriteLine("Compound info for changed row " + currentSynthesis.AllCompounds[e.RowIndex].ToString());
        }
        //handles the user deleting a row - just lets us know it happens currently
        void gridView_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            Console.WriteLine("User deleting a row");
        }
        //When the text is changed in the procedure input, we parse and send that to the output
        void procedureInputBox_TextChanged(object sender, EventArgs e)
        {
            currentSynthesis.Proc.RawText = procedureInputBox.Text;
            procedureOutputBox.Text = currentSynthesis.Proc.ParsedText;
        }
        //DataGridView context menu event handler for deleting a compound from the synthesis
        private void deleteCompoundItem_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in gridView.SelectedRows)
            {
                Compound comp = row.DataBoundItem as Compound;
                if (comp != null)
                {
                    currentSynthesis.RemoveCompound(comp);
                }
            }
           
        }
        //DataGridView context menu event handler for adding a compound to the synthesis
        private void addNewCompoundItem_Click(object sender, EventArgs e)
        {
            AddCompoundForm addCompoundForm = new AddCompoundForm(baseForm, currentSynthesis);
            addCompoundForm.Show();
        }
    }
}
