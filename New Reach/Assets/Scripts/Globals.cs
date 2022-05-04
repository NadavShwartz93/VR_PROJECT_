using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using System.IO;

/// <summary>
/// This class was written by Nadav Shwarz and Gal Sherman.
/// This class saves all the global variables and methods.
/// The classes that we created use these variables.
/// </summary>
public class Globals
{
    private const int sizeOfArr = 17;
    public static double[] simulateUseVector = new double[sizeOfArr];

    public static readonly string datasetFilePath =
        Path.Combine(Application.persistentDataPath, "Dataset.csv");

    public static readonly string CentralVectorsKmeansFilePath =
        Path.Combine(Application.persistentDataPath, "CentralVectorsKmeans.csv");

    public static readonly string KmeansClustersFilePath =
        Path.Combine(Application.persistentDataPath, "KmeansClusters.txt");

    public static readonly string KnnOutputFilePath =
        Path.Combine(Application.persistentDataPath, "KnnOutput.csv");

    public static readonly string CfOutputFilePath =
        Path.Combine(Application.persistentDataPath, "CFOutput.csv");

    public static readonly string PatientDetailsFilePath =
        Path.Combine(Application.persistentDataPath, "PatientDetails.csv");

    /// <summary>
    /// This flag check if this is the first game round.
    /// </summary>
    public static bool startOfGame = true;
    /// <summary>
    /// This variable represent the number of training iteration of the Kmeans algorithm.
    /// </summary>
    public const int numOfTrainingIteration = 10;
    /// <summary>
    /// This variable represent the number of columns in the CentralVectorsKmeans.csv file.
    /// </summary>
    public const int numOfParameters = 4;
    /// <summary>
    /// This variable represent the column number of the first parameter
    /// which is the Velocity average parameter.
    /// </summary>
    public const int firstParameterColumnNumber = 7;
    /// <summary>
    /// The number of clusters.
    /// </summary>
    public const int num_of_classes = 3;
    /// <summary>
    /// K is the number of users in a cluster 
    /// that are the most similar to particular user.
    /// </summary>
    public const int K = 15;
    /// <summary>
    /// This is the number of different columns in the Dataset.csv file.
    /// </summary>
    public const int numOfColumnsInDataSet = 16;
    /// <summary>
    /// This is the number of columns that the bubble_in_space is located.
    /// </summary>
    public const int bubble_in_space_column_number = 6;
    /// <summary>
    /// 
    /// </summary>
    public const int numOfAreas = 8;
    /// <summary>
    /// 
    /// </summary>
    public static int[] numOfApperancce = new int[numOfAreas];

    ////// The area that the bubbles can show up//////////
    public const string FBCInSpace = "Front-Bottom-Center";
    public const string FBRInSpace = "Front-Bottom-Right";
    public const string FBLInSpace = "Front-Bottom-Left";
    public const string FTCInSpace = "Front-Top-Center";
    public const string FTRInSpace = "Front-Top-Right";
    public const string FTLInSpace = "Front-Top-Left";
    public const string BBCInSpace = "Back-Bottom-Center";
    public const string BBRInSpace = "Back-Bottom-Right";
    public const string BBLInSpace = "Back-Bottom-Left";
    public const string BTCInSpace = "Back-Top-Center";
    public const string BTRInSpace = "Back-Top-Right";
    public const string BTLInSpace = "Back-Top-Left";

    private static Dictionary<string, float> BubbleInSpace = new Dictionary<string, float>();

