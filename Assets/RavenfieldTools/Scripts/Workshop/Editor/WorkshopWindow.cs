using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;
using NUnit.Framework;

public class WorkshopWindow : EditorWindow {

	const int MAX_ICON_FILE_SIZE_BYTES = 1000000000;

	const uint APP_ID = 636480;
	[MenuItem("Ravenfield Tools/Publish to Steam Workshop")]
	public static void PublishToWorkshop() {
		WorkshopWindow window = EditorWindow.GetWindow<WorkshopWindow>();
		window.titleContent.text = "Steam Workshop";
		window.Show();
	}

	public static string ProjectPath() {
		string[] pathElements = Application.dataPath.Split('/');
		string path = "";

		for(int i = 0; i < pathElements.Length-1; i++) {
			path += pathElements[i] + "/";
		}

		return path;
	}

	public static string StagingFolderPath() {
		return ProjectPath() + RelativeStagingFolderPath();
	}

	public string CurrentItemStagingPath() {
		return ProjectPath() + RelativeCurrentItemStagingPath();
	}

	public static string RelativeStagingFolderPath() {
		return "Workshop Staging";
	}

	public string RelativeCurrentItemStagingPath() {
		if(!this.steamworks.isInitialized) {
			throw new System.Exception("Steamworks is not initialized");
		}
		if(!this.steamworks.HasCurrentItem()) {
			throw new System.Exception("Steamworks has no current item");
		}

		return RelativeStagingFolderPath() + "/" + this.steamworks.currentItem.itemId.m_PublishedFileId.ToString();
	}

	public static string ExportPath() {
		return ProjectPath() + "/" + MapExport.PATH;
	}




	int localItemIndex;
	ulong[] localItems;
	string[] localItemStrings;
	string changelog = "";

	SteamworksWrapper steamworks;

	FolderModContent exportedContent;
	FolderModContent stagedContent;

	Texture2D iconTexture;

	Vector2 scroll;

	void FindLocalItems() {

		List<ulong> localItemsList = new List<ulong>();

		string[] subfolders = Directory.GetDirectories(StagingFolderPath());

		foreach(string subfolderPath in subfolders) {
			string subFolderPathFixed = subfolderPath.Replace('\\', '/');
			string[] pathElements = subFolderPathFixed.Split('/');
			string subfolder = pathElements[pathElements.Length-1];

			ulong id;
			if(ulong.TryParse(subfolder, out id)) {
				localItemsList.Add(id);
			}
		}

		localItemsList.Sort(delegate(ulong x, ulong y) {
			return y.CompareTo(x);
		});

		this.localItems = localItemsList.ToArray();
		this.localItemStrings = new string[this.localItems.Length];

		for(int i = 0; i < this.localItems.Length; i++) {
			this.localItemStrings[i] = this.localItems[i].ToString();
		}

		if(HasLocalItems()) {
			QueryLocalItemDetails();

			if(this.steamworks.HasCurrentItem()) {
				SelectCurrentItemInDropdown();
			}
			else {
				this.localItemIndex = 0;
			}
		}
	}

	void QueryLocalItemDetails() {
		Steamworks.PublishedFileId_t[] itemIds = new Steamworks.PublishedFileId_t[this.localItems.Length];

		for(int i = 0; i < this.localItems.Length; i++) {
			itemIds[i] = new Steamworks.PublishedFileId_t(this.localItems[i]);
		}

		this.steamworks.QueryItemInfo(itemIds);
	}

	void SelectCurrentItemInDropdown() {

		this.localItemIndex = 0;

		for(int i = 0; i < this.localItems.Length; i++) {
			if(this.localItems[i] == this.steamworks.currentItem.itemId.m_PublishedFileId) {
				this.localItemIndex = i;
				return;
			}
		}
	}

	bool InitializeSteamworks() {

		if(IsSteamworksInitialized()) {
			return true;
		}

		this.steamworks = new SteamworksWrapper();
		return this.steamworks.Initialize();
	}

