using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class FolderModContent {

	public static readonly string[] CONTENT_EXTENSIONS = {".rfl", ".rfc", ".rgc"};
	public static readonly string[] ALLOWED_EXTENSIONS = {".txt", ".png", ".json"};

	string path;

	public List<FileInfo> allContent;
	public List<FileInfo> disallowedFiles;
	public FileInfo iconFile;

	public Dictionary<FileInfo, bool> updated;

	public Vector2 scroll;

	public FolderModContent(string path) {
		this.path = path;
		this.scroll = Vector2.zero;

		Scan();
	}

	public bool HasIconImage() {
		return this.iconFile != null;
	}

	void GetFilesInDirectoryRecursively(string path, List<string> files) {
		files.AddRange(Directory.GetFiles(path));

		string[] subdirectories = Directory.GetDirectories(path);

		foreach(string subdirectory in subdirectories) {
			GetFilesInDirectoryRecursively(subdirectory, files);
		}
	}

	public void Scan() {
		List<string> files = new List<string>();
		GetFilesInDirectoryRecursively(this.path, files);
		//string[] files = Directory.GetFiles(this.path);

		this.updated = new Dictionary<FileInfo, bool>();

		this.allContent = new List<FileInfo>();
		this.disallowedFiles = new List<FileInfo>();

		foreach(string filePath in files) {
			FileInfo file = new FileInfo(filePath);

			if(IsContentExtension(file.Extension)) {
				this.allContent.Add(file);
			}
			else if(IsIconFile(file)) {
				this.iconFile = file;
			}
			else if(!IsAllowedExtension(file.Extension)) {
				this.disallowedFiles.Add(file);
			}

		}

		this.allContent.Sort(SortByName);

		foreach(FileInfo file in this.allContent) {
			this.updated.Add(file, false);
		}

		this.scroll = Vector2.zero;
	}

	bool IsIconFile(FileInfo file) {
		return file.Name == "icon.png";
	}

	public bool IsContentExtension(string extension) {
		extension = extension.ToLower();
		for(int i = 0; i < CONTENT_EXTENSIONS.Length; i++) {
			if(extension == CONTENT_EXTENSIONS[i]) return true;
		}
		return false;
	}

	public bool IsAllowedExtension(string extension) {
		for(int i = 0; i < ALLOWED_EXTENSIONS.Length; i++) {
			if(extension == ALLOWED_EXTENSIONS[i]) return true;
		}
		return false;
	}


	public bool ContainsFileOfName(string name) {
		foreach(FileInfo file in this.allContent) {
			if(file.Name == name) return true;
		}
		return false;
	}

	public bool IsEmpty() {
		return this.allContent.Count == 0;
	}

	public void MarkAsUpdated(FileInfo file, FileInfo newFile) {
		int index = this.allContent.IndexOf(file);
		this.allContent[index] = newFile;

		this.updated[newFile] = true;
	}

	public bool IsMarkedAsUpdated(FileInfo file) {
		return this.updated.ContainsKey(file) && this.updated[file];
	}

	public bool HasAnyContent() {
		return this.allContent.Count > 0;
	}

	public bool HasDisallowedFiles() {
		return this.disallowedFiles.Count > 0;
	}

	public bool HasNewerVersionOfFile(FileInfo file, out FileInfo newerFile) {

		newerFile = null;

		foreach(FileInfo myFile in this.allContent) {
			if(myFile.Name == file.Name) {
				if(myFile.LastWriteTimeUtc > file.LastWriteTimeUtc) {
					newerFile = myFile;
					return true;
				}
				else {
					return false;
				}
			}
		}

		return false;
	}

	bool IsMap(FileInfo file) {
		return file.Extension.ToLower() == ".rfl";
	}

	bool IsGameContent(FileInfo file) {
		return file.Extension.ToLower() == ".rfc";
	}

    bool IsGameConfiguration(FileInfo file) {
        return file.Extension.ToLower() == ".rgc";
    }

    public List<FileInfo> GetMaps() {
		List<FileInfo> maps = new List<FileInfo>();

		foreach(FileInfo content in this.allContent) {
			if(IsMap(content)) {
				maps.Add(content);
			}
		}

		return maps;
	}

	public List<FileInfo> GetGameContent() {

		List<FileInfo> contentList = new List<FileInfo>();

		foreach(FileInfo content in this.allContent) {
			if(IsGameContent(content)) {
				contentList.Add(content);
			}
		}

		return contentList;
	}

    public List<FileInfo> GetGameConfiguration() {

        List<FileInfo> contentList = new List<FileInfo>();

        foreach (FileInfo content in this.allContent) {
            if (IsGameConfiguration(content)) {
                contentList.Add(content);
            }
        }

        return contentList;
    }

    public bool HasGameContent() {
		return GetGameContent().Count > 0;
	}

    public bool HasGameConfiguration() {
        return GetGameConfiguration().Count > 0;
    }

	int SortByName(FileInfo x, FileInfo y) {
		return x.Name.CompareTo(y.Name);
	}


}
