using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
using UnityEditor.SceneManagement;

public class SanityCheck {

	const int RECOMMENDED_MAX_NEIGHBORS_PER_FLAG = 3;
	const string UNITY_ENGINE_MAJOR_VERSION = "5.6";
	const string UNITY_ENGINE_RECOMMENDED_VERSION = "5.6.7f1";

	static string log;

	static System.Type[] requiredTypes = {
		typeof(CapturePoint),
		typeof(SpawnPointNeighborManager),
		typeof(MinimapCamera),
		typeof(PathfindingBox),
		typeof(ReflectionProber),
		typeof(SceneryCamera),
		typeof(TimeOfDay),
		typeof(WaterLevel),
	};

	static string[] prefabs = {
		"Capture Point",
		"Neighbor Manager",
		"Minimap Camera",
		"Infantry And Car Pathfinding Box",
		"Reflection Prober",
		"Scenery Camera",
		"Time Of Day",
		"Water Level"
	};

	[MenuItem("Ravenfield Tools/Content/Sanity Check Content Mod")]
	public static void MaunalSanityCheckContent() {
		if(DoSanityCheckContent()) {
			EditorUtility.DisplayDialog("Sanity Check Done", "No fatal problems detected in content mod!", "Ok");
		}
	}

	public static bool DoSanityCheckContent() {

		bool ok = DoSanityCheckContentReturnable();

		if(ok) {
			if(!string.IsNullOrEmpty(log)) {
				DisplayWarningLog();
			}
		}
		else {
			Log("All these issues must be fixed before the content mod can be exported.");
			DisplayFatalLog();
		}

		return ok;
	}

	static bool DoSanityCheckContentReturnable() {
		Debug.Log("Running content mod sanity check");

		ClearLog();

		GameObject contentModObject = Selection.activeGameObject;

		if(contentModObject == null) {
			Log("No content mod prefab selected");
			return false;
		}

		if(contentModObject.name == "Example Weapon Content Mod" || contentModObject.name == "Example Vehicle Content Mod" || contentModObject.name == "Example Skin Content Mod") {
			Log("Please make a duplicate of the Example Content Mod prefab and name it something original to your mod!");
			return false;
		}

		Object prefab = PrefabUtility.GetPrefabObject(contentModObject);

		if(prefab == null || contentModObject.scene.IsValid()) {
			Log("Selected object is not a prefab");
			return false;
		}

		if(!ContentExporter.IsContentModObject(contentModObject)) {
			Log("Selected prefab does not contain any Content Mod component");
			return false;
		}

		CheckUnityEditorVersion();

		WeaponContentMod weaponMod = contentModObject.GetComponent<WeaponContentMod>();
		VehicleContentMod vehicleMod = contentModObject.GetComponent<VehicleContentMod>();
        ActorSkinContentMod skinMod = contentModObject.GetComponent<ActorSkinContentMod>();
		MutatorContentMod mutatorMod = contentModObject.GetComponent<MutatorContentMod>();

		bool ok = true;

		if(weaponMod != null) {
			ok &= DoSanityCheckWeaponMod(weaponMod);
		}

		if(vehicleMod != null) {
			ok &= DoSanityCheckVehicleMod(vehicleMod);
		}

        if(skinMod != null) {
            ok &= DoSanityCheckSkinMod(skinMod);
        }

		if(mutatorMod != null) {
			ok &= DoSanityCheckMutatorMod(mutatorMod);
		}

        return ok;
	}

	static bool DoSanityCheckWeaponMod(WeaponContentMod weaponMod) {
		bool ok = true;
		foreach(WeaponManager.WeaponEntry entry in weaponMod.weaponEntries) {
			ok &= DoSanityCheckWeaponEntry(entry);
		}

		return ok;
	}

	static bool DoSanityCheckWeaponEntry(WeaponManager.WeaponEntry entry) {
		GameObject weaponPrefab = entry.prefab;
		if(weaponPrefab == null) {
			Log("Weapon entry "+entry.name+" does not have a prefab");
			return false;
		}

		return DoSanityCheckWeapon(weaponPrefab, entry.name);
	}

	static bool DoSanityCheckWeapon(GameObject weaponPrefab, string name) {
		Weapon weapon = weaponPrefab.GetComponent<Weapon>();
		if(weapon == null) {
			Log("Weapon "+name+" does not have a Weapon component");
			return false;
		}

		if(weapon.ShouldHaveProjectilePrefab()) {
			if(weapon.configuration.projectilePrefab == null) {
				Log("Weapon "+name+" does not have a Projectile prefab");
				return false;
			}

			if(weapon.configuration.projectilePrefab.GetComponent<Projectile>() == null) {
				Log("Weapon "+name+"'s projectile " + weapon.configuration.projectilePrefab.name+" does not have a Projectile component");
				return false;
			}
		}

		return true;
	}

