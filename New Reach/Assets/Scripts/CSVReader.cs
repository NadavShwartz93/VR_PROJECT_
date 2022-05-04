using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

using System.IO;

public class CSVReader : MonoBehaviour
{

    public TextAsset textAssetData;
    public static string[] details = new string[8];
    public static bool title = true;

    [System.Serializable]
    public class Patient
    {
        public string handInTherapy;
        public string id;
        public string firstName;
        public string lastName;
        public float height;
        public float armLength;
        public int treatmentTime;
    }

    // Start is called before the first frame update
    void Start()
    {
        ReadCSV();
    }

    //Read an excel file from the Oculus quest and save the patient deatails
    private void ReadCSV()
    {
        Debug.Log("Starting to read patient details: ");
        string path = Path.Combine(Application.persistentDataPath, "PatientDetails.csv");
        Patient patient = new Patient();

        float[] guiRecommendation = new float[Globals.numOfAreas];

        using (var reader = new StreamReader(path))
        {
            Debug.Log("Got here 0 and: " + reader);
            var line = reader.ReadLine();
            line = reader.ReadLine();
            Debug.Log("Got here 2 and: " + line);
            var values = line.Split(',');
            Debug.Log("Got here 3 and: " + values);
            details[0] = values[0];
            details[1] = values[1];
            details[2] = values[2];
            details[3] = values[3];
            details[4] = values[4];
            details[5] = values[5];
            details[6] = values[6];
            details[7] = values[7];

            #region Gal_Nadav_Code_Blocks

            //Read the Area Score data from "PatientDetails.csv" that the therapist gave. 
            for (int i = 0; i < Globals.numOfAreas; i++)
            {
                guiRecommendation[i] = float.Parse(values[i + 8]);
            }
            #endregion

            Debug.Log("The patient details are: " + details[0] + " "
            + details[1] + " " + details[2] + " "
            + details[3] + " " + details[4] + " "
            + details[5] + " " + details[6]);
        }

        #region Gal_Nadav_Code_Blocks
        ReadAreaRecommendationFile();

        for (int i = 0; i < Globals.numOfAreas; i++)
        {
            Globals.matrixOfRecommendation[Globals.numOfActualHistoryRow, i] = guiRecommendation[i];
        }
        Globals.numOfActualHistoryRow++;

        if (Globals.numOfActualHistoryRow == Globals.historyRow)
        {
            Globals.caclcAvg();
            Globals.isPredicted = true;
        }


        #endregion

    }

    private void ReadAreaRecommendationFile()
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

    //Transfer the data to the ButtonListener class to initiate the game's patient details
    public static void transferDetails()
    {
        ButtonListener.patientDetails[0] = details[0];
        ButtonListener.patientDetails[1] = details[1];
        ButtonListener.patientDetails[2] = details[2];
        ButtonListener.patientDetails[3] = details[3];
        ButtonListener.patientDetails[4] = details[4];
        ButtonListener.patientDetails[5] = details[5];
        ButtonListener.patientDetails[6] = details[6];
        ButtonListener.patientDetails[7] = details[7];
    }

    //Write the treatment Results into excel file in the Oculus quest
    public static void CSVWrite()
    {
        Debug.Log("Starting to write patient details: ");
        string path = Path.Combine(Application.persistentDataPath, "PatientResults.csv");
        string bubbleLocation;
        Bubble bubble = GameManager.instance.currentBubble.GetComponent<Bubble>();
        if (bubble == null) return;
        GameManager.instance.lastBubble = bubble;
        using (StreamWriter writer = new StreamWriter(path, true))
        {
            if (title)
            {
                writer.WriteLine("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10}", "", "", "Patient:",
                    ButtonListener.patientDetails[2], ButtonListener.patientDetails[3], "", "", "", "", "", "");
                writer.WriteLine("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10}", "Velocity average (Best=1 Worst=0)",
                    "Max velocity count (Best=1 Worst=0)", "Reaching time (Best=0 Worst=1)",
                    "Path taken (Best=1 Worst=0)", "Jerkiness (Best=0 Worst=1)", "Bubble popped (Best=1 Worst=0)",
                    "Total Score (Best=100 Worst=0)", "Bubble Position X", "Bubble Position Y", "Bubble Position Z",
                    "Bubble in space");
                title = false;
            }
            bubbleLocation = GameManager.instance.findBubbleLocation();

            writer.WriteLine("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10}", bubble.VavgNormal.ToString(),
                bubble.maxVcntNormal.ToString(), bubble.reachTimeNormal.ToString(),
                bubble.pathTakenNormal.ToString(), bubble.jerkNormal.ToString(),
                bubble.bubblePop.ToString(), bubble.totalScore.ToString(),
                bubble.bubblePosition.x.ToString(), bubble.bubblePosition.y.ToString(),
                bubble.bubblePosition.z.ToString(), bubbleLocation);

            //Added new command
            #region Gal_Nadav_Code_Blocks

            //Save this data line for writing in the end of the game in the Dataset.csv
            Dataset.Get_instance().New_data_line(bubble.VavgNormal, bubble.maxVcntNormal,
                bubble.reachTimeNormal, bubble.pathTakenNormal, bubble.jerkNormal,
                bubble.bubblePop, bubble.totalScore, bubble.bubblePosition.x,
                bubble.bubblePosition.y, bubble.bubblePosition.z, bubbleLocation);

            //Initialize the simulateUseVector from Globals class.
            Globals.simulateUseVector[6] = Globals.Initialize_BubbleInSpace_dictionary()[bubbleLocation];
            Globals.simulateUseVector[7] = (double)bubble.VavgNormal;
            Globals.simulateUseVector[8] = (double)bubble.maxVcntNormal;
            Globals.simulateUseVector[9] = (double)bubble.reachTimeNormal;
            Globals.simulateUseVector[10] = (double)bubble.pathTakenNormal;
            Globals.simulateUseVector[11] = (double)bubble.jerkNormal;
            Globals.simulateUseVector[12] = (double)bubble.bubblePop;
            Globals.simulateUseVector[13] = (double)bubble.totalScore;
            Globals.simulateUseVector[14] = (double)bubble.bubblePosition.x;
            Globals.simulateUseVector[15] = (double)bubble.bubblePosition.y;
            Globals.simulateUseVector[16] = (double)bubble.bubblePosition.z;

            #endregion
        }

    }


    public static void writeToPatientDetails()
    {
        string Patient_data = "";
        Patient_data += "Hand in Therapy, Id, First Name, Last Name, Height (cm), Arm Length, ";
        Patient_data += "Standing, Treatment Time (sec), Area Score 0, Area Score 1, Area Score 2, Area Score 3, Area Score 4, " +
            "Area Score 5, Area Score 6, Area Score 7, \n";

        try
        {
            using (StreamWriter writer = new StreamWriter(Globals.PatientDetailsFilePath))
            {
                for (int i = 0; i < 8; i++)
                {
                    Patient_data += details[i];
                    Patient_data += ",";
                }


                Patient_data += Globals.concatAreaScore();

                //Write The data to the .csv file.
                writer.WriteLine(Patient_data);

                Debug.Log("Write PatientDetails.csv .");
            }
        }
        catch (Exception e)
        {
            Debug.Log("Exception: " + e.Message);
        }
    }

    public static void writeToAreaRecommendationOfUser()
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
            Debug.Log("Exception: " + e.Message);
        }
    }


}