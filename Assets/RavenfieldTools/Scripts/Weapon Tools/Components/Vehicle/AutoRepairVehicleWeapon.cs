using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoRepairVehicleWeapon : MountedWeapon {

	public float repairAmount = 100f;

	public override bool ShouldHaveProjectilePrefab ()
	{
		return false;
	}
}
