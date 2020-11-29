using UnityEngine;
using System.Collections;

public class AimingWheel : MonoBehaviour {

	public enum Axis {X, Y, Z};

	public Transform target;
	public float rotationMultiplier = 5f;

	public Axis axis;

}
