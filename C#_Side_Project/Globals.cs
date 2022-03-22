using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Globals
{
    public const string file_name_dataset = "Dataset.csv";
    public const string CentralVectorsKmeans_dataset = "CentralVectorsKmeans.csv";
    public const string KmeansClusters = "KmeansClusters.txt";
    public const string KnnOutput = "KnnOutput.csv";
    /// <summary>
    /// The number of clusters.
    /// </summary>
    public const int num_of_classes = 3;
    /// <summary>
    /// K is the number of users in a cluster 
    /// that are the most similar to particular user.
    /// </summary>
    public const int K = 15;
    /// <summary>
    /// This is the number of different columns in the Dataset.csv file.
    /// </summary>
    public const int numOfColumnsInDataSet = 16;

    ////// The area that the bubbles can show up//////////
    public const string FBCInSpace = "Front-Bottom-Center";
    public const string FBRInSpace = "Front-Bottom-Right";
    public const string FBLInSpace = "Front-Bottom-Left";
    public const string FTCInSpace = "Front-Top-Center";
    public const string FTRInSpace = "Front-Top-Right";
    public const string FTLInSpace = "Front-Top-Left";
    public const string BBCInSpace = "Back-Bottom-Center";
    public const string BBRInSpace = "Back-Bottom-Right";
    public const string BBLInSpace = "Back-Bottom-Left";
    public const string BTCInSpace = "Back-Top-Center";
    public const string BTRInSpace = "Back-Top-Right";
    public const string BTLInSpace = "Back-Top-Left";

    private static Dictionary<string, float> BubbleInSpace = new Dictionary<string, float>();

    public static Dictionary<string, float> Initialize_BubbleInSpace_dictionary()
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

        return BubbleInSpace;
    }

}
