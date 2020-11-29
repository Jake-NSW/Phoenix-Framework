using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(LandingZone))]
public class LandingZoneEditor : Editor {

	LandingZone landingZone;

	void OnEnable() {
		this.landingZone = (LandingZone) this.target;
	}

	void OnSceneGUI() {
		Handles.color = Color.red;
		Handles.ArrowHandleCap(0, this.landingZone.transform.position, this.landingZone.transform.rotation, 4f, EventType.Ignore);
		if(this.landingZone.target != null) {
			Handles.color = Color.magenta;
			Handles.DrawLine(this.landingZone.transform.position, this.landingZone.target.transform.position);
		}
	}
}