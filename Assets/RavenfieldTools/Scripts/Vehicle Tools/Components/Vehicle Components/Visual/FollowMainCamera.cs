using UnityEngine;
using System.Collections;

public class FollowMainCamera : MonoBehaviour {
	
	// Update is called once per frame
	void LateUpdate () {
		this.transform.position = Camera.main.transform.position;
	}
}
