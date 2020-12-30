-- Register the behaviour
behaviour("PFHud_Base")
function PFHud_Base:Start()
	m_HudBase = self

	self.enabled = self.script.mutator.GetConfigurationBool("bool_CustomHudEnabled")
	if not self.enabled then return end

	local hudPresets = {
		[0] = function (self)
			self.targets.preset1.gameObject.SetActive(true)
			self.currentPreset = self.targets.preset1.gameObject.GetComponent(DataContainer)
		end,
		[1] = function (self)
			self.targets.preset2.gameObject.SetActive(true)
			self.currentPreset = self.targets.preset2.gameObject.GetComponent(DataContainer)
		end,
	}

	local preset = self.script.mutator.GetConfigurationDropdown("dropdown_HudPreset")
	hudPresets[preset](self)

	-- Stash objects
	self.spareAmmoText = self.currentPreset.GetGameObject("spareAmmoText").gameObject.GetComponent(TextMeshProUGUI)
	self.currentAmmoText = self.currentPreset.GetGameObject("currentAmmoText").gameObject.GetComponent(TextMeshProUGUI)
	self.firemodeText = self.currentPreset.GetGameObject("firemodeText").gameObject.GetComponent(TextMeshProUGUI)
	self.healthText = self.currentPreset.GetGameObject("healthText").gameObject.GetComponent(TextMeshProUGUI)
	self.vehicleHealthText = self.currentPreset.GetGameObject("vehicleHealthText").gameObject.GetComponent(TextMeshProUGUI)
	self.weaponNameText = self.currentPreset.GetGameObject("weaponNameText").gameObject.GetComponent(TextMeshProUGUI)
	self.vehicleNameText = self.currentPreset.GetGameObject("vehicleNameText").gameObject.GetComponent(TextMeshProUGUI)
	self.healthBar = self.currentPreset.GetGameObject("healthBar").gameObject.GetComponent(Image)
	self.vehicleHealthBar = self.currentPreset.GetGameObject("vehicleHealthBar").gameObject.GetComponent(Image)

	self.bloodVignette = self.targets.bloodVignette.gameObject.GetComponent(Image)
	self.vehicleHealth = self.currentPreset.GetGameObject("vehicleHealth").gameObject
	self.weaponNamePanel = self.currentPreset.GetGameObject("weaponNamePanel").gameObject
	
	self.animator = self.gameObject.GetComponent(Animator)
	self.stateValue = 0
	self.currentState = false

	-- Find Ravenfield UI and disable it
	GameObject.Find("Ingame UI Container(Clone)").Find("Ingame UI/Panel").gameObject.GetComponent(Image).color = Color(0,0,0,0)
	GameObject.Find("Current Ammo Text").gameObject.SetActive(false)
	GameObject.Find("Spare Ammo Text").gameObject.SetActive(false)
	GameObject.Find("Vehicle Health Background").gameObject.SetActive(false)
	GameObject.Find("Resupply Health").gameObject.SetActive(false)
	GameObject.Find("Resupply Ammo").gameObject.SetActive(false)
	GameObject.Find("Squad Text").gameObject.GetComponent(Text).color = Color(0,0,0,0)
	GameObject.Find("Sight Text").gameObject.SetActive(false)
	GameObject.Find("Weapon Image").gameObject.SetActive(false)
	GameObject.Find("Health Text").gameObject.transform.parent.gameObject.SetActive(false)


	-- self.hitmarker = self.targets.hitmarker.gameObject
	self.ammo = self.currentPreset.GetGameObject("ammo").gameObject

	-- self.hitmarker.GetComponent(RawImage).texture = self.rfHitmarker.texture

	self.canvasGroup = self.gameObject.GetComponent(CanvasGroup)

	-- Add value monitor
	self.script.AddValueMonitor("MonitorCurrentAmmo", "OnAmmoChange")
	self.script.AddValueMonitor("MonitorSpareAmmo", "OnSpareAmmoChange")
	self.script.AddValueMonitor("MonitorHUD", "OnHudChange")
	self.script.AddValueMonitor("MonitorHealth", "OnHealthChange")
	self.script.AddValueMonitor("MonitorVehicleHealth", "OnVehicleHealthChange")
	self.script.AddValueMonitor("MonitorActiveVehicle", "OnVehicleChange")
	-- self.script.AddValueMonitor("MonitorHitmarker", "OnHitmarkerChange")
	-- self.script.AddValueMonitor("MonitorWeaponName", "OnWeaponNameChange")

	-- Changes opacity of objects depending on weither they have it enabled
	local groupAlpha = {
		["true"] = function (canvasGroupGO)
			canvasGroupGO.gameObject.GetComponent(CanvasGroup).alpha = 1
		end,
		["false"] = function (canvasGroupGO)
			canvasGroupGO.gameObject.GetComponent(CanvasGroup).alpha = 0
		end,
	}

	groupAlpha[tostring(self.script.mutator.GetConfigurationBool("bool_LowHealthBlood"))](self.bloodVignette)
	groupAlpha[tostring(self.script.mutator.GetConfigurationBool("bool_ShowAmmoCounter"))](self.currentPreset.GetGameObject("ammoPanel").gameObject)
	groupAlpha[tostring(self.script.mutator.GetConfigurationBool("bool_ShowHealthCounter"))](self.currentPreset.GetGameObject("healthPanel").gameObject)
	groupAlpha[tostring(self.script.mutator.GetConfigurationBool("bool_ShowSquad"))](self.currentPreset.GetGameObject("squadPanel").gameObject)
