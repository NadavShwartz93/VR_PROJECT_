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
        private float[] useVectorKmeans = new float[] { 0, 1, (float)0.1, (float)0.9008626, (float)0.1519834, (float)0.02166149, 1, 51, (float)0.8, (float)0.4, (float)0.5 };//User Vector
        private Dictionary<string, List<float[]>> user_data = new Dictionary<string, List<float[]>>();

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
        /*        public static string DictionaryToJson(Dictionary<string, List<float[]>> dict)
                {
                    var entries = dict.Select(d =>
                        string.Format("{0}:{1}", d.Key, string.Join(",", d.Value)));
                    return string.Join(",", entries);
                }*/
        public static Dictionary<string, List<float[]>> JsonToDictionary(string json)
        {
            Dictionary<string, List<float[]>> values = new Dictionary<string, List<float[]>>();
            string[] items = json.Split(',');
            foreach (string item in items)
            {
                string[] keyValue = item.Split(':');
               // values.Add(keyValue[0], keyValue[1]);
            }
            return values;
        }
        public void start()
        {
            Console.WriteLine("Begin k-NN classification demo ");
            var centralVectorList = ReadLines(Globals.CentralVectorsKmeans_dataset);
           // DictionaryToJson(user_data);
            /*  double[][] trainData = LoadData();
              int numFeatures = 2;// Currently we are not using it.This could be useful to our project
              int numClasses = 3; //In this case the classes are 0 || 1 || 2
              int k, predicted;
              double[] unknown = new double[] { 5.25, 1.75 }; ///{X point , Y point}
              Console.WriteLine("Predictor values: 5.25 1.75 ");
            */
            ////With k = 1 predicted class should be 1.Inside the voted array -> (“1,”)///////////
            /*int k = 1;
            Console.WriteLine("With k = 1");
            int predicted = Classify(unknown, trainData,
              numClasses, k);
            Console.WriteLine("Predicted class = " + predicted);
            */
            ////With k = 4 predicted class should be 4.Inside the voted array -> (“1,” “0,” “2,” “2”)///////////

            /*    k = 4;
                Console.WriteLine("With k = 4");
               // predicted = Classify(unknown, trainData, numClasses, k);

                Console.WriteLine("Predicted class = " + predicted);
                Console.WriteLine("End k-NN demo ");
                Console.ReadLine();*/
        }

        public static int Classify(double[] unknown, double[][] trainData, int numClasses, int k)

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
            ////Gal changes/////////////////////////////////////
            var watch = new System.Diagnostics.Stopwatch();
            watch.Start();
            Array.Sort(info); // sort the info array so the lowset value will be in the fisrt index 
            watch.Stop();
            ////////////////////////////////////////////////////

            int result = Vote(info, trainData, numClasses, k);
            return result;
        }
        ////In case the voted array there are classes that shows the same count (“2,” “1,” “1,” “2”)
        //the class that taken is the lowest so in the above simple example 1 should be choosen
        static int Vote(IndexAndDistance[] info, double[][] trainData, int numClasses, int k)

        {
            int[] votes = new int[numClasses];  // One cell per class
            for (int i = 0; i < k; ++i)
            {       // Just first k
                int idx = info[i].idx;            // Which train item
                int c = (int)trainData[idx][2];   // Class in last cell
                ++votes[c];
            }
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
        static double[][] LoadData()
        {
            double[][] data = new double[33][];
            /// {point x, point y, class( 0 || 1 || 2)}
            data[0] = new double[] { 2.0, 4.0, 0 };
            data[1] = new double[] { 2.0, 5.0, 0 };
            data[2] = new double[] { 2.0, 6.0, 0 };
            data[3] = new double[] { 3.0, 3.0, 0 };
            data[4] = new double[] { 4.0, 3.0, 0 };
            data[5] = new double[] { 5.0, 3.0, 0 };
            data[6] = new double[] { 3.0, 7.0, 0 };
            data[7] = new double[] { 4.0, 7.0, 0 };
            data[8] = new double[] { 5.0, 7.0, 0 };
            data[9] = new double[] { 6.0, 4.0, 0 };
            data[10] = new double[] { 6.0, 5.0, 0 };
            data[11] = new double[] { 6.0, 6.0, 0 };
            data[12] = new double[] { 3.0, 4.0, 1 };
            data[13] = new double[] { 4.0, 4.0, 1 };
            data[14] = new double[] { 5.0, 4.0, 1 };
            data[15] = new double[] { 3.0, 5.0, 1 };
            data[16] = new double[] { 4.0, 5.0, 1 };
            data[17] = new double[] { 5.0, 5.0, 1 };
            data[18] = new double[] { 3.0, 6.0, 1 };
            data[19] = new double[] { 4.0, 6.0, 1 };
            data[20] = new double[] { 5.0, 6.0, 1 };
            data[21] = new double[] { 6.0, 1.0, 1 };
            data[22] = new double[] { 7.0, 1.0, 1 };
            data[23] = new double[] { 8.0, 1.0, 1 };
            data[24] = new double[] { 7.0, 2.0, 1 };
            data[25] = new double[] { 8.0, 2.0, 1 };
            data[26] = new double[] { 8.0, 3.0, 1 };
            data[27] = new double[] { 2.0, 1.0, 2 };
            data[28] = new double[] { 3.0, 1.0, 2 };
            data[29] = new double[] { 4.0, 1.0, 2 };
            data[30] = new double[] { 2.0, 2.0, 2 };
            data[31] = new double[] { 3.0, 2.0, 2 };
            data[32] = new double[] { 4.0, 2.0, 2 };
            return data;
        }
    }
}
