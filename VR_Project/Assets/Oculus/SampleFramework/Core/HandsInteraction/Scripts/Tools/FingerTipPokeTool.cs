/************************************************************************************

Copyright (c) Facebook Technologies, LLC and its affiliates. All rights reserved.  

See SampleFramework license.txt for license terms.  Unless required by applicable law 
or agreed to in writing, the sample code is provided “AS IS” WITHOUT WARRANTIES OR 
CONDITIONS OF ANY KIND, either express or implied.  See the license for specific 
language governing permissions and limitations under the license.

************************************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace OculusSampleFramework
{
    /// <summary>
    /// Poke tool used for near-field (touching) interactions. Assumes that it will be placed on
    /// finger tips.
    /// </summary>
    public class FingerTipPokeTool : InteractableTool
    {

        private const int NUM_VELOCITY_FRAMES = 10;
        public Vector3 Accelarate;
        public Vector3 Jerk;

        #region Static Properties
        public static bool bubblePopped = false;
        public static Vector3 maxV = Vector3.zero;
        public static int maxVcnt = 0;
        public static float reachTime = 0;
        public static Vector3 startPosition = Vector3.zero;
        public static float pathTaken = 0;
        public static Vector3 lastPosition = Vector3.zero; // The last position the hand was
        public static Vector3 prevVelocity = Vector3.zero;
        public static Vector3 prevAcc = Vector3.zero;
        public static Vector3 prevJerk = Vector3.zero;
        // public static float prevAcceleration = 0;
        // public static float currAcceleration = 0;
        public static Vector3 averageVelocity = Vector3.zero;

        public static Vector3 averageJerk = Vector3.zero;

        public static bool notMultiply = true;
        public static InitializeStartPostion initStartPos;

        #endregion
        [SerializeField] private FingerTipPokeToolView _fingerTipPokeToolView = null;
        [SerializeField] public OVRPlugin.HandFinger _fingerToFollow = OVRPlugin.HandFinger.Index;

        private void Awake()
        {
            initStartPos = FindObjectOfType<InitializeStartPostion>();
        }


        public override InteractableToolTags ToolTags
        {
            get
            {
                return InteractableToolTags.Poke;
            }
        }
        public override ToolInputState ToolInputState
        {
            get
            {
                return ToolInputState.Inactive;
            }
        }
        public override bool IsFarFieldTool
        {
            get
            {
                return false;
            }
        }
        public override bool EnableState
        {
            get
            {
                return _fingerTipPokeToolView.gameObject.activeSelf;
            }
            set
            {
                _fingerTipPokeToolView.gameObject.SetActive(value);
            }
        }

        #region Movement Derivation per frames
        public static Vector3[] _velocityFrames;
        public static Vector3[] _accelrationFrames;
        public static Vector3[] _jerkFrames;
        private int _currVelocityFrame = 0;
        private bool _sampledMaxFramesAlready;
        private Vector3 _position;
        #endregion

        private BoneCapsuleTriggerLogic[] _boneCapsuleTriggerLogic;

        private float _lastScale = 1.0f;
        private bool _isInitialized = false;
        private OVRBoneCapsule _capsuleToTrack;

        /// <summary>
        /// Initializing the poking tool
        /// </summary>
        public override void Initialize()
        {
            Assert.IsNotNull(_fingerTipPokeToolView);

            InteractableToolsInputRouter.Instance.RegisterInteractableTool(this);
            _fingerTipPokeToolView.InteractableTool = this;

            _velocityFrames = new Vector3[NUM_VELOCITY_FRAMES];
            _accelrationFrames = new Vector3[NUM_VELOCITY_FRAMES];
            _jerkFrames = new Vector3[NUM_VELOCITY_FRAMES];
            Array.Clear(_velocityFrames, 0, NUM_VELOCITY_FRAMES);
            Array.Clear(_accelrationFrames, 0, NUM_VELOCITY_FRAMES);
            Array.Clear(_accelrationFrames, 0, NUM_VELOCITY_FRAMES);
            StartCoroutine(AttachTriggerLogic());
        }

        private IEnumerator AttachTriggerLogic()
        {
            while (!HandsManager.Instance || !HandsManager.Instance.IsInitialized())
            {
                yield return null;
            }

            OVRSkeleton handSkeleton = IsRightHandedTool ? HandsManager.Instance.RightHandSkeleton : HandsManager.Instance.LeftHandSkeleton;

            OVRSkeleton.BoneId boneToTestCollisions = OVRSkeleton.BoneId.Hand_Pinky3;
            switch (_fingerToFollow)
            {
                case OVRPlugin.HandFinger.Thumb:
                    boneToTestCollisions = OVRSkeleton.BoneId.Hand_Index3;
                    break;
                case OVRPlugin.HandFinger.Index:
                    boneToTestCollisions = OVRSkeleton.BoneId.Hand_Index3;
                    break;
                case OVRPlugin.HandFinger.Middle:
                    boneToTestCollisions = OVRSkeleton.BoneId.Hand_Middle3;
                    break;
                case OVRPlugin.HandFinger.Ring:
                    boneToTestCollisions = OVRSkeleton.BoneId.Hand_Ring3;
                    break;
                default:
                    boneToTestCollisions = OVRSkeleton.BoneId.Hand_Pinky3;
                    break;
            }

            List<BoneCapsuleTriggerLogic> boneCapsuleTriggerLogic = new List<BoneCapsuleTriggerLogic>();
            List<OVRBoneCapsule> boneCapsules = HandsManager.GetCapsulesPerBone(handSkeleton, boneToTestCollisions);
            foreach (var ovrCapsuleInfo in boneCapsules)
            {
                var boneCapsuleTrigger = ovrCapsuleInfo.CapsuleRigidbody.gameObject.AddComponent<BoneCapsuleTriggerLogic>();
                ovrCapsuleInfo.CapsuleCollider.isTrigger = true;
                boneCapsuleTrigger.ToolTags = ToolTags;
                boneCapsuleTriggerLogic.Add(boneCapsuleTrigger);
            }

            _boneCapsuleTriggerLogic = boneCapsuleTriggerLogic.ToArray();
            // finger tip should have only one capsule
            if (boneCapsules.Count > 0)
            {
                _capsuleToTrack = boneCapsules[0];
            }

            _isInitialized = true;
        }
        /// <summary>
        /// updating the hand velocity and scaling
        /// </summary>
        private void Update()
        {
            if (!HandsManager.Instance || !HandsManager.Instance.IsInitialized() || !_isInitialized || _capsuleToTrack == null)
            {
                return;
            }

            OVRHand hand = IsRightHandedTool ? HandsManager.Instance.RightHand : HandsManager.Instance.LeftHand;
            float currentScale = hand.HandScale;
            // push tool into the tip based on how wide it is. so negate the direction
            Transform capsuleTransform = _capsuleToTrack.CapsuleCollider.transform;
            Vector3 capsuleDirection = capsuleTransform.right;
            Vector3 trackedPosition = capsuleTransform.position + _capsuleToTrack.CapsuleCollider.height * 0.5f
              * capsuleDirection;
            Vector3 sphereRadiusOffset = currentScale * _fingerTipPokeToolView.SphereRadius *
              capsuleDirection;
            // push tool back so that it's centered on transform/bone
            Vector3 toolPosition = trackedPosition + sphereRadiusOffset;
            transform.position = toolPosition;
            transform.rotation = capsuleTransform.rotation;
            InteractionPosition = trackedPosition;

            UpdateAverageVelocity();

            CheckAndUpdateScale();

            //if the bubble popped or the 10 seconds passed and the patient returned to the start area, calculate the next bubble location accordingly and initiate the relevant variables
            if ((bubblePopped || reachTime > 10.0f) && startMeasure())
            {
                if (bubblePopped)//if popped
                {
                    GameManager.instance.CalcNextBubbleLocation(true);
                }
                if (reachTime > 10.0f)
                {
                    GameManager.instance.CalcNextBubbleLocation(false);//if time passed
                }
                //initializing all the parameters for the reach grading formula
                bubblePopped = false;
                maxV = Vector3.zero;
                maxVcnt = 0;
                reachTime = 0;
                pathTaken = 0;
                prevVelocity = Vector3.zero;
                prevAcc = Vector3.zero;
                prevJerk = Vector3.zero;
                notMultiply = true;
            }








        }
        /// <summary>
        /// Updating the average velocity of the poke tool
        /// Additionaly it will calculate the average of the accelaration and the jerk of the hand
        /// </summary>
        private void UpdateAverageVelocity()
        {
            var prevPosition = _position;
            var currPosition = transform.position;
            prevVelocity = Velocity;//saves last velocity
            prevAcc = Accelarate;
            prevJerk = Jerk;
            var currentVelocity = (currPosition - prevPosition) / Time.deltaTime;
            var currAcceleration = (currentVelocity - prevVelocity);
            var currJerk = (currAcceleration - prevAcc);
            //Debug.Log("finger tip velocity: " + currentVelocity);
            _position = currPosition;
            _velocityFrames[_currVelocityFrame] = currentVelocity;
            _accelrationFrames[_currVelocityFrame] = currAcceleration;
            _jerkFrames[_currVelocityFrame] = currJerk;
            // if sampled more than allowed, loop back toward the beginning
            _currVelocityFrame = (_currVelocityFrame + 1) % NUM_VELOCITY_FRAMES;

            Velocity = Vector3.zero;
            Accelarate = Vector3.zero;
            Jerk = Vector3.zero;
            // edge case; when we first start up, we will have only sampled less than the
            // max frames. so only compute the average over that subset. After that, the
            // frame samples will act like an array that loops back toward to the beginning
            if (!_sampledMaxFramesAlready && _currVelocityFrame == NUM_VELOCITY_FRAMES - 1)
            {
                _sampledMaxFramesAlready = true;
            }

            int numFramesToSamples = _sampledMaxFramesAlready ? NUM_VELOCITY_FRAMES : _currVelocityFrame + 1;
            for (int frameIndex = 0; frameIndex < numFramesToSamples; frameIndex++)
            {
                Velocity += _velocityFrames[frameIndex];
                Accelarate += _accelrationFrames[frameIndex];
                Jerk += _jerkFrames[frameIndex];
            }

            Velocity /= numFramesToSamples;
            Accelarate /= numFramesToSamples;
            Jerk /= numFramesToSamples;
            averageVelocity = Velocity; // save the velocity
            averageJerk = Jerk;
        }



        private void CheckAndUpdateScale()
        {
            float currentScale = IsRightHandedTool ? HandsManager.Instance.RightHand.HandScale
                : HandsManager.Instance.LeftHand.HandScale;
            if (Mathf.Abs(currentScale - _lastScale) > Mathf.Epsilon)
            {
                transform.localScale = new Vector3(currentScale, currentScale, currentScale);
                _lastScale = currentScale;
            }
        }

        public override List<InteractableCollisionInfo> GetNextIntersectingObjects()
        {
            _currentIntersectingObjects.Clear();

            foreach (var boneCapsuleTriggerLogic in _boneCapsuleTriggerLogic)
            {
                var collidersTouching = boneCapsuleTriggerLogic.CollidersTouchingUs;
                foreach (ColliderZone colliderTouching in collidersTouching)
                {
                    _currentIntersectingObjects.Add(new InteractableCollisionInfo(colliderTouching,
                        colliderTouching.CollisionDepth, this));
                }
            }

            return _currentIntersectingObjects;
        }

        public override void FocusOnInteractable(Interactable focusedInteractable,
          ColliderZone colliderZone)
        {
            // no need for focus
        }

        public override void DeFocus()
        {
            // no need for focus
        }

        //Check which hand in use and if the patient is in the start area with this hand, also saves the start position and initiate the last position to it as well
        public static bool startMeasure()
        {
            float x, xOffset;
            float y, yOffset;
            float z, zOffset;
            if (ButtonListener.activePlayer.hand_in_therapy.CompareTo("right") == 0)
            {
                x = HandsManager.Instance.RightHand.transform.position.x;
                y = HandsManager.Instance.RightHand.transform.position.y;
                z = HandsManager.Instance.RightHand.transform.position.z;
            }

            else
            {
                x = HandsManager.Instance.LeftHand.transform.position.x;
                y = HandsManager.Instance.LeftHand.transform.position.y;
                z = HandsManager.Instance.LeftHand.transform.position.z;
            }

            if (initStartPos)
            {

                xOffset = x - initStartPos.xPosition;
                yOffset = y - initStartPos.yPosition;
                zOffset = z - initStartPos.zPosition;
            }
            else
            {
                xOffset = x;
                yOffset = y;
                zOffset = z;
            }

            if ((xOffset >= -0.2 && xOffset <= 0.2))
            {
                // Debug.LogFormat("x ok {0}",xOffset);
                if (yOffset >= -1.0 && yOffset <= 0.1)
                {
                    // Debug.LogFormat("y ok {0}",yOffset);
                    // if (zOffset >= -0.25 && zOffset <= 0.1)
                    if (zOffset <= 0.3)
                    {
                        // Debug.LogFormat("z ok {0}",zOffset);
                        // startPosition = zOffset < 0 ? Vector3.zero : new Vector3(xOffset, yOffset, zOffset);
                        startPosition = new Vector3(xOffset, yOffset, zOffset);
                        lastPosition = startPosition;
                        return true;
                    }
                }
            }
            return false;
        }

        #region Reach Grading Static Methods
        /// <summary>
        /// maxVelocityCount() is counting the number of local maximums (velocity peaks) withing each movement until the bubble is popped
        /// It takes the calculations from the velocityFrames array (which is calculated in the UpdateAverageVelocity Method), the takes the middle element from the array
        /// and checks if it is the local maximum. if it does, it will increase maxVcnt by one.
        /// </summary>
        public static void maxVelocityCount()
        {
            int medianIndex = (int)(_velocityFrames.Length / 2.0f - 1);
            if (_velocityFrames[medianIndex].magnitude > _velocityFrames[medianIndex - 2].magnitude && _velocityFrames[medianIndex].magnitude > _velocityFrames[medianIndex - 1].magnitude && _velocityFrames[medianIndex].magnitude > _velocityFrames[medianIndex + 1].magnitude && _velocityFrames[medianIndex].magnitude > _velocityFrames[medianIndex + 2].magnitude)
            {

                maxVcnt++;
            }
        }

        //Updates reachTime each frame
        public static void calculateReachTime()
        {
            reachTime = reachTime + Time.deltaTime;
        }

        //Calculates the patient's path from the starting point to the bubble
        public static void calculatePathTakenToBubble()
        {
            Vector3 handPosition, temp;
            if (ButtonListener.activePlayer.hand_in_therapy.CompareTo("right") == 0)
            {
                temp = HandsManager.Instance.RightHand.transform.position;
            }
            else
            {
                temp = HandsManager.Instance.LeftHand.transform.position;
            }
            handPosition = initStartPos == null ? temp : new Vector3(temp.x - initStartPos.xPosition, temp.y - initStartPos.yPosition, temp.z - initStartPos.zPosition);
            pathTaken = pathTaken + Vector3.Distance(handPosition, lastPosition);
            lastPosition = handPosition;
        }
        #endregion
    }
}
