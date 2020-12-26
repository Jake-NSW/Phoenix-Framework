-- Register the behaviour
behaviour("PF_InspectAnim")

local InspectAnim = {
	InspectType = function (self)
		local type = {
			[0] = function (self)
				if Input.GetKeyBindButtonDown(KeyBinds.Reload) and not Player.actor.activeWeapon.isReloading then
					self:StartInspect()
				end
			end,
		
			[1] = function (self)
				if Input.GetKeyDown(KeyCode.F) and not Player.actor.activeWeapon.isReloading then
					self:StartInspect()
				end
			end,
		
			["nil"] = function (self)
				print("how tf?")
			end
		}
		type[self.bind](self)
	end
}

function PF_InspectAnim:Initialise()
	PhoenixDebug.Print("PF_InspectAnim | Initialise", "log")

	self.bind = self.script.mutator.GetConfigurationDropdown("dropdown_InspectKeybind")
end

function PF_InspectAnim:Update()
	local player = Player.actor
	if player.isDead or player.activeWeapon == nil or player.isSwimming or player.isFallenOver or player.isInWater or player.isOnLadder or not player.activeVehicle == nil or player.activeWeapon.isLocked then return end

	InspectAnim.InspectType(self)
end

function PF_InspectAnim:StartInspect()
	Player.actor.activeWeapon.animator.SetTrigger("inspect")
	Player.actor.activeWeapon.LockWeapon()
	self.script.GetScript(m_PhoenixBase.FPSHud):FadeUI(false)
	self.script.StartCoroutine("FinishInspect")
end

-- Finishes the inspect 
function PF_InspectAnim:FinishInspect()
	coroutine.yield(WaitForSeconds(Player.actor.activeWeapon.gameObject.GetComponent(DataContainer).GetFloat("inspectTime")))
	self.script.GetScript(m_PhoenixBase.FPSHud):FadeUI(true)
	Player.actor.activeWeapon.UnlockWeapon()
end