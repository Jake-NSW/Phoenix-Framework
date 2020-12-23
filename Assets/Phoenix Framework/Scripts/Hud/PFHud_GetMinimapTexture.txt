-- Register the behaviour
behaviour("PFHud_GetMinimapTexture")

function PFHud_GetMinimapTexture:Start()
	self.gameObject.GetComponent(RawImage).texture = Minimap.texture
end

function PFHud_GetMinimapTexture:Update()
	
end
