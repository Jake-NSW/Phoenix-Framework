using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(PathfindingBox))]
	public class PathfindingBoxEditor : Editor {

	PathfindingBox pathfindingBox;

	void OnEnable() {
		this.pathfindingBox = (PathfindingBox) this.target;
	}

	void OnSceneGUI() {

		Handles.color = Color.red;

		Handles.matrix = this.pathfindingBox.transform.localToWorldMatrix;
		Handles.DrawWireCube(Vector3.zero, Vector3.one);
		Handles.matrix = Matrix4x4.identity;
	}
}