-- Register the behaviour
behaviour("PF_AttachmentPoint")

local AttachmentPointBase = {
	EquipAttachment = function (self, attachmentIndex, animatorOnly, emptyMag)
		attachmentType = {
			["sights"] = function (self, attachment) -- Sights shiets
				local weapon = Player.actor.activeWeapon
				local attachmentData = attachment.gameObject.GetComponent(DataContainer)

				weapon.aimFov = PhoenixData.GetFloat(attachmentData ,"aimFov", false, 60)
			end,
		
			["muzzles"] = function (self, attachment) -- isLoud and muzzle flash visibilty
				local weapon = Player.actor.activeWeapon
				local attachmentData = attachment.gameObject.GetComponent(DataContainer)

				weapon.isLoud = not PhoenixData.GetBool(attachmentData ,"isSilent", false, false)
				PhoenixData.GetGameObject(weapon.gameObject.GetComponent(DataContainer) ,"muzzleFlash", false, nil).SetActive(not PhoenixData.GetBool(attachmentData ,"hideFlash", false, false))

				-- Not needed cause of receiver system
				if PhoenixData.GetBool(weapon.gameObject.GetComponent(DataContainer) ,"useReceiverSystem", false, false) == true then
					self.attachmentBase:RefreshReceiver(false, nil)
					return
				end
				
				weapon.gameObject.GetComponent(AudioSource).clip = PhoenixData.GetAudioClip(attachmentData ,"FiringSound", false, nil)
			end,
		
			["toggleableRail"] = function(self, attachment) -- Toggleable Rail, when pressing x it either enables or disables the rail power
				local attachmentData = attachment.gameObject.GetComponent(DataContainer)
				
				self.attachmentBase.rail = attachmentData.GetGameObject("toggleable").gameObject
			end,
		
			["stock"] = function (self, attachment) -- Changes recoil and aimin speed
				local weapon = Player.actor.activeWeapon
				local attachmentData = attachment.gameObject.GetComponent(DataContainer)

				weapon.recoilBaseKickback = PhoenixData.GetFloat(attachmentData ,"kickback", false, 0)
				weapon.recoilKickbackProneMultiplier = PhoenixData.GetFloat(attachmentData ,"kickbackProneMultiplier", false, 0)
				weapon.recoilRandomKickback = PhoenixData.GetFloat(attachmentData ,"randomKick", false, 0)
				weapon.recoilSnapDuration = PhoenixData.GetFloat(attachmentData ,"snapDuration", false, 0)
				weapon.recoilSnapFrequency = PhoenixData.GetFloat(attachmentData ,"snapFrequency", false, 0)
				weapon.recoilSnapMagnitude = PhoenixData.GetFloat(attachmentData ,"snapMagnitude", false, 0)
				weapon.recoilSnapProneMultiplier = PhoenixData.GetFloat(attachmentData ,"snapProneMultiplier", false, 0)

				self.attachmentBase.statsRecoilSlider.value = PhoenixData.GetInt(attachmentData, "recoilStat", false, 0)
		
			end,
		
			["underbarrel"] = function (self, attachment) -- Controls followup spread
				local weapon = Player.actor.activeWeapon
				local attachmentData = attachment.gameObject.GetComponent(DataContainer)

				weapon.followupSpread.maxSpreadAim = PhoenixData.GetFloat(attachmentData ,"maxSpreadAim", false, 0)
				weapon.followupSpread.maxSpreadHip = PhoenixData.GetFloat(attachmentData ,"maxSpreadHip", false, 0)
				weapon.followupSpread.proneMultiplier = PhoenixData.GetFloat(attachmentData ,"proneMultiplier", false, 0)
				weapon.followupSpread.spreadDissipateTime = PhoenixData.GetFloat(attachmentData ,"spreadDissipateTime", false, 0)
				weapon.followupSpread.spreadGain = PhoenixData.GetFloat(attachmentData ,"spreadGain", false, 0)
				weapon.followupSpread.spreadStayTime = PhoenixData.GetFloat(attachmentData ,"spreadStayTime", false, 0)

				self.attachmentBase.statsSpreadSlider.value = PhoenixData.GetInt(attachmentData, "spreadStat", false, 0)

			end,
		
			["mag"] = function (self, attachment, emptyMag) -- Changes player movement speed and ammo count		
				local weapon = Player.actor.activeWeapon
				local attachmentData = attachment.gameObject.GetComponent(DataContainer)
		
				weapon.maxAmmo = PhoenixData.GetFloat(attachmentData ,"magCapacity", false, 0)
				Player.actor.speedMultiplier = PhoenixData.GetFloat(attachmentData ,"playerSpeedMultiplier", false, 0)
				weapon.SetProjectilePrefab(PhoenixData.GetGameObject(attachmentData ,"projectile", false, 0))
				
				self.attachmentBase.statsFireRateSlider.value = PhoenixData.GetInt(attachmentData, "firerateStat", false, 0)
				self.attachmentBase.statsDamgeSlider.value = PhoenixData.GetInt(attachmentData, "damageStat", false, 0)
				self.attachmentBase.statsMoveSpeedSlider.value = PhoenixData.GetInt(attachmentData, "moveSpeedStat", false, 0)

				if PhoenixData.GetBool(weapon.gameObject.GetComponent(DataContainer) ,"useReceiverSystem", false, false) == true then
					self.attachmentBase:RefreshReceiver(true, PhoenixData.GetGameObject(attachmentData ,"receiver", false, 0).gameObject, self)
				end
				
				if emptyMag then 
					Player.actor.activeWeapon.ammo = 0
				end
			end,
		
			["skin"] = function (self, attachment) -- Changes weapon material... Will finish later, ITS NOT WORKING NOW??? GOTTA FIX IT
				local attachmentPoint = self.weaponPoint.gameObject.GetComponent(DataContainer)
				local attachmentData = attachment.gameObject.GetComponent(DataContainer)
		
				local rendererArray = attachmentPoint.GetGameObjectArray("mesh")
				local materialIndexArray = attachmentPoint.GetIntArray("meshMaterialIndex")
		
				local matArray = {}

				for i = 1, #rendererArray do
					local matIndex = materialIndexArray[i]
					local rendererComp = rendererArray[i].gameObject.GetComponent(Renderer)
		
					for i = 1, #rendererComp.materials + 1 do
						matArray[i] = attachmentPoint.GetMaterial("defaultMat")
					end
					matArray[matIndex] = attachmentData.GetMaterial("skin")

					rendererComp.materials = matArray
				end
			end,
		
			["receiver"] = function (self, attachment) -- Does nothing?
			end,
		
			["poop"] = function (self, attachment) -- Does nothing lol, stupid ass modder cant even set up an attachment correctly
			end,

			["nil"] = function (self) -- literally no way to get this
				print("how??")
			end,
		}
		-- Start Equip Attachment
		local data = self.weaponPoint.gameObject.GetComponent(DataContainer)
		local attachments = PhoenixData.GetGameObject(data, "attachment", true, nil)		
	
		-- Checks if the attachmentIndex is higher then the currnet ammount of attachments and if so sets attachmentIndex to 1
		if attachmentIndex > #attachments then
			attachmentIndex = 1
		end

		-- Visual stuff
		-- Disables old attachment
		attachments[_G.PhoenixGlobalStorage["SavedAttachments"][tostring(Player.actor.activeWeapon)][tostring(self.weaponPoint.gameObject)]].gameObject.SetActive(false)
		
		local attachmentData = attachments[attachmentIndex].gameObject.GetComponent(DataContainer)
		local doAnimator = data.GetBool("useModularAnimator")

		-- Attachment panel stuff if panel is open on current attachment point
		if m_AttachmentPanel.weaponPoint == self.weaponPoint then
			local storage = _G.PhoenixGlobalStorage["SavedAttachments"][tostring(Player.actor.activeWeapon)][tostring(self.weaponPoint.gameObject)]
			m_AttachmentPanel.buttonsNest.transform.GetChild(storage -1).gameObject.GetComponent(Button).interactable = true
			m_AttachmentPanel.buttonsNest.transform.GetChild(attachmentIndex -1).gameObject.GetComponent(Button).interactable = false

			m_AttachmentPanel.statsHeaderText.text = attachmentData.GetString("attachmentName") .. " Stats"
			m_AttachmentPanel.statsDescText.text = attachmentData.GetString("attachmentDesc")
		end
		
		-- Adds new attachment to storage
		_G.PhoenixGlobalStorage["SavedAttachments"][tostring(Player.actor.activeWeapon)][tostring(self.weaponPoint.gameObject)] = attachmentIndex
		
		-- Enables new attachment
		attachments[attachmentIndex].gameObject.SetActive(true)

		--if attachmentData.HasBool("hasNestedAttachmentPoint") and attachmentData.GetBool("hasNestedAttachmentPoint") then
		--	-- m_PhoenixAttachment:RefreshAttachmentPoints()
		--end

		--self.currentAttachText.text = attachmentData.GetString("attachmentName")
		--self.currentAttachIcon.sprite = attachmentData.GetString("attachmentIcon")

		-- Applys stats and stuff
		if animatorOnly then
			if tostring(PhoenixData.GetString(data, "attachmentType", false, "poop")) == "mag" then
				attachmentType["mag"](self, attachments[attachmentIndex].gameObject, emptyMag)
			end

			if tostring(PhoenixData.GetString(data, "attachmentType", false, "poop")) == "toggleableRail" then
				attachmentType["toggleableRail"](self, attachments[attachmentIndex].gameObject, emptyMag)
			end

			if doAnimator then 
				local paramName = attachmentData.GetStringArray("animatorName")
				local paramValue = attachmentData.GetFloatArray("animatorValue")

				self:ModularAnimator(paramName, paramValue) 
			end
			return
		else
			attachmentType[tostring(PhoenixData.GetString(data, "attachmentType", false, "poop"))](self, attachments[attachmentIndex].gameObject, emptyMag)
			if doAnimator then 
				local paramName = attachmentData.GetStringArray("animatorName")
				local paramValue = attachmentData.GetFloatArray("animatorValue")

				self:ModularAnimator(paramName, paramValue) 
			end
		end
	end,

	OnWeaponChange = function (self)
		local bindTable = {
			["1"] = KeyCode.Alpha1,
			["2"] = KeyCode.Alpha2,
			["3"] = KeyCode.Alpha3,
			["4"] = KeyCode.Alpha4,
			["5"] = KeyCode.Alpha5,
			["6"] = KeyCode.Alpha6,
			["7"] = KeyCode.Alpha7,
			["8"] = KeyCode.Alpha8,
			["9"] = KeyCode.Alpha9,
			["0"] = KeyCode.Alpha0,
		}

		local flipped = PhoenixData.GetBool(self.weaponPoint.gameObject.GetComponent(DataContainer), "flipped", false, false)
		local pointData = self.weaponPoint.gameObject.GetComponent(DataContainer)

		if flipped then 
			PhoenixData.GetGameObject(self.targets.right.gameObject.GetComponent(DataContainer), "pointName", false, nil).gameObject.GetComponent(TextMeshProUGUI).text = "[".. tostring(PhoenixData.GetInt(pointData, "bind", false, 1)) .."] " .. PhoenixData.GetString(pointData, "pointName", false, "Yooo Setup ya gun")
			self.targets.left.gameObject.SetActive(false)
			self.targets.right.gameObject.SetActive(true)
			self.currentAttachText = self.targets.right.gameObject.GetComponent(DataContainer).GetGameObject("currentAttachName").gameObject.GetComponent(TextMeshProUGUI)
			self.currentAttachIcon = self.targets.right.gameObject.GetComponent(DataContainer).GetGameObject("currentAttachIcon").gameObject.GetComponent(Image)
		else
			PhoenixData.GetGameObject(self.targets.left.gameObject.GetComponent(DataContainer), "pointName", false, nil).gameObject.GetComponent(TextMeshProUGUI).text = PhoenixData.GetString(pointData, "pointName", false, "Yooo Setup ya gun") .. " [".. tostring(PhoenixData.GetInt(pointData, "bind", false, 1)) .."]"
			self.targets.left.gameObject.SetActive(true)
			self.targets.right.gameObject.SetActive(false)
			self.currentAttachText = self.targets.left.gameObject.GetComponent(DataContainer).GetGameObject("currentAttachName").gameObject.GetComponent(TextMeshProUGUI)
			self.currentAttachIcon = self.targets.left.gameObject.GetComponent(DataContainer).GetGameObject("currentAttachIcon").gameObject.GetComponent(Image)
		end

		self.bind = bindTable[tostring(PhoenixData.GetInt(pointData, "bind", false, 1))]
	end,

	OnClick = function (self)
		self.attachmentBase.sfxSource.PlayOneShot(self.targets.sfx, 7)
		m_AttachmentPanel:RefreshAttachmentPanel(self.weaponPoint, self)
	end,

	RandomAttachment = function (self)
		local data = self.weaponPoint.gameObject.GetComponent(DataContainer)

		if tostring(PhoenixData.GetString(data, "attachmentType", false, "poop")) == "skin" then return end

		local attachment = PhoenixData.GetGameObject(data, "attachment", true, nil)
		local storage = _G.PhoenixGlobalStorage["SavedAttachments"][tostring(Player.actor.activeWeapon)][tostring(self.weaponPoint.gameObject)]
		local randomIndex = Mathf.FloorToInt(Random.Range(1, #attachment + 1))

		if randomIndex == storage then 
			self:EquipRandomAttachment()
			return
		else
			self:EquipAttachment(randomIndex, false, true)
		end
	end,
}

function PF_AttachmentPoint:OnWeaponChange() AttachmentPointBase.OnWeaponChange(self) end
function PF_AttachmentPoint:OnClick() AttachmentPointBase.OnClick(self) end
function PF_AttachmentPoint:EquipAttachment(index, animatorOnly, emptyMag) AttachmentPointBase.EquipAttachment(self, index, animatorOnly, emptyMag) end
function PF_AttachmentPoint:EquipRandomAttachment() AttachmentPointBase.RandomAttachment(self) end

function PF_AttachmentPoint:ModularAnimator(paramName, paramValue)
	if paramName ~= nil and paramValue ~= nil then
		for i = 1, #paramName do

			Player.actor.activeWeapon.animator.SetFloat(paramName[i], paramValue[i])
		end
	else
		print("MODULAR ANIMATOR NIL -- FOR SOME REASON -- PROBABLY BECUASE YOU DIDNT SET THE VALUES CORRECTLY")
	end
end

function PF_AttachmentPoint:Initialise()
	self.targets.left.gameObject.GetComponent(Button).onClick.AddListener(self, "OnClick")
	self.targets.right.gameObject.GetComponent(Button).onClick.AddListener(self, "OnClick")

	self.randomBind = PhoenixInput.RandomAttachmentKeybind()
	
	self.attachmentBase = m_PhoenixAttachment
end

function PF_AttachmentPoint:Update()
	if self.weaponPoint == nil or not self.attachmentBase.menuState then return end

	self.gameObject.transform.position = PlayerCamera.activeCamera.WorldToScreenPoint(self.weaponPoint.gameObject.transform.position)

	if Input.GetKeyDown(self.bind) then
		self:EquipAttachment(_G.PhoenixGlobalStorage["SavedAttachments"][tostring(Player.actor.activeWeapon)][tostring(self.weaponPoint.gameObject)] + 1, false, true)
	end

	if Input.GetKeyDown(self.randomBind) then
		self:EquipRandomAttachment()
	end
end