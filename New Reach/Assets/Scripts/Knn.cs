using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

/// <summary>
/// This class was written by Nadav Shwarz and Gal Sherman.
/// Based on: https://docs.microsoft.com/en-us/archive/msdn-magazine/2017/december/test-run-understanding-k-nn-classification-using-csharp#wrapping-up
/// </summary>
class KNN
{
    private static KNN instance = null;

    //Simulate User Vector
    private double[] useVectorKmeans;

    private Dictionary<int, int[]> kmeansClusters = new Dictionary<int, int[]>();

    private double[][] CentralVectorskmeans;

    private string[] dataset;

    private KNN()
    {
        instance = this;

        //Prepare input for KNN .
        //////////////////////////////////////////

        //Read the CentralVectorsKmeans.csv file into double array.
        this.CentralVectorskmeans = readCentralVector(Globals.CentralVectorsKmeansFilePath,
            Globals.num_of_classes);

        //Read the KmeansClusters.txt file into Dictionary.
        string[] result = File.ReadAllLines(Globals.KmeansClustersFilePath);
        kmeansClusters = JsonToDictionary(result);

        this.dataset = File.ReadAllLines(Globals.datasetFilePath);
    }

    public static KNN Get_instance()
    {
        if (instance == null)
        {
            instance = new KNN();
        }
        return instance;
    }

    private void getParametersColumnsFromData()
    {
        useVectorKmeans = new double[Globals.numOfParameters];

        int start = Globals.firstParameterColumnNumber;

        for (int j = 0; j < Globals.numOfParameters; j++)
        {
            useVectorKmeans[j] = Globals.simulateUseVector[start + j];
        }
    }

    private void Write_To_Csv_File(string path, int[] simUsers)
    {
        try
        {
            //Pass the file-path and filename to the StreamWriter Constructor
            using (StreamWriter writetext = new StreamWriter(path))
            {
                //write the array in the first line of the csv file
                writetext.WriteLine(string.Join(",", simUsers));
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("Exception: " + e.Message);
        }
    }

    private double[][] readCentralVector(string fileName, int numOfClasses)
    {
        double[][] dataFromText = new double[numOfClasses][];

        string[] text = File.ReadAllLines(fileName);
        int skipFirstLineInDataset = 0;
        int numberOfCluster = 0;

        //Save the string array in the List collection.
        foreach (var line in text)
        {
            //Don't need the first line from the dataset.
            if (skipFirstLineInDataset < 1)
            {
                skipFirstLineInDataset++;
            }
            else
            {
                string[] tokens = line.Split(',');
                var doubleArray = Globals.convertToDouble(tokens, 0);

                //Add in the last element  in trainData cluster number! 
                double[] d = { numberOfCluster };

                dataFromText[numberOfCluster] = doubleArray.Concat(d).ToArray();

                numberOfCluster++;
            }
        }
        return dataFromText;
    }


    /// <summary>
    /// Convert the file KmeansClusters.txt from Json style to Dictionary structure
    /// </summary>
    private Dictionary<int, int[]> JsonToDictionary(string[] json)
    {
        Dictionary<int, int[]> values = new Dictionary<int, int[]>();
        for (int i = 1; i < json.Length - 1; i++) // iterate all besides, first and last character  "{" , "}"
        {
            string[] items = json[i].Split(':');
            string rows = items[1].Replace(@"[", string.Empty).Replace(@"],", string.Empty);

            int[] nums = Array.ConvertAll(rows.Split(','), int.Parse);
            values.Add(int.Parse(items[0]), nums);
        }

        return values;
    }

    public void start()
    {
        //General constants.
        const int numClasses = Globals.num_of_classes; //In this case the classes are 0 || 1 || 2 ...
        int numOfColums = Globals.numOfParameters;
        int k = Globals.K;

        //Initialize the useVectorKmeans array.
        this.getParametersColumnsFromData();

        //Find the most similar vectors in the predictedClass
        //and write the data to knnOutput.csv
        int[] predictedClass = Classify(useVectorKmeans, CentralVectorskmeans,
            numClasses, numOfColums); //The predicted class

        Write_To_Csv_File(Globals.KnnOutputFilePath, predictedClass);
        ///////////////////////////////////////////////////

        for (int i = 0; i < Globals.numOfAreas; i++)
        {
            Debug.Log(i + ". is equal:  " + Globals.numOfApperancce[i]);
        }

        Debug.Log("Selected class = " + predictedClass[0]);

        BubblePosition.getInstance().calculateBubblePosition(predictedClass);

        Debug.Log("KNN.cs finished!!!");
    }

    private int[] Classify(double[] unknown, double[][] trainData, int numClasses,
        int numOfColums, int k = 0, bool findFirstK = false)
    {
        int n = trainData.Length;
        IndexAndDistance[] info = new IndexAndDistance[n];
        for (int i = 0; i < n; ++i)
        {
            IndexAndDistance curr = new IndexAndDistance();
            double dist = Distance(unknown, trainData[i]);
            curr.index = i;
            curr.dist = dist;
            info[i] = curr;
        }

        // sort the info array so the lowset value
        // will be in the fisrt index 
        Array.Sort(info);

        return getClasses(info);
    }

    private int[] getClasses(IndexAndDistance[] info)
    {
        int[] result = new int[info.Length];
        for (int i = 0; i < info.Length; i++)
        {
            result[i] = info[i].index;
        }

        return result;
    }


    /// <summary>
    /// Calculation of Euclidean distance.
    /// </summary>
    /// <param name="unknown"></param>
    /// <param name="data"></param>
    /// <returns></returns>
    private double Distance(double[] unknown, double[] data)
    {
        return Globals.Euclidean_distance(unknown, data, unknown.Length);
    }

    /// <summary>
    /// Interface for the distance and relevant index.
    /// </summary>
    private class IndexAndDistance : IComparable<IndexAndDistance>
    {

        /// <summary>
        /// Index of a training item
        /// </summary>
        public int index { get; set; }
        public double dist { get; set; }  // To unknown
                                          // Need to sort these to find k closest
        public int CompareTo(IndexAndDistance other)
        {
            if (this.dist < other.dist) return -1;
            else if (this.dist > other.dist) return +1;
            else return 0;
        }
    }
}