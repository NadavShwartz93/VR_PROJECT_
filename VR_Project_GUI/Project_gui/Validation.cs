using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;

namespace Project_gui
{
    public class Validation
    {
        public static bool isPathExist(string path = Globals.path)
        {
            if (!Directory.Exists(path))
            { 
                MessageBox.Show("The " + path +" don't exist!", "Path Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            return true;
        }
    }
}
