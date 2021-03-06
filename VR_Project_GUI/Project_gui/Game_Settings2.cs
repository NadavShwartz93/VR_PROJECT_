using System;
using System.IO;
using System.Windows.Forms;

namespace Project_gui
{
    public partial class Game_Settings2 : Form
    {
        private string Patient_Results_File_Name = Globals.file_name_Patient_Results;
        private string Patient_Detailes_File_Name = Globals.file_Name_Patient_Detailes;
        private int[] areaScoreArr;

        public Game_Settings2()
        {
            InitializeComponent();
            areaScoreArr = new int[Globals.numOfArea];

            default_Radio_Button.Checked = true;
            changeComponentStatus(false);
        }

        public void initComponents()
        {
            tln_Label.Text = areaScoreArr[0].ToString();
            tln_Track_Bar.Value = areaScoreArr[0];

            trn_Label.Text = areaScoreArr[1].ToString();
            trn_Track_Bar.Value = areaScoreArr[1];

            bln_Label.Text = areaScoreArr[2].ToString();
            bln_Track_Bar.Value = areaScoreArr[2];

            brn_Label.Text = areaScoreArr[3].ToString();
            brn_Track_Bar.Value = areaScoreArr[3];

            tlf_Label.Text = areaScoreArr[4].ToString();
            tlf_Track_Bar.Value = areaScoreArr[4];

            trf_Label.Text = areaScoreArr[5].ToString();
            trf_Track_Bar.Value = areaScoreArr[5];

            blf_Label.Text = areaScoreArr[6].ToString();
            blf_Track_Bar.Value = areaScoreArr[6];

            brf_Label.Text = areaScoreArr[7].ToString();
            brf_Track_Bar.Value = areaScoreArr[7];

        }

        public void resetComponents()
        {
            tln_Label.Text = "0";
            tln_Track_Bar.Value = 0;

            trn_Label.Text = "0";
            trn_Track_Bar.Value = 0;

            bln_Label.Text = "0";
            bln_Track_Bar.Value = 0;

            brn_Label.Text = "0";
            brn_Track_Bar.Value = 0;

            tlf_Label.Text = "0";
            tlf_Track_Bar.Value = 0;

            trf_Label.Text = "0";
            trf_Track_Bar.Value = 0;

            blf_Label.Text = "0";
            blf_Track_Bar.Value = 0;

            brf_Label.Text = "0";
            brf_Track_Bar.Value = 0;

        }

        private void changeComponentStatus(bool Status)
        {
            tln_Track_Bar.Enabled = Status;
            tln_Label.Enabled = Status;

            trn_Track_Bar.Enabled = Status;
            trn_Label.Enabled = Status;

            bln_Track_Bar.Enabled = Status;
            bln_Label.Enabled = Status;

            brn_Track_Bar.Enabled = Status;
            brn_Label.Enabled = Status;

            tlf_Track_Bar.Enabled = Status;
            tlf_Label.Enabled = Status;

            trf_Track_Bar.Enabled = Status;
            trf_Label.Enabled = Status;

            blf_Track_Bar.Enabled = Status;
            blf_Label.Enabled = Status;

            brf_Track_Bar.Enabled = Status;
            brf_Label.Enabled = Status;
        }

        //This method check if the PatientDetails.csv is exist
        public bool isIdExistInFile()
        {
            //The case PatientDetails.csv does not exist
            if (!File.Exists(Globals.file_Name_Patient_Detailes))
                return false;

            var guiId = Patient_Details.get_Instance().get_id_textBox().Trim();
            string IdFromFile;
            bool flag = false;
            using (var reader = new StreamReader(Globals.file_Name_Patient_Detailes))
            {
                const int IdColumn = 1;
                reader.ReadLine();
                var list = reader.ReadLine().Split(',');
                IdFromFile = list[IdColumn].Trim();

                //Read the area score values.
                if (guiId == IdFromFile)
                {
                    for (int i = 0; i < Globals.numOfArea; i++)
                    {
                        if (list[i + 8].Trim() != "NaN")
                        {
                            areaScoreArr[i] = (int)(float.Parse(list[i + 8]) * 10);
                        }
                        else
                            areaScoreArr[i] = 0;
                    }
                    flag = true;
                }
                else
                    flag = false;
            }
            return flag;
        }

