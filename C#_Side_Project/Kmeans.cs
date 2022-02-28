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
    private Dictionary<string, float> BubbleInSpace = new Dictionary<string, float>();
    private float[,] central_vectors;
    private const int num_of_central_vectors = 4;
    private Dictionary<int, HashSet<int>> classification = new Dictionary<int, HashSet<int>>();

    //Constant variables that we are using in this class.
    private const string file_to_read = "Dataset.csv";
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
    public void Read_file()
    {
       
        string[] text = File.ReadAllLines(file_to_read);
        int skipFirstLineInDataset = 0,i = 0;
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

        data_mat = new float[list.Count, numberOfColumns-1];
        foreach (string[] elem in list)
        {
            for (int j = 0; j < numberOfColumns; j++)
            {

                if (j == id_column_number)
                    continue;

                //For columns in table ->where 6 == Bubble in space
                if (j == bubble_in_space_column_number && BubbleInSpace.ContainsKey(elem[j])) 
                {
                    data_mat[i, j-1] = BubbleInSpace[elem[j]];
                }

                else 
                    data_mat[i, j-1] = float.Parse(elem[j]);

            }
            i++;
        }
    }


    /// <summary>
    /// This method calculate the euclidean distance of two given vectors.
    /// </summary>
    /// <param name="row_num1"></param>
    /// <param name="row_num2"></param>
    /// <param name="vector_size"></param>
    /// <returns>The euclidean distance of the two vectors.</returns>
    private float Euclidean_distance(int row_num1, int row_num2, int vector_size)
    {
        float count = 0;
        const double power = 2;

        for (int i = 0; i < vector_size; i++)
        {
            float temp = (float)Math.Pow(data_mat[row_num1, i] - central_vectors[row_num1, i], power);
            count += temp;
        }

        return (float)Math.Sqrt(count);
    }


    /// <summary>
    /// This method initialize the BubbleInSpace dictionary.
    /// </summary>
    private void Initialize_BubbleInSpace_dictionary()
    {
        BubbleInSpace.Add("Front-Bottom-Center", 0);
        BubbleInSpace.Add("Front-Bottom-Right", 1);
        BubbleInSpace.Add("Front-Bottom-Left", 2);
        BubbleInSpace.Add("Front-Top-Center", 3);
        BubbleInSpace.Add("Front-Top-Right", 4);
        BubbleInSpace.Add("Front-Top-Left", 5);
        BubbleInSpace.Add("Back-Bottom-Center", 6);
        BubbleInSpace.Add("Back-Bottom-Right", 7);
        BubbleInSpace.Add("Back-Bottom-Left", 8);
        BubbleInSpace.Add("Back-Top-Center", 9);
        BubbleInSpace.Add("Back-Top-Right", 10);
        BubbleInSpace.Add("Back-Top-Left", 11);
    }


    /// <summary>
    /// This method initialize the Classification dictionary.
    /// </summary>
    private void Initialize_classification_dictionary()
    {
        for (int i = 0; i < num_of_central_vectors; i++)
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
        const int row_size = num_of_central_vectors;
        const int col_size = numberOfColumns - 1;

        central_vectors = new float[row_size, col_size];
        Random rnd = new Random();

        for (int i = 0; i < row_size; i++)
        {
            for (int j = 0; j < col_size; j++)
            {
                float num;
                switch (j)
                {
                    case 0:
                    case 11:
                        {
                            num = rnd.Next(0, 1);
                            break;
                        }
                    case bubble_in_space_column_number-1:
                        {
                            num = rnd.Next(0, 11);
                            break;
                        }
                    default:
                        num = 0;
                        break;
                }
                central_vectors[i, j] = num;
            }
        }
    }

    /// <summary>
    /// This method check if any of the classification Dictionary value contain the vector_num.
    /// If true, remove it.
    /// </summary>
    /// <param name="Key_to_stay"></param>
    /// <param name="vector_num"></param>
    private void Remove_item(int Key_to_stay, int vector_num)
    {
        for (int i = 0; i < num_of_central_vectors; i++)
        {
            if (i != Key_to_stay && classification[i].Contains(vector_num))
                classification[i].Remove(vector_num);
        }
    }

    /// <summary>
    /// This method represent the assignment step in the Kmeans algorithm.
    /// </summary>
    public void Assignment_step() {
        int row_size =  data_mat.GetLength(0);
        int col_size = data_mat.GetLength(1);
        int center_vec_index;

        for (int vector_num = 0; vector_num < row_size; vector_num++)
        {
            float minimum_value = Euclidean_distance(vector_num, 0, col_size);
            center_vec_index = 0;

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
            central_vectors[key_number, i] = cnt / classification[key_number].Count;
        }
    }


    /// <summary>
    /// This method represent the update step in the Kmeans algorithm.
    /// </summary>
    public void Update_step() {

        for (int i = 0; i < num_of_central_vectors; i++)
        {
            Update_central_vectors(i);
        }

    }
}

