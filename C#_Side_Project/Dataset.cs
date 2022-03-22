using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using UnityEngine;
using System.IO;

public class Dataset /*: MonoBehaviour*/
{
    private static Dataset instance = null;
    private Dictionary<string, List<float[]>> user_data = new Dictionary<string, List<float[]>>();
    private string[] PatientDetailes = new string[6];
    private static int numberOfColumns = 11;

    private Dataset()
    {
        instance = this;
        PatientDetailes = new string[6];
        user_data = new Dictionary<string, List<float[]>>();
    }

    public static Dataset Get_instance()
    {
        if (instance == null)
        {
            instance = new Dataset();
        }
        return instance;
    }

    public void New_PatientDetailes(string Id, string Hand_in_Therapy,
     float Height, float Arm_Length, string Standing, float Treatment_Time)
    {
        PatientDetailes[0] = Id;
        PatientDetailes[1] = Hand_in_Therapy == "left" ? "0" : "1";
        PatientDetailes[2] = Height.ToString();
        PatientDetailes[3] = Arm_Length.ToString();
        PatientDetailes[4] = Standing == "yes" ? "1" : "0";
        PatientDetailes[5] = Treatment_Time.ToString();
    }

    public void New_data_line(float Velocity_average, float Max_velocity_count, float Reaching_time,
    float Path_taken, float Jerkiness, int Bubble_popped, int Total_Score,

    float Bubble_Position_X, float Bubble_Position_Y, float Bubble_Position_Z, string Bubble_in_space)
    {
        float[] _vector = new float[10];

        _vector[0] = Velocity_average;
        _vector[1] = Max_velocity_count;
        _vector[2] = Reaching_time;
        _vector[3] = Path_taken;
        _vector[4] = Jerkiness;
        _vector[5] = (float)Bubble_popped;
        _vector[6] = (float)Total_Score;
        _vector[7] = Bubble_Position_X;
        _vector[8] = Bubble_Position_Y;
        _vector[9] = Bubble_Position_Z;

        //The case that there is data for this Key in the map.
        if (user_data.ContainsKey(Bubble_in_space))
        {
            user_data[Bubble_in_space].Add(_vector);
        }
        else
        {
            List<float[]> list = new List<float[]>();

            //Create new Matirx and push new data.
            list.Add(_vector);
            user_data.Add(Bubble_in_space, list);
        }
    }

    public void Write_data_to_file()
    {
        //make preprocessing for the data:
        Preprocess_data();

        foreach (var item in user_data)
        {

            {

                string data_to_write = "";
                for (int i = 0; i < PatientDetailes.Length; i++)
                    data_to_write += PatientDetailes[i] + ",";
                data_to_write += item.Key + ",";

                var val = item.Value;
                for (int i = 0; i < val[0].Count(); i++)
                    data_to_write += val[0][i].ToString("0.00") + ",";


                Write_To_Csv_File(data_to_write);
            }
            //Remove all the keys and the values from the dictionary.
            user_data.Clear();
        }
    }


    private void Write_To_Csv_File(string data_to_write)
    {
        try
        {
            if (File.Exists(Globals.file_name_dataset))
            {
                //Pass the filepath and filename to the StreamWriter Constructor
                using (StreamWriter writetext = File.AppendText(Globals.file_name_dataset))
                {
                    //Write a line of text
                    writetext.WriteLine(data_to_write);
                    //Close the file
                    writetext.Close();
                }
            }
            else
            {
                using (StreamWriter writetext = new StreamWriter(Globals.file_name_dataset))
                {
                    string heading = "Id, Hand in Therapy (0 for left 1 for right), Height(cm), " +
                        "Arm Length, Standing (0 for no 1 for yes), Treatment Time (sec), Bubble in space, Velocity average (Best=1 Worst=0), " +
                        "Max velocity count (Best=1 Worst=0), Reaching time (Best=0 Worst=1), Path taken (Best=1 Worst=0), Jerkiness (Best=0 Worst=1), " +
                        "Bubble popped (Best=1 Worst=0), Total Score (Best=100 Worst=0), Bubble Position X, Bubble Position Y, Bubble Position Z, " +
                        "Bubble size, Distance between bubbles";
                    //Write a line of text
                    writetext.WriteLine(heading);
                    writetext.WriteLine(data_to_write);
                    //Close the file
                    writetext.Close();
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("Exception: " + e.Message);
        }
    }

    private void Preprocess_data()
    {

        foreach (var iter in user_data)

        {
            int row_size = iter.Value.Count();
            int col_size = iter.Value[0].Length;
            for (int i = 0; i < col_size; i++)
            {
                float count = 0;
                // Sumamerize all the values in the column number i.
                for (int j = 0; j < row_size; j++)
                {
                    count += iter.Value[j][i];
                }
                iter.Value[0][i] = count / row_size;
            }
            iter.Value.RemoveRange(1, row_size - 1);
        }
    }

    public void read_file(string file_to_read)
    {
        string[] text = File.ReadAllLines(file_to_read);
        int flag = 0;
        bool skipNextLine = false;
        foreach (var line in text)
        {
            //Don't need the first line from the text.
            if (flag < 2)
                flag++;
            else
            {

                string[] tokens = line.Split(',');
                if (tokens[0] == "" && tokens[1] == "")
                {
                    skipNextLine = true;
                    Write_data_to_file();
                    continue;
                }
                else if (skipNextLine)
                    skipNextLine = false;
                else
                {
                    New_data_line(Convert.ToSingle(tokens[0]), Convert.ToSingle(tokens[1]), Convert.ToSingle(tokens[2]),
                        Convert.ToSingle(tokens[3]), Convert.ToSingle(tokens[4]), Convert.ToInt32(tokens[5]),
                        Convert.ToInt32(tokens[6]), Convert.ToSingle(tokens[7]), Convert.ToSingle(tokens[8]),
                        Convert.ToSingle(tokens[9]), tokens[10]);
                }
                flag++;

            }
        }
        Write_data_to_file();
    }
}