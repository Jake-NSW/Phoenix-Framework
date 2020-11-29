using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiAnimator : MonoBehaviour {

	public Vector3 scale = Vector3.one;
	public Vector3 rotation = Vector3.zero;

	public AnimationCurve curve;

	public float periodTime;

	Action action;


	// Update is called once per frame
	void Update () {
		float t = (Time.time%this.periodTime)/this.periodTime;

		float m = this.curve.Evaluate(t);

		this.transform.localScale = m*this.scale;
		this.transform.localEulerAngles = m*this.rotation;
	}
}
