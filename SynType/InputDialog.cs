using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SynType
{
    public partial class InputDialog : Form
    {
        public string Input { get { return inputTextBox.Text; } }
        public InputDialog(string info)
        {
            InitializeComponent();
            infoLabel.Text = info;
        }

       
    }
}
