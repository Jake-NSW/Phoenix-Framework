#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(MountedTurret), true)] [CanEditMultipleObjects]
public class MountedTurretEditor : Editor {

	void OnSceneGUI() {
		MountedTurret turret = (MountedTurret) this.target;
		Transform muzzle = turret.configuration.muzzles[0];


		if(turret.bearingTransform == null || turret.pitchTransform == null || muzzle == null) return;

		Handles.color = new Color(0f, 1f, 0f, 0.2f);

		if(turret.clampX.enabled) {
			Handles.DrawSolidArc(turret.bearingTransform.position, turret.bearingTransform.up, Vector3.ProjectOnPlane(muzzle.forward, turret.bearingTransform.up), turret.clampX.max, 1f);
			Handles.DrawSolidArc(turret.bearingTransform.position, turret.bearingTransform.up, Vector3.ProjectOnPlane(muzzle.forward, turret.bearingTransform.up), turret.clampX.min, 1f);
		}
		else {
			Handles.DrawSolidArc(turret.bearingTransform.position, turret.bearingTransform.up, Vector3.ProjectOnPlane(muzzle.forward, turret.bearingTransform.up), 360f, 1f);
		}

		Handles.color = new Color(1f, 0f, 0f, 0.2f);

		if(turret.clampY.enabled) {
			Handles.DrawSolidArc(turret.pitchTransform.position, turret.pitchTransform.right, Vector3.ProjectOnPlane(muzzle.forward, turret.pitchTransform.right), turret.clampY.max, 1f);
			Handles.DrawSolidArc(turret.pitchTransform.position, turret.pitchTransform.right, Vector3.ProjectOnPlane(muzzle.forward, turret.pitchTransform.right), turret.clampY.min, 1f);
		}
		else {
			Handles.DrawSolidArc(turret.pitchTransform.position, turret.pitchTransform.right, Vector3.ProjectOnPlane(muzzle.forward, turret.pitchTransform.right), 360f, 1f);
		}
	}
}
#endif