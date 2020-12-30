-- Register the behaviour
behaviour("PF_AimReload")

function PF_AimReload:Initialise()
	PhoenixDebug.Print("PF_AimReload | Initialise", "log")
end

function PF_AimReload:OnEnable()
	if Player.actor.isDead then return end
	self.transitionDamping = Player.actor.activeWeapon.gameObject.GetComponent(DataContainer).GetFloat("transitionDamping")
	self.weaponAnimator = Player.actor.activeWeapon.animator
end

function PF_AimReload:Update()
	local player = Player.actor
	if player.isDead or player.isSwimming or player.isFallenOver or player.isInWater or player.isOnLadder then return end
	
	if Input.GetKeyBindButton(KeyBinds.Aim) then self.weaponAnimator.SetFloat("reloadaim", 1, self.transitionDamping, Time.deltaTime) return end
	self.weaponAnimator.SetFloat("reloadaim", 0, self.transitionDamping, Time.deltaTime)
end