    public static Dictionary<string, float> Initialize_BubbleInSpace_dictionary()
    {
        if (BubbleInSpace.Count != 0)
            return BubbleInSpace;

        BubbleInSpace.Add(Globals.FBCInSpace, 0);
        BubbleInSpace.Add(Globals.FBRInSpace, 1);
        BubbleInSpace.Add(Globals.FBLInSpace, 2);
        BubbleInSpace.Add(Globals.FTCInSpace, 3);
        BubbleInSpace.Add(Globals.FTRInSpace, 4);
        BubbleInSpace.Add(Globals.FTLInSpace, 5);
        BubbleInSpace.Add(Globals.BBCInSpace, 6);
        BubbleInSpace.Add(Globals.BBRInSpace, 7);
        BubbleInSpace.Add(Globals.BBLInSpace, 8);
        BubbleInSpace.Add(Globals.BTCInSpace, 9);
        BubbleInSpace.Add(Globals.BTRInSpace, 10);
        BubbleInSpace.Add(Globals.BTLInSpace, 11);

        return BubbleInSpace;
    }

    public static float getBubbleNumber(string bubbleName)
    {
        return Initialize_BubbleInSpace_dictionary()[bubbleName];
    }

    //
    #region Conversion Methods

    public static double[] convertToDouble(string[] stringRow, int startIdx,
        int numOfIndexToRemove = -1)
    {
        int len;
        if (numOfIndexToRemove == -1)
            len = stringRow.Length;
        else
            len = stringRow.Length - numOfIndexToRemove;

        return Enumerable.Range(startIdx, len).
            Select(x => double.Parse(stringRow[x])).ToArray();
    }

    /// <summary>
    /// This method convert an string array into int array.
    /// </summary>
    /// <param name="stringRow"></param>
    /// <returns>int array.</returns>
    public static int[] convertToInt(string[] stringRow, int startIdx)
    {
        var len = stringRow.Length;
        return Enumerable.Range(startIdx, len)
            .Select(x => int.Parse(stringRow[x]))
            .ToArray();
    }

    /// <summary>
    /// This method convert an string array into float array.
    /// </summary>
    /// <param name="stringRow"></param>
    /// <returns>float array.</returns>
    public static float[] convertToFloat(string[] stringRow, int startIdx, int lastIdx)
    {
        var len = stringRow.Length;
        return Enumerable.Range(startIdx, len - lastIdx)
            .Select(x => float.Parse(stringRow[x]))
            .ToArray();
    }

    #endregion

    /// <summary>
    /// This method is calculating the Euclidean distance of two vectors.
    /// </summary>
    /// <param name="v1"></param>
    /// <param name="v2"></param>
    /// <param name="vector_size"></param>
    /// <returns>The Euclidean distance of two vectors.</returns>
    public static float Euclidean_distance(float[] v1, float[] v2, int vector_size)
    {
        float counter = 0;
        const double power = 2;

        for (int i = 0; i < vector_size; i++)
        {
            float temp = (float)Math.Pow(v1[i] - v2[i], power);
            counter += temp;
        }

        return (float)Math.Sqrt(counter);
    }

    public static double Euclidean_distance(double[] v1, double[] v2, int vector_size)
    {
        double counter = 0;
        const double power = 2;

        for (int i = 0; i < vector_size; i++)
        {
            double temp = Math.Pow(v1[i] - v2[i], power);
            counter += temp;
        }

        return Math.Sqrt(counter);
    }

    public static T[] GetRow<T>(T[,] matrix, int rowNumber)
    {
        return Enumerable.Range(0, matrix.GetLength(1))
                .Select(x => matrix[rowNumber, x])
                .ToArray();
    }

    public static T[] GetRow<T>(T[][] matrix, int rowNumber)
    {
        int count = matrix[0].Length;
        return Enumerable.Range(0, count)
                .Select(x => matrix[rowNumber][x])
                .ToArray();
    }

    /// <summary>
    /// This method return a specific range of elements that start in start index.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="array"></param>
    /// <param name="rowNumber"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    public static T[] GetRangeFromArr<T>(T[] array, int start, int count)
    {
        return Enumerable.Range(start, count)
                .Select(x => array[x])
                .ToArray();
    }

    public static T[] GetCol<T>(T[][] matrix, int colNumber)
    {
        int count = matrix.Length;
        return Enumerable.Range(0, count)
                .Select(x => matrix[x][colNumber])
                .ToArray();
    }
}
