using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SGCommander
{
    [Serializable]
    public class Settings
    {
        public int column_name;
        public int column_type;
        public int column_size;
        public int column_date;
        public int column_attributes;
        public bool show_hidden_elem = false;
        public int list1_sort = 0; //0,1-byName 2,3-byExtension 4,5-bySize 6,7-byDate 8,9-byAttr
        public int list2_sort = 0;
        public System.Windows.Forms.View list1_view;
        public System.Windows.Forms.View list2_view;

        public Settings() { }

        public int Column_name { get { return column_name; } set { column_name = value; } }
        public int Column_type { get { return column_type; } set { column_type = value; } }
        public int Column_size { get { return column_size; } set { column_size = value; } }
        public int Column_date { get { return column_date; } set { column_date = value; } }
        public int List1_sort { get { return list1_sort; } set { list1_sort = value; } }
        public int List2_sort { get { return list2_sort; } set { list2_sort = value; } }
        public int Column_attributes { get { return column_attributes; } set { column_attributes = value; } }
        public bool Show_hidden_elem { get { return show_hidden_elem; } set { show_hidden_elem = value; } }
    }
}