end

-- Event functions
function PFHud_Base:Update()
	if not self.enabled then return end
	self.animator.SetFloat("Blend", self.stateValue, 0.18, Time.unscaledDeltaTime)
end

-- Event Functions
function PFHud_Base:OnAmmoChange(ammo) 
	if ammo == -1 or ammo == -2 then self.currentAmmoText.text = "∞" return end
	if ammo == -5 then self.ammo.SetActive(false) return end

	self.ammo.SetActive(true)
	self.currentAmmoText.text = tostring(ammo) 
end

function PFHud_Base:OnSpareAmmoChange(spareAmmo)
	if spareAmmo == -1 or spareAmmo == -2 then self.spareAmmoText.text = "∞" return end
	self.spareAmmoText.text = tostring(spareAmmo) 
end

function PFHud_Base:OnHudChange(bool)
	self.gameObject.GetComponent(Canvas).enabled = bool
end

function PFHud_Base:OnHealthChange(health)
	self.healthText.text = tostring(health)
	self.healthBar.fillAmount = PhoenixMath.Normalize(health, 0, 100, 0, 1)
	self.bloodVignette.color = Color(1, 1, 1, PhoenixMath.Normalize(health, 0, 40, 0.5, 0)) 
end

function PFHud_Base:OnVehicleHealthChange(vehicleHealth)
	if vehicleHealth == nil then
		self.vehicleHealth.SetActive(false)
		return
	end
	self.vehicleHealth.SetActive(true)
	self.vehicleHealthBar.fillAmount = PhoenixMath.Normalize(vehicleHealth, 0, Player.actor.activeVehicle.maxHealth, 0, 1)
	self.vehicleHealthText.text = tostring(Mathf.FloorToInt(vehicleHealth))

end

function PFHud_Base:OnVehicleChange(newVehicle)
	if newVehicle == nil then self.weaponNamePanel.SetActive(true) return end
	PhoenixUI.PrintOutString(self.vehicleNameText, newVehicle.name)
	if Player.actor.activeWeapon == nil or Player.actor.activeWeapon.weaponEntry == nil then self.weaponNamePanel.SetActive(false) end
end

function PFHud_Base:OnHitmarkerChange(bool)
	self.hitmarker.SetActive(bool)
end

-- Monitor Functions
function PFHud_Base:MonitorCurrentAmmo()
	local player = Player.actor
	if player.isDead or player.isSwimming or player.isFallenOver or player.isInWater or player.isOnLadder or not player.activeVehicle == nil then return 0 end

	if Player.actor.activeWeapon == nil then return -5 end
	return Player.actor.activeWeapon.ammo
end

function PFHud_Base:MonitorSpareAmmo()
	local player = Player.actor
	if player.isDead or player.isSwimming or player.isFallenOver or player.isInWater or player.isOnLadder or not player.activeVehicle == nil then return 0 end

	if Player.actor.activeWeapon == nil then return -5 end
	return Player.actor.activeWeapon.spareAmmo 
end

function PFHud_Base:MonitorHealth()
	local player = Player.actor
	if player == nil or player.isDead then return 0 end
	return Mathf.Clamp(Mathf.FloorToInt(Player.actor.health), 0, 999)
end

function PFHud_Base:MonitorVehicleHealth()
	if Player.actor.activeVehicle == nil then return nil end
	return Player.actor.activeVehicle.health
end

function PFHud_Base:MonitorActiveVehicle()
	if Player.actor.activeVehicle == nil then return nil end
	return Player.actor.activeVehicle
end

function PFHud_Base:MonitorHitmarker()
	return self.rfHitmarker.IsActive()
end

-- Other
function PFHud_Base:OnWeaponChange()
	if not self.enabled then return end

	local player = Player.actor
	if player.isDead or player.isSwimming or player.isFallenOver or player.isInWater or player.isOnLadder or not player.activeVehicle == nil then return end
	self:FiremodeText()

	local data = Player.actor.activeWeapon.gameObject.GetComponent(DataContainer)

	if data ~= nil and data.GetString("weaponName") ~= nil then
		PhoenixUI.PrintOutString(self.weaponNameText, Player.actor.activeWeapon.gameObject.GetComponent(DataContainer).GetString("weaponName"))
		return
	end
	PhoenixUI.PrintOutString(self.weaponNameText, Player.actor.activeWeapon.weaponEntry.name)

	--self.weaponNameText.text = Player.actor.activeWeapon.weaponEntry.name
end

function PFHud_Base:MonitorHUD() return GameManager.hudPlayerEnabled end

function PFHud_Base:FiremodeText()
	local state = {
		-- Is Auto
		["true"] = function (self)
			self.firemodeText.text = "[AUTO]" 
		end,
		-- Is Single
		["false"] = function (self)
			self.firemodeText.text = "[SINGLE]" 
		end
	}
	if not self.script.mutator.GetConfigurationBool("bool_CustomHudEnabled") then return end
	state[tostring(Player.actor.activeWeapon.isAuto)](self)
end

function PFHud_Base:FadeUI(stateBool)
	local state = {
		["true"] = function (self)
			self.stateValue = 1
			self.currentState = true
		end,
		["false"] = function (self)
			self.stateValue = 0
			self.currentState = false
		end
	}
	if not self.script.mutator.GetConfigurationBool("bool_CustomHudEnabled") then return end
	state[tostring(stateBool)](self)
end