using UnityEngine;
using System.Collections;

public class RollGuide : MonoBehaviour {

	void LateUpdate () {
		this.transform.eulerAngles = new Vector3(this.transform.parent.eulerAngles.x, this.transform.parent.eulerAngles.y, 0f);
	}
}
