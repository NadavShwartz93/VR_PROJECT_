using OculusSampleFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InitializeStartPostion : MonoBehaviour
{
    public float xPosition, yPosition, zPosition;
    private InteractableToolsCreator interactableTools;


    // Start is called before the first frame update
    void Start()
    {

        // N is the length of the space which the camera rig will spawn
        // We need to place the player in the middle of the range
        yPosition = xPosition = ((2*GameManager.instance.current_player.N-1) / 20.0f);
        zPosition = 0f;
        transform.position = new Vector3(xPosition, yPosition, zPosition);
        interactableTools = GameObject.Find("InteractableToolsSDKDriver").GetComponent<InteractableToolsCreator>();
        if (interactableTools == null) return;
        StartGame();
        OVRManager.display.RecenterPose();
    }

    public void StartGame()
    {
        //if (scene.name != "Game Scene") return;
        GameManager.instance.gameStarted = true;

        //Debug.Log("algorithm controller started");
        GameObject handToRemove;
        GameManager.instance.timeText = GameObject.FindGameObjectWithTag("TimeText").GetComponent<Text>();
        GameManager.instance.scoreText = GameObject.FindGameObjectWithTag("ScoreText").GetComponent<Text>();
        GameManager.instance.gameScore = 0;
        if (GameManager.instance.current_player.hand_in_therapy.CompareTo("left") == 0)
        {
            handToRemove = GameObject.Find("OVRHandRightPrefab");
            Array.Clear(interactableTools.RightHandTools, 0, interactableTools.RightHandTools.Length);

        }
        else
        {
            handToRemove = GameObject.Find("OVRHandLeftPrefab");
            Array.Clear(interactableTools.LeftHandTools, 0, interactableTools.LeftHandTools.Length);
        }
        handToRemove.gameObject.SetActive(false);

        Vector3 lastBubblePosition = GameManager.instance.current_player.lastBubblePos;
        GameManager.instance.bubbleInSessionCount = 0;
        GameManager.instance.sumOfVelocities = 0;
        GameManager.instance.sumOfJerks=0;
        if (GameManager.instance.isFirstBubble)
        {
            //randomizing the bubble location at the middle of the play area

            int avgX, avgY, avgZ;
            avgX = (int)((GameManager.instance.rightBoundry + GameManager.instance.leftBoundry) / 2.0f);
            avgY = (int)((GameManager.instance.upBoundry + GameManager.instance.downBoundry) / 2.0f);
            avgZ = (int)((GameManager.instance.forwardBoundry + GameManager.instance.backBoundry) / 2.0f);

            GameManager.instance.xPosition = UnityEngine.Random.Range(avgX - 2 > GameManager.instance.leftBoundry ? avgX - 2 : GameManager.instance.leftBoundry, avgX + 3 < GameManager.instance.rightBoundry ? avgX + 3 : GameManager.instance.rightBoundry);
            GameManager.instance.yPosition = UnityEngine.Random.Range(avgY - 2 > GameManager.instance.downBoundry ? avgY - 2 : GameManager.instance.downBoundry, avgY + 3 < GameManager.instance.upBoundry ? avgY + 3 : GameManager.instance.upBoundry);
            GameManager.instance.zPosition = UnityEngine.Random.Range(avgZ - 2 > GameManager.instance.backBoundry ? avgZ - 2 : GameManager.instance.backBoundry, avgZ + 3 < GameManager.instance.forwardBoundry ? avgZ + 3 : GameManager.instance.forwardBoundry);
            GameManager.instance.startingRoundCounter = GameManager.instance.startingRoundCounter > 0 ? GameManager.instance.startingRoundCounter : 15;
            GameManager.instance.prevSessionVelocityAvg = 1;
            GameManager.instance.prevSessionJerkAvg = 10;
        }
        else
        {
            GameManager.instance.xPosition = (int)lastBubblePosition.x;
            GameManager.instance.yPosition = (int)lastBubblePosition.y;
            GameManager.instance.zPosition = (int)lastBubblePosition.z;
            GameManager.instance.prevSessionVelocityAvg = GameManager.instance.current_player.prevSessionVelocityAverage;
            GameManager.instance.prevSessionJerkAvg = GameManager.instance.current_player.prevSessionJerkAverage;
        }
        StartCoroutine(GameManager.instance.DelayLoadLevel(GameManager.instance.treatmentTime));

    }

}