	void OnCreateItemDone(bool ok, Steamworks.PublishedFileId_t itemId) {
		if(!ok) {
			string message = this.steamworks.errorMessage;
			if(this.steamworks.lastResult == Steamworks.EResult.k_EResultTimeout) {
				message += "\n\nPlease try again later.";
			}

			EditorUtility.DisplayDialog("Could not create item", message, "Ok");
			return;
		}

		if(this.steamworks.HasCurrentItem()) {
			// Create staging folder for new item.
			if(!Directory.Exists(CurrentItemStagingPath())) {
				Directory.CreateDirectory(CurrentItemStagingPath());
			}

			OpenedItem();
			FindLocalItems();

			this.changelog = "Created a new item.";
		}
	}

	void OnSubmitItemDone(bool ok) {

		EditorUtility.ClearProgressBar();

		if(!ok) {
			string message = this.steamworks.errorMessage;
			if(this.steamworks.lastResult == Steamworks.EResult.k_EResultTimeout) {
				message += "\n\nPlease try again later.";
			}

			EditorUtility.DisplayDialog("Could not submit item", message, "Ok");
			return;
		}
		else {
			EditorUtility.DisplayDialog("Item successfully uploaded!", "Your item was uploaded to Steam Workshop!", "Ok");
			this.steamworks.OpenCommunityFilePage(this.steamworks.currentItem.itemId);
		}
	}

	void OnItemInfoQueryDone(bool ok, Steamworks.SteamUGCDetails_t[] details) {
		if(!ok) {
			Debug.LogWarning("Unable to query item info. "+this.steamworks.errorMessage);
			return;
		}

		foreach(Steamworks.SteamUGCDetails_t detail in details) {

			string tag = detail.m_eResult == Steamworks.EResult.k_EResultOK ? detail.m_rgchTitle : "NOT FOUND ON STEAM";

			for(int i = 0; i < this.localItems.Length; i++) {
				if(this.localItems[i] == detail.m_nPublishedFileId.m_PublishedFileId) {
					this.localItemStrings[i] = this.localItems[i]+": "+tag;
					break;
				}
			}
		}
	}

	bool IsSteamworksInitialized() {
		return this.steamworks != null && this.steamworks.isInitialized;
	}

	void Update() {
		if(this.steamworks != null) {
			this.steamworks.Update();

			if(this.steamworks.IsUploadingItem()) {
				EditorUtility.DisplayProgressBar("Uploading Item", this.steamworks.IsPreparingContentUpload() ? "Preparing Content for Upload" : "Uploading Content", this.steamworks.GetUploadProgress());
			}
		}
	}

	bool ConnectToSteam() {
		bool ok = InitializeSteamworks();

		Debug.Log("Steamworks started? "+ok);

		if(ok) {
			this.steamworks.OnStateChanged = Repaint;
			this.steamworks.OnCreateItemDone = OnCreateItemDone;
			this.steamworks.OnSubmitItemDone = OnSubmitItemDone;
			this.steamworks.OnItemInfoQueryDone = OnItemInfoQueryDone;

			this.exportedContent = new FolderModContent(ExportPath());
			FindLocalItems();
		}

		return ok;
	}

	void LoadSelectedItem() {
		if(!HasLocalItems()) return;

		Debug.Log("Loading item: "+this.localItemStrings[this.localItemIndex]);
		this.steamworks.SetCurrentItemId(this.localItems[this.localItemIndex]);

		OpenedItem();
	}

	bool HasLocalItems() {
		return this.localItems.Length > 0;
	}

	void PublishItem() {
		Debug.Log("Publishing content from staging folder: "+CurrentItemStagingPath());

		if(!Directory.Exists(CurrentItemStagingPath())) {
			EditorUtility.DisplayDialog("Could not publish item", "No item folder found at path "+CurrentItemStagingPath()+". You can create one youself.", "Ok");
			return;
		}

		if(this.stagedContent.IsEmpty()) {
			EditorUtility.DisplayDialog("Could not publish item", "Item folder is empty. Add some content to this item and try again!", "Ok");
			return;
		}

		if(this.stagedContent.HasDisallowedFiles()) {
			string disallowedFiles = "";

			foreach(FileInfo file in this.stagedContent.disallowedFiles) {
				disallowedFiles += "\n- "+file.FullName;
			}

			EditorUtility.DisplayDialog("Could not publish item", "Disallowed files detected. Please remove the following files and click Refresh Content:\n"+disallowedFiles, "Ok");
			return;
		}

		SetupItemTags();

		this.steamworks.SubmitCurrentItem(CurrentItemStagingPath(), this.changelog);
	}

