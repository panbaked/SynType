namespace SynType
{
    partial class TabPageControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.gridView = new System.Windows.Forms.DataGridView();
            this.gvContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.addNewCompoundItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteCompoundItem = new System.Windows.Forms.ToolStripMenuItem();
            this.procedureInputBox = new System.Windows.Forms.RichTextBox();
            this.procedureOutputBox = new System.Windows.Forms.RichTextBox();
            this.reactionViewer1 = new SynType.ReactionViewer();
            ((System.ComponentModel.ISupportInitialize)(this.gridView)).BeginInit();
            this.gvContextMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // gridView
            // 
            this.gridView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.gridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridView.ContextMenuStrip = this.gvContextMenuStrip;
            this.gridView.Location = new System.Drawing.Point(4, 210);
            this.gridView.Name = "gridView";
            this.gridView.Size = new System.Drawing.Size(1139, 195);
            this.gridView.TabIndex = 1;
            // 
            // gvContextMenuStrip
            // 
            this.gvContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addNewCompoundItem,
            this.deleteCompoundItem});
            this.gvContextMenuStrip.Name = "gvContextMenuStrip";
            this.gvContextMenuStrip.Size = new System.Drawing.Size(188, 48);
            // 
            // addNewCompoundItem
            // 
            this.addNewCompoundItem.Name = "addNewCompoundItem";
            this.addNewCompoundItem.Size = new System.Drawing.Size(187, 22);
            this.addNewCompoundItem.Text = "Add New Compound";
            this.addNewCompoundItem.Click += new System.EventHandler(this.addNewCompoundItem_Click);
            // 
            // deleteCompoundItem
            // 
            this.deleteCompoundItem.Name = "deleteCompoundItem";
            this.deleteCompoundItem.Size = new System.Drawing.Size(187, 22);
            this.deleteCompoundItem.Text = "Delete Compound";
            this.deleteCompoundItem.Click += new System.EventHandler(this.deleteCompoundItem_Click);
            // 
            // procedureInputBox
            // 
            this.procedureInputBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.procedureInputBox.Location = new System.Drawing.Point(4, 428);
            this.procedureInputBox.Name = "procedureInputBox";
            this.procedureInputBox.Size = new System.Drawing.Size(539, 247);
            this.procedureInputBox.TabIndex = 2;
            this.procedureInputBox.Text = "";
            // 
            // procedureOutputBox
            // 
            this.procedureOutputBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.procedureOutputBox.Location = new System.Drawing.Point(550, 428);
            this.procedureOutputBox.Name = "procedureOutputBox";
            this.procedureOutputBox.ReadOnly = true;
            this.procedureOutputBox.Size = new System.Drawing.Size(593, 247);
            this.procedureOutputBox.TabIndex = 3;
            this.procedureOutputBox.Text = "";
            // 
            // reactionViewer1
            // 
            this.reactionViewer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.reactionViewer1.Location = new System.Drawing.Point(3, 3);
            this.reactionViewer1.Name = "reactionViewer1";
            this.reactionViewer1.Size = new System.Drawing.Size(1140, 200);
            this.reactionViewer1.Synth = null;
            this.reactionViewer1.TabIndex = 0;
            // 
            // TabPageControl
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Controls.Add(this.procedureOutputBox);
            this.Controls.Add(this.procedureInputBox);
            this.Controls.Add(this.gridView);
            this.Controls.Add(this.reactionViewer1);
            this.Name = "TabPageControl";
            this.Size = new System.Drawing.Size(1146, 678);
            ((System.ComponentModel.ISupportInitialize)(this.gridView)).EndInit();
            this.gvContextMenuStrip.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private ReactionViewer reactionViewer1;
        private System.Windows.Forms.DataGridView gridView;
        private System.Windows.Forms.RichTextBox procedureInputBox;
        private System.Windows.Forms.RichTextBox procedureOutputBox;
        private System.Windows.Forms.ContextMenuStrip gvContextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem addNewCompoundItem;
        private System.Windows.Forms.ToolStripMenuItem deleteCompoundItem;
    }
}
