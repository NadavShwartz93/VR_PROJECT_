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
		private int flag = 0;

		public static bool bubblePopped = false;
		public static Vector3 maxV = Vector3.zero;
		public static int maxVcnt = 0;
		public static float reachTime = 0;
		public static Vector3 startPosition = Vector3.zero;
		public static float pathTaken = 0;
		public static Vector3 lastPosition = Vector3.zero; // The last position the hand was
		public static Vector3 prevVelocity = Vector3.zero;
		public static float prevAcceleration = 0;
		public static float currAcceleration = 0;
		public static float jerk = 0;
		public static Vector3 averageVelocity = Vector3.zero;
		public static bool bubbleExist = true;
		public static bool notMultiply = true;

		[SerializeField] private FingerTipPokeToolView _fingerTipPokeToolView = null;
		[SerializeField] private OVRPlugin.HandFinger _fingerToFollow = OVRPlugin.HandFinger.Index;

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

		private Vector3[] _velocityFrames;
		private int _currVelocityFrame = 0;
		private bool _sampledMaxFramesAlready;
		private Vector3 _position;

		private BoneCapsuleTriggerLogic[] _boneCapsuleTriggerLogic;

		private float _lastScale = 1.0f;
		private bool _isInitialized = false;
		private OVRBoneCapsule _capsuleToTrack;

		public override void Initialize()
		{
			Assert.IsNotNull(_fingerTipPokeToolView);

			InteractableToolsInputRouter.Instance.RegisterInteractableTool(this);
			_fingerTipPokeToolView.InteractableTool = this;

			_velocityFrames = new Vector3[NUM_VELOCITY_FRAMES];
			Array.Clear(_velocityFrames, 0, NUM_VELOCITY_FRAMES);

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
			if((bubblePopped || reachTime > 10.0f) && startMeasure())
			{
				if(bubblePopped)//if popped
				{
					AlgorithmController.CalcNextBubbleLoacation(true);
				}
				if(reachTime > 10.0f)
				{
					AlgorithmController.CalcNextBubbleLoacation(false);//if time passed
				}
				bubblePopped = false;
				maxV = Vector3.zero;
				maxVcnt = 0;
				reachTime = 0;
				pathTaken = 0;
				jerk = 0;
				Bubble.numOfFrames = 0;
				prevVelocity = Vector3.zero;
				prevAcceleration = 0;
				currAcceleration = 0;
				bubbleExist = true;
				notMultiply = true;
				Debug.Log("start calculation: ");
			}

			

			

			

			
		}

		private void UpdateAverageVelocity()
		{
			var prevPosition = _position;
			var currPosition = transform.position;
			prevVelocity = Velocity;//saves last velocity
			var currentVelocity = (currPosition - prevPosition) / Time.deltaTime;
			Debug.Log("finger tip velocity: " + currentVelocity);
			_position = currPosition;
			_velocityFrames[_currVelocityFrame] = currentVelocity;
			// if sampled more than allowed, loop back toward the beginning
			_currVelocityFrame = (_currVelocityFrame + 1) % NUM_VELOCITY_FRAMES;

			Velocity = Vector3.zero;
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
			}

			Velocity /= numFramesToSamples;
			averageVelocity = Velocity; // save the velocity
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
			float x;
			float y;
			float z;
			
			if(ButtonListener.activePlayer.hand_in_therapy.CompareTo("right") == 0)
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

			if((x >= 0.48 && x <= 0.7 && ButtonListener.activePlayer.hand_in_therapy.CompareTo("right") == 0) || (x >= 0.26 && x <= 0.48 && ButtonListener.activePlayer.hand_in_therapy.CompareTo("left") == 0))
			{
				Debug.Log("x position is OK = " + x);
				if(y >= 0.8 && y <= 0.96 && ButtonListener.patientDetails[6] == "yes" || y >= 0.3 && y <= 0.5 && ButtonListener.patientDetails[6] == "no")
				{
					Debug.Log("y position is OK = " + y);
					if(z >= -0.25 && z <= 0.02)
					{
						Debug.Log("z position is OK = " + z);
						if(ButtonListener.activePlayer.hand_in_therapy.CompareTo("right") == 0) //Gets the patient start position of the hand to reach a bubble
						{
							startPosition = z < 0 ? Vector3.zero : HandsManager.Instance.RightHand.transform.position;
						}
						else startPosition = z < 0 ? Vector3.zero : HandsManager.Instance.LeftHand.transform.position;
						Debug.Log("the start position is: " + startPosition);
						lastPosition = startPosition;
						Debug.Log("the last position is: " + lastPosition);
						return true;
					}
				}
			}
			Debug.Log("the position is: " + x + " " + y + " " + z);
			return false;
		}

		//Checks if the current velocity is higher than the last maxV, if it is, updates maxV
		public static void maxVelocity()
		{
			if(averageVelocity.magnitude - maxV.magnitude > 0) //if Velocity > maxV
			{
				maxV = averageVelocity;
				maxVcnt = 0; // if we update maxV so we update maxVcnt as well
			}
			Debug.Log("current velocity is: " + averageVelocity.magnitude);
			Debug.Log("max velocity is: " + maxV.magnitude);
		}

		//If the saved maxV equals to the current Velocity and maxV is not 0, raise the counter by 1
		public static void maxVelocityCount()
		{
			if(Math.Floor(averageVelocity.magnitude) == Math.Floor(maxV.magnitude) && maxV.magnitude != 0)
				maxVcnt++;
			Debug.Log("maxV counter is: " + maxVcnt);
		}

		//Updates reachTime each frame
		public static void calculateReachTime()
		{
			reachTime = reachTime + Time.deltaTime;
		}

		//Calculates the patient's path from the starting point to the bubble
		public static void calculatePathTakenToBubble()
		{
			if(ButtonListener.activePlayer.hand_in_therapy.CompareTo("right") == 0)
			{
				pathTaken = pathTaken + Vector3.Distance(HandsManager.Instance.RightHand.transform.position, lastPosition);
				lastPosition = HandsManager.Instance.RightHand.transform.position;
			}
			else 
			{
				pathTaken = pathTaken + Vector3.Distance(HandsManager.Instance.LeftHand.transform.position, lastPosition);
				lastPosition = HandsManager.Instance.LeftHand.transform.position;
			}	
		}

		
		//Calculates the patient's jerk
		public static void calculateJerk()
		{
			prevAcceleration = currAcceleration;
			currAcceleration = Vector3.Distance(averageVelocity, prevVelocity) / Time.deltaTime;
			if(prevAcceleration == 0)
				Debug.Log("prevAcceleration is: " + prevAcceleration);
			Debug.Log("The current Acceleration is: " + currAcceleration);
			jerk = jerk + (currAcceleration - prevAcceleration) / Time.deltaTime;
			Debug.Log("The jerkiness is: " + jerk);
		}
	}
}
