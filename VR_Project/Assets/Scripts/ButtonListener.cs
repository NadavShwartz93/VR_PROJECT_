using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OculusSampleFramework;
using UnityEngine.Events;


using UnityEngine.SceneManagement;
using UnityEngine.Networking;

public class ButtonListener : MonoBehaviour
{
    public UnityEvent proximityEvent;
    public UnityEvent contactEvent;
    public UnityEvent actionEvent;
    public UnityEvent defaultEvent;
    public static Player activePlayer;
    public static string[] patientDetails;
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<ButtonController>().InteractableStateChanged.AddListener(InitiateEvent);
    }

    void InitiateEvent(InteractableStateArgs state)
    {
        if (state.NewInteractableState == InteractableState.ProximityState)
            proximityEvent.Invoke();
        else if (state.NewInteractableState == InteractableState.ContactState)
            contactEvent.Invoke();
        else if (state.NewInteractableState == InteractableState.ActionState)
            actionEvent.Invoke();
        else
            defaultEvent.Invoke();
    }
    // Update is called once per frame
    void Update()
    {

    }

    public void startGame()
    {
        Debug.Log("*******start game*******");
        // StartCoroutine(GetRequest());
        GetRequest();
    }

    public void loadStartScreen()
    {
        SceneManager.LoadScene("StartScene");
    }

    public void quit()
    {
        Application.Quit();
    }



    //Calls transferDetails funcion to get the patient details and create player object with those details, and finally initiate the game details with it
    void GetRequest()
    {
        patientDetails = new string[8];
        CSVReader.transferDetails();
        bool isFirstBubble = false;
        Debug.Log("The patient details are: " + patientDetails[0] + " "
        + patientDetails[1] + " " + patientDetails[2] + " "
        + patientDetails[3] + " " + patientDetails[4] + " "
        + patientDetails[5] + " " + patientDetails[6]
        + patientDetails[7] + " ");

        Debug.Log("*******GetRequest*******");
        //Need to load the player here, if it does not exist then create a new player
        DataManager.instance.Load(patientDetails[1] + ".txt");

        if (!DataManager.instance.fileIsLoaded)
        {
            activePlayer = new Player(patientDetails[0], patientDetails[1], patientDetails[2], patientDetails[3], float.Parse(patientDetails[4]), float.Parse(patientDetails[5]), 0.8, 0.95, 1, 10, int.Parse(patientDetails[7]), "", "", "", 6, 1, Vector3.zero, 1,1);
            isFirstBubble = true;
        }
        else
        {
            isFirstBubble = false;
            activePlayer = new Player(patientDetails[0], DataManager.instance.data.id, DataManager.instance.data.first_name, DataManager.instance.data.last_name, DataManager.instance.data.height,
                DataManager.instance.data.arm_length, DataManager.instance.data.learning_rate, DataManager.instance.data.discount_factor, DataManager.instance.data.random_explore,
                DataManager.instance.data.bubble_time_out, int.Parse(patientDetails[7]), DataManager.instance.data.reward_table,
                DataManager.instance.data.last_appearance, DataManager.instance.data.qtable, DataManager.instance.data.N, DataManager.instance.data.iterations_number, DataManager.instance.data.lastBubblePosition, DataManager.instance.data.prevSessionVelocityAverage,DataManager.instance.data.prevSessionJerkAvg);
        }
        DataManager.instance.file = patientDetails[1] + ".txt";
        Debug.Log("activePlayer.N = " + activePlayer.N);


        GameManager.instance.initializeGameDetails(activePlayer);
        GameManager.instance.isFirstBubble = isFirstBubble;

        SceneManager.LoadScene("GameScene");


    }
}