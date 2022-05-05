using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

/// <summary>
/// This class was written by Nadav Shwarz and Gal Sherman.
/// Based on: https://en.wikipedia.org/wiki/K-means_clustering
/// </summary>
class Kmeans
{
    private static Kmeans instance = null;
    private float[,] data_mat;
    private Dictionary<string, float> BubbleInSpace;
    private float[,] central_vectors;
    private const int num_of_classes = Globals.num_of_classes; // number of clusters
    private Dictionary<int, HashSet<int>> classification = new Dictionary<int, HashSet<int>>();

    //Constant variables that we are using in this class.
    private const int numberOfColumns = Globals.numOfParameters;
    private const int id_column_number = 0;
    private const int bubble_in_space_column_number = 6;

    private Kmeans()
    {
        instance = this;

        //Initialize the BubbleInSpace dictionary.
        this.InitializeBubbleInSpaceDictionary();

        //Initialize the central vectors!
        this.InitializeCentralVectors();

        //Initialize the classification Dictionary.
        this.InitializeClassificationDictionary();

        //Read all the data from Dataset.csv and save to data_mat 2D array.
        this.ReadFileToArray();
    }


    /// <summary>
    /// Kmeans is a Singleton class, 
    /// this method return instance of Kmeans class.
    /// </summary>
    /// <returns></returns>
    public static Kmeans GetInstance()
    {
        if (instance == null)
        {
            instance = new Kmeans();
        }
        return instance;
    }

