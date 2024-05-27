using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WindowsFormsApplication1
{
    public partial class gridStepDialog : Form
    {
        public int gridStep = 10;

        public gridStepDialog()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            gridStep = (int)numericUpDown1.Value;
        }

        private void gridStepDialog_Shown(object sender, EventArgs e)
        {
            numericUpDown1.Value = gridStep;
        }
    }
}
