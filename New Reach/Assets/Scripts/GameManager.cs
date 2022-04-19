using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;
using System;
using System.Collections;
using UnityEngine.UI;
using OculusSampleFramework;
public class GameManager : MonoBehaviour
{

    public static GameManager instance;

    public Player current_player;
    public int xPosition, yPosition, zPosition;
    public double discountFactor;
    public double randomExplore;
    public int directionIndex;
    public int iterationsCounter;
    public string[] directions = new string[6] { "UP", "RIGHT", "DOWN", "LEFT", "FORWARD", "BACKWARD" };
    public bool testFlag = false;
    public float treatmentTime;
    public bool isPopBubble = false;
    public int gameScore = 0;
    public bool isFirstBubble = false;
    public bool isRandomExplore = false;
    public int startingRoundCounter = 15;
    public int bubbleInSessionCount = 0;
    public float sumOfVelocities = 0;
    public float prevSessionVelocityAvg = 1;

    public float prevSessionJerkAvg = 1;

    public float sumOfJerks = 0;
    public bool gameStarted = false;
    public GameObject currentBubble;
    public Bubble lastBubble;


    int movementMultiplyier = 1;
    public int leftBoundry, rightBoundry, upBoundry, downBoundry, forwardBoundry, backBoundry;

    #region Gal_Nadav_Code_Block

    /// <summary>
    /// This class field will hold the number of the class that the next bubble will show in.
    /// </summary>    
    public int classNumber = 0;

    #endregion

    #region Properties

    public const int numOfLastAppearance = 1;
    bool loadingStarted = false;
    float secondsLeft = 0;

    #endregion

    #region Cached References

    public GameObject myPrefab;
    public Text timeText;
    public Text scoreText;

    #endregion


