using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(AlternativePathSet)), CanEditMultipleObjects]
	public class AlternativePathSetEditor : Editor {

	void OnSceneGUI() {

		AlternativePathSet set = (AlternativePathSet) this.target;

		for(int i = 0; i < set.transform.childCount; i++) {
			Transform box = set.transform.GetChild(i);

			if(set.spawnA != null && set.spawnB != null) {
				Handles.color = Color.cyan;
				Handles.DrawLine(set.spawnA.transform.position, box.transform.position);
				Handles.DrawLine(set.spawnB.transform.position, box.transform.position);
			}

			Handles.matrix = box.transform.localToWorldMatrix;
			Color c = Color.blue;

			Handles.color = c;
			Handles.zTest = UnityEngine.Rendering.CompareFunction.Less;
			Handles.DrawWireCube(Vector3.zero, Vector3.one);

			c.a = 0.2f;
			Handles.color = c;
			Handles.zTest = UnityEngine.Rendering.CompareFunction.Always;
			Handles.DrawWireCube(Vector3.zero, Vector3.one);

			Handles.matrix = Matrix4x4.identity;
		}
	}
}