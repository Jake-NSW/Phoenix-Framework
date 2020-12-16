behaviour("PF_GlobalStorage")
function PF_GlobalStorage:Start() 
	_G.PhoenixGlobalStorage = {
		["SavedAttachments"] = {}
	}

end