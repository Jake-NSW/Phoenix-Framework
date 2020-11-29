using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SpeedLimitZone), true)]
public class SpeedLimitZoneEditor : Editor {

	void OnSceneGUI() {
		SpeedLimitZone speedLimitZone = (SpeedLimitZone) this.target;
		Handles.color = new Color(0.3f, 0.3f, 1f, 0.5f);
		Handles.matrix = speedLimitZone.transform.localToWorldMatrix;
		Handles.zTest = UnityEngine.Rendering.CompareFunction.Less;

		Handles.CubeHandleCap(0, Vector3.zero, Quaternion.identity, 1f, EventType.Repaint);

		Handles.zTest = UnityEngine.Rendering.CompareFunction.Always;
		Handles.matrix = Matrix4x4.identity;
	}
}
