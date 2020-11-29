using UnityEngine;
using System.Collections;

public class VehicleSpawner : MonoBehaviour {

	static readonly string[] meshSources = new string[] {
		"Preview/jeep",
		"Preview/jeep_machinegun",
		"Preview/quad",
		"Preview/tank",
		"Preview/helicopter",
		"Preview/plane",
		"Preview/rhib",
		"Preview/attackboat",
		"Preview/bomber",
		"Preview/helicopter_transport",
        "Preview/apc",
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

	public enum VehicleSpawnType {Jeep, JeepMachineGun, Quad, Tank, AttackHelicopter, AttackPlane, Rhib, AttackBoat, BombPlane, TransportHelicopter, Apc};

	public float spawnTime = 16f;
	public enum RespawnType {AfterDestroyed, AfterMoved, Never};
	public RespawnType respawnType = RespawnType.AfterDestroyed;

	public VehicleSpawnType typeToSpawn;
	public GameObject prefab;

	public byte priority = 0;

	public bool isRelevantPathfindingPointForBoats = true;

	void OnValidate() {
		if(previewMeshes == null) {
			SetupPreviewMeshes();
		}

		try {
			MeshFilter meshFilter = GetComponent<MeshFilter>();
			try {
				meshFilter.mesh = previewMeshes[(int) this.typeToSpawn];
			}
			catch(System.Exception e) {
				meshFilter.mesh = previewMeshes[0];
			}
		}
		catch(System.Exception) { }
	}
}
