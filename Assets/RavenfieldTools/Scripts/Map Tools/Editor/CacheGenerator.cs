using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
using UnityEditor.SceneManagement;

public class CacheGenerator {

	[MenuItem("Ravenfield Tools/Map/Scan Pathfinding")]
	public static void ScanPathfinding() {

		if(!Paths.HasExecutablePath()) {
			EditorUtility.DisplayDialog("Could not export map", "No executable set. Please find your game executable file with Ravenfield Tools -> Set Game Executable", "Ok");
			return;
		}

		// Remove existing graph cache files because some users have been unable to update the existing files.
		// Also this guarantees that if we run scan pathfinding, either we will get an up to date cache or none at all.
		RemoveGraphCache();

		string filepath;
		bool ok = MapExport.BuildBundle(BuildTargetSelection.buildTarget, out filepath, true);

		if(!ok) {
			Debug.Log("Cancelling Scan Pathfinding as map export failed.");
			return;
		}

		Debug.Log("Export completed, launching pathfinding generator!");

		string cacheWritebackPath = Paths.GraphCachePath(EditorSceneManager.GetSceneAt(0));
		string parameters = "-nointro \"-generatenavcache "+cacheWritebackPath+"\" \"-custommap "+filepath+"\"";

		Paths.LaunchGame(parameters);
	}

	static void RemoveGraphCache() {
		CustomGraphCache cache = GetGraphCacheSceneObject();

		if(cache != null) {
			GameObject.DestroyImmediate(cache.gameObject);
		}

		AssetDatabase.DeleteAsset(GetGraphCachePath());
		AssetDatabase.DeleteAsset(GetGraphCacheCoverPointPath());
	}

	public static CustomGraphCache GetGraphCacheSceneObject() {
		return GameObject.FindObjectOfType<CustomGraphCache>();
	}

	public static bool GraphCacheSceneObjectExists() {
		return GetGraphCacheSceneObject() != null;
	}

	public static TextAsset GetGraphCacheFile() {
		TextAsset graphCacheAsset = AssetDatabase.LoadAssetAtPath<TextAsset>(GetGraphCachePath());
		return graphCacheAsset;
	}

	public static TextAsset GetGraphCacheCoverPointFile() {
		TextAsset graphCacheAsset = AssetDatabase.LoadAssetAtPath<TextAsset>(GetGraphCacheCoverPointPath());
		return graphCacheAsset;
	}

	public static string GetGraphCachePath() {
		Scene currentScene = EditorSceneManager.GetSceneAt(0);
		return currentScene.path.Substring(0, currentScene.path.Length-6)+"_graphcache.bytes";
	}

	public static string GetGraphCacheCoverPointPath() {
		Scene currentScene = EditorSceneManager.GetSceneAt(0);
		return currentScene.path.Substring(0, currentScene.path.Length-6)+"_graphcache_coverpoints.bytes";
	}
}
