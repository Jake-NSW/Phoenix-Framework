using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpawnPoint : MonoBehaviour {

	public enum DefenseStrategy {Default, NeverAutoDefend, AlwaysAutoDefend};
	public enum Team {Neutral=-1, Blue=0, Red=1};

	public Team defaultOwner = Team.Blue;

	public float protectRange = 60f;
	public DefenseStrategy defenseStrategy = DefenseStrategy.Default;

	public VehicleFilter vehicleFilter;
	public string shortName = "SPAWN";

	public Transform spawnpointContainer;

	public bool isRelevantPathfindingPoint = true;

	public CaptureAnimation captureAnimation;

	public Transform[] helicopterLandingZones;

	void Start() {
		foreach(Animation animation in this.captureAnimation.animators) {
			if(this.defaultOwner == Team.Blue && string.IsNullOrEmpty(this.captureAnimation.blueCapturedAnimation)) {
				animation.Play(this.captureAnimation.blueCapturedAnimation);
			}
			else if(this.defaultOwner == Team.Red && string.IsNullOrEmpty(this.captureAnimation.redCapturedAnimation)) {
				animation.Play(this.captureAnimation.redCapturedAnimation);
			}
			else if(this.defaultOwner == Team.Neutral && string.IsNullOrEmpty(this.captureAnimation.neutralCapturedAnimation)) {
				animation.Play(this.captureAnimation.neutralCapturedAnimation);
			}
		}
	}

	[System.Serializable]
	public class CaptureAnimation {
		public Animation[] animators;
		public string neutralCapturedAnimation;
		public string blueCapturedAnimation;
		public string redCapturedAnimation;
	}
}
