using UnityEngine;
using System.Collections;

public class HeadingGuide : MonoBehaviour {

	const float ROTATION_SPEED = 200f;
	const float VELOCITY_THRESHOLD = 0.2f;

	new Rigidbody rigidbody;

	void Awake() {
		rigidbody = GetComponentInParent<Rigidbody>();
	}

	void LateUpdate() {
		Quaternion targetRotation = this.transform.parent.rotation;

		if(this.rigidbody.velocity.magnitude > VELOCITY_THRESHOLD) {
			targetRotation = Quaternion.LookRotation(this.rigidbody.velocity, this.rigidbody.transform.up);
		}

		this.transform.rotation = Quaternion.RotateTowards(this.transform.rotation, targetRotation, ROTATION_SPEED*Time.deltaTime);
	}
}