	void SetupItemTags() {

		this.steamworks.currentItem.tags = new List<string>();

		bool hasWeaponContent = false;
		bool hasVehicleContent = false;
        bool hasSkinContent = false;
		bool hasMutatorContent = false;
		List<FileInfo> gameContentFiles = this.stagedContent.GetGameContent();

		foreach(FileInfo gameContentFile in gameContentFiles) {
			try {
				AssetBundle bundle = AssetBundle.LoadFromFile(gameContentFile.FullName);
				GameObject contentObject = bundle.LoadAsset<GameObject>(bundle.GetAllAssetNames()[0]);

				if(!hasWeaponContent) {
					WeaponContentMod weaponContentMod = contentObject.GetComponent<WeaponContentMod>();

					if(weaponContentMod != null && weaponContentMod.weaponEntries.Count > 0) {
						hasWeaponContent = true;
					}
				}

				if(!hasVehicleContent) {
					VehicleContentMod vehicleContentMod = contentObject.GetComponent<VehicleContentMod>();

					if(vehicleContentMod != null) {
						hasVehicleContent = true;
					}
				}

                if(!hasSkinContent) {
                    ActorSkinContentMod skinContentMod = contentObject.GetComponent<ActorSkinContentMod>();

                    if(skinContentMod != null) {
                        hasSkinContent = true;
                    }
                }

				if (!hasMutatorContent) {
					MutatorContentMod mutatorContentMod = contentObject.GetComponent<MutatorContentMod>();

					if (mutatorContentMod != null) {
						hasMutatorContent = true;
					}
				}

				bundle.Unload(true);
			}
			catch(System.Exception) {}

			//if(hasWeaponContent && hasVehicleContent) {
			if(hasWeaponContent) {
				break;
			}
		}

		if(this.stagedContent.HasGameContent()) {
			this.steamworks.currentItem.tags.Add("Modded Content");
		}

		if(hasWeaponContent) {
			this.steamworks.currentItem.tags.Add("Weapons");
		}

		if(hasVehicleContent) {
			this.steamworks.currentItem.tags.Add("Vehicles");
		}

        if (hasSkinContent) {
            this.steamworks.currentItem.tags.Add("Skins");
        }

		if (hasMutatorContent) {
			this.steamworks.currentItem.tags.Add("Mutators");
		}

		if (this.stagedContent.HasGameConfiguration()) {
            this.steamworks.currentItem.tags.Add("Game Configuration");
        }

        if (this.stagedContent.GetMaps().Count > 0) {
			this.steamworks.currentItem.tags.Add("Maps");
		}
	}
		
	void SetupIconTexture() {
		this.iconTexture = new Texture2D(4, 4, TextureFormat.RGB24, false);
		if(this.stagedContent != null && this.stagedContent.HasIconImage()) {
			LoadIconTexture();
		}
	}

	void LoadIconTexture() {
		this.iconTexture.LoadImage(File.ReadAllBytes(this.stagedContent.iconFile.FullName));
	}

	void ChangeIcon() {
		string srcFilePath = EditorUtility.OpenFilePanel("Select Icon", CurrentItemStagingPath(), "png");

		if(string.IsNullOrEmpty(srcFilePath)) {
			return;
		}

		FileInfo file = new FileInfo(srcFilePath);

		if(!file.Exists) {
			EditorUtility.DisplayDialog("Could not load icon", "Icon file was not found", "Ok");
			return;
		}

		if(file.Length > MAX_ICON_FILE_SIZE_BYTES) {
			EditorUtility.DisplayDialog("Could not load icon", "Icon file is too large, the icon file size must be less than 1 MB", "Ok");
			return;
		}

		string dstFilePath = CurrentItemStagingPath()+"/icon.png";
		if(!string.IsNullOrEmpty(srcFilePath) && srcFilePath != dstFilePath) {
			File.Copy(srcFilePath, dstFilePath, true);
		}

		this.steamworks.currentItem.previewImagePath = dstFilePath;

		FindModContent();
		SetupIconTexture();
	}

