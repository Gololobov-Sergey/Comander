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
using System.Xml.Serialization;
using System.Runtime.Serialization.Formatters.Soap;
using System.Runtime.InteropServices;
using System.Diagnostics;


namespace SGCommander
{
    public partial class Form1 : Form
    {
        private int indexOfItemUnderMouseToDrag;
        private Rectangle dragBoxFromMouseDown;
        Settings set = null;
        public Form1()
        {
            InitializeComponent();
            /// load Settings 
            set = new Settings();
            FileStream fs = new FileStream("settings.xml", FileMode.Open, FileAccess.Read);
            SoapFormatter formatter = new SoapFormatter();
            set = (Settings)formatter.Deserialize(fs);
            fs.Close();
            columnHeader1.Width = columnHeader6.Width = set.Column_name;
            columnHeader2.Width = columnHeader7.Width = set.Column_type;
            columnHeader3.Width = columnHeader8.Width = set.Column_size;
            columnHeader4.Width = columnHeader9.Width = set.Column_date;
            columnHeader5.Width = columnHeader10.Width = set.Column_attributes;
            switch (set.List1_sort)
            {
                case 0: listView1.Columns[0].Text = "↓Имя"; break;
                case 1: listView1.Columns[0].Text = "↑Имя"; break;
                case 2: listView1.Columns[1].Text = "↓Тип"; break;
                case 3: listView1.Columns[1].Text = "↑Тип"; break;
                case 4: listView1.Columns[2].Text = "↓Размер"; break;
                case 5: listView1.Columns[2].Text = "↑Размер"; break;
                case 6: listView1.Columns[3].Text = "↓Дата"; break;
                case 7: listView1.Columns[3].Text = "↑Дата"; break;
                default: break;
            }
            switch (set.List2_sort)
            {
                case 0: listView2.Columns[0].Text = "↓Имя"; break;
                case 1: listView2.Columns[0].Text = "↑Имя"; break;
                case 2: listView2.Columns[1].Text = "↓Тип"; break;
                case 3: listView2.Columns[1].Text = "↑Тип"; break;
                case 4: listView2.Columns[2].Text = "↓Размер"; break;
                case 5: listView2.Columns[2].Text = "↑Размер"; break;
                case 6: listView2.Columns[3].Text = "↓Дата"; break;
                case 7: listView2.Columns[3].Text = "↑Дата"; break;
                default: break;
            }

            splitContainer1.SplitterDistance = splitContainer1.Width / 2;
            comboBox1.Items.AddRange(logicDriver);
            comboBox2.Items.AddRange(logicDriver);
            for (int i = 0; i < logicDriver.Length; i++ )
            {
                if(logicDriver[i].IsReady)
                {
                    comboBox1.SelectedIndex = i;
                    comboBox2.SelectedIndex = i;
                    list_l.Add(logicDriver[i].RootDirectory.ToString()); list_r.Add(logicDriver[i].RootDirectory.ToString());
                    break;
                }
            }
            SetDir(Program.active_dir(list_l), listView1, set);
            SetDir(Program.active_dir(list_r), listView2, set);
            comboBox1.SelectedIndexChanged += new System.EventHandler(comboBox1_SelectedIndexChanged);
            comboBox2.SelectedIndexChanged += new System.EventHandler(comboBox2_SelectedIndexChanged);
            driverButton = new Button[logicDriver.Length * 2];
            for (int i = 0; i < logicDriver.Length * 2; i++)
            {
                driverButton[i] = new System.Windows.Forms.Button();
                driverButton[i].UseVisualStyleBackColor = true;
                string name_but = logicDriver[i / 2].RootDirectory.ToString().TrimEnd('\\');
                driverButton[i].Text = name_but;
                driverButton[i].TextAlign = ContentAlignment.MiddleRight;
                driverButton[i].ImageAlign = ContentAlignment.MiddleLeft;
                driverButton[i].Width = 40;
                driverButton[i].Height = 21;
                driverButton[i].Margin = new System.Windows.Forms.Padding(1);
                driverButton[i].ImageList = imageList1;
                switch (logicDriver[i / 2].DriveType)
                {
                    case DriveType.Fixed:
                        driverButton[i].ImageIndex = 1;
                        break;
                    case DriveType.CDRom:
                        driverButton[i].ImageIndex = 2;
                        break;
                    case DriveType.Network:
                        driverButton[i].ImageIndex = 3;
                        break;
                    case DriveType.Removable:
                        driverButton[i].ImageIndex = 4;
                        break;
                }

                if (i % 2 == 0)
                {
                    panel1.Controls.Add(driverButton[i]);
                    driverButton[i].Location = new Point(i * 20, 2);
                }
                else
                {
                    panel2.Controls.Add(driverButton[i]);
                    driverButton[i].Location = new Point(i / 2 * 40, 2);
                }
                driverButton[i].Tag = i;
                if (logicDriver[i / 2].IsReady)
                    toolTip1.SetToolTip(driverButton[i], logicDriver[i / 2].VolumeLabel);
                driverButton[i].Click += new EventHandler(driverButton_Click);
                driverButton[i].Click += new EventHandler(label56_update);
            }
            toolStripStatusLabel1.Text = Program.active_dir(list_l);
            listView1.View = set.list1_view;
            listView2.View = set.list2_view;
        }

