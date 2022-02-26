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
    private int numCentralVec = 0; // m1,m2.... mk
    private float[,] data_mat;
    private string file_to_read = "Dataset.csv";
    const int numberOfColumns = 11;
    /*    enum BubbleInSpace
        {
            Front_Bottom_Center,    // 0
            Front-Bottom-Right,
            Front-Top-Center,
            Front-Top-Right,
            Front-Top-Left,

        }
    */
    Dictionary<string, float> BubbleInSpace = new Dictionary<string, float>();
    
    //private const string Front-Bottom-Center = 0;
    Kmeans(int number_of_central_vectors)
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
    public static Kmeans get_instance()
    {
        if (instance == null)
        {
            instance = new Kmeans(11);
        }
        return instance;
    }
    public void read_file()
    {
        
        string[] text = File.ReadAllLines(file_to_read);
        int skipLineInDataset = 0,i = 0;
        List<string[]> list = new List<string[]>();
        foreach (var line in text)
        {
            //Don't need the first line from the text.
            if (skipLineInDataset < 1)
            {

                skipLineInDataset++;
            }
            else
            {
                string[] tokens = line.Split(',');
                list.Add(tokens);
                /*   new_data_line(Convert.ToSingle(tokens[0]), Convert.ToSingle(tokens[1]), Convert.ToSingle(tokens[2]),
                       Convert.ToSingle(tokens[3]), Convert.ToSingle(tokens[4]), Convert.ToInt32(tokens[5]),
                       Convert.ToInt32(tokens[6]), Convert.ToSingle(tokens[7]), Convert.ToSingle(tokens[8]),
                       Convert.ToSingle(tokens[9]), tokens[10]);*/
                skipLineInDataset++;
            }
        }
        data_mat = new float[list.Count, numberOfColumns];
        foreach (string[] elem in list)
        {
            for (int j = 0; j < numberOfColumns; j++)
            {
                if (j == 6 && BubbleInSpace.ContainsKey(elem[j])) //For columns in table ->where 6 == Bubble in space
                {
                    data_mat[i, j] = BubbleInSpace[elem[j]];
                }
                else 
                    data_mat[i, j] = float.Parse(elem[j]);

            }
        }
    }
    public void euclidean_distance(float[] v1, float[] v2, int vector_size) { }

    public void assignment_step() { }
    public void update_step() { }



}

