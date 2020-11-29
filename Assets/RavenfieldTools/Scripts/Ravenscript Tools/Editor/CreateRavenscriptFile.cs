using UnityEngine;
using UnityEditor;
using System.IO;

[CreateAssetMenu(fileName = "NewRavenScript.txt", menuName = "Ravenscript", order = 50)]
public class CreateRavenscriptFile : ScriptableObject, ISerializationCallbackReceiver {

	const string NEWLINE = "\n";
	const string INDENT = "\t";
	const string DEFAULT_CONTENT =
		"-- Register the behaviour" + NEWLINE +
		"behaviour(\"{0}\")" + NEWLINE +
		NEWLINE +
		"function {0}:Start()" + NEWLINE +
		INDENT + "-- Run when behaviour is created" + NEWLINE +
		INDENT + "print(\"Hello World\")" + NEWLINE +
		"end" + NEWLINE +
		NEWLINE +
		"function {0}:Update()" + NEWLINE +
		INDENT + "-- Run every frame" + NEWLINE +
		INDENT + NEWLINE +
		"end" +
		NEWLINE;

	public void OnBeforeSerialize() {
		var path = AssetDatabase.GetAssetPath(this);
		if(!string.IsNullOrEmpty(path) && File.Exists(path)) {
			//Debug.Log("Initializing file " + path);
			try {
				File.WriteAllText(path, GenerateContent(path), System.Text.Encoding.UTF8);
			}
			catch(System.Exception e) {
				Debug.LogException(e);
			}
			DestroyImmediate(this, true);
		}
	}

	public string GenerateContent(string filePath) {
		var fileInfo = new FileInfo(filePath);
		string behaviourName = fileInfo.Name.Substring(0, fileInfo.Name.Length - 4);
		return string.Format(DEFAULT_CONTENT, behaviourName);
	}

	public void OnAfterDeserialize() {
		
	}
}
