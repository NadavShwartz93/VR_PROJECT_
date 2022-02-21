using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OculusSampleFramework;
using System;


/// <summary>
/// Bubble game object is the instance of a bubble in the world space,
/// if the time is up or the bubble was popped by a finger, then it will calculate the reach grade
/// </summary>
public class Bubble : MonoBehaviour
{
    public Bubble instance;
    private float timePass = 0.0f;
    public Vector3 bubblePosition = Vector3.zero;
    public float directPath = 0;
    public float reachTimeNormal = 0;
    public float maxVcntNormal = 0;
    public float pathTakenNormal = 0;
    public float jerkNormal = 0;
    public int numOfUpdates = 0;
    public int numOfFrames = 0;
    public float VavgNormal = 0;
    public int totalScore = 0;
    public float fTotalScore = 0;
    public int bubblePop = 0;

    [SerializeField] private float bubbleLifeTime = 10.0f;

    /// <summary>
    /// When the game starts we want to make sure there is one instance of the bubble in every game session. 
    /// </summary>
    private void Awake()
    {
        if (!instance)
            instance = this;
    }

    private void Start()
    {
       Init();
    }

    /// <summary>
    /// When the bubble is back to be enabled (seen on the scene), 
    /// this method will initillize all of the parameters.
    /// </summary>
    private void OnEnable()
    {
        Init();
    }

    private void Init()
    {
        timePass = 0.0f;
        bubblePosition = Vector3.zero;
        directPath = 0;
        reachTimeNormal = 0;
        maxVcntNormal = 0;
        pathTakenNormal = 0;
        jerkNormal = 0;
        numOfUpdates = 0;
        numOfFrames = 0;
        VavgNormal = 0;
        totalScore = 0;
        fTotalScore = 0;
        bubblePop = 0;
    }

    /// <summary>
    /// When bubble is popped the game object is disabled and the bubble in session count is increased 
    /// </summary>
    private void OnDisable()
    {
        GameManager.instance.bubbleInSessionCount++;
    }

    /// <summary>
    /// This method is calculating the reach grade with this parameters:
    /// vAvgNormal: calculates the average velocity, and normalizing it in relation of the previous session
    /// maxVcntNormal: calculates the number of peaks of velocities
    /// reachTimeNormal: how much time does it takes to reach the bubble in relation of the bubble life time
    /// pathTakenNormal: The calculated path in which the hand taken to reach the bubble in relation of the direct path from the starting position 
    /// to the bubble position
    /// jerkNormal: jerk is the second degree derivation of the velocity, the jerk is calculated by the 
    /// UpdateAverageVelocity method in the FingerTipPookTool instance
    /// bubblePopped: integer form of the isBubblePopped boolean
    /// </summary>
    /// <param name="isBubblePopped">The bubble is popped?</param>

    void CalculateAccuracy(bool isBubblePopped)
    {
        bubblePosition = transform.position;
        // saving normalized Vavg
        float vAvg = FingerTipPokeTool.averageVelocity.magnitude;
        GameManager.instance.sumOfVelocities += vAvg;
        VavgNormal = vAvg / GameManager.instance.prevSessionVelocityAvg;

        //saving normalized maxVcnt
        if (FingerTipPokeTool.maxVcnt == 0)
            maxVcntNormal = 1;
        else maxVcntNormal = (float)1 / FingerTipPokeTool.maxVcnt;


        //saving normalized reachTime
        reachTimeNormal = FingerTipPokeTool.reachTime / bubbleLifeTime;
        if (reachTimeNormal < 0.2f)
            reachTimeNormal = 0;
        else if (reachTimeNormal > 1)
            reachTimeNormal = 1;


        //direct path from the poketool starting positoin to the bubble
        directPath = Vector3.Distance(bubblePosition, FingerTipPokeTool.startPosition);

        //saving normalized pathTaken
        if (FingerTipPokeTool.pathTaken - directPath < 0)
            pathTakenNormal = 1;
        else
            pathTakenNormal = 1 / ((FingerTipPokeTool.pathTaken - directPath) + 1);


        //saving normalized jerk
        float jerkAvg = FingerTipPokeTool.averageJerk.magnitude;
        GameManager.instance.sumOfJerks += jerkAvg;
        jerkNormal = jerkAvg / GameManager.instance.prevSessionJerkAvg;
        jerkNormal = jerkNormal > 1 ? 1 : jerkNormal;
        // jerkNormal = 1 - jerkNormal;
        VavgNormal = VavgNormal > 1 ? 1 : VavgNormal;
        // saving bubble pop
        bubblePop = isBubblePopped ? 1 : 0;

        // Total Score calculating
        fTotalScore = (20 * VavgNormal) + (10 * maxVcntNormal) - (20 * reachTimeNormal) + (25 * pathTakenNormal) - (10 * jerkNormal) + 45 * bubblePop;
        totalScore = (int)fTotalScore;
        if (totalScore < 0)
            totalScore = 0;
        // Check if the trigger happens multiple times to call CSVWrite only once
        if (FingerTipPokeTool.notMultiply)
        {
            FingerTipPokeTool.notMultiply = false;
            CSVReader.CSVWrite();
        }
    }


    // Update is called once per frame
    void Update()
    {
        timePass += Time.deltaTime;

        numOfFrames++;
        numOfUpdates++;
        FingerTipPokeTool.calculateReachTime();
        if (numOfUpdates >= 10)
        {
            FingerTipPokeTool.maxVelocityCount();
            numOfUpdates = 0;
        }

        FingerTipPokeTool.calculatePathTakenToBubble();
        //If 10 seconds passed and there is a bubble on the screen
        if (timePass > bubbleLifeTime)
        {
            timePass = 0.0f;
            CalculateAccuracy(false);
            this.gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider collider)
    {
        int HandIdx = GetIndexFingerHandId(collider);
        if (collider.CompareTag("Finger Tip"))
        {
            CalculateAccuracy(true);
            FingerTipPokeTool.bubblePopped = true;
            //Destroy(this.gameObject);
            this.gameObject.SetActive(false);
        }

    }
    /// <summary>
    /// Gets the hand id associated with the index finger of the collider passed as parameter, if any
    /// </summary>
    /// <param name="collider">Collider of interest</param>
    /// <returns>0 if the collider represents the finger tip of left hand, 1 if it is the one of right hand, -1 if it is not an index fingertip</returns>

    private int GetIndexFingerHandId(Collider collider)
    {
        //Checking Oculus code, it is possible to see that physics capsules gameobjects always end with _CapsuleCollider
        if (collider.gameObject.name.Contains("_CapsuleCollider"))
        {
            //get the name of the bone from the name of the gameobject, and convert it to an enum value
            string boneName = collider.gameObject.name.Substring(0, collider.gameObject.name.Length - 16);
            OVRPlugin.BoneId boneId = (OVRPlugin.BoneId)Enum.Parse(typeof(OVRPlugin.BoneId), boneName);

            //if it is the tip of the Index
            if (boneId == OVRPlugin.BoneId.Hand_Index3 || boneId == OVRPlugin.BoneId.Hand_Index2
                || boneId == OVRPlugin.BoneId.Hand_Index1)
                //check if it is left or right hand, and change color accordingly.
                //Notice that absurdly, we don't have a way to detect the type of the hand
                //so we have to use the hierarchy to detect current hand
                return 0;
        }

        return -1;
    }
}