        private void driverButton_Click(object sender, EventArgs e)
        {
            if ((int)((Button)sender).Tag % 2 == 0)
            {
                comboBox1.Text = ((Button)sender).Text;
                list_l.Clear();
                list_l.Add(((Button)sender).Text + "\\");
                SetDir(Program.active_dir(list_l), listView1, set);
                label3.Text = Program.active_dir(list_l);
                listView1.Items[0].Focused = true;
                listView1.Items[0].Selected = true;

            }
            else
            {
                comboBox2.Text = ((Button)sender).Text;
                list_r.Clear();
                list_r.Add(((Button)sender).Text + "\\");
                SetDir(Program.active_dir(list_r), listView2, set);
                label4.Text = Program.active_dir(list_r);
                listView2.Items[0].Focused = true;
                listView2.Items[0].Selected = true;
            }
            toolStripStatusLabel1.Text = ((Button)sender).Text;

        }

        public void SetDir(string dir, ListView List, Settings set)
        {
            try
            {
                IntPtr hImgSmall; //the handle to the system image listIntPtr hImgLarge; //the handle to the system image list
                SHFILEINFO shinfo = new SHFILEINFO();

                long file_size = 0;
                int count_folder = 0;
                int count_files = 0;
                List.Items.Clear();
                List<FileSystemInfo> dirFolder = new List<FileSystemInfo>();
                ListViewItem list1 = new ListViewItem("[..]");
                list1.SubItems.Add("");
                list1.SubItems.Add("<Folder>");
                list1.SubItems.Add("");
                list1.SubItems.Add("");
                list1.ImageIndex = 5;
                List.Items.Add(list1);

                dirFolder = setListDir(dir, List, set);
                foreach (FileSystemInfo item in dirFolder)
                {
                    if (item is FileInfo)
                    {
                        list1 = new ListViewItem(Path.GetFileNameWithoutExtension(item.Name));
                        list1.SubItems.Add(item.Extension.TrimStart('.'));
                        list1.SubItems.Add(((FileInfo)item).Length.ToString());
                        if (item.Extension == ".exe")
                        {
                            hImgSmall = Win32.SHGetFileInfo(item.FullName, 0, ref shinfo, (uint)Marshal.SizeOf(shinfo), Win32.SHGFI_ICON | Win32.SHGFI_SMALLICON);
                            System.Drawing.Icon myIcon = System.Drawing.Icon.FromHandle(shinfo.hIcon);
                            imageList1.Images.Add(item.Extension, myIcon);
                            list1.ImageIndex = imageList1.Images.Count - 1;
                        }
                        else
                        {
                            if (imageList1.Images.ContainsKey(item.Extension))
                                list1.ImageIndex = imageList1.Images.IndexOfKey(item.Extension);
                            else
                            {
                                hImgSmall = Win32.SHGetFileInfo(item.FullName, 0, ref shinfo, (uint)Marshal.SizeOf(shinfo), Win32.SHGFI_ICON | Win32.SHGFI_SMALLICON);
                                System.Drawing.Icon myIcon = System.Drawing.Icon.FromHandle(shinfo.hIcon);
                                imageList1.Images.Add(item.Extension, myIcon);
                                list1.ImageIndex = imageList1.Images.Count - 1;
                            }
                        }

                        file_size += ((FileInfo)item).Length;
                        count_files++;

                    }
                    else
                    {
                        list1 = new ListViewItem(item.Name);
                        list1.SubItems.Add("");
                        list1.SubItems.Add("<Folder>");
                        if ((item.Attributes & FileAttributes.Hidden) == FileAttributes.Hidden)
                            list1.ImageIndex = 6;
                        else
                            list1.ImageIndex = 0;
                        count_folder++;
                    }

                    list1.SubItems.Add(item.LastWriteTime.ToShortDateString());
                    list1.SubItems.Add(item.Attributes.ToString());
                    List.Items.Add(list1);
                }
                string tmp_label = "0 is " + (int)(file_size / 1000) + " Kb, 0 is " + count_folder + " folder, 0 is " + count_files + " files";
                if ((string)List.Tag == "1")
                {
                    label1.Text = tmp_label;
                    label3.Text = Program.active_dir(list_l);
                }

                else
                {
                    label2.Text = tmp_label;
                    label4.Text = Program.active_dir(list_r);
                }

            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }

        }

