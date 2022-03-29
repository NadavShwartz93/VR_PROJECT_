using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

/// <summary>
/// Based on: https://en.wikipedia.org/wiki/K-means_clustering
/// </summary>
class Kmeans
{
    private static Kmeans instance = null;
    private int numCentralVec; // m1,m2.... mk
    private float[,] data_mat;
    private Dictionary<string, float> BubbleInSpace;
    private float[,] central_vectors;
    private const int num_of_classes = Globals.num_of_classes; // number of clusters
    private Dictionary<int, HashSet<int>> classification = new Dictionary<int, HashSet<int>>();

    //Constant variables that we are using in this class.
    private const int numberOfColumns = 17;
    private const int id_column_number = 0;
    private const int bubble_in_space_column_number = 6;

    //private const string Front-Bottom-Center = 0;
    private Kmeans(int number_of_central_vectors)
    {
        instance = this;
        this.numCentralVec = number_of_central_vectors;

        //Initialize the BubbleInSpace dictionary.
        this.Initialize_BubbleInSpace_dictionary();

        //Initialize the central vectors!
        this.Initialize_central_vectors();

        //Initialize the classification Dictionary.
        this.Initialize_classification_dictionary();

        //Read all the data from Dataset.csv and save to data_mat 2D array.
        this.Read_file_to_array();
    }


    /// <summary>
    /// Kmeans is a Singleton class, 
    /// this method return instance of Kmeans class.
    /// </summary>
    /// <returns></returns>
    public static Kmeans Get_instance()
    {
        if (instance == null)
        {
            instance = new Kmeans(11);
        }
        return instance;
    }

    /// <summary>
    /// This method read the Dataset.csv file, 
    /// and save the data inside data_mat of 2D float array.
    /// </summary>
    private void Read_file_to_array()
    {
        List<string[]> list = ReadLines(Globals.file_name_dataset);

        data_mat = new float[list.Count, numberOfColumns - 1];

        ParsingStringToFloat(list, data_mat);
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

    private void ParsingStringToFloat(List<string[]> list, float[,] mtx)
    {
        int i = 0;
        foreach (string[] elem in list)
        {
            for (int j = 0; j < numberOfColumns; j++)
            {

                if (j == id_column_number)
                    continue;

                //For columns in table ->where 6 == Bubble in space
                if (j == bubble_in_space_column_number && BubbleInSpace.ContainsKey(elem[j]))
                {
                    mtx[i, j - 1] = BubbleInSpace[elem[j]];
                }

                else
                    mtx[i, j - 1] = float.Parse(elem[j]);
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
    private float Euclidean_distance(int row_num1, int central_vectors_row_num, int vector_size)
    {
        return Globals.Euclidean_distance(GetRow(data_mat, row_num1),
            GetRow(central_vectors, central_vectors_row_num), vector_size);
    }


    /// <summary>
    /// This method initialize the BubbleInSpace dictionary.
    /// </summary>
    private void Initialize_BubbleInSpace_dictionary()
    {
        BubbleInSpace = Globals.Initialize_BubbleInSpace_dictionary();
    }


    /// <summary>
    /// This method initialize the Classification dictionary.
    /// </summary>
    private void Initialize_classification_dictionary()
    {
        for (int i = 0; i < num_of_classes; i++)
        {
            classification.Add(i, new HashSet<int>());
        }
    }


    /// <summary>
    /// This method initialize the central vectors that K-means need to operate.
    /// </summary>
    private void Initialize_central_vectors()
    {
        //Local variables.
        const int row_size = num_of_classes;
        const int col_size = numberOfColumns - 1;
        var list = ReadLines(Globals.CentralVectorsKmeans_dataset);
        central_vectors = new float[row_size, col_size];
        ParsingStringToFloat(list, central_vectors);
    }

    /// <summary>
    /// This method check if any of the classification Dictionary value contain the vector_num.
    /// If true, remove it.
    /// </summary>
    /// <param name="Key_to_stay"></param>
    /// <param name="vector_num"></param>
    private void Remove_item(int Key_to_stay, int vector_num)
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
    private void Assignment_step()
    {
        int row_size = data_mat.GetLength(0);
        int col_size = data_mat.GetLength(1);
        int center_vec_index;

        for (int vector_num = 0; vector_num < row_size; vector_num++)
        {
            center_vec_index = 0;
            float minimum_value = Euclidean_distance(vector_num, center_vec_index, col_size);

            //Find the closest central vector to the vector_num.
            for (int j = 1; j < central_vectors.GetLength(0); j++)
            {
                float temp = Euclidean_distance(vector_num, j, col_size);
                if (temp < minimum_value)
                {
                    minimum_value = temp;
                    center_vec_index = j;
                }
            }

            //Update the classification Dictionary.
            if (!classification[center_vec_index].Contains(vector_num))
            {
                Remove_item(center_vec_index, vector_num);
                classification[center_vec_index].Add(vector_num);
            }
        }

    }


    private void Update_central_vectors(int key_number)
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
    private void Update_step()
    {
        for (int i = 0; i < num_of_classes; i++)
        {
            Update_central_vectors(i);

        }
    }

    public void Train(int numOfIteration)
    {
        for (int i = 0; i < numOfIteration; i++)
        {
            Assignment_step();
            Update_step();
        }
        Write_To_Csv_File();
        DictionaryToJson();

    }

    private void Write_To_Csv_File()
    {
        string path = Globals.CentralVectorsKmeans_dataset;
        try
        {
            //Pass the file-path and filename to the StreamWriter Constructor
            using (StreamWriter writetext = new StreamWriter(path))
            {
                string header = "Id,Hand in Therapy (0 for left 1 for right),Height(cm),Arm Length,Standing (0 for no 1 for yes),Treatment Time (sec),Bubble in space,Velocity average (Best=1 Worst=0),Max velocity count (Best=1 Worst=0),Reaching time (Best=0 Worst=1),Path taken (Best=1 Worst=0),Jerkiness (Best=0 Worst=1),Bubble popped (Best=1 Worst=0),Total Score (Best=100 Worst=0),Bubble Position X,Bubble Position Y,Bubble Position Z,Bubble size,Distance between bubbles";
                writetext.WriteLine(header);

                //Write a line of text
                for (int i = 0; i < central_vectors.GetLength(0); i++)
                {
                    string data_to_write = ",";
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
        /*return Enumerable.Range(0, matrix.GetLength(1))
                .Select(x => matrix[rowNumber, x])
                .ToArray();*/
        return Globals.GetRow(matrix, rowNumber);
    }

    private void DictionaryToJson()
    {
        var entries = classification.Select(d =>
            string.Format("{0} " +
            ": [{1}]", d.Key, string.Join(",", d.Value)));
        string s = "{\n " + string.Join(",\n", entries) + ",\n}";

        //Pass the file-path and filename to the StreamWriter Constructor
        using (StreamWriter writetext = new StreamWriter(Globals.KmeansClusters))
        {
            writetext.WriteLine(s);
        }
    }

}

