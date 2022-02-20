using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Project_gui
{
    public partial class Game_Setting1 : Form
    {
        private static Game_Setting2 gs2 = null;
        private static Game_Setting1 gs1 = null;
        public Game_Setting1()
        {
            InitializeComponent();
            gs1 = this;
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {

        }

        // continue button to the next screen
       private void button1_Click(object sender, EventArgs e) 
        {
          
            if (gs2 == null)
            {
                gs2 = new Game_Setting2();
            }
            gs2.Show();
            this.Visible = false;
        }

        public static Game_Setting1 get_Instance() 
        {
            return gs1;
        }

        private void Moti_Click(object sender, EventArgs e)
        {

        }
    }
}
