using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace SGCommander
{
    public partial class Form4 : Form
    {
        public Form4()
        {
            InitializeComponent();
        }

        public Form4(string file)
        {
            InitializeComponent();
            List<string> arrpictures = new List<string> { ".jpg", ".png", ".ico", ".gif", ".bmp" };
            if(arrpictures.Contains(Path.GetExtension(file).ToLower()))
            {
                pictureBox1.Visible = true;
                textBox1.Visible = false;
                pictureBox1.Dock = DockStyle.Fill;
                pictureBox1.SizeMode = PictureBoxSizeMode.AutoSize;
                pictureBox1.Image = new Bitmap(file);
            }
            else
            {
                pictureBox1.Visible = false;
                textBox1.Visible = true;
                textBox1.Text = File.ReadAllText(file);
            }
            this.Text = "SG View - SG Commander " + file;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
