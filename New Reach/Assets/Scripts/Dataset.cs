using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using System.IO;


/// <summary>
/// This class was written by Nadav Shwarz and Gal Sherman.
/// This class save all the user results into the Dataset.
/// </summary>
public class Dataset
{
    private static Dataset instance = null;
    private Dictionary<string, List<float[]>> user_data =
        new Dictionary<string, List<float[]>>();

    private string[] PatientDetailes = new string[6];

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

            //Create new Matrix and push new data.
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


    private void Write_To_Csv_File(string data_to_write)
    {
        string path = Globals.datasetFilePath;
        try
        {
            //Pass the file-path and filename to the StreamWriter Constructor
            using (StreamWriter writetext = new StreamWriter(path, true))
            {
                //Write a line of text
                writetext.WriteLine(data_to_write);
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
                // Summarize all the values in the column number i.
                for (int j = 0; j < row_size; j++)
                {
                    count += iter.Value[j][i];
                }
                iter.Value[0][i] = count / row_size;
            }
            iter.Value.RemoveRange(1, row_size - 1);
        }
    }
}