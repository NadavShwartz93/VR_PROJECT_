using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;



/// Based on: https://docs.microsoft.com/en-us/archive/msdn-magazine/2017/december/test-run-understanding-k-nn-classification-using-csharp#wrapping-up
class KNN
{
    private static KNN instance = null;

    //Simulate User Vector
    private double[] useVectorKmeans;

    private Dictionary<int, int[]> kmeansClusters = new Dictionary<int, int[]>();

    private KNN()
    {
        instance = this;

        //Initialize the useVectorKmeans array.
        this.getParametersColumnsFromData();
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
    public static Dictionary<int, int[]> JsonToDictionary(string[] json)
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
        //int numOfColums = Globals.numOfColumnsInDataSet;
        int numOfColums = Globals.numOfParameters;
        int k = Globals.K;

        //Prepare input for KNN .

        //Read the CentralVectorsKmeans.csv file into double array.
        double[][] CentralVectorskmeans = readCentralVector(Globals.CentralVectorsKmeansFile,
            numClasses);

        //Read the KmeansClusters.txt file into Dictionary.
        string[] result = File.ReadAllLines(Globals.KmeansClustersFile);
        kmeansClusters = JsonToDictionary(result);

        string[] dataset = File.ReadAllLines(Globals.datasetFile);

        //Find the most similar vectors in the predictedClass
        //and write the data to knnOutput.csv
        int[] predictedClass = Classify(useVectorKmeans, CentralVectorskmeans,
            numClasses, numOfColums); //The predicted class

        double[][] datasetVectors = CreateSpesificVectors(dataset,
            kmeansClusters[predictedClass[0]]);

        int[] predictedVectors = Classify(useVectorKmeans, datasetVectors, numClasses,
        numOfColums, k, true); //The predicted vectors inside the predited class                          

        Write_To_Csv_File(Globals.KnnOutputFile, predictedVectors);
        ///////////////////////////////////////////////////
    }

    private double[][] CreateSpesificVectors(string[] dataset, int[] rows)
    {
        //local variable.
        int rowInDataSet = 0;
        int arrayrows = 0;
        int i = 0;
        double[][] datVec = new double[rows.Length][];

        foreach (string line in dataset)
        {
            if (rowInDataSet != 0 && rowInDataSet == rows[arrayrows])
            {
                var temp = Globals.GetRangeFromArr<string>(line.Split(','), 
                    Globals.firstParameterColumnNumber, Globals.numOfParameters);

                datVec[i] = Globals.convertToDouble(temp, 0);

                i++;

                if (rows.Length > arrayrows + 1) //Terminate overflow in rows[arrayrows]
                    arrayrows++;
            }

            rowInDataSet++;
        }

        return datVec;
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
            curr.idx = i;
            curr.dist = dist;
            info[i] = curr;
        }

        // sort the info array so the lowset value
        // will be in the fisrt index 
        Array.Sort(info);

        if (findFirstK)
            return VoteMostClosestK(info, trainData, k);

        //int[] result = VoteMostClosetClass(info, trainData, numClasses, numOfColums);
        int[] result = new int[1];
        result[0] = info[0].idx;
        return result;
    }

    static int[] VoteMostClosestK(IndexAndDistance[] info, 
        double[][] trainData, int k)
    {
        int[] votes = new int[k];  // One cell per class
        for (int i = 0; i < k; ++i)
        {       // Just first k
            votes[i] = info[i].idx;
        }

        Array.Sort(votes);//!!

        return votes;
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
        public int idx;  // Index of a training item
        public double dist;  // To unknown
                             // Need to sort these to find k closest
        public int CompareTo(IndexAndDistance other)
        {
            if (this.dist < other.dist) return -1;
            else if (this.dist > other.dist) return +1;
            else return 0;
        }
    }
}