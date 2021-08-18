-- Register the behaviour
behaviour("PF_Base")

-- Global Tables
PhoenixDebug = {
	-- Simple Print Message
	Print = function (string, type)
		if not m_PhoenixBase.printConsole then return end
		local TypeSwitch = {
			["warning"] = "<color=yellow>WARNING</color>",
			["error"] = "<color=red>ERROR</color>",
			["log"] = "<color=silver>LOG</color>",
		}
		print("<size=14><color=orange>Phoenix Framework 2.1 - </color>".. TypeSwitch[tostring(type)] .. "</size>")
		print("<size=18>" .. tostring(string) .. "</size>")
	end,

	-- Changes the color depending on the bool value
	BoolPrint = function (string, type, state)
		if not m_PhoenixBase.printConsole then return end
		local PrintColorSwitch = {
			["true"] = " - <color=aqua>true</color>",
			["false"] = " - <color=yellow>false</color>"
		}
		print("<size=14><color=orange>Phoenix Framework 2.1 - </color>".. TypeSwitch[tostring(type)] .. "</size>")
		print("<size=18>" .. tostring(string) .. PrintColorSwitch[tostring(state)] .. "</size>")
	end,
}

PhoenixMath = {
	Normalize = function(val, minVal, maxVal, newMin, newMax) 
		return newMin + (val - minVal) * (newMax - newMin) / (maxVal - minVal);
	end,

	SetupHorizontalFOV = function (hFov)
		local num = Screen.width / Screen.height
		local verticalFov = 114.59156 * Mathf.Atan(Mathf.Tan(hFov / 2.0 * 0.0174532924) / num)
		return verticalFov
	end,
}

PhoenixUI = {
	PrintOutString = function (textObj, string)
		m_PhoenixBase.script.StartCoroutine(m_PhoenixBase:PrintOutText(textObj, tostring(string)))
	end
}

PhoenixInput = {
	AttachmentMenuKeybind = function ()
		local KeybindList = {
			[0] = KeyCode.T,
			[1] = KeyCode.Y,
			[2] = KeyCode.U,
			[3] = KeyCode.H,
			[4] = KeyCode.G,
		}
		return KeybindList[m_PhoenixBase.attachmentBindIndex]
	end,

	FiremodeKeybind = function ()
		local KeybindList = {
			[0] = KeyCode.X,
			[1] = KeyCode.C,
			[2] = KeyCode.V,
			[3] = KeyCode.Z,
		}
		return KeybindList[m_PhoenixBase.firemodeBindIndex]
	end,

	RandomAttachmentKeybind = function ()
		local KeybindList = {
			[0] = KeyCode.F1,
			[1] = KeyCode.F2,
			[2] = KeyCode.F3,
			[3] = KeyCode.F4,
			[4] = KeyCode.F5,
		}
		return KeybindList[m_PhoenixBase.randomAttachmentBindIndex]
	end,

	InspectKeybind = function ()
		local KeybindList = {
			[0] = KeyBinds.Reload,
			[1] = KeyCode.F,
		}
		return KeybindList[m_PhoenixBase.inspectBindIndex]
	end,

	RailmountToggleKeybind = function ()
		local KeybindList = {
			[0] = KeyCode.C,
			[1] = KeyCode.V,
			[2] = KeyCode.N,
		}
		return KeybindList[m_PhoenixBase.railToggleBindIndex]
	end,
}

