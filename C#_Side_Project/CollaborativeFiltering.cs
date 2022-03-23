using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


class CollaborativeFiltering
{  
    private static CollaborativeFiltering instance = null;
    private int[] neighborsNumbersFromKnn;
    private float[,] neighborsData;

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


        //Read the Dataset.csv file, and save only the lines
        //that appears in the neighborsNumbersFromKnn array.
        File.ReadAllLines(Globals.file_name_dataset);
        
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
        return Enumerable.Range(1, len - 1)
            .Select(x => float.Parse(stringRow[x]))
            .ToArray();
    }
}
