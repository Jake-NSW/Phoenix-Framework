using UnityEngine;
using System.Collections;

public class DeltaRotationIndicator : MonoBehaviour {

	public Transform target, relativeTo;
	public Vector3 targetRotationAxis = new Vector3(0f, 1f, 0f), indicatorRotationAxis = new Vector3(0f, 0f, 1f);
	
	// Update is called once per frame
	void Update () {
		Quaternion delta = this.target.rotation*Quaternion.Inverse(this.relativeTo.rotation);
		this.transform.localEulerAngles = this.indicatorRotationAxis*Vector3.Dot(delta.eulerAngles, this.targetRotationAxis);
	}
}
