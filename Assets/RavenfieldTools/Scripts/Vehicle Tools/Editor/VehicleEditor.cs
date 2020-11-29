#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(Vehicle), true)]
public class VehicleEditor : Editor {

	Vehicle vehicle;

	void OnEnable() {
		this.vehicle = (Vehicle) this.target;
	}

	/*public override void OnInspectorGUI()
	{
		DrawDefaultInspector();

		if(GUILayout.Button("Generate"))
		{
			SceneView.RepaintAll();
		}
	}*/

	void OnSceneGUI() {
		Handles.color = Color.red;

		Vector3 flatForward = this.vehicle.transform.forward;
		flatForward.y = 0f;
		flatForward.Normalize();

		Vector3 flatRight = new Vector3(-flatForward.z, 0f, flatForward.x);

		flatForward *= this.vehicle.avoidanceSize.y;
		flatRight *= this.vehicle.avoidanceSize.x;

		float coarseAvoidanceRadius = this.vehicle.avoidanceSize.magnitude;

		Vector3[] corners = new Vector3[5];

		corners[0] = this.vehicle.transform.position + flatForward + flatRight;
		corners[1] = this.vehicle.transform.position + flatForward - flatRight;
		corners[2] = this.vehicle.transform.position - flatForward - flatRight;
		corners[3] = this.vehicle.transform.position - flatForward + flatRight;
		corners[4] = this.vehicle.transform.position + flatForward + flatRight;

		Handles.DrawPolyLine(corners);
		Handles.DrawWireArc(this.vehicle.transform.position, Vector3.up, Vector3.forward, 360f, coarseAvoidanceRadius);

		Handles.color = Color.white;
		Handles.matrix = this.vehicle.transform.localToWorldMatrix;
		Handles.DrawWireCube(this.vehicle.ramOffset, this.vehicle.ramSize*2f);
		Handles.matrix = Matrix4x4.identity;
	}
}
#endif