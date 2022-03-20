using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// Based on: https://docs.microsoft.com/en-us/archive/msdn-magazine/2017/december/test-run-understanding-k-nn-classification-using-csharp#wrapping-up
namespace ConsoleApp
{


    class KNN
    {
        private static KNN instance = null;
        private Double[] useVectorKmeans = new Double[] { 0, 1, (float)0.1, (float)0.9008626, (float)0.1519834, (float)0.02166149, 1, 51, (float)0.8, (float)0.4, (float)0.5 };//User Vector
        private Dictionary<string, List<float[]>> user_data = new Dictionary<string, List<float[]>>();
        private Dictionary<int, int[]> kmeansClusters = new Dictionary<int, int[]>();
/*        private Dictionary<string, float> BubbleInSpace = new Dictionary<string, float>();
*/
        private KNN()
        {
            instance = this;

/*          Initialize_BubbleInSpace_dictionary();
*/        }
        public static KNN Get_instance()
        {
            if (instance == null)
            {
                instance = new KNN();
            }
            return instance;
        }

        /*        private void Initialize_BubbleInSpace_dictionary()
                {
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
                }*/

        private void Write_To_Csv_File(string path, List<int[]> simUsers,int k)
        {
            try
            {
                //Pass the file-path and filename to the StreamWriter Constructor
                using (StreamWriter writetext = new StreamWriter(path))
                {
                    string data_to_write="";
                    foreach (int[] user in simUsers)
                    {
                        for (int i = 0; i < k; i++)
                        {
                            data_to_write += $"{user[i]},";
                        }
                        //Write a line of text
                        writetext.WriteLine(data_to_write);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);
            }

        }
        private List<string[]> ReadLines(string fileName)
        {

            string[] text = File.ReadAllLines(fileName);
            int skipFirstLineInDataset = 0;
            List<string[]> list = new List<string[]>();

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
                    list.Add(tokens);
                }
            }
            return list;
        }

        /// <summary>
        /// Convert the file KmeansClusters.txt from json to Dictionary
        /// </summary>
        public static Dictionary<int, int[]> JsonToDictionary(string[] json)
        {
            Dictionary<int, int[]> values = new Dictionary<int, int[]>();
            for (int i = 1; i < json.Length - 1; i++) // iterate all besides, first and last character  "{" , "}"
            {
                string[] items = json[i].Split(':');
                string rows = items[1].Replace(@"[", string.Empty).Replace(@"]", string.Empty);
                string result = rows.Remove(rows.Length - 1); // remove last comma -> ,
                int[] nums = Array.ConvertAll(result.Split(','), int.Parse);
                values.Add(int.Parse(items[0]), nums);
            }

            return values;
        }

        /// <summary>
        /// Convert Vectors from excel to List of float array.
        /// </summary>
        private float[,] ListStringToFloat(List<string[]> listStr)
        {
            int columnNumbers = 11;
            float[,] floatArray = new float[3, columnNumbers];

            int j = 0;
            int i = 6;
            foreach (string[] stringArray in listStr)
            {
                for (i = 6; i < stringArray.Length - 2; i++) //Iterate over relevant fields iin csv !!
                {
                    string str = stringArray[i];
               /*     if (i == 6) //When column number its not relevant
                    {
                        float num;
                        num = BubbleInSpace[str];
                        floatArray[j, i - 6] = num;
                    }
                    else
                    {*/
                        floatArray[j, i - 6] = float.Parse(str);
                    //}
                }
                j++;


            }
            return floatArray;
        }
        public void start()

        {   //Prepare input for KNN /////////////////////////////
            List<string[]> readCentralVector = ReadLines(Globals.CentralVectorsKmeans_dataset);
            float[,] centralVectorList = ListStringToFloat(readCentralVector);
            string[] result = File.ReadAllLines(Globals.KmeansClusters);
            kmeansClusters = JsonToDictionary(result);
            List<int[]> mostSimilarVec = new List<int[]>();
            string[] dataset = File.ReadAllLines(Globals.file_name_dataset);
            int numClasses = 3; //In this case the classes are 0 || 1 || 2
            int numOfColums = 11;
            int k = 15;
            double[][] trainData = new double[numClasses][];

            ////////////////////////////////////////////////////
         
            for (int i = 0; i < numClasses ; i++)//Convert from float[,] to double[][] array
            {
                //Add in the last element  in trainData cluster number! 
                trainData[i] = new double[] { centralVectorList[i,0], centralVectorList[i, 1], centralVectorList[i, 2], centralVectorList[i, 3], centralVectorList[i, 4], centralVectorList[i, 5], centralVectorList[i, 6], centralVectorList[i, 7], centralVectorList[i, 8], centralVectorList[i, 9], centralVectorList[i, 10], i };              
            }

            //Find the most similar vectors in the predictedClass
            int predictedClass = Classify(useVectorKmeans, trainData, numClasses, numOfColums); //This is the predicted class
            Console.WriteLine("Predicted class = " + predictedClass);
            mostSimilarVec.Add(kmeansClusters[predictedClass]);
            Write_To_Csv_File(Globals.KnnOutput, mostSimilarVec,k);
            ///////////////////////////////////////////////////
        }

        public static int Classify(double[] unknown, double[][] trainData, int numClasses,int numOfColums)
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

            int result = Vote(info, trainData, numClasses, numOfColums);
            return result;
        }
        ////In case the voted array there are classes that shows the same count (“2,” “1,” “1,” “2”)
        //the class that taken is the lowest so in the above simple example 1 should be choosen
        static int Vote(IndexAndDistance[] info, double[][] trainData, int numClasses, int numOfColums)

        {
            int[] votes = new int[numClasses];  // One cell per class     
                int idx = info[0].idx;            // Which train item
                int c = (int)trainData[idx][numOfColums];   // Class in last cell
                ++votes[c];

            int mostVotes = 0;
            int classWithMostVotes = 0;
            for (int j = 0; j < numClasses; ++j)
            {
                if (votes[j] > mostVotes)
                {
                    mostVotes = votes[j];
                    classWithMostVotes = j;
                }
            }
            return classWithMostVotes;
        }
        static double Distance(double[] unknown, double[] data)
        {
            double sum = 0.0;
            for (int i = 0; i < unknown.Length; ++i)
                sum += (unknown[i] - data[i]) * (unknown[i] - data[i]);
            return Math.Sqrt(sum);
        }
        public class IndexAndDistance : IComparable<IndexAndDistance> //Interface for the info array
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
}
