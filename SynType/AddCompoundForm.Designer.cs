namespace SynType
{
    partial class AddCompoundForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.typeSelection = new System.Windows.Forms.ComboBox();
            this.pasteMDL = new System.Windows.Forms.Button();
            this.formulaTextBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.molWeightLabel = new System.Windows.Forms.Label();
            this.addCompoundButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.molsTextBox = new System.Windows.Forms.TextBox();
            this.massTextBox = new System.Windows.Forms.TextBox();
            this.volumeTextBox = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.stateSelection = new System.Windows.Forms.ComboBox();
            this.label8 = new System.Windows.Forms.Label();
            this.densityTextBox = new System.Windows.Forms.TextBox();
            this.concentrationTextBox = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.isLimitingCheckBox = new System.Windows.Forms.CheckBox();
            this.molUnitSelection = new System.Windows.Forms.ComboBox();
            this.massUnitSelection = new System.Windows.Forms.ComboBox();
            this.volumeUnitSelection = new System.Windows.Forms.ComboBox();
            this.nameTextBox = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.solventTextBox = new System.Windows.Forms.TextBox();
            this.errorLabel = new System.Windows.Forms.Label();
            this.molViewer = new SynType.MolViewer();
            this.SuspendLayout();
            // 
            // typeSelection
            // 
            this.typeSelection.FormattingEnabled = true;
            this.typeSelection.Location = new System.Drawing.Point(70, 49);
            this.typeSelection.Name = "typeSelection";
            this.typeSelection.Size = new System.Drawing.Size(121, 21);
            this.typeSelection.TabIndex = 2;
            this.typeSelection.SelectedIndexChanged += new System.EventHandler(this.typeSelection_SelectedIndexChanged);
            // 
            // pasteMDL
            // 
            this.pasteMDL.Location = new System.Drawing.Point(12, 12);
            this.pasteMDL.Name = "pasteMDL";
            this.pasteMDL.Size = new System.Drawing.Size(121, 23);
            this.pasteMDL.TabIndex = 0;
            this.pasteMDL.Text = "Paste MDL";
            this.pasteMDL.UseVisualStyleBackColor = true;
            this.pasteMDL.Click += new System.EventHandler(this.pasteMDL_Click);
            // 
            // formulaTextBox
            // 
            this.formulaTextBox.Location = new System.Drawing.Point(70, 103);
            this.formulaTextBox.Name = "formulaTextBox";
            this.formulaTextBox.Size = new System.Drawing.Size(118, 20);
            this.formulaTextBox.TabIndex = 6;
           
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(11, 49);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(31, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Type";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 103);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(44, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Formula";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(9, 126);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(61, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "Mol Weight";
            // 
            // molWeightLabel
            // 
            this.molWeightLabel.AutoSize = true;
            this.molWeightLabel.Location = new System.Drawing.Point(70, 126);
            this.molWeightLabel.Name = "molWeightLabel";
            this.molWeightLabel.Size = new System.Drawing.Size(0, 13);
            this.molWeightLabel.TabIndex = 8;
            // 
            // addCompoundButton
            // 
            this.addCompoundButton.Location = new System.Drawing.Point(12, 348);
            this.addCompoundButton.Name = "addCompoundButton";
            this.addCompoundButton.Size = new System.Drawing.Size(98, 23);
            this.addCompoundButton.TabIndex = 25;
            this.addCompoundButton.Text = "Add Compound";
            this.addCompoundButton.UseVisualStyleBackColor = true;
            this.addCompoundButton.Click += new System.EventHandler(this.addCompoundButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.Location = new System.Drawing.Point(116, 348);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 26;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(9, 171);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(29, 13);
            this.label4.TabIndex = 9;
            this.label4.Text = "Mols";
            // 
            // molsTextBox
            // 
            this.molsTextBox.Location = new System.Drawing.Point(70, 171);
            this.molsTextBox.Name = "molsTextBox";
            this.molsTextBox.Size = new System.Drawing.Size(100, 20);
            this.molsTextBox.TabIndex = 10;
            this.molsTextBox.TextChanged += new System.EventHandler(this.molsTextBox_TextChanged);
            // 
            // massTextBox
            // 
            this.massTextBox.Location = new System.Drawing.Point(70, 197);
            this.massTextBox.Name = "massTextBox";
            this.massTextBox.Size = new System.Drawing.Size(100, 20);
            this.massTextBox.TabIndex = 13;
            this.massTextBox.TextChanged += new System.EventHandler(this.massTextBox_TextChanged);
            // 
            // volumeTextBox
            // 
            this.volumeTextBox.Location = new System.Drawing.Point(70, 223);
            this.volumeTextBox.Name = "volumeTextBox";
            this.volumeTextBox.Size = new System.Drawing.Size(100, 20);
            this.volumeTextBox.TabIndex = 16;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(9, 197);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(32, 13);
            this.label5.TabIndex = 12;
            this.label5.Text = "Mass";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(9, 223);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(42, 13);
            this.label6.TabIndex = 15;
            this.label6.Text = "Volume";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(12, 76);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(32, 13);
            this.label7.TabIndex = 3;
            this.label7.Text = "State";
            // 
            // stateSelection
            // 
            this.stateSelection.FormattingEnabled = true;
            this.stateSelection.Location = new System.Drawing.Point(70, 76);
            this.stateSelection.Name = "stateSelection";
            this.stateSelection.Size = new System.Drawing.Size(121, 21);
            this.stateSelection.TabIndex = 4;
            this.stateSelection.SelectedIndexChanged += new System.EventHandler(this.stateSelection_SelectedIndexChanged);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(9, 249);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(42, 13);
            this.label8.TabIndex = 18;
            this.label8.Text = "Density";
            // 
            // densityTextBox
            // 
            this.densityTextBox.Location = new System.Drawing.Point(70, 249);
            this.densityTextBox.Name = "densityTextBox";
            this.densityTextBox.Size = new System.Drawing.Size(100, 20);
            this.densityTextBox.TabIndex = 19;
            this.densityTextBox.TextChanged += new System.EventHandler(this.densityTextBox_TextChanged);
            // 
            // concentrationTextBox
            // 
            this.concentrationTextBox.Location = new System.Drawing.Point(70, 275);
            this.concentrationTextBox.Name = "concentrationTextBox";
            this.concentrationTextBox.Size = new System.Drawing.Size(100, 20);
            this.concentrationTextBox.TabIndex = 21;
            this.concentrationTextBox.TextChanged += new System.EventHandler(this.concentrationTextBox_TextChanged);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(11, 275);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(53, 13);
            this.label9.TabIndex = 20;
            this.label9.Text = "Sol Conc.";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(176, 249);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(28, 13);
            this.label13.TabIndex = 24;
            this.label13.Text = "g/ml";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(176, 275);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(34, 13);
            this.label14.TabIndex = 25;
            this.label14.Text = "mol/L";
            // 
            // isLimitingCheckBox
            // 
            this.isLimitingCheckBox.AutoSize = true;
            this.isLimitingCheckBox.Checked = true;
            this.isLimitingCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.isLimitingCheckBox.Location = new System.Drawing.Point(12, 325);
            this.isLimitingCheckBox.Name = "isLimitingCheckBox";
            this.isLimitingCheckBox.Size = new System.Drawing.Size(80, 17);
            this.isLimitingCheckBox.TabIndex = 24;
            this.isLimitingCheckBox.Text = "checkBox1";
            this.isLimitingCheckBox.UseVisualStyleBackColor = true;
            // 
            // molUnitSelection
            // 
            this.molUnitSelection.FormattingEnabled = true;
            this.molUnitSelection.Location = new System.Drawing.Point(176, 171);
            this.molUnitSelection.Name = "molUnitSelection";
            this.molUnitSelection.Size = new System.Drawing.Size(47, 21);
            this.molUnitSelection.TabIndex = 11;
            // 
            // massUnitSelection
            // 
            this.massUnitSelection.FormattingEnabled = true;
            this.massUnitSelection.Location = new System.Drawing.Point(176, 196);
            this.massUnitSelection.Name = "massUnitSelection";
            this.massUnitSelection.Size = new System.Drawing.Size(47, 21);
            this.massUnitSelection.TabIndex = 14;
            // 
            // volumeUnitSelection
            // 
            this.volumeUnitSelection.FormattingEnabled = true;
            this.volumeUnitSelection.Location = new System.Drawing.Point(176, 220);
            this.volumeUnitSelection.Name = "volumeUnitSelection";
            this.volumeUnitSelection.Size = new System.Drawing.Size(47, 21);
            this.volumeUnitSelection.TabIndex = 17;
            // 
            // nameTextBox
            // 
            this.nameTextBox.Location = new System.Drawing.Point(70, 145);
            this.nameTextBox.Name = "nameTextBox";
            this.nameTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Horizontal;
            this.nameTextBox.Size = new System.Drawing.Size(153, 20);
            this.nameTextBox.TabIndex = 8;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(9, 145);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(35, 13);
            this.label10.TabIndex = 7;
            this.label10.Text = "Name";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(12, 302);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(43, 13);
            this.label11.TabIndex = 22;
            this.label11.Text = "Solvent";
            // 
            // solventTextBox
            // 
            this.solventTextBox.Location = new System.Drawing.Point(70, 302);
            this.solventTextBox.Name = "solventTextBox";
            this.solventTextBox.Size = new System.Drawing.Size(100, 20);
            this.solventTextBox.TabIndex = 23;
            // 
            // errorLabel
            // 
            this.errorLabel.AutoSize = true;
            this.errorLabel.Location = new System.Drawing.Point(231, 357);
            this.errorLabel.Name = "errorLabel";
            this.errorLabel.Size = new System.Drawing.Size(0, 13);
            this.errorLabel.TabIndex = 32;
            // 
            // molViewer
            // 
            this.molViewer.Location = new System.Drawing.Point(249, 20);
            this.molViewer.Mol = null;
            this.molViewer.Name = "molViewer";
            this.molViewer.Size = new System.Drawing.Size(275, 275);
            this.molViewer.TabIndex = 31;
            this.molViewer.Load += new System.EventHandler(this.molViewer_Load);
            // 
            // AddCompoundForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(534, 383);
            this.Controls.Add(this.errorLabel);
            this.Controls.Add(this.solventTextBox);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.nameTextBox);
            this.Controls.Add(this.molViewer);
            this.Controls.Add(this.volumeUnitSelection);
            this.Controls.Add(this.massUnitSelection);
            this.Controls.Add(this.molUnitSelection);
            this.Controls.Add(this.isLimitingCheckBox);
            this.Controls.Add(this.label14);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.concentrationTextBox);
            this.Controls.Add(this.densityTextBox);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.stateSelection);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.volumeTextBox);
            this.Controls.Add(this.massTextBox);
            this.Controls.Add(this.molsTextBox);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.addCompoundButton);
            this.Controls.Add(this.molWeightLabel);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.formulaTextBox);
            this.Controls.Add(this.pasteMDL);
            this.Controls.Add(this.typeSelection);
            this.Name = "AddCompoundForm";
            this.Text = "AddCompoundForm";
            this.Load += new System.EventHandler(this.AddCompoundForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox typeSelection;
        private System.Windows.Forms.Button pasteMDL;
        private System.Windows.Forms.TextBox formulaTextBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label molWeightLabel;
        private System.Windows.Forms.Button addCompoundButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox molsTextBox;
        private System.Windows.Forms.TextBox massTextBox;
        private System.Windows.Forms.TextBox volumeTextBox;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.ComboBox stateSelection;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox densityTextBox;
        private System.Windows.Forms.TextBox concentrationTextBox;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.CheckBox isLimitingCheckBox;
        private System.Windows.Forms.ComboBox molUnitSelection;
        private System.Windows.Forms.ComboBox massUnitSelection;
        private System.Windows.Forms.ComboBox volumeUnitSelection;
        private MolViewer molViewer;
        private System.Windows.Forms.TextBox nameTextBox;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox solventTextBox;
        private System.Windows.Forms.Label errorLabel;
    }
}