        private List<FileSystemInfo> setListDir(string dir, ListView List, Settings set)
        {
            DirectoryInfo dir1 = new DirectoryInfo(dir);
            FileSystemInfo[] dirList = dir1.GetFileSystemInfos();
            List<FileSystemInfo> dirFolder = new List<FileSystemInfo>();
            List<FileInfo> dirFiles = new List<FileInfo>();

            for (int i = 0; i < dirList.Length; i++)
            {
                if (set.Show_hidden_elem)
                {
                    if ((dirList[i].Attributes & FileAttributes.Directory) == FileAttributes.Directory)
                        dirFolder.Add(dirList[i]);
                    else
                        dirFiles.Add((FileInfo)dirList[i]);
                }
                else
                {
                    if ((dirList[i].Attributes & FileAttributes.Hidden) != FileAttributes.Hidden)
                    {
                        if ((dirList[i].Attributes & FileAttributes.Directory) == FileAttributes.Directory)
                            dirFolder.Add(dirList[i]);
                        else
                            dirFiles.Add((FileInfo)dirList[i]);
                    }
                }
            }
            IOrderedEnumerable<FileInfo> sort = null;
            int sort_ = set.List2_sort;
            if ((string)List.Tag == "1")
                sort_ = set.List1_sort;

            switch (sort_)
            {
                case 0:
                    sort = dirFiles.OrderByDescending(a => a.Name);
                    break;
                case 1:
                    sort = dirFiles.OrderBy(a => a.Name);
                    break;
                case 2:
                    sort = dirFiles.OrderByDescending(a => a.Extension);
                    break;
                case 3:
                    sort = dirFiles.OrderBy(a => a.Extension);
                    break;
                case 4:
                    sort = dirFiles.OrderByDescending(a => a.Length);
                    break;
                case 5:
                    sort = dirFiles.OrderBy(a => a.Length);
                    break;
                case 6:
                    sort = dirFiles.OrderByDescending(a => a.LastWriteTime);
                    break;
                case 7:
                    sort = dirFiles.OrderBy(a => a.LastWriteTime);
                    break;
            }

            dirFolder.AddRange(sort);
            if ((string)List.Tag == "1")
                setListDir1 = dirFolder;
            else
                setListDir2 = dirFolder;
            return dirFolder;
        }


        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            list_r.Clear();
            list_r.Add(comboBox2.Text);
            SetDir(Program.active_dir(list_r), listView2, set);
            label4.Text = Program.active_dir(list_r);
            listView2.Focus();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            list_l.Clear();
            list_l.Add(comboBox1.Text);
            SetDir(Program.active_dir(list_l), listView1, set);
            label3.Text = Program.active_dir(list_l);
            listView2.Items[0].Focused = true;
            listView2.Items[0].Selected = true;
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e) //F3
        {
            string file;
            if (active_listView == 1)
            {
                int index = listView1.FocusedItem.Index;
                file = setListDir1[index - 1].FullName;
            }
            else
            {
                int index = listView2.FocusedItem.Index;
                file = setListDir2[index - 1].FullName;
            }
            if (Path.HasExtension(file))
            {
                Form4 form4 = new Form4(file);
                form4.Show();
            }
            else
                return;
        }

        private void button2_Click(object sender, EventArgs e) //F4
        {
            string file;
            if (active_listView == 1)
            {
                int index = listView1.FocusedItem.Index;
                file = setListDir1[index - 1].FullName;
            }
            else
            {
                int index = listView2.FocusedItem.Index;
                file = setListDir2[index - 1].FullName;
            }
            Process.Start("C:\\Windows\\System32\\notepad.exe", file);
        }

