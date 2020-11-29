using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveToAndSetParent : MonoBehaviour {

	public Transform[] targets;

	public void MoveTo(Transform target) {
		this.transform.parent = target;
		this.transform.localPosition = Vector3.zero;
		this.transform.localRotation = Quaternion.identity;
		this.transform.localScale = Vector3.one;
	}

	public void MoveToTargetIndex(int index) {
		MoveTo(this.targets[index]);
	}
}
