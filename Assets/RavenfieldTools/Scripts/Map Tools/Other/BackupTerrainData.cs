using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using System.IO;
using UnityEditor;
#endif

[ExecuteInEditMode]
public class BackupTerrainData : MonoBehaviour {

	#if UNITY_EDITOR

	const string DATA_NAME = "ExampleScene Terrain";

	// This script create a modified version of the ExampleScene terrain data, so users don't accidentally overwrite it when updating the terrain tools.
	// If the user has a keepterrain file in the project root folder, the data is kept.

	void OnEnable () {
		if(ShouldBackupData()) {
			Debug.Log("Backing up Example Scene terrain data!");
			Terrain terrain = GetComponent<Terrain>();
			string source = AssetDatabase.GetAssetPath(terrain.terrainData);
			string destination = source.Substring(0, source.Length-6) + "_modified.asset";

			AssetDatabase.CopyAsset(source, destination);

			TerrainData data = AssetDatabase.LoadAssetAtPath<TerrainData>(destination);
			terrain.terrainData = data;
			GetComponent<TerrainCollider>().terrainData = data;
		}
	}

	bool ShouldBackupData() {
		return !File.Exists("keepterrain") && GetComponent<Terrain>().terrainData.name == DATA_NAME;
	}

	#endif
}
