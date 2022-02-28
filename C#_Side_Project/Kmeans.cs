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
    public static Kmeans instance = null;
    private int numCentralVec; // m1,m2.... mk
    private float[,] data_mat;
    private Dictionary<string, float> BubbleInSpace = new Dictionary<string, float>();

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
        
        //We must initiali the central vector !
    }

    /// <summary>
    /// Kmeans is a Singleton class, 
    /// this method return instance of Kmeans class.
    /// </summary>
    /// <returns></returns>
    public static Kmeans get_instance()
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
    public void read_file()
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
    /// <param name="v1">The first vector.</param>
    /// <param name="v2">The second vector.</param>
    /// <param name="vector_size"></param>
    /// <returns>The euclidean distance of the two vectors</returns>
    private float euclidean_distance(float[] v1, float[] v2, int vector_size) {
        float count = 0;
        const double power = 2;

        for (int i = 0; i < vector_size; i++)
        {
            float temp = (float)Math.Pow((v1[i] - v2[i]), power);
            count += temp;
        }

        return (float)Math.Sqrt(count);
    }


    public void assignment_step() { }


    public void update_step() { }

}

