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
    public int[,,] rewardsTable;
    public Queue<int>[,,] lastAppearance;
    public int xPosition, yPosition, zPosition;
    public Dictionary<string, Dictionary<string, double>> qTable;
    public double learningRate;
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
    public int learnThreshold = 150, smartChoiseThreshold = 400, startingRoundCounter = 15;
    public int bubbleInSessionCount = 0;
    public float sumOfVelocities = 0;
    public float prevSessionVelocityAvg = 1;

    public float prevSessionJerkAvg=1;

    public float sumOfJerks=0;
    public bool gameStarted = false;
    public GameObject currentBubble;
    public Bubble lastBubble;
 

    int movementMultiplyier = 1;
    public int leftBoundry, rightBoundry, upBoundry, downBoundry, forwardBoundry, backBoundry;



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
        yield return new WaitForSeconds(0.3f);
        currentBubble.transform.position = new Vector3((float)xPosition / 10, (float)yPosition / 10, (float)zPosition / 10);
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
                currentBubble = Instantiate(myPrefab, new Vector3(xPosition / 10.0f, yPosition / 10.0f, zPosition / 10.0f), Quaternion.identity);
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
        DataManager.instance.data.learning_rate = learningRate;
        DataManager.instance.data.discount_factor = discountFactor;
        DataManager.instance.data.random_explore = randomExplore;
        DataManager.instance.data.bubble_time_out = current_player.bubble_time_out;
        DataManager.instance.data.treatment_time = Mathf.RoundToInt(treatmentTime);
        DataManager.instance.data.reward_table = Utils.RewardsTableToJson(rewardsTable);
        DataManager.instance.data.last_appearance = Utils.LastAppearanceTableToJson(lastAppearance);
        DataManager.instance.data.qtable = Utils.QtableToJson(qTable);
        DataManager.instance.data.iterations_number = iterationsCounter;
        DataManager.instance.data.lastBubblePosition = new Vector3(xPosition, yPosition, zPosition);
        DataManager.instance.data.prevSessionVelocityAverage = bubbleInSessionCount > 0 ? sumOfVelocities / bubbleInSessionCount : 1;
        DataManager.instance.data.prevSessionJerkAvg = bubbleInSessionCount > 0 ? sumOfJerks / bubbleInSessionCount : 10;

    }

    #region Static Methods
    /// <summary>
    /// This method initialize the player data for the play session, it calculates the player properties - N, play boundries, all the relevent tables ( reward Table, qTable, Last Appearence table),
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
            rightBoundry=(int)((2*current_player.N-1)*(7.0f/8.0f));
            leftBoundry = (int)(2 * current_player.N / 4.0f) - 1;
        }
        else
        {
            rightBoundry = (int)(3 * current_player.N / 2f) - 1;
            leftBoundry = (int)((2*current_player.N-1)*(3.0f/16.0f));
        }

        upBoundry = 3 * current_player.N / 2 - 1;
        downBoundry = 2 * current_player.N / 8 - 1;

        forwardBoundry = (int)((current_player.N - 1) *(7/8.0f));
        backBoundry = current_player.N / 2;

        movementMultiplyier = 1;



        if (activePlayer.reward_table == "")
        {
            rewardsTable = new int[2 * current_player.N, 2 * current_player.N, current_player.N];
            for (int i = leftBoundry; i <= rightBoundry; i++)
            {
                for (int j = downBoundry; j <= upBoundry; j++)
                {
                    for (int k = backBoundry; k <= forwardBoundry; k++)
                    {

                        rewardsTable[i, j, k] = 0;

                    }
                }
            }
        }
        else
        {
            rewardsTable = Utils.JsonToRewardsTable(activePlayer.reward_table);
        }


        if (activePlayer.last_appearance == "")
        {
            lastAppearance = new Queue<int>[2 * current_player.N, 2 * current_player.N, current_player.N];
            InitializeLastAppearance(lastAppearance);
        }
        else
        {
            lastAppearance = Utils.JsonToLastAppearanceTable(current_player.last_appearance);
            //Debug.Log("last apperence: " + current_player.last_appearance);
        }

        if (activePlayer.qtable == "")
        {
            qTable = new Dictionary<string, Dictionary<string, double>>();
            InitializeQTable(qTable);
        }
        else
            qTable = Utils.JsonToQtable(current_player.qtable);


        learningRate = current_player.learning_rate != 0 ? current_player.learning_rate : 0.8;
        discountFactor = current_player.discount_factor != 0 ? current_player.discount_factor : 0.95;
        randomExplore = current_player.random_explore != 0 ? current_player.random_explore : 1;
        iterationsCounter = current_player.iterations_number != 0 ? current_player.iterations_number : 1;
        treatmentTime = current_player.treatment_time != 0 ? current_player.treatment_time : 120;
    }


    /// <summary>
    /// initializing the last appearance table
    /// </summary>
    /// <param name="lastApperence">queue for each bubble location</param>
    void InitializeLastAppearance(Queue<int>[,,] lastApperence)
    {
        for (int i = leftBoundry; i <= rightBoundry; i++)
        {
            for (int j = downBoundry; j <= upBoundry; j++)
            {
                for (int k = backBoundry; k <= forwardBoundry; k++)
                {
                    lastApperence[i, j, k] = new Queue<int>();
                    for (int g = 0; g < numOfLastAppearance; g++)
                    {
                        //Enqueing the relevent play area
                        lastApperence[i, j, k].Enqueue(50); //Changed

                    }
                }
            }
        }
        this.lastAppearance = lastApperence;
    }

    /// <summary>
    /// initilizing the qTable
    /// </summary>
    /// <param name="qTable">the qtable is a key-value pair which the key is a location, and the value is a sub-dictionary which inclued a key(direction) and a value(which is calculated by the qFunction)</param>

    void InitializeQTable(Dictionary<string, Dictionary<string, double>> qTable)
    {

        for (int i = leftBoundry; i <= rightBoundry; i++)
        {
            for (int j = downBoundry; j <= upBoundry; j++)
            {
                for (int k = backBoundry; k <= forwardBoundry; k++)
                {
                    Dictionary<string, double> subDict = new Dictionary<string, double>();
                    foreach (string direction in directions)
                    {
                        subDict.Add(direction, 0);
                    }
                    qTable.Add(i.ToString() + "^" + j.ToString() + "^" + k.ToString(), subDict);
                }
            }
        }
        this.qTable = qTable;
    }


    /// <summary>
    /// This method is calculating the next position of the next bubble.
    /// </summary>
    /// <param name="direction">The direction in which the bubble will be moved in the next iteration = {up, down, left, right, back ,front}</param>
    void ChangeBubblePosition(string direction)
    {


        if (direction.Equals("UP"))
        {
            yPosition += movementMultiplyier;
            if (yPosition > upBoundry)
            {
                yPosition = downBoundry;
            }

        }

        if (direction.Equals("DOWN"))
        {
            yPosition -= movementMultiplyier;
            if (yPosition < downBoundry)
                yPosition = upBoundry;

        }

        if (direction.Equals("RIGHT"))
        {

            xPosition += movementMultiplyier;
            if (xPosition > rightBoundry)
            {
                xPosition = leftBoundry;

            }


        }

        if (direction.Equals("LEFT"))
        {
            xPosition -= movementMultiplyier;
            if (xPosition < leftBoundry)
            {
                xPosition = rightBoundry;
            }
        }

        if (direction.Equals("FORWARD"))
        {
            zPosition += movementMultiplyier;
            if (zPosition > forwardBoundry)
            {
                zPosition = backBoundry;

            }
        }

        if (direction.Equals("BACKWARD"))
        {
            zPosition -= movementMultiplyier;
            if (zPosition < backBoundry)
            {
                zPosition = forwardBoundry;
            }
        }
    }


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
        
        if (iterationsCounter > learnThreshold && iterationsCounter<=smartChoiseThreshold)
        {
            learningRate = 0.6f;
            randomExplore = 0.4f;
        }else if(iterationsCounter>smartChoiseThreshold){
            learningRate = 0.2f;
            randomExplore=0.1f;
        }

        /*update details about the last apperance of the bubble*/
        string lastBubbleState;
        lastBubbleState = xPosition.ToString() + "^" + yPosition.ToString() + "^" + zPosition.ToString();


        // change last appearance queue
        if (lastAppearance[xPosition, yPosition, zPosition] != null && lastAppearance[xPosition, yPosition, zPosition].Count() == numOfLastAppearance)
            lastAppearance[xPosition, yPosition, zPosition].Dequeue();

        lastAppearance[xPosition, yPosition, zPosition].Enqueue(bubbleScore); // Changed

        //calc the new reward value of the last place of the bubble
        CalcNewReward();

        /* choose what action to take and update the qtable values*/

        //choose action to take
        string maxRewardAction;
        float randomNumber = UnityEngine.Random.Range(0.0f, 1.0f);
        if (randomNumber <= randomExplore)
        {
            isRandomExplore = true;
            if (startingRoundCounter > 0)
            {
                maxRewardAction = directions[directionIndex];
                directionIndex++;
                movementMultiplyier += 1 % current_player.N;
                directionIndex %= 4;
                startingRoundCounter--;
            }
            else
            {
                directionIndex = UnityEngine.Random.Range(0, 6);
                maxRewardAction = directions[directionIndex];
                movementMultiplyier = UnityEngine.Random.Range(1, current_player.N);
            }
        }
        else
        {
            isRandomExplore = false;
            movementMultiplyier = 1;
            Dictionary<string, double> qTableColumn = qTable[lastBubbleState];
            maxRewardAction = qTableColumn.Aggregate((l, r) => l.Value > r.Value ? l : r).Key;
        }

        //take the next move
        ChangeBubblePosition(maxRewardAction);
        string currentState = xPosition.ToString() + "^" + yPosition.ToString() + "^" + zPosition.ToString();
        int NewReward = rewardsTable[xPosition, yPosition, zPosition];
        QFunction(learningRate, discountFactor, NewReward, currentState, lastBubbleState, maxRewardAction);
        testFlag = true;
    }





    /// <summary>
    /// in the randomized step, we would like to learn all of the the area in which the bubble had popped, therefore the bubble's location itself will increase by two time of the power,
    /// and the surrounding area by only incremented by the power
    /// </summary>
    /// <param name="power"></param>
    private void calculateRewardArea(int power)
    {
        int addingX, addingY, addingZ;
        int decX, decY, decZ;

        if (isRandomExplore)
        {
            addingX = xPosition + 1 > rightBoundry ? rightBoundry : xPosition + 1;
            addingY = yPosition + 1 > upBoundry ? upBoundry : yPosition + 1;
            addingZ = zPosition + 1 > forwardBoundry ? forwardBoundry : zPosition + 1;
            decX = xPosition - 1 < leftBoundry ? leftBoundry : xPosition - 1;
            decY = yPosition - 1 < downBoundry ? downBoundry : yPosition - 1;
            decZ = zPosition - 1 < backBoundry ? backBoundry : zPosition - 1;
            rewardsTable[xPosition, yPosition, zPosition] += (2 * power);
            rewardsTable[xPosition, yPosition, addingZ] += power;
            rewardsTable[xPosition, yPosition, decZ] += power;
            rewardsTable[xPosition, addingY, zPosition] += power;
            rewardsTable[xPosition, addingY, addingZ] += power;
            rewardsTable[xPosition, addingY, decZ] += power;
            rewardsTable[xPosition, decY, zPosition] += power;
            rewardsTable[xPosition, decY, addingZ] += power;
            rewardsTable[xPosition, decY, decZ] += power;
            rewardsTable[addingX, yPosition, zPosition] += power;
            rewardsTable[addingX, yPosition, addingZ] += power;
            rewardsTable[addingX, yPosition, decZ] += power;
            rewardsTable[addingX, addingY, zPosition] += power;
            rewardsTable[addingX, addingY, addingZ] += power;
            rewardsTable[addingX, addingY, decZ] += power;
            rewardsTable[addingX, decY, zPosition] += power;
            rewardsTable[addingX, decY, addingZ] += power;
            rewardsTable[addingX, decY, decZ] += power;
            rewardsTable[decX, yPosition, zPosition] += power;
            rewardsTable[decX, yPosition, addingZ] += power;
            rewardsTable[decX, yPosition, decZ] += power;
            rewardsTable[decX, addingY, zPosition] += power;
            rewardsTable[decX, addingY, addingZ] += power;
            rewardsTable[decX, addingY, decZ] += power;
            rewardsTable[decX, decY, zPosition] += power;
            rewardsTable[decX, decY, addingZ] += power;
            rewardsTable[decX, decY, decZ] += power;
        }
        else
        {
            rewardsTable[xPosition, yPosition, zPosition] += power;
        }
    }
    /// <summary>
    /// Calculating next reward according to the patient overall score
    /// Average range explantion (ragard to bubble score calculation in the bubble class): 
    /// If bubble popped:
    ///     BEST CASE: 100
    ///     WORST CASE: 15
    /// else:
    ///     BEST CASE:55
    ///     WORST CASE : 0
    ///     
    /// Average score Bubble popped:
    /// (100+15)/2= 57.5
    /// 
    /// Average score bubble not popped:
    /// 55/2 = 27.5
    /// 
    /// 
    /// EASY ->72<average<=100
    /// HARD -> 45<average<=72
    /// VERY HARD -> 0<=average<=45
    /// 
    /// </summary>
    void CalcNewReward()
    {
        int totalScore = 0;
        //int[] array = lastAppearance[xPosition, yPosition, zPosition].ToArray(); 
        float average;
        //foreach (int isPop in lastAppearance[xPosition, yPosition, zPosition]) { counter += isPop; }
        foreach (int score in lastAppearance[xPosition, yPosition, zPosition]) { totalScore += score; } //Changed
        average = lastAppearance[xPosition, yPosition, zPosition].Count() > 0 ? totalScore / lastAppearance[xPosition, yPosition, zPosition].Count() : 0;
        //lastBubbleLocationScore = array[array.Length-1];
        // Debug.Log("AVERAGE " + average);
        if (average > 72)
            //EASY
            calculateRewardArea(-2);
        else if (average > 45 && average <= 72)
            //HARD
            calculateRewardArea(4);
        else if (average <= 45)
            //VERY HARD
            calculateRewardArea(3);
    }

    void QFunction(double learningRate, double discountFactor, int NewReward, string currentState, string lastState, string chosenAction)
    {
        double OpimtalFutureValue = qTable[currentState].Aggregate((l, r) => l.Value > r.Value ? l : r).Value;
        double DiscountedOpimtalFutureValue = OpimtalFutureValue * discountFactor;
        double LearnedValue = NewReward + DiscountedOpimtalFutureValue;
        qTable[lastState][chosenAction] = Math.Round((1 - learningRate) * qTable[lastState][chosenAction] + learningRate * LearnedValue, 3);
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




