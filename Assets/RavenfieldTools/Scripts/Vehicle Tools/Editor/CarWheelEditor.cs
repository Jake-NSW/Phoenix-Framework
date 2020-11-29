#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(CarWheel), true)] [CanEditMultipleObjects]
public class CarWheelEditor : Editor {

	void OnSceneGUI() {
		CarWheel wheel = (CarWheel) this.target;

		Transform wheelModelTransform = wheel.transform;

		if(wheel.wheelModel != null) {
			wheelModelTransform = wheel.wheelModel;
		}

		Handles.color = new Color(0f, 1f, 0f, 0.4f);

		Vector3 topSuspensionPosition = wheel.transform.position + Vector3.up*wheel.suspensionHeight;
		Handles.DrawDottedLine(wheel.transform.position, topSuspensionPosition, 2f);
		Handles.DrawWireArc(topSuspensionPosition, wheelModelTransform.forward, wheel.transform.up, 360f, wheel.wheelRadius);

		Handles.color = Color.red;

		Handles.DrawWireArc(wheel.transform.position, wheelModelTransform.forward, wheel.transform.up, 360f, wheel.wheelRadius);
		if(wheel.rotate) {
			Vector3 arrowDirection = Vector3.Cross(wheelModelTransform.forward, wheel.transform.up);
			Handles.ArrowHandleCap(0, wheel.transform.position+wheel.transform.up*wheel.wheelRadius, Quaternion.LookRotation(arrowDirection), 0.5f, EventType.Repaint);
		}
		if(wheel.turnAngle != 0f) {
			Handles.color = new Color(0f, 0f, 1f, 0.5f);
			Quaternion turnQuaternion = Quaternion.AngleAxis(wheel.turnAngle, wheel.transform.up);
			Handles.DrawWireArc(wheel.transform.position, turnQuaternion*wheelModelTransform.forward, wheel.transform.up, 180f, wheel.wheelRadius);

			turnQuaternion = Quaternion.AngleAxis(-wheel.turnAngle, wheel.transform.up);
			Handles.DrawWireArc(wheel.transform.position, turnQuaternion*wheelModelTransform.forward, wheel.transform.up, 180f, wheel.wheelRadius);
		}
	}
}
#endif