	void OnGUI() {
		this.scroll = EditorGUILayout.BeginScrollView(this.scroll);
		EditorGUILayout.BeginVertical();

		GUIStyle titleStyle = new GUIStyle(GUI.skin.label);
		titleStyle.fontSize = 14;
		titleStyle.alignment = TextAnchor.LowerCenter;

		EditorGUILayout.Space();
		EditorGUILayout.BeginHorizontal();
		GUILayout.Label("- Steam Workshop Uploader -", titleStyle);
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.Space();

		EditorGUILayout.BeginHorizontal();

		if(GUILayout.Button("Connect to Steam")) {
			ConnectToSteam();
		}

		GUILayout.Label(IsSteamworksInitialized() ? "Connected as "+this.steamworks.Username(): "Not Connected");

		EditorGUILayout.EndHorizontal();

		if(IsSteamworksInitialized()) {

			//EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
			EditorGUILayout.Space();

			//this.selectedItemIndex = EditorGUILayout.Popup(this.selectedItemIndex, this.myItemNames);

			EditorGUILayout.BeginHorizontal();
			GUILayout.Label("Your Local Items:");

			if(GUILayout.Button("Refresh items")) {
				FindLocalItems();
			}
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.BeginHorizontal();

			this.localItemIndex = EditorGUILayout.Popup(this.localItemIndex, this.localItemStrings);
			//EditorGUILayout.IntPopup(localItemIndex, localItemStrings, localIt

			if(GUILayout.Button("Load Item")) {
				LoadSelectedItem();
			}

			EditorGUILayout.EndHorizontal();

			EditorGUILayout.Space();

			if(GUILayout.Button("Create new item")) {
				this.steamworks.CreateWorkshopItem();
			}

		if(this.steamworks.HasCurrentItem()) {

				EditorGUILayout.Space();
				EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
				EditorGUILayout.Space();

				EditorGUILayout.BeginVertical();
				DisplayCurrentItemGui(titleStyle);
				EditorGUILayout.EndVertical();
			}

		}



		EditorGUILayout.EndVertical();
		EditorGUILayout.EndScrollView();
	}

	void DisplayCurrentItemGui(GUIStyle titleStyle) {

		SteamworksWrapper.WorkshopItem item = this.steamworks.currentItem;

		GUILayout.Label("Item ID: "+this.steamworks.currentItem.itemId.m_PublishedFileId, titleStyle);

		EditorGUILayout.BeginHorizontal();

		EditorGUILayout.Space();
		EditorGUILayout.LabelField("", GUILayout.Width(128f), GUILayout.Height(128f));

		if(this.iconTexture == null) {
			SetupIconTexture();
		}
		Rect imageRect = GUILayoutUtility.GetLastRect();
		EditorGUI.DrawPreviewTexture(imageRect, this.iconTexture);

		EditorGUILayout.Space();

		EditorGUILayout.EndHorizontal();

		Rect buttonRect = imageRect;
		buttonRect.position = new Vector2(buttonRect.position.x, buttonRect.position.y+buttonRect.height);
		buttonRect.height = 20f;
		if(GUI.Button(buttonRect, "Change Image")) {
			ChangeIcon();
		}

		EditorGUILayout.Space();
		EditorGUILayout.Space();
		EditorGUILayout.Space();
		EditorGUILayout.Space();
		EditorGUILayout.Space();

		EditorGUILayout.LabelField("Leave fields blank for no change");
		item.title = EditorGUILayout.TextField("New Title", item.title);
		item.description = EditorGUILayout.TextField("New Description", item.description);

		EditorGUILayout.Space();

		DisplayCurrentItemContent();

		EditorGUILayout.Space();

		if(GUILayout.Button("Refresh & Update Content")) {
			FindModContent();
		}

		EditorGUILayout.Space();
		EditorGUILayout.Space();

		this.changelog = EditorGUILayout.TextField("Changelog", changelog);

		bool itemPublic = EditorGUILayout.Toggle("Make Public", item.visibility == SteamworksWrapper.WorkshopItem.Visibility.Public);
		item.visibility = itemPublic ? SteamworksWrapper.WorkshopItem.Visibility.Public : SteamworksWrapper.WorkshopItem.Visibility.NoChange;

		EditorGUILayout.Space();

		EditorStyles.label.wordWrap = true;

		EditorGUILayout.Space();

		EditorGUILayout.LabelField("By publishing this workshop item, you agree to the workshop terms of service.");
		if (GUILayout.Button("View workshop terms of service")) {
			Application.OpenURL(SteamworksWrapper.WORKSHOP_TERMS_OF_SERVICE_URL);
		}

		EditorGUILayout.Space();

		EditorGUILayout.LabelField("By publishing this workshop item, you warrant that you are the original creator of all item content, or that you have the right to submit the content on behalf of their original creator(s).");

		if (GUILayout.Button("Publish!")) {
			PublishItem();
		}
	}

