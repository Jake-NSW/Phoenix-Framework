using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Ladder), true)]
public class LadderEditor : Editor {

	void OnSceneGUI() {
		Ladder ladder = (Ladder) this.target;
		Handles.color = Color.red;

		Vector3 startPoint = ladder.GetBottomPosition();
		Vector3 endPoint = ladder.GetTopPosition();
		Vector3 bottomExitPoint = ladder.GetBottomExitPosition();
		Vector3 topExitPoint = ladder.GetTopExitPosition();

		Handles.DrawLine(startPoint, endPoint);
		Handles.CubeHandleCap(-1, startPoint, ladder.transform.rotation, 10f, EventType.Ignore);
		Handles.CubeHandleCap(-1, endPoint, ladder.transform.rotation, 10f, EventType.Ignore);
		Handles.DrawWireArc(bottomExitPoint, Vector3.up, Vector3.forward, 360f, 0.4f);
		Handles.DrawWireArc(topExitPoint, Vector3.up, Vector3.forward, 360f, 0.4f);
	}
}
