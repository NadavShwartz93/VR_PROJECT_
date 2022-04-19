using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class EndGame : MonoBehaviour
{

    void Start()
    {
        Debug.Log("EndGame start.");
        Debug.Log(GameManager.instance.current_player.id);
        GameManager.instance.SaveData();
        DataManager.instance.Save(GameManager.instance.current_player.id + ".txt");

        //Added 2 new commands
        #region Gal_Nadav_Code_Block

        //Update the dataset.csv file
        Debug.Log("Write to the Dataset.csv");
        Dataset.Get_instance().Write_data_to_file();

        //Run the Kmeans.cs script in order to make a new classification 
        //of the dataset.
        Debug.Log("Run the Kmeans.cs in order to generate: KmeansClusters.txt and update CentralVectorsKmeans.");
        Kmeans.Get_instance().Train(Globals.numOfTrainingIteration);

        #endregion

        Debug.Log("EndGame.cs finished!!!");
    }

    // Start is called before the first frame update
    /* void Start()
     {
         Debug.Log("end start");
         Debug.Log(AlgorithmController.current_player.id);

         string json1 = "{ \"id\": \"" + AlgorithmController.current_player.id + "\"," +
                        " \"learning_rate\":" + AlgorithmController.learningRate + "," +
                        " \"hand_in_therapy\":\"" + AlgorithmController.current_player.hand_in_therapy + "\"," +
                        " \"game_name\":\"bubbles\"," +
                        " \"random_explore\":" + AlgorithmController.randomExplore + "," +
                        " \"iterations_number\":" + AlgorithmController.iterationsCounter + "," +
                        " \"discount_factor\":" + AlgorithmController.discountFactor + "}";

         string json2 = "{ \"id\": \"" + AlgorithmController.current_player.id + "\"," +         
                        " \"hand_in_therapy\":\"" + AlgorithmController.current_player.hand_in_therapy + "\"," +
                        " \"game_name\":\"bubbles\"," +                       
                        "\"rewards_table\":\"" + Utils.RewardsTableToJson(AlgorithmController.rewardsTable) + "\"}";

         string json3 = "{ \"id\": \"" + AlgorithmController.current_player.id + "\"," +                  
                        " \"hand_in_therapy\":\"" + AlgorithmController.current_player.hand_in_therapy + "\"," +
                        " \"game_name\":\"bubbles\"," +                      
                        " \"qtable\":\"" + Utils.QtableToJson(AlgorithmController.qTable) + "\"}";

         string json4 = "{ \"id\": \"" + AlgorithmController.current_player.id + "\"," +                      
                        " \"hand_in_therapy\":\"" + AlgorithmController.current_player.hand_in_therapy + "\"," +
                        " \"game_name\":\"bubbles\"," +
                        "\"last_apperance_table\": \"" + Utils.LastAppearanceTableToJson(AlgorithmController.lastAppearance) + "\"}";

         //System.IO.File.WriteAllText("C:\\Users\\bkorkos\\Desktop\\WriteLines1.txt", json);

         Debug.Log("end finish");

         StartCoroutine(PostRequest("https://vr-final-project.herokuapp.com/savePlayerResults", json1));
         Debug.Log("finish query 1!!!");

         StartCoroutine(PostRequest("https://vr-final-project.herokuapp.com/savePlayerRewardTable", json2));
         Debug.Log("finish query 2!!!");

         StartCoroutine(PostRequest("https://vr-final-project.herokuapp.com/savePlayerQTable", json3));
         Debug.Log("finish query 3!!!");

         StartCoroutine(PostRequest("https://vr-final-project.herokuapp.com/savePlayerLastAppearnce", json4));
         Debug.Log("finish query 4!!!");
         // StartCoroutine(PostRequest("http://localhost:8080/savePlayerResults", json));
         Debug.Log("finish all querys");
     }

     IEnumerator PostRequest(string url, string json)
     {
         var uwr = new UnityWebRequest(url, "POST");
         byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(json);
         uwr.uploadHandler = (UploadHandler)new UploadHandlerRaw(jsonToSend);
         uwr.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
         uwr.SetRequestHeader("Content-Type", "application/json");

         //Send the request then wait here until it returns
         yield return uwr.SendWebRequest();

         if (uwr.isNetworkError)
         {
             Debug.Log("Error While Sending: " + uwr.error);
         }
         else
         {
             Debug.Log("!!!!!!!!!!!!!!!!!!!!!!!!!Received: " + uwr.isHttpError +" "+ uwr.error);
         }
     }*/
}