	void DisplayCurrentItemContent() {

		EditorGUILayout.BeginHorizontal();

		DisplayModContent(this.exportedContent, "All Exported Content", StageExportedContent);
		DisplayModContent(this.stagedContent, "Content to Publish", RemoveStagedContent);

		EditorGUILayout.EndHorizontal();
	}

	void StageExportedContent(FileInfo file) {

		Debug.Log("Staging file: "+file);

		if(!this.stagedContent.ContainsFileOfName(file.Name)) {
			file.CopyTo(CurrentItemStagingPath()+"/"+file.Name);
		}

		FindModContent();
		Repaint();
	}

	void RemoveStagedContent(FileInfo file) {

		Debug.Log("Removing staged file: "+file);

		if(!this.exportedContent.ContainsFileOfName(file.Name)) {
			Debug.Log("File not present in export folder, making a backup.");
			file.CopyTo(ExportPath()+"/"+file.Name);
		}

		file.Delete();

		FindModContent();
		Repaint();
	}

	delegate void DelContentClicked(FileInfo file);
	void DisplayModContent(FolderModContent content, string title, DelContentClicked OnContentClicked) {

		EditorGUILayout.BeginVertical();

		EditorGUILayout.LabelField(title);

		content.scroll = EditorGUILayout.BeginScrollView(content.scroll, false, false, GUILayout.Height(200));

		EditorGUILayout.BeginVertical(GUI.skin.textField);

		if(content.IsEmpty()) {
			GUILayout.Label("- Empty -");
		}
		else {
			foreach(FileInfo file in content.allContent) {
				string label = file.Name;

				if(content.IsMarkedAsUpdated(file)) {
					label = "UPDATED: "+file.Name;
				}

				if(GUILayout.Button(label, GUI.skin.label)) {
					OnContentClicked(file);
				}
			}
		}

		EditorGUILayout.EndVertical();

		EditorGUILayout.EndScrollView();

		EditorGUILayout.EndVertical();
	}

	void OpenedItem() {
		this.changelog = "";
		this.stagedContent = new FolderModContent(CurrentItemStagingPath());

		FindModContent();
	}

	public void FindModContent() {
		this.exportedContent.Scan();
		this.stagedContent.Scan();

		foreach(FileInfo file in this.stagedContent.allContent.ToArray()) {
			FileInfo newerFile;
			if(this.exportedContent.HasNewerVersionOfFile(file, out newerFile)) {
				Debug.Log("Updating staged file: "+file.Name);
				FileInfo replacementFile = newerFile.CopyTo(file.FullName, true);

				this.stagedContent.MarkAsUpdated(file, replacementFile);
			}
		}

		SetupIconTexture();
	}

	void OnDisable() {
		if(IsSteamworksInitialized()) {
			Debug.Log("Shutting down steamworks");
			this.steamworks.Shutdown();
		}

		Repaint();
	}

	void OnEnable() {
		SetupIconTexture();
		Repaint();
	}
}
