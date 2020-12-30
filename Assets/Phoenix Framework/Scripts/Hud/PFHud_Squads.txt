-- Register the behaviour
behaviour("PFHud_Squads")

function PFHud_Squads:Start()
	self.rfOrderText = GameObject.Find("Squad Text").gameObject.GetComponent(Text)
	self.orderText = self.targets.orderText.gameObject.GetComponent(TextMeshProUGUI)

	self.script.AddValueMonitor("MonitorSquadOrder", "OnOrderChange")
end

function PFHud_Squads:MonitorSquadOrder()
	if Player.actor == nil then return end
	return self.rfOrderText.text
end

function PFHud_Squads:OnOrderChange(order) 
	if string.find(order, "FOLLOWING") then
		self.orderText.text = string.match(order, "%d+") .. " Following"
	elseif string.find(order, "MOVING") then
		self.orderText.text = string.match(order, "%d+") .. " Moving"
	elseif string.find(order, "HOLDING") then
		self.orderText.text = string.match(order, "%d+") .. " Holding"
	elseif string.find(order, "NO SQUAD") then
		self.orderText.text = "No Squad"
	end
end