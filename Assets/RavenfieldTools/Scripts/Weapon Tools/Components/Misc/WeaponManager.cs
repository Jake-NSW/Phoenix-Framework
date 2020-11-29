using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WeaponManager : MonoBehaviour {

	public enum WeaponSlot {Primary, Secondary, Gear, LargeGear}

	[System.Serializable]
	public class WeaponEntry {
		public string name = "Weapon";
		public GameObject prefab;
		public WeaponSlot slot;
		public bool usableByAi = true;
		public bool usableByAiAllowOverride = true;

		public enum LoadoutType {Normal, Stealth, AntiArmor, Repair, ResupplyAmmo, ResupplyHealth};

		public LoadoutType type;
		public int sortPriority;

		public string[] tags;
	}
}