    /// <summary>
    /// This method read the Dataset.csv file, 
    /// and save the data inside data_mat of 2D float array.
    /// </summary>
    private void ReadFileToArray()
    {
        List<string[]> list = ReadLines(Globals.datasetFilePath);

        data_mat = new float[list.Count, numberOfColumns];

        ParametersColumnsFromData(list, data_mat);
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
    /// This method parsing string to float value. 
    /// the parsing perform on the parameters columns only.
    /// </summary>
    /// <param name="list"></param>
    /// <param name="mtx"></param>
    private void ParametersColumnsFromData(List<string[]> list, float[,] mtx)
    {
        int i = 0;
        int start = Globals.firstParameterColumnNumber;

        foreach (string[] elem in list)
        {
            for (int j = 0; j < numberOfColumns; j++)
            {
                mtx[i, j] = float.Parse(elem[start + j]);
            }
            i++;
        }
    }

    /// <summary>
    /// This method calculate the euclidean distance of two given vectors.
    /// </summary>
    /// <param name="row_num1"></param>
    /// <param name="central_vectors_row_num"></param>
    /// <param name="vector_size"></param>
    /// <returns>The euclidean distance of the two vectors.</returns>
    private float EuclideanDistance(int row_num1, int central_vectors_row_num, int vector_size)
    {
        return Globals.Euclidean_distance(GetRow(data_mat, row_num1),
            GetRow(central_vectors, central_vectors_row_num), vector_size);
    }


    /// <summary>
    /// This method initialize the BubbleInSpace dictionary.
    /// </summary>
    private void InitializeBubbleInSpaceDictionary()
    {
        BubbleInSpace = Globals.InitializeBubbleInSpaceDictionary();
    }


    /// <summary>
    /// This method initialize the Classification dictionary.
    /// </summary>
    private void InitializeClassificationDictionary()
    {
        for (int i = 0; i < num_of_classes; i++)
        {
            classification.Add(i, new HashSet<int>());
        }
    }


    /// <summary>
    /// This method initialize the central vectors that K-means need to operate.
    /// </summary>
    private void InitializeCentralVectors()
    {
        //Local variables.
        const int row_size = num_of_classes;
        const int col_size = numberOfColumns;
        var list = ReadLines(Globals.CentralVectorsKmeansFilePath);
        central_vectors = new float[row_size, col_size];


        int j = 0;
        foreach (string[] elem in list)
        {
            for (int i = 0; i < elem.Length; i++)
            {
                central_vectors[j, i] = float.Parse(elem[i]);
            }
            j++;
        }
    }

    /// <summary>
    /// This method check if any of the classification Dictionary value contain the vector_num.
    /// If true, remove it.
    /// </summary>
    /// <param name="Key_to_stay"></param>
    /// <param name="vector_num"></param>
    private void RemoveItem(int Key_to_stay, int vector_num)
    {
        for (int i = 0; i < num_of_classes; i++)
        {
            if (i != Key_to_stay && classification[i].Contains(vector_num))
                classification[i].Remove(vector_num);
        }
    }

    /// <summary>
    /// This method represent the assignment step in the Kmeans algorithm.
    /// </summary>
    private void AssignmentStep()
    {
        int row_size = data_mat.GetLength(0);
        int col_size = data_mat.GetLength(1);
        int center_vec_index;

        for (int vector_num = 0; vector_num < row_size; vector_num++)
        {
            center_vec_index = 0;
            float minimum_value = EuclideanDistance(vector_num, center_vec_index, col_size);

            //Find the closest central vector to the vector_num.
            for (int j = 1; j < central_vectors.GetLength(0); j++)
            {
                float temp = EuclideanDistance(vector_num, j, col_size);
                if (temp < minimum_value)
                {
                    minimum_value = temp;
                    center_vec_index = j;
                }
            }

            //Update the classification Dictionary.
            if (!classification[center_vec_index].Contains(vector_num))
            {
                RemoveItem(center_vec_index, vector_num);
                classification[center_vec_index].Add(vector_num);
            }
        }

    }


    private void UpdateCentralVectors(int key_number)
    {
        int col_size = central_vectors.GetLength(1);
        for (int i = 0; i < col_size; i++)
        {
            float cnt = 0;
            foreach (var v_num in classification[key_number])
            {
                cnt += data_mat[v_num, i];
            }

            float res = cnt / classification[key_number].Count;
            if (float.IsNaN(res))
            {
                string msg = "res is Nan the reason: \nThere is a duplicates values in the Dataset.csv. " +
                    "\nPlease remove them.";
                throw new InvalidOperationException(msg);
            }
            central_vectors[key_number, i] = float.IsNaN(res) ? 0 : res;
        }
    }


    /// <summary>
    /// This method represent the update step in the Kmeans algorithm.
    /// </summary>
    private void UpdateStep()
    {
        for (int i = 0; i < num_of_classes; i++)
        {
            UpdateCentralVectors(i);

        }
    }

    public void Train(int numOfIteration)
    {
        for (int i = 0; i < numOfIteration; i++)
        {
            AssignmentStep();
            UpdateStep();
        }
        WriteToCsvFile();
        DictionaryToJson();

    }

    private void WriteToCsvFile()
    {
        string path = Globals.CentralVectorsKmeansFilePath;
        try
        {
            //Pass the file-path and filename to the StreamWriter Constructor
            using (StreamWriter writetext = new StreamWriter(path))
            {
                string header = "Velocity average (Best=1 Worst=0),Max velocity count (Best=1 Worst=0),Reaching time (Best=0 Worst=1),Path taken (Best=1 Worst=0)";
                writetext.WriteLine(header);

                //Write a line of text
                for (int i = 0; i < central_vectors.GetLength(0); i++)
                {
                    string data_to_write = "";
                    data_to_write += string.Join(",", GetRow(central_vectors, i));
                    writetext.WriteLine(data_to_write);
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("Exception: " + e.Message);
        }

    }

    private float[] GetRow(float[,] matrix, int rowNumber)
    {
        return Globals.GetRow(matrix, rowNumber);
    }

    private void DictionaryToJson()
    {
        var entries = classification.Select(d =>
            string.Format("{0} " +
            ": [{1}]", d.Key, string.Join(",", d.Value)));
        string s = "{\n " + string.Join(",\n", entries) + ",\n}";

        //Pass the file-path and filename to the StreamWriter Constructor
        using (StreamWriter writeText = new StreamWriter(Globals.KmeansClustersFilePath))
        {
            writeText.WriteLine(s);
        }
    }
}