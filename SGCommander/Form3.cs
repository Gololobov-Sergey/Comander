using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SGCommander
{
    public partial class Form3 : Form
    {
        public Form3()
        {
            InitializeComponent();
        }
        public Form3(ref Settings set)
        {
            InitializeComponent();
            this.set = set;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch(listBox1.SelectedIndex)
            {
                case 0:
                    panel1.Visible = true;
                    break;
                case 1:
                    break;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            set.Show_hidden_elem = checkBox1.Checked;
        }

        private void Form3_Load(object sender, EventArgs e)
        {
            checkBox1.Checked = set.Show_hidden_elem;
        }
    }
}
