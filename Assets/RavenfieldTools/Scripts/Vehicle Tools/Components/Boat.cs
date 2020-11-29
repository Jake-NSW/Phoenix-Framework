using UnityEngine;
using System.Collections;

public class Boat : Vehicle {

	public float floatAcceleration = 10f;
	public float floatDepth = 0.5f;

	public float speed = 5f;
	public float reverseMultiplier = 0.5f;
	public float turnSpeed = 5f;
	public float stability = 1f;

	public Transform[] floatingSamplers;
	public bool requireDeepWater = false;
}
