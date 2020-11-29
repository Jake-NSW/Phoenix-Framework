using UnityEngine;
using System.Collections;

public class ForceHeight : MonoBehaviour {

	public float height = 0f;
	
	// Update is called once per frame
	void LateUpdate () {
		Vector3 position = this.transform.position;
		position.y = this.height;
		this.transform.position = position;
	}
}
