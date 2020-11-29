using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationDrivenVehicle : Vehicle {

	public GroundChecker[] groundCheckers;
	public int inputSmoothness = 16;

	[System.Serializable]
	public class GroundChecker {
		public Transform checker;
		public float rayLength = 1f;
	}
}
