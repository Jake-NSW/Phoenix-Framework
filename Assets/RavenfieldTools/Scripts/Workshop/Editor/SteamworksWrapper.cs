using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;

public class SteamworksWrapper {

	public const string WORKSHOP_TERMS_OF_SERVICE_URL = "http://steamcommunity.com/sharedfiles/workshoplegalagreement";
	public const string COMMUNITY_FILE_PAGE_URL = "steam://url/CommunityFilePage/";

	public bool isInitialized = false;

	protected CallResult<CreateItemResult_t> createItemResult;
	protected CallResult<SubmitItemUpdateResult_t> submitItemResult;
	protected CallResult<SteamUGCQueryCompleted_t> steamUGCQueryCompleted;
	protected Callback<ItemInstalled_t> itemInstalled;
	protected Callback<RemoteStoragePublishedFileSubscribed_t> remotePublishedFileSubscribed;
	protected Callback<RemoteStoragePublishedFileUnsubscribed_t> remotePublishedFileUnsubscribed;

	public delegate void DelOnStateChanged();
	public delegate void DelOnCreateItemDone(bool ok, PublishedFileId_t itemId);
	public delegate void DelOnSubmitItemDone(bool ok);
	public delegate void DelOnItemInfoQueryDone(bool ok, SteamUGCDetails_t[] details);
	public delegate void DelOnItemInstalled(PublishedFileId_t itemId);
	public delegate void DelOnRemotePublishedFileSubscribed(PublishedFileId_t itemId);
	public delegate void DelOnRemotePublishedFileUnsubscribed(PublishedFileId_t itemId);

	public DelOnStateChanged OnStateChanged;
	public DelOnCreateItemDone OnCreateItemDone;
	public DelOnSubmitItemDone OnSubmitItemDone;
	public DelOnItemInfoQueryDone OnItemInfoQueryDone;
	public DelOnItemInstalled OnItemInstalled;
	public DelOnRemotePublishedFileSubscribed OnRemotePublishedFileSubscribed;
	public DelOnRemotePublishedFileUnsubscribed OnRemotePublishedFileUnsubscribed;

	public bool needsToAcceptWorkshopLegalAgreement = false;

	public WorkshopItem currentItem;

	public EResult lastResult;
	public string errorMessage;

	public PublishedFileId_t[] subscribedItems;

	bool isUpdatingItem = false;
	UGCUpdateHandle_t lastUpdateHandle;

	public SteamworksWrapper() {
		
	}

	public bool Initialize() {
		this.isInitialized = SteamAPI.Init();

		if(this.isInitialized) {
			RegisterCallbacks();
		}

		SignalStateChanged();

		return this.isInitialized;
	}

	public void RegisterCallbacks() {
		Debug.Log("Registering callbacks");
		this.createItemResult = CallResult<CreateItemResult_t>.Create(OnCreateItemResult);
		this.submitItemResult = CallResult<SubmitItemUpdateResult_t>.Create(OnSubmitItemResult);
		this.steamUGCQueryCompleted = CallResult<SteamUGCQueryCompleted_t>.Create(OnSteamUGCQueryCompleted);
		this.itemInstalled = Callback<ItemInstalled_t>.Create(OnItemInstalledInt);
		this.remotePublishedFileSubscribed = Callback<RemoteStoragePublishedFileSubscribed_t>.Create(OnRemotePublishedFileSubscribedInt);
		this.remotePublishedFileUnsubscribed = Callback<RemoteStoragePublishedFileUnsubscribed_t>.Create(OnRemotePublishedFileUnsubscribedInt);
	}

	void SignalStateChanged() {
		if(OnStateChanged != null) {
			OnStateChanged();
		}
	}

	public bool HasCurrentItem() {
		return this.currentItem != null;
	}

	public void Shutdown() {
		SteamAPI.Shutdown();

		this.isInitialized = false;

		SignalStateChanged();
	}