        private void button3_Click(object sender, EventArgs e) //F5
        {
            ListView listView_on = null, listView_to = null;
            List<string> list_on = null, list_to = null;
            if (active_listView == 1)
            {
                listView_on = listView1;
                listView_to = listView2;
                list_on = list_l;
                list_to = list_r;
            }
            else
            {
                listView_on = listView2;
                listView_to = listView1;
                list_on = list_r;
                list_to = list_l;
            }

            string copy_name;
            if (list_action.Count == 0)
            {
                MessageBox.Show("Нет выбраных файлов!", "SG Commander", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (list_action.Count == 1)
                copy_name = list_action[0].ToString();
            else
                copy_name = list_action.Count.ToString() + @" файла\папки";

            Form2 form2 = new Form2("Копировать ", copy_name, listView_to, list_to, 1, list_action);
            string copy_to_name = null;
            if (form2.ShowDialog() == DialogResult.Yes)
            {
                copy_to_name = form2.comboBox1.Text;
                foreach (var item in list_action)
                {
                    if (Path.HasExtension(Path.Combine(Program.active_dir(list_on) + item)))
                    {
                        if (File.Exists(Path.Combine(Program.active_dir(list_on), item)))
                            File.Copy(Path.Combine(Program.active_dir(list_on), item), Path.Combine(copy_to_name, item)); //Program.active_dir(list_to)
                    }
                    else
                    {
                        if (Directory.Exists(Path.Combine(Program.active_dir(list_to), item)) != true)
                            Directory.CreateDirectory(Path.Combine(Program.active_dir(list_to), item));
                        CopyDir(Path.Combine(Program.active_dir(list_on), item), Path.Combine(copy_to_name, item));
                    }
                }
            }
            list_action.Clear();
            SetDir(Program.active_dir(list_to), listView_to, set);
            SetDir(Program.active_dir(list_on), listView_on, set);
        }

        private void button4_Click(object sender, EventArgs e) //F6
        {
            ListView listView_on = null, listView_to = null;
            List<string> list_on = null, list_to = null;
            if (active_listView == 1)
            {
                listView_on = listView1;
                listView_to = listView2;
                list_on = list_l;
                list_to = list_r;
            }
            else
            {
                listView_on = listView2;
                listView_to = listView1;
                list_on = list_r;
                list_to = list_l;
            }

            string copy_name;
            if (list_action.Count == 0)
            {
                MessageBox.Show("Нет выбраных файлов!", "SG Commander", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (list_action.Count == 1)
                copy_name = list_action[0].ToString();
            else
                copy_name = list_action.Count.ToString() + @" файла\папки";

            Form2 form2 = new Form2("Переместить ", copy_name, listView_to, list_to, 1, list_action);
            string move_to_name = null;
            if (form2.ShowDialog() == DialogResult.Yes)
            {
                move_to_name = form2.comboBox1.Text;
                foreach (var item in list_action)
                {
                    if (Path.HasExtension(Path.Combine(Program.active_dir(list_on), item)))
                    {
                        if (File.Exists(Path.Combine(Program.active_dir(list_on), item)))
                            File.Move(Path.Combine(Program.active_dir(list_on), item), Path.Combine(move_to_name, item));
                    }
                    else
                    {
                        if (Directory.Exists(Path.Combine(Program.active_dir(list_to), item)) != true)
                            Directory.Move(Path.Combine(Program.active_dir(list_on), item), Path.Combine(move_to_name, item));
                    }
                }
                list_action.Clear();
                SetDir(Program.active_dir(list_on), listView_on, set);
                SetDir(Program.active_dir(list_to), listView_to, set);
            }
        }

        private void button5_Click(object sender, EventArgs e) //F7
        {
            ListView listView_to = null;
            List<string> list_to = null;
            if (active_listView == 1)
            {
                listView_to = listView1;
                list_to = list_l;
            }
            else
            {
                listView_to = listView2;
                list_to = list_r;
            }
            Form2 form2 = new Form2("Создать новый каталог (папку) ", Program.active_dir(list_to), listView_to, list_to, 2, list_action);
            if (form2.ShowDialog() == DialogResult.Yes && form2.textBox2.Text != "")
            {
                Directory.CreateDirectory(Path.Combine(Program.active_dir(list_to), form2.textBox2.Text));
            }
            SetDir(Program.active_dir(list_to), listView_to, set);
        }

        private void button6_Click(object sender, EventArgs e) //F8
        {
            ListView listView_on = null;
            List<string> list_on = null;
            if (active_listView == 1)
            {
                listView_on = listView1;
                list_on = list_l;
            }
            else
            {
                listView_on = listView2;
                list_on = list_r;
            }

            string del_name;
            if (list_action.Count == 0)
            {
                MessageBox.Show("Нет выбраных файлов!", "SG Commander", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (list_action.Count == 1)
                del_name = list_action[0].ToString();
            else
                del_name = list_action.Count.ToString() + @" файла\папки";
            if (MessageBox.Show("Вы действительно хотите удалить " + del_name, "SG Commander", MessageBoxButtons.YesNo, MessageBoxIcon.Stop) == DialogResult.Yes)
            {
                foreach (var item in list_action)
                {
                    if (Path.HasExtension(Path.Combine(Program.active_dir(list_on), item)))
                    {
                        try
                        {
                            File.Delete(Path.Combine(Program.active_dir(list_on), item));
                        }
                        catch (System.IO.IOException ex)
                        {
                            MessageBox.Show(ex.ToString());
                        }
                    }
                    else
                    {
                        try
                        {
                            Directory.Delete(Path.Combine(Program.active_dir(list_on), item));
                        }
                        catch (System.IO.IOException ex)
                        {
                            if (MessageBox.Show("Папка содержит подкаталоги! Удалить всё содержимое?", "SG Commander", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) == DialogResult.OK)
                                Directory.Delete(Path.Combine(Program.active_dir(list_on), item), true);
                        }
                    }
                }
            }
            SetDir(Program.active_dir(list_on), listView_on, set);
            list_action.Clear();
        }

        private void button7_Click(object sender, EventArgs e) //F9
        {

        }

        private void button8_Click(object sender, EventArgs e) //F10
        {

        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.F3:
                    button1_Click(sender, e);
                    break;
                case Keys.F4:
                    button2_Click(sender, e);
                    break;
                case Keys.F5:
                    button3_Click(sender, e);
                    break;
                case Keys.F6:
                    button4_Click(sender, e);
                    break;
                case Keys.F7:
                    button5_Click(sender, e);
                    break;
                case Keys.F8:
                    button6_Click(sender, e);
                    break;
            }
            return;
        }

        private void button_reset(System.Windows.Forms.View list_view)
        {
            toolStripButton2.Checked = toolStripButton3.Checked = toolStripButton4.Checked = false;
            switch (list_view)
            {
                case View.Details: toolStripButton2.Checked = true; break;
                case View.List: toolStripButton3.Checked = true; break;
                case View.Tile: toolStripButton4.Checked = true; break;
                default: break;
            }
        }

        private void listView1_KeyUp(object sender, KeyEventArgs e)
        {

            switch (e.KeyCode)
            {
                case Keys.Enter:
                    {
                        int index;

                        try
                        {
                            index = listView1.FocusedItem.Index;
                        }
                        catch (Exception ex)
                        {
                            index = 0;
                        }
                        if (index == 0)
                        {
                            if (list_l.Count > 1)
                            {
                                DirectoryInfo dir1 = new DirectoryInfo(Program.active_dir(list_l));
                                dir1 = dir1.Parent;
                                list_l.RemoveAt(list_l.Count - 1);
                            }
                            SetDir(Program.active_dir(list_l), listView1, set);
                        }
                        else
                        {
                            if (Path.HasExtension(setListDir1[index - 1].FullName))
                            {
                                Process.Start(setListDir1[index - 1].FullName);
                            }
                            else
                            {
                                list_l.Add(listView1.Items[index].SubItems[0].Text);
                                SetDir(Program.active_dir(list_l), listView1, set);
                            }
                        }
                        toolStripStatusLabel1.Text = Program.active_dir(list_l);
                        label3.Text = Program.active_dir(list_l);
                        break;
                    }
                case Keys.Space:
                    {
                        int index = listView1.FocusedItem.Index;
                        if (listView1.Items[index].ForeColor != Color.Red)
                        {
                            listView1.Items[index].ForeColor = Color.Red;
                            List<FileSystemInfo> dirFolder = new List<FileSystemInfo>();
                            dirFolder = setListDir(Program.active_dir(list_l), listView1, set);
                            list_action.Add(dirFolder[index - 1].Name);
                        }
                        else
                        {
                            listView1.Items[index].ForeColor = Color.Black;
                        }
                        break;
                    }
            }
        }

        private void listView2_KeyUp(object sender, KeyEventArgs e)
        {

            switch (e.KeyCode)
            {
                case Keys.Enter:
                    {
                        int index;

                        try
                        {
                            index = listView2.FocusedItem.Index;
                        }
                        catch (Exception ex)
                        {
                            index = 0;
                        }

                        if (index == 0)
                        {
                            if (list_r.Count > 1)
                            {
                                DirectoryInfo dir1 = new DirectoryInfo(Program.active_dir(list_r));
                                dir1 = dir1.Parent;
                                list_r.RemoveAt(list_r.Count - 1);
                            }
                            SetDir(Program.active_dir(list_r), listView2, set);
                        }
                        else
                        {
                            if (Path.HasExtension(setListDir2[index - 1].FullName))
                            {
                                Process.Start(setListDir2[index - 1].FullName);
                            }
                            else
                            {
                                list_r.Add(listView2.Items[index].SubItems[0].Text);
                                SetDir(Program.active_dir(list_r), listView2, set);
                            }
                        }
                        toolStripStatusLabel1.Text = Program.active_dir(list_r);
                        label4.Text = Program.active_dir(list_r);
                        break;
                    }
                case Keys.Space:
                    {
                        int index = listView2.FocusedItem.Index;
                        if (listView2.Items[index].ForeColor != Color.Red)
                        {
                            listView2.Items[index].ForeColor = Color.Red;
                            List<FileSystemInfo> dirFolder = new List<FileSystemInfo>();
                            dirFolder = setListDir(Program.active_dir(list_r), listView2, set);
                            list_action.Add(dirFolder[index - 1].Name);
                        }
                        else
                        {
                            listView2.Items[index].ForeColor = Color.Black;
                        }
                        break;
                    }
            }
        }

        private void сохранитьПозициюToolStripMenuItem_Click(object sender, EventArgs e)
        {
            set.Column_name = columnHeader1.Width;
            set.Column_type = columnHeader2.Width;
            set.Column_size = columnHeader3.Width;
            set.Column_date = columnHeader4.Width;
            set.Column_attributes = columnHeader5.Width;
            FileStream fs = new FileStream("settings.xml", FileMode.Create, FileAccess.ReadWrite);
            SoapFormatter formatter = new SoapFormatter();
            formatter.Serialize(fs, set);
            fs.Close();
        }

        void CopyDir(string dir_on, string dir_to)
        {
            DirectoryInfo dir_inf = new DirectoryInfo(dir_on);
            foreach (DirectoryInfo dir in dir_inf.GetDirectories())
            {
                if (Directory.Exists(Path.Combine(dir_to, dir.Name)) != true)
                    Directory.CreateDirectory(Path.Combine(dir_to, dir.Name));
                CopyDir(dir.FullName, Path.Combine(dir_to, dir.Name));
            }
            FileSystemInfo[] fileList = dir_inf.GetFiles();
            foreach (FileInfo file in fileList)
            {
                File.Copy(file.FullName, Path.Combine(dir_to, file.Name), true);
            }
        }

        private void listView2_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void настройкаToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form3 form3 = new Form3(ref set);

            if (form3.ShowDialog() == DialogResult.Yes)
            {
                SetDir(Program.active_dir(list_l), listView1, set);
                SetDir(Program.active_dir(list_r), listView2, set);
            }
        }

        private void label56_update(object sender, EventArgs e)
        {
            string tmp;
            string name;
            if (sender is Button)
            {
                if (logicDriver[(int)(((Button)sender).Tag) / 2].VolumeLabel == "")
                    name = "_no name_";
                else
                    name = logicDriver[(int)(((Button)sender).Tag) / 2].VolumeLabel;
                tmp = "[" + name + "]  " +
                     logicDriver[(int)(((Button)sender).Tag) / 2].TotalFreeSpace / 1024 + "Kb is " +
                     logicDriver[(int)(((Button)sender).Tag) / 2].TotalSize / 1024 + "Kb";
                if ((int)((Button)sender).Tag % 2 == 0)
                    label5.Text = tmp;
                else
                    label6.Text = tmp;
            }
            else
            {
                try
                {
                    if (logicDriver[((ComboBox)sender).SelectedIndex].VolumeLabel == "")
                        name = "_no name_";
                    else
                        name = logicDriver[((ComboBox)sender).SelectedIndex].VolumeLabel;
                    tmp = "[" + name + "]  " +
                       logicDriver[((ComboBox)sender).SelectedIndex].TotalFreeSpace / 1024 + "Kb is " +
                       logicDriver[((ComboBox)sender).SelectedIndex].TotalSize / 1024 + "Kb"; ;
                    if ((string)((ComboBox)sender).Tag == "1")
                        label5.Text = tmp;
                    else
                        label6.Text = tmp;
                }
                catch (Exception ex)
                {

                }
            }
        }

        private void listView1_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            SelectSort((ListView)sender, e, set.List1_sort);
            SetDir(Program.active_dir(list_l), listView1, set);
        }

        private void listView2_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            SelectSort((ListView)sender, e, set.List2_sort);
            SetDir(Program.active_dir(list_r), listView2, set);
        }

        public void SelectSort(ListView listView, ColumnClickEventArgs e, int sort)
        {
            if (e.Column != 4)
            {
                listView.Columns[0].Text = "Имя";
                listView.Columns[1].Text = "Тип";
                listView.Columns[2].Text = "Размер";
                listView.Columns[3].Text = "Дата";
                switch (e.Column)
                {
                    case 0:
                        if (sort == 0)
                        {
                            sort = 1;
                            listView.Columns[0].Text = "↑Имя";
                        }
                        else
                        {
                            sort = 0;
                            listView.Columns[0].Text = "↓Имя";
                        }
                        break;
                    case 1:
                        if (sort == 2)
                        {
                            sort = 3;
                            listView.Columns[1].Text = "↑Тип";
                        }
                        else
                        {
                            sort = 2;
                            listView.Columns[1].Text = "↓Тип";
                        }
                        break;
                    case 2:
                        if (sort == 4)
                        {
                            sort = 5;
                            listView.Columns[2].Text = "↑Размер";
                        }
                        else
                        {
                            sort = 4;
                            listView.Columns[2].Text = "↓Размер";
                        }
                        break;
                    case 3:
                        if (sort == 6)
                        {
                            sort = 7;
                            listView.Columns[3].Text = "↑Дата";
                        }
                        else
                        {
                            sort = 6;
                            listView.Columns[3].Text = "↓Дата";
                        }
                        break;
                    default:
                        break;
                }
            }
            if ((string)listView.Tag == "1")
                set.List1_sort = sort;
            else
                set.List2_sort = sort;
        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {

        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            if (toolStripButton2.Checked)
            {
                toolStripButton3.Checked = false;
                toolStripButton4.Checked = false;
                if (active_listView == 1)
                {
                    listView1.View = View.Details;
                    set.list1_view = View.Details;
                }
                else
                {
                    listView2.View = View.Details;
                    set.list2_view = View.Details;
                }

            }
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            if (toolStripButton3.Checked)
            {
                toolStripButton2.Checked = false;
                toolStripButton4.Checked = false;
                if (active_listView == 1)
                {
                    listView1.View = View.List;
                    set.list1_view = View.List;
                }
                else
                {
                    listView2.View = View.List;
                    set.list2_view = View.List;
                }

            }
        }

        private void toolStripButton4_Click_1(object sender, EventArgs e)
        {
            if (toolStripButton4.Checked)
            {
                toolStripButton3.Checked = false;
                toolStripButton2.Checked = false;
                if (active_listView == 1)
                {
                    listView1.View = View.Tile;
                    set.list1_view = View.Tile;
                }
                else
                {
                    listView2.View = View.Tile;
                    set.list2_view = View.Tile;
                }

            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            set.Column_name = columnHeader1.Width;
            set.Column_type = columnHeader2.Width;
            set.Column_size = columnHeader3.Width;
            set.Column_date = columnHeader4.Width;
            set.Column_attributes = columnHeader5.Width;
            FileStream fs = new FileStream("settings.xml", FileMode.Create, FileAccess.ReadWrite);
            SoapFormatter formatter = new SoapFormatter();
            formatter.Serialize(fs, set);
            fs.Close();
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            listView1.Focus();
            active_listView = 1;
            button_reset(set.list1_view);
            listView1.Items[0].Focused = true;
            listView1.Items[0].Selected = true;

        }

        private void notifyIcon1_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Normal;
        }

        private void сравнитьКаталогиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int pos1 = 0;
            foreach (var item1 in setListDir1)
            {
                int i = 0;
                if (item1 is FileInfo)
                {
                    foreach (var item2 in setListDir2)
                        if (item1.Name == item2.Name)
                            i = 1;
                    if (i == 0)
                        listView1.Items[pos1 + 1].ForeColor = Color.Red;
                }
                pos1++;
            }
            int pos2 = 0;
            foreach (var item2 in setListDir2)
            {
                int i = 0;
                if (item2 is FileInfo)
                {
                    foreach (var item1 in setListDir1)
                        if (item2.Name == item1.Name)
                            i = 1;
                    if (i == 0)
                        listView2.Items[pos2 + 1].ForeColor = Color.Red;
                }
                pos2++;
            }
        }

        private void listView1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Clicks == 2)
            {
                indexOfItemUnderMouseToDrag = listView1.FocusedItem.Index;
                if (indexOfItemUnderMouseToDrag != null)
                {
                    Size dragSize = SystemInformation.DragSize;
                    dragBoxFromMouseDown = new Rectangle(new Point(e.X - (dragSize.Width / 2), e.Y - (dragSize.Height / 2)), dragSize);
                }
                else
                    // Reset the rectangle if the mouse is not over an item in the ListBox.
                    dragBoxFromMouseDown = Rectangle.Empty;

                if (indexOfItemUnderMouseToDrag == 0)
                {
                    if (list_l.Count > 1)
                    {
                        DirectoryInfo dir1 = new DirectoryInfo(Program.active_dir(list_l));
                        dir1 = dir1.Parent;
                        list_l.RemoveAt(list_l.Count - 1);
                    }
                    SetDir(Program.active_dir(list_l), listView1, set);
                }
                else
                {
                    if (Path.HasExtension(setListDir1[indexOfItemUnderMouseToDrag - 1].FullName))
                    {
                        Process.Start(setListDir1[indexOfItemUnderMouseToDrag - 1].FullName);
                    }
                    else
                    {
                        list_l.Add(listView1.Items[indexOfItemUnderMouseToDrag].SubItems[0].Text);
                        SetDir(Program.active_dir(list_l), listView1, set);
                    }
                }
                toolStripStatusLabel1.Text = Program.active_dir(list_l);
                label3.Text = Program.active_dir(list_l);
            }
            else
            {
                active_listView = 1;
                button_reset(set.list1_view);
            }
        }

