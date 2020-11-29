using UnityEngine;
using UnityEditor;
using System.Collections;
using Pathfinding;

[CustomEditor(typeof(PathfindingLink))]
	public class PathfindingLinkEditor : Editor {

	PathfindingLink link;

	void OnEnable() {
		this.link = (PathfindingLink) this.target;
	}

	void OnSceneGUI() {

		if(this.link.end == null) return;

		Handles.color = Color.magenta;

		Vector3 direction = this.link.end.position - this.link.transform.position;
		Vector3 normal = Vector3.Cross(Vector3.up, direction).normalized;
		Vector3 down = Vector3.Cross(normal, direction).normalized;
		Vector3 center = 0.5f*(this.link.transform.position + this.link.end.position) + down*direction.magnitude/2f;

		float arcRadius = (direction.magnitude/2f)*Mathf.Sqrt(2f);

		Handles.DrawWireArc(center, normal, this.link.transform.position-center, 90f, arcRadius);
	}
}