	void CheckInitialized() {
		if(!this.isInitialized) {
			throw new System.Exception("SteamworksWrapper is not initialized.");
		}
	}

	public void Update() {
		if(this.isInitialized) {
			SteamAPI.RunCallbacks();
		}
	}

	public string Username() {
		CheckInitialized();

		return SteamFriends.GetPersonaName();
	}

	// Callbacks

	void OnItemInstalledInt(ItemInstalled_t p) {
		if(p.m_unAppID == SteamUtils.GetAppID()) {
			if(OnItemInstalled != null) {
				OnItemInstalled(p.m_nPublishedFileId);
			}
		}

		SignalStateChanged();
	}

	void OnRemotePublishedFileSubscribedInt(RemoteStoragePublishedFileSubscribed_t p) {
		if(p.m_nAppID == SteamUtils.GetAppID()) {
			if(OnRemotePublishedFileSubscribed != null) {
				OnRemotePublishedFileSubscribed(p.m_nPublishedFileId);
			}
		}

		SignalStateChanged();
	}

	void OnRemotePublishedFileUnsubscribedInt(RemoteStoragePublishedFileUnsubscribed_t p) {
		if(p.m_nAppID == SteamUtils.GetAppID()) {
			if(OnRemotePublishedFileUnsubscribed != null) {
				OnRemotePublishedFileUnsubscribed(p.m_nPublishedFileId);
			}
		}

		SignalStateChanged();
	}




	// Create workshop item

	public void CreateWorkshopItem() {
		CheckInitialized();

		Debug.Log("Creating item");
		SteamAPICall_t callback = SteamUGC.CreateItem(SteamUtils.GetAppID(), EWorkshopFileType.k_EWorkshopFileTypeCommunity);
		this.createItemResult.Set(callback);
	}

	void OnCreateItemResult(CreateItemResult_t p, bool ioFailure) {

		bool ok = true;
		this.lastResult = p.m_eResult;

		if(ioFailure) {
			SetError("Create Item failed, IO Failure!");
			ok = false;
		}
		else if(p.m_eResult != EResult.k_EResultOK) {
			SetError("Create Item failed, error: "+p.m_eResult.ToString());
			ok = false;
		}
		else {
			
			this.needsToAcceptWorkshopLegalAgreement = p.m_bUserNeedsToAcceptWorkshopLegalAgreement;

			this.currentItem = new WorkshopItem(p.m_nPublishedFileId);
			this.currentItem.title = "My new item";
		}

		if(OnCreateItemDone != null) {
			OnCreateItemDone(ok, p.m_nPublishedFileId);
		}

		SignalStateChanged();
	}




	// Set workshop item

	public void SetCurrentItemId(ulong id) {
		SetCurrentItem(new WorkshopItem(new PublishedFileId_t(id)));
	}

	public void SetCurrentItem(WorkshopItem item) {
		CheckInitialized();

		this.currentItem = item;

		SignalStateChanged();
	}


	// Submit current workshop item

