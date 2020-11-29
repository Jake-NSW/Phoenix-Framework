using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;
using UnityEditor;

public class MapExport {

	public const string PATH = "Assets/Export";

	[MenuItem("Ravenfield Tools/Map/Export Open Scene as Map")]
	public static void ExportMap() {

		if(!Paths.HasExecutablePath()) {
			EditorUtility.DisplayDialog("Could not export map", "No executable set. Please find your game executable file with Ravenfield Tools -> Set Game Executable", "Ok");
			return;
		}

		string path;
		bool ok = BuildBundle(BuildTargetSelection.buildTarget, out path, false);

		if(ok) {
			EditorUtility.DisplayDialog("Export completed", "Map was successfully exported to "+path, "Ok");
		}
		else {
			EditorUtility.DisplayDialog("Export failed", "Map couldn't export properly. Please see the console for error messages.", "Ok");
		}
	}

	public static bool BuildBundle(BuildTarget buildTarget, out string filepath, bool ignoreGraphCacheWarning) {

		filepath = "";

		Debug.ClearDeveloperConsole();
		Debug.Log("\n\n");
        Debug.Log("\n--- Building Map, Build Target: " + buildTarget + " ---");

        AssetBundleBuild build = new AssetBundleBuild();

		if(!AssetDatabase.IsValidFolder(PATH)) {
			Debug.Log("No Export folder found, creating one.");
			AssetDatabase.CreateFolder("Assets", "Export");
		}

		try {
			EditorSceneManager.SaveOpenScenes();

			if(!SanityCheck.DoSanityCheck(ignoreGraphCacheWarning)) {
				Debug.Log("Sanity check failed, aborting export!");
				return false;
			}



			//Bundle graphcache if available.
			TextAsset graphCacheAsset = CacheGenerator.GetGraphCacheFile();
			TextAsset graphCacheCoverPointAsset = CacheGenerator.GetGraphCacheCoverPointFile();

			if(graphCacheAsset != null && graphCacheCoverPointAsset != null) {
				Debug.Log("Graphcache was found, bundling!");
				SetupGraphCache(graphCacheAsset, graphCacheCoverPointAsset);
				EditorSceneManager.SaveOpenScenes();
			}
			else {
				Debug.Log("WARNING: Couldn't find graphcache.");
			}

			Scene currentScene = EditorSceneManager.GetSceneAt(0);

			build.assetBundleName = currentScene.name+".rfl";

			List<string> assetNames = new List<string>();

			assetNames.Add(currentScene.path);


			build.assetNames = assetNames.ToArray();

			AssetBundleBuild[] buildMap = new AssetBundleBuild[] {build};

			BuildPipeline.BuildAssetBundles(PATH, buildMap, BuildAssetBundleOptions.None, buildTarget);
		}
		catch(System.Exception e) {
			EditorUtility.DisplayDialog("Could not export map", "Could not create the .rfl file.\n\nDetails: "+e.Message, "Ok");
			Debug.LogException(e);
			return false;
		}

		Debug.Log("Successfully exported "+build.assetBundleName);

		if(Paths.HasExecutablePath()) {
			string exportPath = Paths.ProjectPath() + PATH + "/" + build.assetBundleName;

			string levelDestinationPath = Paths.ExecutableToolsStagingPath() + "/" + build.assetBundleName;

			try {
				File.Copy(exportPath, levelDestinationPath, true);
			}
			catch(System.Exception e) {
				EditorUtility.DisplayDialog("Could not export map", "Could not copy map to the mod staging folder.\n\nDetails: "+e.Message, "Ok");
				Debug.LogException(e);
				return false;
			}

			Debug.Log("Copied exported map to "+levelDestinationPath);
			filepath = levelDestinationPath;
			return true;
		}

		return false;
	}

	/*[MenuItem("Ravenfield Tools/Debug Path")]
	static void DebugPath() {
		Debug.Log("Executable: "+Paths.ExecutablePath(true));
		Debug.Log("Data Path: "+Paths.ExecutableDataPath());
		Debug.Log("Staging: "+Paths.ExecutableToolsStagingPath());
		Debug.Log("Saved Game Path: "+PlayerPrefs.GetString("executable path"));
	}*/

	static void SetupGraphCache(TextAsset asset, TextAsset coverPointAsset) {
		CustomGraphCache cacheComponent = GameObject.FindObjectOfType<CustomGraphCache>();
		if(cacheComponent == null) {
			GameObject graphCacheObject = new GameObject("Graph Cache (Automatically Generated)");
			cacheComponent = graphCacheObject.AddComponent<CustomGraphCache>();
		}

		cacheComponent.cache = asset;
		cacheComponent.cacheCoverPoints = coverPointAsset;
	}
}
