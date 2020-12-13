-- Register the behaviour
behaviour("PF_SetMixer")

function PF_SetMixer:Start()
	local SetMixer = {
		["Master"] = function (self) self.gameObject.GetComponent(AudioSource).SetOutputAudioMixer(AudioMixer.Master) end,
		["Ingame"] = function (self) self.gameObject.GetComponent(AudioSource).SetOutputAudioMixer(AudioMixer.Ingame) end,
		["Important"] = function (self) self.gameObject.GetComponent(AudioSource).SetOutputAudioMixer(AudioMixer.Important) end,
		["FirstPerson"] = function (self) self.gameObject.GetComponent(AudioSource).SetOutputAudioMixer(AudioMixer.FirstPerson) end,
		["PlayerVehicle"] = function (self) self.gameObject.GetComponent(AudioSource).SetOutputAudioMixer(AudioMixer.PlayerVehicle) end,
		["World"] = function (self) self.gameObject.GetComponent(AudioSource).SetOutputAudioMixer(AudioMixer.World) end,
		["Music"] = function (self) self.gameObject.GetComponent(AudioSource).SetOutputAudioMixer(AudioMixer.Music) end,
		["MusicSting"] = function (self) self.gameObject.GetComponent(AudioSource).SetOutputAudioMixer(AudioMixer.MusicSting) end,
	}

	SetMixer[tostring(self.gameObject.GetComponent(DataContainer).GetString("mixer"))](self)
end