	public void SubmitCurrentItem(string contentPath, string changeNote) {
		CheckInitialized();

		if(!HasCurrentItem()) {
			throw new System.Exception("Unable to submit item, there is no current item");
		}

		Debug.Log("Submitting item #"+this.currentItem.itemId.m_PublishedFileId+" from path "+contentPath);

		UGCUpdateHandle_t handle = SteamUGC.StartItemUpdate(SteamUtils.GetAppID(), this.currentItem.itemId);

		if(!string.IsNullOrEmpty(this.currentItem.title)) {
			SteamUGC.SetItemTitle(handle, this.currentItem.title);
		}

		if(!string.IsNullOrEmpty(this.currentItem.description)) {
			SteamUGC.SetItemDescription(handle, this.currentItem.description);
		}

		if(!string.IsNullOrEmpty(this.currentItem.previewImagePath)) {
			SteamUGC.SetItemPreview(handle, this.currentItem.previewImagePath);
		}

		if(this.currentItem.tags != null && this.currentItem.tags.Count > 0) {
			SteamUGC.SetItemTags(handle, this.currentItem.tags);
		}

		if(this.currentItem.visibility != WorkshopItem.Visibility.NoChange) {
			ERemoteStoragePublishedFileVisibility visibility;

			if(this.currentItem.visibility == WorkshopItem.Visibility.Private) {
				visibility = ERemoteStoragePublishedFileVisibility.k_ERemoteStoragePublishedFileVisibilityPrivate;
			}
			else if(this.currentItem.visibility == WorkshopItem.Visibility.FriendsOnly) {
				visibility = ERemoteStoragePublishedFileVisibility.k_ERemoteStoragePublishedFileVisibilityFriendsOnly;
			}
			else {
				visibility = ERemoteStoragePublishedFileVisibility.k_ERemoteStoragePublishedFileVisibilityPublic;
			}

			SteamUGC.SetItemVisibility(handle, visibility);
		}

		SteamUGC.SetItemContent(handle, contentPath);

		SteamAPICall_t callback = SteamUGC.SubmitItemUpdate(handle, changeNote);
		this.submitItemResult.Set(callback);

		this.isUpdatingItem = true;
		this.lastUpdateHandle = handle;
	}

	public bool IsUploadingItem() {
		return this.isUpdatingItem;
	}

	public bool IsPreparingContentUpload() {
		ulong processed, total;
		EItemUpdateStatus status = SteamUGC.GetItemUpdateProgress(this.lastUpdateHandle, out processed, out total);

		return status == EItemUpdateStatus.k_EItemUpdateStatusPreparingContent || status == EItemUpdateStatus.k_EItemUpdateStatusPreparingConfig;
	}

	public float GetUploadProgress() {
		ulong processed, total;

		//EItemUpdateStatus status = SteamUGC.GetItemUpdateProgress(this.lastUpdateHandle, out processed, out total);
		SteamUGC.GetItemUpdateProgress(this.lastUpdateHandle, out processed, out total);

		//Debug.Log("Processed: "+processed + " out of "+total+", status: "+status);

		if(total == 0) {
			return 0f;
		}
		else {
			return Mathf.Clamp01(((float) (processed/1000))/((float) (total/1000)));
		}
	}

	void OnSubmitItemResult(SubmitItemUpdateResult_t p, bool ioFailure) {

		bool ok = true;
		this.lastResult = p.m_eResult;

		this.isUpdatingItem = false;

		if(ioFailure) {
			SetError("Submit Item failed, IO Failure!");
			ok = false;
		}
		else if(p.m_eResult != EResult.k_EResultOK) {
			SetError("Submit Item failed, error: "+p.m_eResult.ToString());
			ok = false;
		}
		else {
			this.needsToAcceptWorkshopLegalAgreement = p.m_bUserNeedsToAcceptWorkshopLegalAgreement;
			Debug.Log("Item was successfully uploaded!");
		}

		if(OnSubmitItemDone != null) {
			OnSubmitItemDone(ok);
		}

		SignalStateChanged();
	}



	// Query Items

	public void QueryItemInfo(PublishedFileId_t[] fileIds) {
		CheckInitialized();

		UGCQueryHandle_t handle = SteamUGC.CreateQueryUGCDetailsRequest(fileIds, (uint) fileIds.Length);

		SteamAPICall_t callback = SteamUGC.SendQueryUGCRequest(handle);
		this.steamUGCQueryCompleted.Set(callback);
	}

