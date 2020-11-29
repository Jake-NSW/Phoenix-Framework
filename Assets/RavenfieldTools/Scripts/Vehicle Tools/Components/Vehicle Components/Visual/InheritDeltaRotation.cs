using UnityEngine;
using System.Collections;

public class InheritDeltaRotation : MonoBehaviour {

	public Transform target;
	public Vector3 multiplier = Vector3.one;

	Vector3 initialLocalEuler;
	//Quaternion deltaLocalPosition;

	void Start() {
		this.initialLocalEuler = this.transform.localEulerAngles;
		//this.deltaLocalPosition = this.transform.parent.worldToLocalMatrix.MultiplyPoint(target.position) - this.localOrigin;
	}

	void LateUpdate () {
		this.transform.localEulerAngles = Vector3.Scale(this.target.localEulerAngles, this.multiplier)+this.initialLocalEuler;
		//this.transform.localRotation = this.localOrigin + Vector3.Scale(delta, this.multiplier);
	}
}
