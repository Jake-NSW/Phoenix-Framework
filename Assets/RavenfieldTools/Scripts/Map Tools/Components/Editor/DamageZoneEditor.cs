using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(DamageZone), true)]
public class DamageZoneEditor : Editor {

	void OnSceneGUI() {
		DamageZone damageZone = (DamageZone) this.target;
		Handles.color = new Color(1f, 0.3f, 0.3f, 0.5f);
		Handles.matrix = damageZone.transform.localToWorldMatrix;
		Handles.zTest = UnityEngine.Rendering.CompareFunction.Less;

		Handles.CubeHandleCap(0, Vector3.zero, Quaternion.identity, 1f, EventType.Repaint);

		Handles.zTest = UnityEngine.Rendering.CompareFunction.Always;
		Handles.matrix = Matrix4x4.identity;
	}
}