	void OnSteamUGCQueryCompleted(SteamUGCQueryCompleted_t p, bool ioFailure) {
		bool ok = true;
		this.lastResult = p.m_eResult;
		SteamUGCDetails_t[] details = null;

		if(ioFailure) {
			SetError("Item query failed, IO Failure!");
			ok = false;
		}
		else if(p.m_eResult != EResult.k_EResultOK) {
			SetError("Item query failed, error: "+p.m_eResult.ToString());
			ok = false;
		}
		else {
			details = new SteamUGCDetails_t[p.m_unNumResultsReturned];

			for(uint i = 0; i < p.m_unNumResultsReturned; i++) {
				SteamUGCDetails_t detail;
				SteamUGC.GetQueryUGCResult(p.m_handle, i, out detail);
				details[i] = detail;
			}
		}

		if(OnItemInfoQueryDone != null) {
			OnItemInfoQueryDone(ok, details);
		}

		SignalStateChanged();
	}





	public uint FetchSubscribedItems() {
		CheckInitialized();

		uint numSubscribedItems = SteamUGC.GetNumSubscribedItems();

		this.subscribedItems = new PublishedFileId_t[numSubscribedItems];
		uint count = SteamUGC.GetSubscribedItems(subscribedItems, numSubscribedItems);

		return count;
	}

	public bool HasFetchedSubscribedItems() {
		CheckInitialized();

		return this.subscribedItems != null;
	}

	public bool AllSubscribedItemsAreInstalled() {
		CheckInitialized();

		if(!HasFetchedSubscribedItems()) {
			throw new System.Exception("No subscribed items list has been fetched");
		}

		foreach(PublishedFileId_t itemId in this.subscribedItems) {
			if(!IsSubscribedItemInstalled(itemId)) {
				return false;
			}
		}

		return true;
	}

	public bool IsSubscribedItemInstalled(PublishedFileId_t itemId) {
		return (SteamUGC.GetItemState(itemId) & (uint) EItemState.k_EItemStateInstalled) != 0;
	}

	public string[] GetSubscribedItemPaths() {
		CheckInitialized();

		if(!HasFetchedSubscribedItems()) {
			throw new System.Exception("No subscribed items list has been fetched");
		}

		string[] paths = new string[this.subscribedItems.Length];

		ulong size;
		uint timestamp;
		string folderPath;

		for(int i = 0; i < this.subscribedItems.Length; i++) {
			SteamUGC.GetItemInstallInfo(this.subscribedItems[i], out size, out folderPath, 0, out timestamp);
			paths[i] = folderPath;
		}

		return paths;
	}

	public string GetSubscribedItemPath(PublishedFileId_t id) {
		ulong size;
		uint timestamp;
		string folderPath;

		// No idea what this is, the facepunch implementation uses 4096 here so I guess we'll do it too!
		uint cchFolderSize = 4096;

		SteamUGC.GetItemInstallInfo(id, out size, out folderPath, cchFolderSize, out timestamp);

		return folderPath;
	}


	public void DropCurrentItem() {
		this.currentItem = null;
	}

	void SetError(string error) {
		Debug.LogError(error);
		this.errorMessage = error;
	}

	public void OpenCommunityFilePage(PublishedFileId_t itemId) {
		OpenUrl(COMMUNITY_FILE_PAGE_URL+itemId.m_PublishedFileId.ToString(), false);
	}

	public void OpenUrl(string url, bool inSteamOverlay = true) {
		if(inSteamOverlay) {
			CheckInitialized();
			SteamFriends.ActivateGameOverlayToWebPage(url);
		}
		else {
			Application.OpenURL(url);
		}
	}

	public class WorkshopItem {

		public enum Visibility {NoChange, Private, FriendsOnly, Public};

		public PublishedFileId_t itemId;
		public string title;
		public string description;
		public string previewImagePath;
		public List<string> tags;
		public Visibility visibility;

		public WorkshopItem(PublishedFileId_t itemId) {
			this.itemId = itemId;
			this.visibility = Visibility.NoChange;
		}

		public override string ToString ()
		{
			return "WokshopItem #"+this.itemId.m_PublishedFileId+": "+
				this.title+", "+
				this.description+", ";
		}
	}
}
