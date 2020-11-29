using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarWheel : MonoBehaviour {

	public float suspensionHeight = 0.5f;
	public float wheelRadius = 0.3f;

	public float suspensionAcceleration = 10f;
	public float suspensionDrag = 100f;

	public Transform wheelModel;

	public bool rotate = true;
    public bool invertRotation = false;

    public float turnAngle = 20f;

    public Transform speedSampleTransform;
}
