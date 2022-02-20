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

namespace Project_gui
{
    public partial class Game_Setting2 : Form
    {
        private string file_Name = "Data.csv";
        public Game_Setting2()
        {
            InitializeComponent();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label10_Click(object sender, EventArgs e)
        {

        }

        private void progressBar1_Click(object sender, EventArgs e)
        {

        }

        private void label9_Click(object sender, EventArgs e)
        {

        }

        // Approve Game Settings
        private void button1_Click(object sender, EventArgs e)
        {
            write_To_Csv_File();
            this.Close();
            Game_Setting1.get_Instance().Close();
        }

        // Go back to the main Game Settings Screen
        private void button2_Click(object sender, EventArgs e)
        {
            this.Visible = false;
            Game_Setting1.get_Instance().Visible = true;
            
        }


        private void read_From_Csv_File()
        {
            String line;
            try
            {
                //Pass the file path and file name to the StreamReader constructor
                StreamReader sr = new StreamReader(file_Name);
                //Read the first line of text
                line = sr.ReadLine();
                //Continue to read until you reach end of file
                while (line != null)
                {
                    //write the line to console window
                    Console.WriteLine(line);
                    //Read the next line
                    line = sr.ReadLine();
                }
                //close the file
                sr.Close();
                Console.ReadLine();
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);
            }
            finally
            {
                Console.WriteLine("Executing finally block.");
            }

        }
        private void write_To_Csv_File()
        {
            try
            {
                //Pass the filepath and filename to the StreamWriter Constructor
                using(StreamWriter writetext = new StreamWriter(file_Name)){

                    //Write a line of text
                    writetext.WriteLine("Hello,World!!");
                    //Write a second line of text
                    writetext.WriteLine("From the StreamWriter class");
                    //Close the file
                    writetext.Close();
                }

            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);
            }
            finally
            {
                Console.WriteLine("Executing finally block.");
            }
        }

    }
}
