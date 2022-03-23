﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


class CollaborativeFiltering
{  
    private static CollaborativeFiltering instance = null;
    private int[] neighborsNumbersFromKnn;
    private float[][] neighborsData;

    private CollaborativeFiltering()
    {
        instance = this;

    }

    public static CollaborativeFiltering Get_instance()
    {
        if (instance == null)
        {
            instance = new CollaborativeFiltering();
        }
        return instance;
    }

    /// <summary>
    /// This method is reading and the inputs from the deferents resources.
    /// </summary>
    public void getInputs()
    {
        //Read the KnnOutput.csv file and save it in int array.
        string[] row = File.ReadAllLines(Globals.KnnOutput);
        row = row[0].Split(',');
        neighborsNumbersFromKnn = convertToInt(row);

        //Initialize array size.
        int size = neighborsNumbersFromKnn.Count();
        neighborsData = new float[size][];

        //Read the Dataset.csv file, and save only the lines
        //that appears in the neighborsNumbersFromKnn array.
        string[] dataset = File.ReadAllLines(Globals.file_name_dataset);
        getUseresData(dataset);
        
    }


    private void getUseresData(string[] dataset)
    {
        //local variable.
        int rowCounter = 0;
        int arrayIndex = 0;

        foreach(string line in dataset)
        {

            //The case the count is not in neighborsNumbersFromKnn array.
            if (rowCounter != 0 && neighborsNumbersFromKnn.Contains(rowCounter)) {
                var temp = line.Split(',');

                temp[Globals.bubble_in_space_column_number] = 
                    Globals.getBubbleNumber(temp[Globals.bubble_in_space_column_number]).ToString();

                neighborsData[arrayIndex] = convertToFloat(temp);
                arrayIndex++;
            }

            rowCounter++;
        }
    }

    /// <summary>
    /// This method convert an string array into int array.
    /// </summary>
    /// <param name="stringRow"></param>
    /// <returns>double array.</returns>
    private int[] convertToInt(string[] stringRow)
    {
        var len = stringRow.Length;
        return Enumerable.Range(0, len - 1)
            .Select(x => int.Parse(stringRow[x]))
            .ToArray();
    }

    private float[] convertToFloat(string[] stringRow)
    {
        var len = stringRow.Length;
        return Enumerable.Range(1, len - 3)
            .Select(x => float.Parse(stringRow[x]))
            .ToArray();
    }
}
