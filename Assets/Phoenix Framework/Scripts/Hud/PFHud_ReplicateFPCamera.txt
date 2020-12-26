-- Register the behaviour
behaviour("PFHud_ReplicateFPCamera")
function PFHud_ReplicateFPCamera:Update()
	local cam = PlayerCamera.fpCameraLocalRotation.eulerAngles
	self.transform.localRotation = Quaternion.Euler(Vector3(cam.x, cam.y, cam.z))
end
