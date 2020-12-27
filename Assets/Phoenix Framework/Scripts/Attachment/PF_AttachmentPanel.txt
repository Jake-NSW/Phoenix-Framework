-- Register the behaviour
behaviour("PF_AttachmentPanel")

local AttachmentPanel = {
	StoreComponenets = function (self)
		self.nilText = self.targets.nilText.gameObject.GetComponent(TextMeshProUGUI)
		self.headerText = self.targets.headerText.gameObject.GetComponent(TextMeshProUGUI)
		self.statsHeaderText = self.targets.statsHeaderText.gameObject.GetComponent(TextMeshProUGUI)
		self.statsDescText = self.targets.statsDescText.gameObject.GetComponent(TextMeshProUGUI)

		self.panel = self.targets.panel.gameObject
		self.buttonsNest = self.targets.buttonsNest.gameObject
		
		self.attachment = self.targets.attachment

		for i = 1, 20 do
			local go = GameObject.Instantiate(self.attachment)
			go.transform.SetParent(self.buttonsNest.transform, false)
			go.GetComponent(Button).onClick.AddListener(self, "EquipAttachment", i)
		end
	end,
}

function PF_AttachmentPanel:Initialise()
	m_AttachmentPanel = self

	AttachmentPanel.StoreComponenets(self)
	PhoenixDebug.Print("PF_AttachmentPanel | Initialise", "log")
end

function PF_AttachmentPanel:OnWeaponChange()
	self.currentPoint = nil
	self.weaponPoint = nil
	self.headerText.text = "... Attachments"
	self.panel.SetActive(false)
	self.nilText.gameObject.SetActive(true)
end

function PF_AttachmentPanel:RefreshAttachmentPanel(attachmentPoint, phoenixAttachmentPoint)
	local data = attachmentPoint.gameObject.GetComponent(DataContainer)
	self.weaponPoint = attachmentPoint
	self.currentPoint = phoenixAttachmentPoint

	for i = 1, 20 do
		ob = self.buttonsNest.transform.GetChild(i -1).gameObject
		ob.SetActive(false)
		ob.GetComponent(Button).interactable = true
	end

	for i, v in pairs(data.GetGameObjectArray("attachment")) do
		local data = self.buttonsNest.transform.GetChild(i -1).gameObject.GetComponent(DataContainer)
		data.gameObject.SetActive(true)

		data.GetGameObject("text").gameObject.GetComponent(TextMeshProUGUI).text = v.gameObject.GetComponent(DataContainer).GetString("attachmentName")
		data.GetGameObject("icon").gameObject.GetComponent(Image).sprite = v.gameObject.GetComponent(DataContainer).GetSprite("attachmentIcon")
	end
	
	local storage = _G.PhoenixGlobalStorage["SavedAttachments"][tostring(Player.actor.activeWeapon)][tostring(self.weaponPoint.gameObject)]
	self.buttonsNest.transform.GetChild(storage -1).gameObject.GetComponent(Button).interactable = false

	self.panel.SetActive(true)
	self.nilText.gameObject.SetActive(false)

	self.statsHeaderText.text = data.GetGameObjectArray("attachment")[storage].gameObject.GetComponent(DataContainer).GetString("attachmentName") .. " Stats"
	self.statsDescText.text = data.GetGameObjectArray("attachment")[storage].gameObject.GetComponent(DataContainer).GetString("attachmentDesc")
	self.headerText.text = data.GetString("pointName") .. " Attachments"
end

function PF_AttachmentPanel:EquipAttachment()
	local index = CurrentEvent.listenerData
	--local storage = _G.PhoenixGlobalStorage["SavedAttachments"][tostring(Player.actor.activeWeapon)][tostring(self.weaponPoint.gameObject)]
	--self.buttonsNest.transform.GetChild(storage -1).gameObject.GetComponent(Button).interactable = true

	self.currentPoint:EquipAttachment(index, false, true)
	--self.buttonsNest.transform.GetChild(index -1).gameObject.GetComponent(Button).interactable = false
end