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
    public partial class Game_Settings1 : Form
    {
        private static Game_Settings2 gs2 = null;
        private static Game_Settings1 gs1 = null;

        public Game_Settings1()
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
       private void click_continue_button(object sender, EventArgs e) 
        {
          
            if (gs2 == null)
            {
                gs2 = new Game_Settings2();
            }
            gs2.Show();
            this.Visible = false;
        }

        // back button to the previous screen
        private void click_back_button(object sender, EventArgs e)
        {
            this.Visible = false;
            Patient_Detailes.get_Instance().Visible = true;
        }

        //Return instance of this class, and create an instantion if needed.
        public static Game_Settings1 get_Instance() 
        {
            if (gs1 == null)
                gs1 = new Game_Settings1();
            return gs1;
        }



        private void splitContainer1_Panel2_Paint(object sender, PaintEventArgs e)
        {

        }



        private void Game_Settings1_Load(object sender, EventArgs e)
        {

        }

        /*
         Change the value of the distance Label Text according to the distance TrackBar value.
         */
        private void distance_Scroll_Bar(object sender, EventArgs e)
        {
            distance_Label.Text = distance_Track_Bar.Value.ToString();
        }



        /*
         Change the value of the size Label Text according to the size TrackBar value.
         */
        private void size_Scroll_Bar(object sender, EventArgs e)
        {
            size_Label.Text = size_Track_Bar.Value.ToString();
        }

 
        /*
         Change the value of the hand Label Text according to the hand TrackBar value.
         */
         private void label3_Click(object sender, EventArgs e)
        {

        }
    }
}