    #region Unity Messages

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            gameStarted = false;
            DontDestroyOnLoad(gameObject);
        }
    }


    void Update()
    {
        if (!gameStarted) return;
        if (testFlag)
        {
            testFlag = false;
            iterationsCounter++;

            if (isPopBubble)
            {
                gameScore += 10;
                scoreText.text = "Score:  " + gameScore.ToString();
            }
            StartCoroutine(SpawnBubble());

        }
        if (loadingStarted && timeText != null)
            timeText.text = "Time:  " + secondsLeft.ToString();

    }

    #endregion

    #region Coroutines
    public IEnumerator SpawnBubble()
    {
        // TODO: Do not change this method implementation. 

        yield return new WaitForSeconds(0.3f);
        currentBubble.transform.position = new Vector3((float)xPosition / 10,
            (float)yPosition / 10, (float)zPosition / 10);
        currentBubble.gameObject.SetActive(true);
    }


    public IEnumerator DelayLoadLevel(float seconds)
    {
        secondsLeft = seconds;
        loadingStarted = true;
        bool bubbleSpawned = false;
        do
        {
            yield return new WaitForSeconds(1);
            if (!bubbleSpawned && FingerTipPokeTool.startMeasure())
            {
                if (zPosition == 0.0) { zPosition = 1; }
                currentBubble = Instantiate(myPrefab, new Vector3(xPosition / 10.0f,
                    yPosition / 10.0f, zPosition / 10.0f), Quaternion.identity);
                bubbleSpawned = true;
            }
        } while (--secondsLeft > 0);

        SceneManager.LoadScene("EndScene");
        gameStarted = false;
    }

    #endregion



    public void SaveData()
    {
        DataManager.instance.data.hand_in_therapy = current_player.hand_in_therapy;
        DataManager.instance.data.id = current_player.id;
        DataManager.instance.data.N = current_player.N;
        DataManager.instance.data.first_name = current_player.first_name;
        DataManager.instance.data.last_name = current_player.last_name;
        DataManager.instance.data.height = current_player.height;
        DataManager.instance.data.arm_length = current_player.arm_length;
        DataManager.instance.data.discount_factor = discountFactor;
        DataManager.instance.data.random_explore = randomExplore;
        DataManager.instance.data.bubble_time_out = current_player.bubble_time_out;
        DataManager.instance.data.treatment_time = Mathf.RoundToInt(treatmentTime);
        DataManager.instance.data.iterations_number = iterationsCounter;
        DataManager.instance.data.lastBubblePosition = new Vector3(xPosition, yPosition, zPosition);
        DataManager.instance.data.prevSessionVelocityAverage = bubbleInSessionCount > 0 ? sumOfVelocities / bubbleInSessionCount : 1;
        DataManager.instance.data.prevSessionJerkAvg = bubbleInSessionCount > 0 ? sumOfJerks / bubbleInSessionCount : 10;

    }

    #region Static Methods
    /// <summary>
    /// This method initialize the player data for the play session, 
    /// it calculates the player properties - N, play boundries, all the relevent tables 
    /// ( reward Table, qTable, Last Appearence table),
    /// learning rate, exploration factor,discount factor, iteration counter, and treatment time 
    /// </summary>
    /// <param name="activePlayer">The current active player is initialized for the game</param>
    public void initializeGameDetails(Player activePlayer)
    {
        current_player = new Player(activePlayer);
        current_player.N = (int)Math.Floor((current_player.height * current_player.arm_length) / 10);

        if (current_player.hand_in_therapy.Equals("right"))
        {
            // rightBoundry = 2 * current_player.N - 1;
            rightBoundry = (int)((2 * current_player.N - 1) * (7.0f / 8.0f));
            leftBoundry = (int)(2 * current_player.N / 4.0f) - 1;
        }
        else
        {
            rightBoundry = (int)(3 * current_player.N / 2f) - 1;
            leftBoundry = (int)((2 * current_player.N - 1) * (3.0f / 16.0f));
        }

        upBoundry = 3 * current_player.N / 2 - 1;
        downBoundry = 2 * current_player.N / 8 - 1;

        forwardBoundry = (int)((current_player.N - 1) * (7 / 8.0f));
        backBoundry = current_player.N / 2;

        movementMultiplyier = 1;

        discountFactor = current_player.discount_factor != 0 ? current_player.discount_factor : 0.95;
        randomExplore = current_player.random_explore != 0 ? current_player.random_explore : 1;
        iterationsCounter = current_player.iterations_number != 0 ? current_player.iterations_number : 1;
        treatmentTime = current_player.treatment_time != 0 ? current_player.treatment_time : 120;
    }


    /// <summary>
    /// This method is calculating the next position of the next bubble.
    /// The method change the x,y,z position of the next bubble. 
    /// The changes is based on the class field : classNumber
    /// </summary>
    void ChangeBubblePosition()
    {
        // TODO: Change this function will cause the change of the bubble position.

        // class number 0 : the bubble x,y,z position will randomly set in all the game area.
        if (this.classNumber == 0)
        {
            this.xPosition = UnityEngine.Random.Range(leftBoundry, rightBoundry);
            this.yPosition = UnityEngine.Random.Range(downBoundry, upBoundry);
            this.zPosition = UnityEngine.Random.Range(backBoundry, forwardBoundry);
        }
        // class number 1 : the bubble x,y,z position will randomly set in the left game area.
        else if (this.classNumber == 1)
        {
            int xMax = (int)Math.Floor((leftBoundry + rightBoundry) / 2.0);
            this.xPosition = UnityEngine.Random.Range(leftBoundry, xMax);
            this.yPosition = UnityEngine.Random.Range(downBoundry, upBoundry);
            this.zPosition = UnityEngine.Random.Range(backBoundry, forwardBoundry);
        }
        // class number 2 : the bubble x,y,z position will randomly set in the right game area.
        else if (this.classNumber == 2)
        {
            int xMin = (int)Math.Floor((leftBoundry + rightBoundry) / 2.0);
            this.xPosition = UnityEngine.Random.Range(xMin, rightBoundry);
            this.yPosition = UnityEngine.Random.Range(downBoundry, upBoundry);
            this.zPosition = UnityEngine.Random.Range(backBoundry, forwardBoundry);
        }
    }


    // TODO: Change this function will cause the change of the bubble position.
    public void CalcNextBubbleLocation(bool isPop)
    {
        Bubble currBubble = lastBubble;
        if (currBubble == null)
        {
            Debug.Log("bubble not located");
            return;
        }
        int bubbleScore = currBubble.totalScore;

        isPopBubble = isPop;

        /*update details about the last apperance of the bubble*/
        string lastBubbleState;
        lastBubbleState = xPosition.ToString() + "^" + yPosition.ToString() + "^" + zPosition.ToString();


        KNN.Get_instance().start();

        // TODO: This method set the x,y,z position of the next bubble.
        ChangeBubblePosition();

        testFlag = true;
    }


    /// <summary>
    /// for printing purposes
    /// </summary>
    /// <returns></returns>
    public string findBubbleLocation()
    {
        string bubbleLocation = "";
        Bubble currBubble = lastBubble;
        if (currBubble == null) return "";
        if (currBubble.bubblePosition.z * 10 <= (current_player.N / 2) - 1)
        {
            bubbleLocation += "Back";
        }
        else if (currBubble.bubblePosition.z * 10 > (current_player.N / 2) - 1)
        {
            bubbleLocation += "Front";
        }

        if (currBubble.bubblePosition.y * 10 <= current_player.N - 1)
        {
            bubbleLocation += "-Bottom";
        }
        else if (currBubble.bubblePosition.y * 10 > current_player.N - 1)
        {
            bubbleLocation += "-Top";
        }

        if (currBubble.bubblePosition.x * 10 >= 6 && currBubble.bubblePosition.x * 10 <= 9)
        {
            bubbleLocation += "-Center";
        }
        else if (currBubble.bubblePosition.x * 10 <= current_player.N - 1)
        {
            bubbleLocation += "-Left";
        }
        else if (currBubble.bubblePosition.x * 10 > current_player.N - 1)
        {
            bubbleLocation += "-Right";
        }

        return bubbleLocation;
    }
    #endregion
}