using UnityEngine;
using System.Collections;

public class TurretSpawner : MonoBehaviour {

	static readonly string[] meshSources = new string[] {
		"Preview/machinegun",
		"Preview/tow",
		"Preview/antiair",
	};

	static Mesh[] previewMeshes;

	static void SetupPreviewMeshes() {
		previewMeshes = new Mesh[meshSources.Length];

		for(int i = 0; i < meshSources.Length; i++) {
			GameObject prefab = Resources.Load(meshSources[i]) as GameObject;
			if(prefab != null) {
				previewMeshes[i] = prefab.GetComponentInChildren<MeshFilter>().sharedMesh;
			}
		}
	}

	public enum TurretSpawnType {MachineGun, AntiTank, AntiAir};
	public TurretSpawnType typeToSpawn;
	public GameObject prefab;

	public byte priority = 0;

	void OnValidate() {
		if(previewMeshes == null) {
			SetupPreviewMeshes();
		}

		try {
			MeshFilter meshFilter = GetComponent<MeshFilter>();
			meshFilter.mesh = previewMeshes[(int) this.typeToSpawn];
		}
		catch(System.Exception) { }
	}
}
