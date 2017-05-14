using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using System.IO;

namespace SGCommander
{
    static class Program
    {
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Settings set = new Settings();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
            
        }

        static public string active_dir(List<string> list)
        {
            string tmp = "";
            for (int i = 0; i < list.Count; i++)
            {
                tmp = Path.Combine(tmp, list[i].ToString());
            }
            return tmp;
        }



    }
}
