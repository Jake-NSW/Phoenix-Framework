using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(CapturePoint))]
public class CapturePointEditor : Editor {

	CapturePoint capturePoint;

	void OnEnable() {
		this.capturePoint = (CapturePoint) this.target;
	}

	void OnSceneGUI() {
		Color c = Color.white;
		if(this.capturePoint.defaultOwner == SpawnPoint.Team.Blue) {
			c = Color.blue;
		}
		else if(this.capturePoint.defaultOwner == SpawnPoint.Team.Red) {
			c = Color.red;
		}


		Handles.color = c;
		Handles.DrawWireDisc(this.capturePoint.transform.position, Vector3.up, this.capturePoint.protectRange);

		c.a = 0.5f;
		Handles.DrawWireDisc(this.capturePoint.transform.position+Vector3.down*this.capturePoint.captureFloor, Vector3.up, this.capturePoint.captureRange);
		Handles.DrawWireDisc(this.capturePoint.transform.position+Vector3.up*this.capturePoint.captureCeiling, Vector3.up, this.capturePoint.captureRange);

		c.a = 0.2f;
		Handles.color = c;
		Handles.DrawSolidDisc(this.capturePoint.transform.position, Vector3.up, this.capturePoint.captureRange);
	}
}