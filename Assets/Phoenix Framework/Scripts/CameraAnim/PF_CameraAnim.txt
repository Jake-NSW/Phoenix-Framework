-- Register the behaviour
behaviour("PF_CameraAnim")
function PF_CameraAnim:Initialise()
	PhoenixDebug.Print("PF_CameraAnim | Initialise", "log")
end

function PF_CameraAnim:Update()
	local player = Player.actor
	if player.isDead or player.isSwimming or player.isFallenOver or player.isInWater or player.isOnLadder or not player.activeVehicle == nil then return end

	local rotationFromProxy = self.cameraAnimProxy.gameObject.transform.localRotation.eulerAngles
	PlayerCamera.fpCameraLocalRotation = Quaternion.Euler(Vector3(rotationFromProxy.x, rotationFromProxy.z, rotationFromProxy.y * -1))

end

function PF_CameraAnim:OnEnable()
	if Player.actor == nil or Player.actor.isDead then return end
	self.cameraAnimProxy = Player.actor.activeWeapon.gameObject.GetComponent(DataContainer).GetGameObject("cameraAnimProxy")
end

function PF_CameraAnim:OnDisable()
	if Player.actor == nil or Player.actor.isDead then return end
	PlayerCamera.fpCameraLocalRotation = Quaternion.Euler(0, 0, 0)
end
