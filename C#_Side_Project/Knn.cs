using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;



/// Based on: https://docs.microsoft.com/en-us/archive/msdn-magazine/2017/december/test-run-understanding-k-nn-classification-using-csharp#wrapping-up
class KNN
{
    private static KNN instance = null;

    //Simulate User Vector
    private double[] useVectorKmeans = Globals.simulateUseVectorKmeans;

    private Dictionary<int, int[]> kmeansClusters = new Dictionary<int, int[]>();

    private KNN()
    {
        instance = this;
    }

    public static KNN Get_instance()
    {
        if (instance == null)
        {
            instance = new KNN();
        }
        return instance;
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

    private List<double[]> ReadLines(string fileName)
    {

        string[] text = File.ReadAllLines(fileName);
        int skipFirstLineInDataset = 0;
        List<double[]> list = new List<double[]>();

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
                var floatArray = Globals.convertToDouble(tokens, 1, 1);
                list.Add(floatArray);
            }
        }
        return list;
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
        int numOfColums = Globals.numOfColumnsInDataSet;
        int k = Globals.K;

        //Prepare input for KNN .
        /////////////////////////////////////////////////

        //Read and prepare the CentralVectorsKmeans.csv file.
        List<double[]> readCentralVector = ReadLines(Globals.CentralVectorsKmeans_dataset);
        double[][] CentralVectorskmeans = toMatrixOfDouble(numClasses, readCentralVector);

        //Read and prepare the KmeansClusters.txt
        string[] result = File.ReadAllLines(Globals.KmeansClusters);
        kmeansClusters = JsonToDictionary(result);

        List<int[]> mostSimilarVec = new List<int[]>();
        string[] dataset = File.ReadAllLines(Globals.file_name_dataset);

        //Find the most similar vectors in the predictedClass and write the data to knnOutput.csv
        int[] predictedClass = Classify(useVectorKmeans, CentralVectorskmeans, numClasses,
            numOfColums); //The predicted class

        double[][] datasetVectors = CreateSpesificVectors(dataset, kmeansClusters[predictedClass[0]]);
        int[] predictedVectors = Classify(useVectorKmeans, datasetVectors, numClasses,
        numOfColums, k, true); //The predicted vectors inside the predited class                          

        Write_To_Csv_File(Globals.KnnOutput, predictedVectors);
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
                var temp = line.Split(',');
                temp[Globals.bubble_in_space_column_number] =
                Globals.getBubbleNumber(temp[Globals.bubble_in_space_column_number]).ToString();
                var t = Globals.convertToDouble(temp, 0, 2);
                Array.Resize(ref t, t.Length + 1);
                t[t.Length - 1] = (double)rows[arrayrows]; //After resize int the last element will be the number that represent the class.
                datVec[i++] = t;

                if (rows.Length > arrayrows + 1) //Terminate overflow in rows[arrayrows]
                    arrayrows++;
            }

            rowInDataSet++;
        }

        return datVec;
    }

    /// <summary>
    /// This method convert from List<double[]> to double[][] array.
    /// </summary>
    /// <param name="numClasses"></param>
    /// <param name="centralVector"></param>
    /// <returns></returns>
    private double[][] toMatrixOfDouble(int numClasses, List<double[]> centralVector)
    {
        double[][] trainData = new double[numClasses][];


        for (int i = 0; i < numClasses; i++)//
        {
            //Add in the last element  in trainData cluster number! 
            double[] d = { i };
            trainData[i] = centralVector[i].Concat(d).ToArray();
        }

        return trainData;
    }

    public static int[] Classify(double[] unknown, double[][] trainData, int numClasses,
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
        /////////////////////////////////////////////////////
        Array.Sort(info); // sort the info array so the lowset value will be in the fisrt index 
        ////////////////////////////////////////////////////
        if (findFirstK)
            return VoteMostClosestK(info, trainData, k);

        int[] result = VoteMostClosetClass(info, trainData, numClasses, numOfColums);
        return result;
    }

    ////In case the voted array there are classes that shows the same count (“2,” “1,” “1,” “2”)
    //the class that taken is the lowest so in the above simple example 1 should be choosen
    static int[] VoteMostClosetClass(IndexAndDistance[] info, double[][] trainData, int numClasses, int numOfColums)

    {
        int[] votes = new int[numClasses];  // One cell per class     
        int idx = info[0].idx;            // Which train item
        int c = (int)trainData[idx][numOfColums];   // Class in last cell
        ++votes[c];

        int mostVotes = 0;
        int[] classWithMostVotes = new int[1];
        for (int j = 0; j < numClasses; ++j)
        {
            if (votes[j] > mostVotes)
            {
                mostVotes = votes[j];
                classWithMostVotes[0] = j;
            }
        }
        return classWithMostVotes;
    }
    static int[] VoteMostClosestK(IndexAndDistance[] info, double[][] trainData,
     int k)
    {
        int[] votes = new int[k];  // One cell per class
        for (int i = 0; i < k; ++i)
        {       // Just first k
            int idx = info[i].idx;            // Which train item
            int c = (int)trainData[idx][trainData[0].Length - 1];   // Class in last cell
            votes[i] = c;
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
    static double Distance(double[] unknown, double[] data)
    {
        double sum = 0.0;
        for (int i = 0; i < unknown.Length; ++i)
            sum += (unknown[i] - data[i]) * (unknown[i] - data[i]);
        return Math.Sqrt(sum);
    }
    /// <summary>
    /// Interface for the distance and relevant index.
    /// </summary>
    public class IndexAndDistance : IComparable<IndexAndDistance>
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