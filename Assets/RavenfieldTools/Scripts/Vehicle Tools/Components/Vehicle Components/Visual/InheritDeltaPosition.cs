using UnityEngine;
using System.Collections;

public class InheritDeltaPosition : MonoBehaviour {

	public Transform target;
	public Vector3 multiplier = Vector3.one;

	Vector3 localOrigin;
	Vector3 deltaLocalPosition;

	void Start() {
		this.localOrigin = this.transform.localPosition;
		this.deltaLocalPosition = this.transform.parent.worldToLocalMatrix.MultiplyPoint(target.position) - this.localOrigin;
	}

	void LateUpdate () {
		Vector3 delta = this.transform.parent.worldToLocalMatrix.MultiplyPoint(target.position) - this.localOrigin - deltaLocalPosition;
		this.transform.localPosition = this.localOrigin + Vector3.Scale(delta, this.multiplier);
	}
}
