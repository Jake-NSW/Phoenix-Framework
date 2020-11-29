using UnityEngine;
using System.Collections;

public class PitchRollGuide : MonoBehaviour {

	void LateUpdate () {
		this.transform.eulerAngles = new Vector3(0f, this.transform.parent.eulerAngles.y, 0f);
	}
}
