using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(SpawnPointNeighborManager))]
public class SpawnPointNeighborManagerEditor : Editor {

	SpawnPointNeighborManager neighborManager;

	void OnEnable() {
		this.neighborManager = (SpawnPointNeighborManager) this.target;
	}

	void OnSceneGUI() {

		if(this.neighborManager.neighbors == null) return;

		foreach(SpawnPointNeighborManager.SpawnPointNeighbor neighbor in this.neighborManager.neighbors) {
			if(neighbor.a != null && neighbor.b != null) {
				Handles.color = Color.magenta;
				
				Vector3 center = (neighbor.a.transform.position + neighbor.b.transform.position)*0.5f;

				if(!neighbor.oneWay) {
					Handles.DrawLine(neighbor.a.transform.position, neighbor.b.transform.position);
				}
				else {
					Vector3 delta = neighbor.b.transform.position - neighbor.a.transform.position;
					Vector3 deltaDirection = delta.normalized;
					Vector3 bEndPosition = neighbor.b.transform.position - deltaDirection*10f;

					Vector3 arrowDelta = new Vector3(deltaDirection.z, 0f, -deltaDirection.x).normalized;

					Handles.DrawLine(neighbor.a.transform.position, bEndPosition);
					Handles.DrawLine(bEndPosition, bEndPosition + (-deltaDirection + arrowDelta)*10f);
					Handles.DrawLine(bEndPosition, bEndPosition + (-deltaDirection - arrowDelta)*10f);

					Handles.DrawLine(center, center + (-deltaDirection + arrowDelta)*10f);
					Handles.DrawLine(center, center + (-deltaDirection - arrowDelta)*10f);
				}

				if(neighbor.waterConnection) {
					Handles.color = Color.blue;
					Handles.SphereHandleCap(-1, center-Vector3.up*5f, Quaternion.identity, 4f, EventType.Repaint);
				}

				if(neighbor.landConnection) {
					Handles.color = Color.green;
					Handles.SphereHandleCap(-1, center+Vector3.up*5f, Quaternion.identity, 4f, EventType.Repaint);
				}
			}
		}
	}
}