	static bool DoSanityCheckVehicleMod(VehicleContentMod vehicleMod) {
		bool ok = true;
		foreach(GameObject vehiclePrefab in vehicleMod.AllEntries()) {
			if(vehiclePrefab != null) {
				ok &= DoSanityCheckVehicle(vehiclePrefab);
			}
		}

		return ok;
	}

	static bool DoSanityCheckVehicle(GameObject prefab) {

		Vehicle vehicle = prefab.GetComponent<Vehicle>();
		if(vehicle == null) {
			Log("Vehicle prefab "+prefab.name+" has no Vehicle component.");
			return false;
		}

		if(vehicle.seats.Count == 0) {
			Log("Vehicle "+vehicle.name+" has no seats.");
			return false;
		}

		foreach(Seat seat in vehicle.seats) {
			if(seat == null) {
				Log("Vehicle "+vehicle.name+" has one or more unassigned seats.");
				return false;
			}
		}

		foreach(Seat seat in vehicle.seats) {
			string seatName = vehicle.name + " - " + seat.name;
			foreach(Weapon weapon in seat.weapons) {
				if(weapon == null) {
					Log("Seat "+seatName+" has one or more unassigned weapons");
					return false;
				}
				else {
					if(!DoSanityCheckWeapon(weapon.gameObject, seatName + " - " + weapon.gameObject.name)) {
						return false;
					}
				}
			}
		}



		// WARNINGS

		AudioListener audioListener = vehicle.GetComponent<AudioListener>();

		if(audioListener != null) {
			Log("Audio listener component exists on object "+audioListener.gameObject.name+". This may introduce audio bugs and should be removed.");
		}

		return true;
	}

    static bool DoSanityCheckSkinMod(ActorSkinContentMod mod) {
        foreach(var skin in mod.skins) {
            if(skin.characterSkin.mesh == null) {
                Log("Custom Actor Skin " + skin.name + " has no character mesh");
                return false;
            }
        }

        return true;
    }

	static bool DoSanityCheckMutatorMod(MutatorContentMod mod) {
		foreach(var mutator in mod.mutators) {
			if(string.IsNullOrEmpty(mutator.name)) {
				Log("Mutator entry has no name");
				return false;
			}
			if(mutator.mutatorPrefab == null) {
				Log("Mutator " + mutator.name + " has no mutator prefab set");
				return false;
			}
		}

		return true;
	}

	[MenuItem("Ravenfield Tools/Map/Sanity Check Map")]
	public static void ManualSanityCheck() {
		if(DoSanityCheck(false)) {
			EditorUtility.DisplayDialog("Sanity Check Done", "No fatal problems detected in map!", "Ok");
		}
	}