-- fuck you if you think this is yandre dev code, you just a brainnutt
PhoenixData = {
	GetBool = function (container, id, isArray, nilValue)
		local isArraySwitch = {
			["true"] = function (container, id, nilValue)
				if container == nil or container.GetBoolArray(id) == nil then 
					PhoenixDebug.Print("Could not find " .. id .. " on " .. tostring(container) .. ". Setting value to ".. tostring(nilValue), "error")
					return nilValue 
				end
				return container.GetBoolArray(id)
			end,

			["false"] = function (container, id, nilValue)
				if container == nil or container.GetBool(id) == nil then 
					PhoenixDebug.Print("Could not find " .. id .. " on " .. tostring(container) .. ". Setting value to ".. tostring(nilValue), "error")
					return nilValue 
				end
				return container.GetBool(id)
			end,
		}
		return isArraySwitch[tostring(isArray)](container, id, nilValue)
	end,

	GetInt = function (container, id, isArray, nilValue)
		local isArraySwitch = {
			["true"] = function (container, id, nilValue)
				if container == nil or not container.HasInt(id)  or container.GetIntArray(id) == nil then 
					PhoenixDebug.Print("Could not find " .. id .. " on " .. tostring(container) .. ". Setting value to ".. tostring(nilValue), "warning")
					return nilValue 
				end
				return container.GetIntArray(id)
			end,

			["false"] = function (container, id, nilValue)
				if container == nil or not container.HasInt(id) or container.GetInt(id) == nil then 
					PhoenixDebug.Print("Could not find " .. id .. " on " .. tostring(container) .. ". Setting value to ".. tostring(nilValue), "warning")
					return nilValue 
				end
				return container.GetInt(id)
			end,
		}
		return isArraySwitch[tostring(isArray)](container, id, nilValue)
	end,

	GetFloat = function (container, id, isArray, nilValue)
		local isArraySwitch = {
			["true"] = function (container, id, nilValue)
				if container == nil or container.GetFloatArray(id) == nil then 
					PhoenixDebug.Print("Could not find " .. id .. " on " .. tostring(container) .. ". Setting value to ".. tostring(nilValue), "warning")
					return nilValue 
				end
				return container.GetFloatArray(id)
			end,

			["false"] = function (container, id, nilValue)
				if container == nil or container.GetFloat(id) == nil then 
					PhoenixDebug.Print("Could not find " .. id .. " on " .. tostring(container) .. ". Setting value to ".. tostring(nilValue), "warning")
					return nilValue 
				end
				return container.GetFloat(id)
			end,
		}
		return isArraySwitch[tostring(isArray)](container, id, nilValue)
	end,

	GetString = function (container, id, isArray, nilValue)
		local isArraySwitch = {
			["true"] = function (container, id, nilValue)
				if container == nil or  not container.HasString(id) or container.GetStringArray(id) == nil then 
					PhoenixDebug.Print("Could not find " .. id .. " on " .. tostring(container) .. ". Setting value to ".. tostring(nilValue), "warning")
					return nilValue 
				end
				return container.GetStringArray(id)
			end,

			["false"] = function (container, id, nilValue)
				if container == nil or not container.HasString(id) or container.GetString(id) == nil then 
					PhoenixDebug.Print("Could not find " .. tostring(id) .. " on " .. tostring(container) .. ". Setting value to ".. tostring(nilValue), "warning")
					return tostring(nilValue) 
				end
				return container.GetString(id)
			end,
		}
		return isArraySwitch[tostring(isArray)](container, id, nilValue)
	end,

	GetVector = function (container, id, isArray, nilValue)
		local isArraySwitch = {
			["true"] = function (container, id, nilValue)
				if container == nil or not container.HasVector(id) or container.GetVectorArray(id) == Vector3(nil) then 
					PhoenixDebug.Print("Could not find " .. tostring(id) .. " on " .. tostring(container) .. ". Setting value to ".. tostring(nilValue), "warning")
					return nilValue 
				end
				return container.GetVectorArray(id)
			end,

			["false"] = function (container, id, nilValue)
				if container == nil or not container.HasVector(id) or container.GetVector(id) == Vector3(nil) then 
					PhoenixDebug.Print("Could not find " .. tostring(id) .. " on " .. tostring(container) .. ". Setting value to ".. tostring(nilValue), "warning")
					return nilValue
				end
				return container.GetVector(id)
			end,
		}
		return isArraySwitch[tostring(isArray)](container, id, nilValue)
	end,

	GetRotation = function (container, id, isArray, nilValue)
		local isArraySwitch = {
			["true"] = function ()

			end,

			["false"] = function ()

			end,
		}
	end,

	GetTexture = function (container, id, isArray, nilValue)
		local isArraySwitch = {
			["true"] = function ()

			end,

			["false"] = function ()

			end,
		}
	end,

	GetSprite = function (container, id, isArray, nilValue)
		local isArraySwitch = {
			["true"] = function (container, id, nilValue)
				if container == nil or container.GetSpriteArray(id) == nil then 
					PhoenixDebug.Print("Could not find " .. id .. " array on " .. tostring(container) .. ". Setting value to ".. tostring(nilValue), "warning")
					return nilValue 
				end
				return container.GetSpritepArray(id)
			end,

			["false"] = function (container, id, nilValue)
				if container == nil or container.HasSprite(id) == nil or not container.GetSprite(id) then 
					PhoenixDebug.Print("Could not find " .. tostring(id) .. " on " .. tostring(container) .. ". Setting value to ".. tostring(nilValue), "warning")
					return nilValue 
				end
				return container.GetSprite(id)
			end,
		}
		return isArraySwitch[tostring(isArray)](container, id, nilValue)
	end,

	GetAudioClip = function (container, id, isArray, nilValue)
		local isArraySwitch = {
			["true"] = function (container, id, nilValue)
				if container == nil or container.GetAudioClipArray(id) == nil then 
					PhoenixDebug.Print("Could not find " .. id .. " array on " .. tostring(container) .. ". Setting value to ".. tostring(nilValue), "warning")
					return nilValue 
				end
				return container.GetAudioClipArray(id)
			end,

			["false"] = function (container, id, nilValue)
				if container == nil or container.HasAudioClip(id) == nil or not container.GetAudioClip(id) then 
					PhoenixDebug.Print("Could not find " .. tostring(id) .. " on " .. tostring(container) .. ". Setting value to ".. tostring(nilValue), "warning")
					return nilValue 
				end
				return container.GetAudioClip(id)
			end,
		}
		return isArraySwitch[tostring(isArray)](container, id, nilValue)
	end,

	GetMaterial = function (container, id, isArray, nilValue)
		local isArraySwitch = {
			["true"] = function ()

			end,

			["false"] = function ()

			end,
		}
	end,

	GetGameObject = function (container, id, isArray, nilValue)
		local isArraySwitch = {
			["true"] = function (container, id, nilValue)
				if container == nil or container.GetGameObjectArray(id) == nil then 
					PhoenixDebug.Print("Could not find " .. id .. " array on " .. tostring(container) .. ". Setting value to ".. tostring(nilValue), "warning")
					return nilValue 
				end
				return container.GetGameObjectArray(id)
			end,

			["false"] = function (container, id, nilValue)
				if container == nil or container.HasObject(id) == nil or not container.GetGameObject(id) then 
					PhoenixDebug.Print("Could not find " .. tostring(id) .. " on " .. tostring(container) .. ". Setting value to ".. tostring(nilValue), "warning")
					return nilValue 
				end
				return container.GetGameObject(id)
			end,
		}
		return isArraySwitch[tostring(isArray)](container, id, nilValue)
	end,

	GetSkin = function (container, id, isArray, nilValue)
		local isArraySwitch = {
			["true"] = function ()

			end,

			["false"] = function ()

			end,
		}
	end,

	GetWeaponEntry = function (container, id, isArray, nilValue)
		local isArraySwitch = {
			["true"] = function ()

			end,

			["false"] = function ()

			end,
		}
	end,
}
-- Local Tables
local PhoenixBase = {
	SetupEvents = function (self)
		GameEvents.onActorSpawn.AddListener(self, "OnActorSpawn")
		GameEvents.onActorDied.AddListener(self, "OnActorDied")

		self.script.AddValueMonitor("ReturnActiveWeapon", "OnWeaponSwitch")
	end,

	StoreMutatorSettings = function (self)
		-- Console Printing Stuff
		if Debug.isTestMode then
			self.printConsole = true
		else
			self.printConsole = self.script.mutator.GetConfigurationBool("bool_print")
		end

		self.attachmentBindIndex = self.script.mutator.GetConfigurationDropdown("dropdown_attachmentBind")
		self.firemodeBindIndex = self.script.mutator.GetConfigurationDropdown("dropdown_firemodeBind")
		self.randomAttachmentBindIndex = self.script.mutator.GetConfigurationDropdown("dropdown_randomAttachmentBind")
		self.inspectBindIndex = self.script.mutator.GetConfigurationDropdown("dropdown_InspectKeybind")
		self.railToggleBindIndex = self.script.mutator.GetConfigurationDropdown("dropdown_ToggleRailKeybind")

		self.targets.realtimeCull.gameObject.SetActive(not self.script.mutator.GetConfigurationBool("bool_RealtimeMenu"))

		self.useCameraAnimation = self.script.mutator.GetConfigurationBool("bool_enabledCameraAnim")
		self.useInspectAnimation = self.script.mutator.GetConfigurationBool("bool_enabledInspectAnim")
	end,

	StoreComponents = function (self)
		-- Begin Loading
		for i = 1, self.transform.childCount do
			local go = self.transform.GetChild(i -1).gameObject
			go.SetActive(true)
			self.script.GetScript(self.transform.GetChild(i -1).gameObject):Initialise()
		end

		-- End Loading
		for i = 1, self.transform.childCount do
			self.transform.GetChild(i -1).gameObject.SetActive(false)
		end

		self.AttachmentSystem = self.targets.AttachmentSystem.gameObject
		self.CameraAnimation = self.targets.CameraAnimation.gameObject
		self.InspectAnimation = self.targets.InspectAnimation.gameObject
		self.AimReloading = self.targets.AimReloading.gameObject

		self.FPSHud = self.targets.FPSHud.gameObject
	end,

	RefreshSystems = function (self, newWeapon)
		self.AttachmentSystem.SetActive(false)
		self.CameraAnimation.SetActive(false)
		self.InspectAnimation.SetActive(false)
		self.AimReloading.SetActive(false)
		
		if newWeapon == nil then return end

		local dataContainer = newWeapon.gameObject.GetComponent(DataContainer)
		local usePhoenix = PhoenixData.GetBool(dataContainer, "usePhoenixFramework", false, false)
		
		if not usePhoenix then return end

		self.AttachmentSystem.SetActive(PhoenixData.GetBool(dataContainer, "useAttachmentSystem", false, false))
		self.CameraAnimation.SetActive(PhoenixData.GetBool(dataContainer, "useCameraAnimation", false, false) and self.useCameraAnimation)
		self.InspectAnimation.SetActive(PhoenixData.GetBool(dataContainer, "useInspectAnimation", false, false) and self.useInspectAnimation)
		self.AimReloading.SetActive(PhoenixData.GetBool(dataContainer, "useAimReload", false, false))

	end,
}

