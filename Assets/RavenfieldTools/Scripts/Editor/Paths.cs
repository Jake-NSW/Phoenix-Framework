using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
using System.IO;

public static class Paths {

	public const string EXECUTABLE_PATH_KEY = "executable path";
	const string OSX_EXECUTABLE_RELATIVE_PATH = "Contents/MacOS/ravenfield";
	const string OSX_RELATIVE_DATA_PATH = "Contents";
	const string WIN_LINUX_RELATIVE_DATA_PATH = "ravenfield_Data";
	const string MOD_STAGING_FOLDER = "Mods";
	const string MOD_TOOLS_EXPORT_STAGING_FOLDER = "Ravenfield Tools Export";

	[MenuItem("Ravenfield Tools/Set Game Executable")]
	public static void SetGamePath() {
		string path = Application.dataPath;

		if(HasExecutablePath()) {
			path = ExecutablePath(false);
		}

		path = EditorUtility.OpenFilePanel("Set Game Executable (.exe, .app, .x86, .x86_64)", path, "");

		if(!string.IsNullOrEmpty(path)) {

			string[] pathParts = path.Split('.');

			string extension = pathParts[pathParts.Length-1];

			if(extension != "exe" && extension != "app" && extension != "x86" && extension != "x86_64") {
				EditorUtility.DisplayDialog("Invalid executable path", "Please select the ravenfield .exe, .app, .x86 or .x86_64 file in your game directory.", "ok");
			}
			else {
				PlayerPrefs.SetString(EXECUTABLE_PATH_KEY, path);
			}
		}
	}

	public static bool HasExecutablePath() {
		return PlayerPrefs.HasKey(EXECUTABLE_PATH_KEY) && ExecutablePath(false) != "";
	}

	public static bool ExecutableIsOsxApp() {
		string path = PlayerPrefs.GetString(EXECUTABLE_PATH_KEY);
		return path.Substring(path.Length-4, 4) == ".app";
	}

	public static string ExecutablePath(bool fullPath) {
		string path = PlayerPrefs.GetString(EXECUTABLE_PATH_KEY);

		if(fullPath && ExecutableIsOsxApp()) {
			return path + "/" + OSX_EXECUTABLE_RELATIVE_PATH;
		}

		return path;
	}

	public static string ExecutableDataPath() {
		string executablePath = ExecutablePath(false);

		if(!ExecutableIsOsxApp()) {
			string[] pathElements = executablePath.Split('/');
			string path = "";

			for(int i = 0; i < pathElements.Length-1; i++) {
				path += pathElements[i] + "/";
			}

			return path + WIN_LINUX_RELATIVE_DATA_PATH;
		}
		else {
			return executablePath + "/" + OSX_RELATIVE_DATA_PATH;
		}
	}

	public static string ExecutableModsPath() {
		string modPath = ExecutableDataPath() + "/" + MOD_STAGING_FOLDER;

		if (!Directory.Exists(modPath))	{
			Debug.Log("Creating mod staging folder at " + modPath);
			Directory.CreateDirectory(modPath);
		}

		return modPath;
	}

	public static string ExecutableToolsStagingPath() {

		string modPath = ExecutableModsPath();
		string exportModPath = modPath + "/" + MOD_TOOLS_EXPORT_STAGING_FOLDER;

		if(!Directory.Exists(exportModPath)) {
			Debug.Log("Creating export mod folder at "+exportModPath);
			Directory.CreateDirectory(exportModPath);
		}

		return exportModPath;
	}

	public static string ProjectPath() {
		return Application.dataPath.Substring(0, Application.dataPath.Length-6);
	}

	public static string GraphCachePath(Scene scene) {
		return ProjectPath() + scene.path.Substring(0, scene.path.Length-6)+"_graphcache.bytes";
	}

	public static void LaunchGame(string parameters) {
		Debug.Log("Launching Ravenfield with parameters: "+parameters);

		try {
			System.Diagnostics.Process.Start(Paths.ExecutablePath(true), parameters);
		}
		catch(System.Exception e) {
			EditorUtility.DisplayDialog("Could not launch Ravenfield", "The specified game executable could not be started, did you specify the right executable? Please find your game executable file with Ravenfield Tools -> Set Game Executable\n\nDetails: "+e.Message, "Ok");
			Debug.LogException(e);
		}
	}
}
