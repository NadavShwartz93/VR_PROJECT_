using System;
using System.IO;
using System.Linq;


class CollaborativeFiltering
{
    //Simulate User Vector
    private double[] useVectorCF = new double[] {0, 158, 0.438, 1, 180, 3,
        0.52, 0.4, 0.49, 0.03, 0.04, 0.67, 41, 0.43, 0.67, 0.4};
    //private double[] useVectorCF = Globals.simulateUseVector;

    private static CollaborativeFiltering instance = null;
    private int[] neighborsNumbersFromKnn;
    private float[][] neighborsData;
    private float[] predictedValues;
    private int predictedStartIdx;
    private int predictedLastIdx;

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
    private void getInputs()
    {
        //Read the KnnOutput.csv file and save it in int array.
        string[] row = File.ReadAllLines(Globals.KnnOutput);
        row = row[0].Split(',');
        neighborsNumbersFromKnn = Globals.convertToInt(row, 0, 1);

        //Initialize array size.
        int size = neighborsNumbersFromKnn.Count();
        neighborsData = new float[size][];

        //Read the Dataset.csv file, and save only the lines
        //that appears in the neighborsNumbersFromKnn array.
        string[] dataset = File.ReadAllLines(Globals.file_name_dataset);
        getUseresData(dataset);

        //Initialize array size.
        predictedValues = new float[useVectorCF.Length];

        //Initialize constants.
        predictedStartIdx = 6;
        predictedLastIdx = useVectorCF.Length;
    }


    private void getUseresData(string[] dataset)
    {
        //local variable.
        int rowCounter = 0;
        int arrayIndex = 0;

        foreach (string line in dataset)
        {

            //The case the count is not in neighborsNumbersFromKnn array.
            if (rowCounter != 0 && neighborsNumbersFromKnn.Contains(rowCounter))
            {
                var temp = line.Split(',');

                temp[Globals.bubble_in_space_column_number] =
                    Globals.getBubbleNumber(temp[Globals.bubble_in_space_column_number]).ToString();

                neighborsData[arrayIndex] = Globals.convertToFloat(temp, 1, 3);
                arrayIndex++;
            }
            rowCounter++;
        }
    }

    private float getAverageValue(double[] v, int vector_size)
    {
        float counter = 0;
        for (int i = 0; i < vector_size; i++)
        {
            counter += (float)v[i];
        }
        return counter / vector_size;
    }

    /// <summary>
    /// The average value of game result values for the player (patient).
    /// </summary>
    /// <returns></returns>
    private float calculate_V_a()
    {
        int left = predictedStartIdx;
        int count = predictedLastIdx - predictedStartIdx;

        var playerGameResult = Enumerable.Range(left, count)
            .Select(x => useVectorCF[x])
            .ToArray();

        return getAverageValue(playerGameResult, predictedLastIdx - predictedStartIdx);
    }


    /// <summary>
    /// Copy the first n's values of the user vectors.
    /// This values are constants, not going to change.
    /// The number of the first n's values = predictedStartIdx.
    /// </summary>
    private void copyFirstValues()
    {
        for (int i = 0; i < predictedStartIdx; i++)
            predictedValues[i] = (float)useVectorCF[i];
    }


    private void Write_To_Csv_File()
    {
        string path = Globals.CfOutput;

        try
        {
            //Pass the file-path and filename to the StreamWriter Constructor
            using (StreamWriter writetext = new StreamWriter(path))
            {
                string header = "Id,Hand in Therapy (0 for left 1 for right)," +
                    "Height(cm),Arm Length,Standing (0 for no 1 for yes)," +
                    "Treatment Time (sec),Bubble in space,Velocity average (Best=1 Worst=0)" +
                    ",Max velocity count (Best=1 Worst=0),Reaching time (Best=0 Worst=1)," +
                    "Path taken (Best=1 Worst=0),Jerkiness (Best=0 Worst=1)" +
                    ",Bubble popped (Best=1 Worst=0),Total Score (Best=100 Worst=0)" +
                    ",Bubble Position X,Bubble Position Y,Bubble Position Z" +
                    ",Bubble size,Distance between bubbles";

                writetext.WriteLine(header);

                string data_to_write = ",";
                data_to_write += string.Join(",", predictedValues);
                writetext.WriteLine(data_to_write);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("Exception: " + e.Message);
        }
    }

    private float formulaUpperPart()
    {
        return (float)1.2;
    }

    private float formulaDownSide()
    {
        return (float)1.2;
    }

    /// <summary>
    /// This method is the prediction function that calculate 
    /// the setting parameters.
    /// </summary>
    public void calculate_CF()
    {
        getInputs();
        copyFirstValues();

        for (int i = predictedStartIdx; i < predictedLastIdx; i++)
        {
            predictedValues[i] = calculate_V_a() + formulaUpperPart() / formulaDownSide();
        }

        Write_To_Csv_File();
    }
}
