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
    public partial class Patient_Details : Form
    {
        private static Patient_Details P_D= null;
        private static Game_Settings1 gs1 = null;

        // This string will hold the Patient Details.
        private static string Patient_data = null;

        // Boolean flag used to determine that number is entered. 
        private bool nonNumberEntered = false;

        // Boolean flag used to determine that letter is entered. 
        private bool letterEntered = true;

        // This boolean flag check that only 1 period is entered and only number.
        private bool period_number_ = false;

        public Patient_Details()
        {
            InitializeComponent();
            this.Hand_CheckBox.BackColor = System.Drawing.Color.Green;
            this.Standing_CheckBox.BackColor = System.Drawing.Color.Green;
            P_D = this;
        }

        private void Patient_Detailes_Load(object sender, EventArgs e)
        {
        }

        private void id_textBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void continue_button_Click(object sender, EventArgs e)
        {
            if (gs1 == null)
                gs1 = Game_Settings1.get_Instance();

            this.Visible = false;
            gs1.Show();
            insert_data_to_string();
        }

        private void insert_data_to_string()
        {
            Patient_data = "";
            Patient_data += "Hand in Therapy, Id, First Name, Last Name, Height (cm), Arm Length, ";
            Patient_data += "Standing, Treatment Time (sec), Area Score 0, Area Score 1, Area Score 2\n";
            Patient_data += Hand_CheckBox.Text + ", " + id_textBox.Text + ", " + First_Name_textBox.Text + ", ";
            Patient_data += Last_Name_textBox.Text + ", " + Height_textBox.Text + ", ";
            Patient_data += Arm_Length_textBox.Text + ", " + Standing_CheckBox.Text + ", ";
            Patient_data += Treatment_Time_textBox.Text;
        }

        public static string get_Data() 
        {
            return Patient_data;
        }

        // Return instance of this class, and create an instance if needed.
        public static Patient_Details get_Instance()
        {
            if (P_D == null)
                P_D = new Patient_Details();
            return P_D;
        }

        // Handle the KeyDown event to determine the type of character entered into the control.
        private void numerical_KeyDown(object sender, KeyEventArgs e)
        {
            // Initialize the flag to false.
            nonNumberEntered = false;

            // Determine whether the keystroke is a number from the top of the keyboard.
            if (e.KeyCode < Keys.D0 || e.KeyCode > Keys.D9)
            {
                // Determine whether the keystroke is a number from the keypad.
                if (e.KeyCode < Keys.NumPad0 || e.KeyCode > Keys.NumPad9)
                {
                    // Determine whether the keystroke is a backspace.
                    if (e.KeyCode != Keys.Back)
                    {
                        // A non-numerical keystroke was pressed.
                        // Set the flag to true and evaluate in KeyPress event.
                        nonNumberEntered = true;
                    }
                }
            }
            //If shift key was pressed, it's not a number.
            if (Control.ModifierKeys == Keys.Shift)
            {
                nonNumberEntered = true;
            }
        }

        // This event occurs after the KeyDown event and can be used to prevent
        // characters from entering the control.
        private void numerical_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Check for the flag being set in the KeyDown event.
            if (nonNumberEntered == true)
            {
                // Stop the character from being entered into the control since it is non-numerical.
                e.Handled = true;
            }
        }

        // Handle the letters_KeyDown event to determine that only letters is entered into the control.
        private void letters_KeyDown(object sender, KeyEventArgs e)
        {
            // The case the keystroke is upper-case letter OR lower-case letter.
            if (e.KeyCode >= Keys.A && e.KeyCode <= Keys.Z)
                letterEntered = true;
            // The case the keystroke is space.
            else if (e.KeyCode == Keys.Space)
                letterEntered = true;
            // Determine whether the keystroke is a backspace.
            else if (e.KeyCode == Keys.Back)
                letterEntered = true;
            // The input is not correct.
            else
                letterEntered = false;
        }

        // This event occurs after the letters_KeyDown event and can be used to prevent
        // numbers from entering the control.
        private void letters_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Check for the flag being equal to false in the KeyDown event.
            if(letterEntered == false)
            {
                // Stop the character from being entered into the control since it is non-letter.
                e.Handled = true;
            }
        }

        //This method change Standing_CheckBox Color and Text after every click.
        private void Standing_CheckedChanged(object sender, EventArgs e)
        {
            if (Standing_CheckBox.Checked)
            {
                Standing_CheckBox.ImageIndex = 1; Standing_CheckBox.Text = "No";
                Standing_CheckBox.BackColor = System.Drawing.Color.Red;
            }
            else
            {
                Standing_CheckBox.ImageIndex = 2; Standing_CheckBox.Text = "Yes";
                Standing_CheckBox.BackColor = System.Drawing.Color.Green;
            }
        }

        //This method change Hand_CheckBox Color and Text after every click.
        private void Hand_CheckedChanged(object sender, EventArgs e)
        {
            if (Hand_CheckBox.Checked)
            {
                Hand_CheckBox.ImageIndex = 1; Hand_CheckBox.Text = "Right";
                Hand_CheckBox.BackColor = System.Drawing.Color.Red;
            }
            else
            {
                Hand_CheckBox.ImageIndex = 2; Hand_CheckBox.Text = "Left";
                Hand_CheckBox.BackColor = System.Drawing.Color.Green;
            }
        }


        // This event occurs after the numerical_period_KeyDown event and can be used to prevent
        // letters or more then 1 period from entering the control.
        private void numerical_period_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Check for the flag being set in the KeyDown event.
            if (period_number_ == false)
            {
                    // Stop the character from being entered into the control since it is non-numerical.
                    e.Handled = true;
            }
        }

        // Handle the numerical_period_KeyDown event to determine that only
        // numbers or 1 period is entered into the control.
        private void numerical_period_KeyDown(object sender, KeyEventArgs e)
        {
            this.period_number_ = false;

            //Local variables.
            bool txt_contain_period;
            string text = this.Arm_Length_textBox.Text;

            if (!text.Contains("."))
                txt_contain_period = false;
            else
                txt_contain_period = true;

            this.numerical_KeyDown(sender, e);
            if (nonNumberEntered == true && txt_contain_period == false && 
                (e.KeyCode == Keys.OemPeriod || e.KeyCode == Keys.Decimal))
                period_number_ = true;
            else if (nonNumberEntered == true && txt_contain_period == true &&
                (e.KeyCode == Keys.OemPeriod || e.KeyCode == Keys.Decimal))
                period_number_ = false;
            else if(nonNumberEntered == false)
                period_number_ = true;
        }

        public string get_id_textBox()
        {
            return id_textBox.Text;
        }

        private void Patient_Details_FromClosing(object sender, FormClosingEventArgs e)
        {
        }
    }
}
