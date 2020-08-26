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
        public Form2(string message, string name, ListView listView_to, List<string> list_to, List<string> list_on, int type_form, List<string> list_action)
        {
            InitializeComponent();
            this.message = message;
            this.name = name;
            this.listView_to = listView_to;
            this.list_to = list_to;
            this.list_on = list_on;
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
            progressBar1.Visible = true;
            string copy_to_name = comboBox1.Text;
            foreach (var item in list_action)
            {
                if (Path.HasExtension(Path.Combine(Program.active_dir(list_on) + item)))
                {
                    if (File.Exists(Path.Combine(Program.active_dir(list_on), item)))
                    {
                        for (int i = 0; i < 100000000; i++)
                        {
                            progressBar1.Value = i / 1000000;

                        }
                        //FileInfo f = new FileInfo(Path.Combine(Program.active_dir(list_on), item));
                        //long len = f.Length;
                        //StreamReader sr = new StreamReader(Path.Combine(Program.active_dir(list_on), item));
                        
                        //FileStream fs = new FileStream(Path.Combine(Program.active_dir(list_on), item),);
                        //StreamReader read = new StreamReader ()

                        //File.Copy(Path.Combine(Program.active_dir(list_on), item), Path.Combine(copy_to_name, item)); //Program.active_dir(list_to)
                    }
                       
                }
                else
                {
                    if (Directory.Exists(Path.Combine(Program.active_dir(list_to), item)) != true)
                        Directory.CreateDirectory(Path.Combine(Program.active_dir(list_to), item));
                    //CopyDir(Path.Combine(Program.active_dir(list_on), item), Path.Combine(copy_to_name, item));
                }
            }
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