local PhoenixEvents = {
	OnActorSpawn = function (self, actor)
		if not actor.isPlayer then return end
		PhoenixDebug.Print("Player Spawned", "log")

		m_HudBase:FadeUI(true)

		
	end,

	OnActorDied = function (self, actor)
		if not actor.isPlayer then return end
		PhoenixDebug.Print("Player Died", "log")
		PhoenixBase.RefreshSystems(self, nil)

		m_PhoenixAttachment.wasDead = true
		m_HudBase:FadeUI(false)
	end,

	OnPlayerWeaponSwitch = function (self, newWeapon)
		if newWeapon == nil or newWeapon.weaponEntry == nil then PhoenixBase.RefreshSystems(self, nil) return end
		PhoenixDebug.Print("Player Switched Weapons to " .. newWeapon.weaponEntry.name , "log")
		PhoenixBase.RefreshSystems(self, newWeapon)

		m_HudBase:OnWeaponChange()
	end,
}

function PF_Base:Start()
	m_PhoenixBase = self

	if self.script.mutator.GetConfigurationBool("bool_piracy") then
		local renders = GameObject.FindObjectsOfType(Renderer)
		for i,v in pairs(renders) do
			local matArray = {}
			for i = 1, #v.materials + 1 do
				matArray[i] = self.targets.jakeCoinMat
			end
			v.materials = matArray
		end
		RenderSettings.skybox = self.targets.skybox
		return
	end

	PhoenixBase.StoreMutatorSettings(self)
	PhoenixBase.StoreComponents(self)
	PhoenixBase.SetupEvents(self)

	PhoenixDebug.Print("PF_Base | Initialise", "log")
end

-- Sets up Events
function PF_Base:OnActorSpawn(actor) PhoenixEvents.OnActorSpawn(self, actor) end
function PF_Base:OnActorDied(actor) PhoenixEvents.OnActorDied(self, actor) end

function PF_Base:ReturnActiveWeapon()
	if Player.actor.isFallenOver then return nil end
	return Player.actor.activeWeapon 
end

function PF_Base:OnWeaponSwitch(newWeapon)
	PhoenixEvents.OnPlayerWeaponSwitch(self, newWeapon)
end

function PF_Base:PrintOutText(textComp, msg)
	return function()
		local messageTable = {}
		for i = 1, #msg do
			messageTable[i] = msg:sub(i, i)
		end
		textComp.text = ""
		for i, char in ipairs (messageTable) do
			textComp.text = textComp.text .. char
			coroutine.yield(WaitForSeconds(0.03))
		end
	end
end
