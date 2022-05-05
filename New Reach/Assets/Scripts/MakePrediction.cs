using System;
using System.IO;
using System.Linq;
using UnityEngine;

/// <summary>
/// This class was written by Nadav Shwarz and Gal Sherman.
/// Singleton class for making the prediction.
/// </summary>
class MakePrediction
{

    private static MakePrediction instance = null;

    private MakePrediction()
    {

    }

    public static MakePrediction GetInstance()
    {
        if (instance == null)
            instance = new MakePrediction();

        return instance;
    }

    /// <summary>
    /// This method read the 'AreaRecommendationOfUser.csv' file and save the data into 
    /// Global.matrixOfRecommendation data structure.
    /// </summary>
    public void ReadAreaRecommendationFile()
    {
        using (var reader = new StreamReader(Globals.AreaRecommendationOfUser))
        {
            //Don't need the first line.
            var line = reader.ReadLine();

            int row = 0;
            while (!reader.EndOfStream)
            {
                //Update the size of the variable.
                Globals.numOfActualHistoryRow++;

                var values = reader.ReadLine().Split(',');
                for (int col = 0; col < Globals.numOfAreas; col++)
                {
                    Globals.matrixOfRecommendation[row, col] = float.Parse(values[col]);
                }
                row++;
            }
        }
    }

    /// <summary>
    /// This method append the GUI recommendation that the therapist gave into the 
    /// Globals.matrixOfRecommendation data structure.
    /// </summary>
    /// <param name="guiRecommendation"> 
    /// The recommendation that the therapist gave.</param>
    public void AppendGuiRecommendationToMatrix(float[] guiRecommendation)
    {
        for (int i = 0; i < Globals.numOfAreas; i++)
        {
            Globals.matrixOfRecommendation[Globals.numOfActualHistoryRow, i] = guiRecommendation[i];
        }
        Globals.numOfActualHistoryRow++;
    }

    /// <summary>
    /// This method calculate the Avg of every columns in the 
    /// Globals.matrixOfRecommendation data structure, and save it in the
    /// last row.
    /// </summary>
    private void CalcAvgOfRecommendationMatrix()
    {
        for (int i = 0; i < Globals.numOfAreas; i++)
        {
            Globals.matrixOfRecommendation[Globals.historyRow, i] =
                Globals.GetCol(Globals.matrixOfRecommendation, i).Average();
        }
    }

    /// <summary>
    /// This method check if prediction should made.
    /// prediction is made only if existed "Globals.historyRow" row in 
    /// Globals.matrixOfRecommendation data structure.
    /// </summary>
    public void CheckPrediction()
    {
        if (Globals.numOfActualHistoryRow == Globals.historyRow)
        {
            this.CalcAvgOfRecommendationMatrix();
            Globals.isPredicted = true;
        }
    }

    /// <summary>
    /// This method write the Globals.matrixOfRecommendation data structure
    /// into 'AreaRecommendationOfUser.csv' file.
    /// </summary>
    public void WriteToAreaRecommendationOfUser()
    {
        try
        {
            using (StreamWriter writer = new StreamWriter(Globals.AreaRecommendationOfUser))
            {
                writer.WriteLine("Area Score 0, Area Score 1, Area Score 2, Area Score 3, " +
                    "Area Score 4, Area Score 5, Area Score 6, Area Score 7, \n");

                string str = "";

                for (int i = 0; i < Globals.numOfActualHistoryRow; i++)
                {
                    for (int j = 0; j < Globals.numOfAreas; j++)
                    {
                        str += Globals.matrixOfRecommendation[i, j] + ",";
                    }
                    str += "\n";
                }

                writer.WriteLine(str);
            }
        }
        catch (Exception e)
        {
            Debug.Log("Exception in MakePrediction.WriteToAreaRecommendationOfUser(): " + e.Message);
        }
    }
}