        private string addAreaSocreDataToCSV()
        {
            string str = Patient_Details.get_Data();

            float sum = tln_Track_Bar.Value + trn_Track_Bar.Value + bln_Track_Bar.Value +
                brn_Track_Bar.Value + tlf_Track_Bar.Value + trf_Track_Bar.Value + blf_Track_Bar.Value + brf_Track_Bar.Value;
            if(sum != 0) { 
            str += ", " + tln_Track_Bar.Value / sum;
            str += ", " + trn_Track_Bar.Value / sum;
            str += ", " + bln_Track_Bar.Value / sum;
            str += ", " + brn_Track_Bar.Value / sum;
            str += ", " + tlf_Track_Bar.Value / sum;
            str += ", " + trf_Track_Bar.Value / sum;
            str += ", " + blf_Track_Bar.Value / sum;
            str += ", " + brf_Track_Bar.Value / sum;
            }
            else
            {
                str += ", 0, 0, 0, 0, 0, 0, 0, 0, ";
            }

            return str;
        }

        // Approve Game Settings
        private void click_approve_button(object sender, EventArgs e)
        {
            //Write the Patient Details to PatientDetails.csv 

            write_To_Csv_File(this.addAreaSocreDataToCSV(), Patient_Detailes_File_Name);

            //Close the open forms.
            this.close_all_WinForm();

            //Game_Settings1 GUI don't close with close_all_WinForm() so I close it.
            Game_Settings1.get_Instance().Close();

        }

        void close_all_WinForm()
        {
            Patient_Details.get_Instance().Close();

            Game_Settings1.get_Instance().Close();
        }

        // Go back to the main Game Settings Screen
        private void click_back_button(object sender, EventArgs e)
        {
            this.Visible = false;
            Patient_Details.get_Instance().Visible = true;
        }

        private void write_To_Csv_File(string data_to_write, string fileName)
        {
            string fileToWrite = Path.Combine(Globals.path, fileName);

            try
            {
                //Pass the file path and filename to the StreamWriter Constructor
                using (StreamWriter writetext = new StreamWriter(fileToWrite))
                {
                    //Write a line of text
                    writetext.WriteLine(data_to_write);
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

        private void splitContainer1_Panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        /*
         Change the value of the Top Left Near Label Text according to the Top Left Near TrackBar value.
         */
        private void tln_Scroll_bar(object sender, EventArgs e)
        {
            tln_Label.Text = tln_Track_Bar.Value.ToString();
        }

        /*
         Change the value of the Top Right Near Label Text according to the Top Right Near TrackBar value.
         */
        private void trn_Scroll_bar(object sender, EventArgs e)
        {
            trn_Label.Text = trn_Track_Bar.Value.ToString();
        }

        /*
         Change the value of the Bottom Left Near Label Text according to the Bottom Left Near TrackBar value.
         */
        private void bln_Scroll_bar(object sender, EventArgs e)
        {
            bln_Label.Text = bln_Track_Bar.Value.ToString();
        }

        /*
         Change the value of the Bottom Right Near Label Text according to the Bottom Right Near TrackBar value.
         */
        private void brn_Scroll_bar(object sender, EventArgs e)
        {
            brn_Label.Text = brn_Track_Bar.Value.ToString();
        }

        /*
        Change the value of the Top Left Far Label Text according to the Top Left Far TrackBar value.
        */
        private void tlf_Scroll_bar(object sender, EventArgs e)
        {
            tlf_Label.Text = tlf_Track_Bar.Value.ToString();
        }

        /*
        Change the value of the Top Right Far Label Text according to the Top Right Far TrackBar value.
        */
        private void trf_Scroll_bar(object sender, EventArgs e)
        {
            trf_Label.Text = trf_Track_Bar.Value.ToString();
        }

        /*
        Change the value of the Bottom Left Far Label Text according to the Bottom Left Far TrackBar value.
        */
        private void blf_Scroll_bar(object sender, EventArgs e)
        {
            blf_Label.Text = blf_Track_Bar.Value.ToString();
        }

        /*
        Change the value of the Bottom Right Far Label Text according to the Bottom Right Far TrackBar value.
        */
        private void brf_Scroll_bar(object sender, EventArgs e)
        {
            brf_Label.Text = brf_Track_Bar.Value.ToString();
        }

        private void Adjust_RadioButton_Click(object sender, EventArgs e)
        {
            this.changeComponentStatus(true);
        }

        private void Default_RadioButton_Click(object sender, EventArgs e)
        {
            this.changeComponentStatus(false);
        }

        private void Game_Setting2_FromClosing(object sender, FormClosingEventArgs e)
        {
            Patient_Details.get_Instance().Close();

            Game_Settings1.get_Instance().Close();
        }
    }
}
