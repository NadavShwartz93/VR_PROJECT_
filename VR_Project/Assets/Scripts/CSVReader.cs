using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

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
    void ReadCSV()
    {
        Debug.Log("Starting to read patient details: ");
        string path = Path.Combine(Application.persistentDataPath, "PatientDetails.csv");
        Patient patient = new Patient();
        using(var reader = new StreamReader(path))
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
            Debug.Log("The patient details are: " + details[0] + " "
            + details[1] + " " + details[2] + " "
            + details[3] + " " + details[4] + " "
            + details[5] + " " + details[6]);
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
        GameManager.instance.lastBubble=bubble;
        using (StreamWriter writer = new StreamWriter(path,true))
        {
            if(title)
            {
                writer.WriteLine("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10}","","","Patient:",ButtonListener.patientDetails[2],ButtonListener.patientDetails[3],"","","","","","");
                writer.WriteLine("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10}","Velocity average (Best=1 Worst=0)", "Max velocity count (Best=1 Worst=0)", "Reaching time (Best=0 Worst=1)", "Path taken (Best=1 Worst=0)", "Jerkiness (Best=0 Worst=1)", "Bubble popped (Best=1 Worst=0)", "Total Score (Best=100 Worst=0)", "Bubble Position X", "Bubble Position Y", "Bubble Position Z", "Bubble in space");
                title = false;
            }
            bubbleLocation = GameManager.instance.findBubbleLocation();
            
            writer.WriteLine("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10}",bubble.VavgNormal.ToString(), bubble.maxVcntNormal.ToString(), bubble.reachTimeNormal.ToString(), bubble.pathTakenNormal.ToString(), bubble.jerkNormal.ToString(), bubble.bubblePop.ToString(), bubble.totalScore.ToString(), bubble.bubblePosition.x.ToString(), bubble.bubblePosition.y.ToString(), bubble.bubblePosition.z.ToString(), bubbleLocation);
        }
        
    }
}