        private void listView1_MouseUp(object sender, MouseEventArgs e)
        {
            dragBoxFromMouseDown = Rectangle.Empty;
        }

        private void listView1_MouseMove(object sender, MouseEventArgs e)
        {

            if ((e.Button & MouseButtons.Left) == MouseButtons.Left)
            {
                if (dragBoxFromMouseDown != Rectangle.Empty &&
                    !dragBoxFromMouseDown.Contains(e.X, e.Y))
                {
                    int index = listView1.FocusedItem.Index;
                    if (list_action.Count == 0)
                        list_action.Add(setListDir1[index - 1].Name);
                    DragDropEffects dropEffect = listView1.DoDragDrop(list_action, DragDropEffects.All | DragDropEffects.Link);
                }
            }
        }

        private void listView1_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Effect == DragDropEffects.Copy)
                button3_Click(sender, e);
            if (e.Effect == DragDropEffects.Move)
                button4_Click(sender, e);
        }

        private void listView_DragOver(object sender, DragEventArgs e)
        {
            if ((e.KeyState & (8 + 32)) == (8 + 32) &&
                (e.AllowedEffect & DragDropEffects.Link) == DragDropEffects.Link)
            {
                // KeyState 8 + 32 = CTL + ALT

                // Link drag-and-drop effect.
                e.Effect = DragDropEffects.Link;

            }
            else if ((e.KeyState & 32) == 32 &&
              (e.AllowedEffect & DragDropEffects.Link) == DragDropEffects.Link)
            {

                // ALT KeyState for link.
                e.Effect = DragDropEffects.Link;

            }
            else if ((e.KeyState & 4) == 4 &&
              (e.AllowedEffect & DragDropEffects.Move) == DragDropEffects.Move)
            {

                // SHIFT KeyState for move.
                e.Effect = DragDropEffects.Move;

            }
            else if ((e.KeyState & 8) == 8 &&
              (e.AllowedEffect & DragDropEffects.Copy) == DragDropEffects.Copy)
            {

                // CTL KeyState for copy.
                e.Effect = DragDropEffects.Copy;

            }
            else if ((e.AllowedEffect & DragDropEffects.Move) == DragDropEffects.Move)
            {

                // By default, the drop action should be move, if allowed.
                e.Effect = DragDropEffects.Move;

            }
            else
                e.Effect = DragDropEffects.None;
        }


        private void listView2_DragDrop(object sender, DragEventArgs e)
        {
            if(e.Effect == DragDropEffects.Copy)
                button3_Click(sender, e);
            if(e.Effect == DragDropEffects.Move)
                button4_Click(sender, e);
        }

        private void listView2_MouseUp(object sender, MouseEventArgs e)
        {
            dragBoxFromMouseDown = Rectangle.Empty;
        }

        private void listView2_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Clicks == 2)
            {
                indexOfItemUnderMouseToDrag = listView2.FocusedItem.Index;
                if (indexOfItemUnderMouseToDrag != null)
                {
                    Size dragSize = SystemInformation.DragSize;
                    dragBoxFromMouseDown = new Rectangle(new Point(e.X - (dragSize.Width / 2), e.Y - (dragSize.Height / 2)), dragSize);
                }
                else
                    dragBoxFromMouseDown = Rectangle.Empty;

                if (indexOfItemUnderMouseToDrag == 0)
                {
                    if (list_r.Count > 1)
                    {
                        DirectoryInfo dir1 = new DirectoryInfo(Program.active_dir(list_r));
                        dir1 = dir1.Parent;
                        list_r.RemoveAt(list_r.Count - 1);
                    }
                    SetDir(Program.active_dir(list_r), listView2, set);
                }
                else
                {
                    if (Path.HasExtension(setListDir2[indexOfItemUnderMouseToDrag - 1].FullName))
                    {
                        Process.Start(setListDir2[indexOfItemUnderMouseToDrag - 1].FullName);
                    }
                    else
                    {
                        list_r.Add(listView2.Items[indexOfItemUnderMouseToDrag].SubItems[0].Text);
                        SetDir(Program.active_dir(list_r), listView2, set);
                    }
                }
                toolStripStatusLabel1.Text = Program.active_dir(list_r);
                label4.Text = Program.active_dir(list_r);
            }
            else
            {
                active_listView = 2;
                button_reset(set.list2_view);
            }
        }

        private void listView2_MouseMove(object sender, MouseEventArgs e)
        {
            if ((e.Button & MouseButtons.Left) == MouseButtons.Left)
            {
                if (dragBoxFromMouseDown != Rectangle.Empty &&
                    !dragBoxFromMouseDown.Contains(e.X, e.Y))
                {
                    int index = listView2.FocusedItem.Index;
                    if (list_action.Count == 0)
                        list_action.Add(setListDir2[index - 1].Name);
                    DragDropEffects dropEffect = listView2.DoDragDrop(list_action, DragDropEffects.All | DragDropEffects.Link);
                }
            }
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct SHFILEINFO
    {
        public IntPtr hIcon;
        public int iIcon;
        public uint dwAttributes;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
        public string szDisplayName;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
        public string szTypeName;
    };

    class Win32
    {
        public const uint SHGFI_ICON = 0x100;
        public const uint SHGFI_LARGEICON = 0x0; // 'Large icon
        public const uint SHGFI_SMALLICON = 0x1; // 'Small icon

        [DllImport("shell32.dll")]
        public static extern IntPtr SHGetFileInfo(string pszPath, uint dwFileAttributes, ref SHFILEINFO psfi, uint cbSizeFileInfo, uint uFlags);
    }
}

