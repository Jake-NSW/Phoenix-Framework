using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(VehicleSpawner))]
public class VehicleSpawnerEditor : Editor {

	//public enum VehicleSpawnType {Jeep, JeepMachineGun, Quad, Tank, AttackHelicopter, FighterPlane, Rhib, AttackBoat};
	public static Mesh[] previewMeshes;

	public static void SetupMeshes() {
		
	}

	//VehicleSpawner spawner;

	void OnEnable() {

		if(previewMeshes == null) {
			SetupMeshes();
		}

		//this.spawner = (VehicleSpawner) this.target;
	}

	void OnSceneGUI() {
		Color c = Color.white;

	}
}