	public static bool DoSanityCheck(bool ignoreGraphCacheWarning) {

		Debug.Log("Running map sanity check");

		ClearLog();

		SpawnPoint[] spawnPoints = GameObject.FindObjectsOfType<SpawnPoint>();
		SpawnPointNeighborManager neighbors = GameObject.FindObjectOfType<SpawnPointNeighborManager>();
		Camera[] cameras = GameObject.FindObjectsOfType<Camera>();
		PathfindingBox[] pathfindingBoxes = GameObject.FindObjectsOfType<PathfindingBox>();

		bool ok = true;
		for(int i = 0; i < requiredTypes.Length; i++) {
			if(GameObject.FindObjectOfType(requiredTypes[i]) == null) {
				Log("No "+requiredTypes[i]+" found in scene, you should place a "+prefabs[i]+" from the \"Prefabs/Map Elements/Must Haves\" folder!");
				ok = false;
			}
		}

		bool blueTeamSpawn = false;
		bool redTeamSpawn = false;

		foreach(SpawnPoint spawnPoint in spawnPoints) {
			if(spawnPoint.defaultOwner == SpawnPoint.Team.Blue) {
				blueTeamSpawn = true;
			}
			else if(spawnPoint.defaultOwner == SpawnPoint.Team.Red) {
				redTeamSpawn = true;
			}
		}

		if(!blueTeamSpawn || !redTeamSpawn) {
			Log("Not all teams have spawn points. Make sure both teams is the default owner of at least one spawn point!");
			ok = false;
		}

		if(neighbors != null && (neighbors.neighbors == null || neighbors.neighbors.Length == 0)) {
			Log("There are no neighbors set up in the Neighbor Manager.");
			ok = false;
		}

		if(!ok) {
			Log("All these issues must be fixed before the map can be exported.");
			DisplayFatalLog();
			return false;
		}




		// WARNINGS

		CheckUnityEditorVersion();

		foreach(Camera camera in cameras) {
			if(camera.enabled && !CameraHasSupportedScript(camera)) {
				Log("The camera "+camera.gameObject.name+" doesn't have a game script on it, and is likely unintetionally placed. Consider deleting it!");
			}
		}

		foreach(PathfindingBox box in pathfindingBoxes) {
			/*int cellNumber = Mathf.RoundToInt((box.transform.localScale.x/box.cellSize)*(box.transform.localScale.z/box.cellSize));
			if(cellNumber > PathfindingBox.RECOMMENDED_MAX_CELLS) {
				
			}*/

			if(box.characterRadius < box.cellSize*2) {
				Log("Character radius of PathfindingBox "+box.gameObject.name+" is less than double the cell size. You should increase the character radius to something larger than "+box.cellSize*2+"!");
			}
		}

		Camera minimapCamera = GameObject.FindObjectOfType<MinimapCamera>().GetComponent<Camera>();

		foreach(SpawnPoint spawn in spawnPoints) {
			Vector3 viewportPoint = minimapCamera.WorldToViewportPoint(spawn.transform.position);
			if(viewportPoint.x < 0 || viewportPoint.x > 11f || viewportPoint.y < 0 || viewportPoint.y > 1f) {
				Log("Spawnpoint "+spawn.gameObject.name+" is outside of the minimap camera, move the minimap or increase the minimap camera's field of view to solve this.");
			}
		}


		if(EditorSceneManager.GetSceneAt(0).name == "ExampleScene") {
			Log("You are working in the ExampleScene, you should save it as your own scene so it doesn't get accidentally overwritten if you update the Ravenfield Tools.");
		}

		if(RenderSettings.ambientMode != UnityEngine.Rendering.AmbientMode.Trilight) {
			Log("Environment Lighting Source is not set to Gradient, consider changing this (in Window -> Lighting -> Settings)!");
		}

		if(neighbors.neighbors.Length > spawnPoints.Length*RECOMMENDED_MAX_NEIGHBORS_PER_FLAG) {
			Log("You have a very large number of neighbor elements, consider simplifying the neighbor connections!");
		}

		if(UnityEditor.Lightmapping.bakedGI) {
			Log("Your scene is set up to use Baked Global Illumination. This is not required for Ravenfield, consider disabling it (in Window -> Lighting -> Settings)!");
		}
		if(UnityEditor.Lightmapping.realtimeGI) {
			Log("Your scene is set up to use Realtime Global Illumination. This is not required for Ravenfield, consider disabling it (in Window -> Lighting -> Settings)!");
		}

		if(!ignoreGraphCacheWarning && !CacheGenerator.GraphCacheSceneObjectExists() && (CacheGenerator.GetGraphCacheFile() == null || CacheGenerator.GetGraphCacheCoverPointFile() == null)) {
			Log("You haven't generated a navmesh graph cache for your scene. You can generate one by running Ravenfield Tools -> Scan Pathfinding. This will significantly speed up load times!");
		}

		if(!string.IsNullOrEmpty(log)) {
			DisplayWarningLog();
		}

		return ok;
	}

	public static bool CheckUnityEditorVersion() {

		try {
			string version = Application.unityVersion;
			string majorVersion = version.Substring(0, 3);

			if (majorVersion != UNITY_ENGINE_MAJOR_VERSION) {
				Log("You are not running the recommended Unity Editor version "+UNITY_ENGINE_RECOMMENDED_VERSION+". This can lead to unexpected issues with your map/mod. (You are running version "+Application.unityVersion+")");
				return false;
			}
		}
		catch(System.Exception 	e) {

		}

		return true;
	}

	static bool CameraHasSupportedScript(Camera camera) {
		return camera.GetComponent<SceneryCamera>() != null || camera.GetComponent<MinimapCamera>() != null || camera.GetComponent<UnityStandardAssets.ImageEffects.PostEffectsBase>() != null;
	}

	static void ClearLog() {
		log = "";
	}

	static void Log(string text) {
		Debug.Log(text);
		log += text+"\n\n";
	}

	static void DisplayFatalLog() {
		EditorUtility.DisplayDialog("There are issues with your map", log, "Ok");
	}

	static void DisplayWarningLog() {
		EditorUtility.DisplayDialog("Your map is ok, but there are warnings", log, "Ok");
	}
}
