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
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }
        public Form2(string message, string name, ListView listView_to, List<string> list_to, int type_form, List<string> list_action)
        {
            InitializeComponent();
            this.message = message;
            this.name = name;
            this.listView_to = listView_to;
            this.list_to = list_to;
            this.type_form = type_form;
            this.list_action = list_action;
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            switch (type_form)
            {
                case 1:
                    textBox1.Text = message + name + " в:";
                    DirectoryInfo dir1 = new DirectoryInfo(Program.active_dir(list_to));
                    DirectoryInfo[] dirList = dir1.GetDirectories();
                    //if(list_action.Count == 1)
                    //    comboBox1.Items.Add(Path.Combine(Program.active_dir(list_to), name));
                    //else
                        comboBox1.Items.Add(Program.active_dir(list_to));
                    foreach (DirectoryInfo item in dirList)
                        comboBox1.Items.Add(Program.active_dir(list_to) + "\\" + item.Name);
                    comboBox1.Text = comboBox1.Items[0].ToString();
                    break;
                case 2:
                    textBox1.Text = message + name + " в ";
                    comboBox1.Visible = false;
                    textBox2.Visible = true;
                   
                    break;
                default:
                    break;
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {

        }

        private void Form2_Shown(object sender, EventArgs e)
        {
            textBox2.Focus();
        }
    }
}
