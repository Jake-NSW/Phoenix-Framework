using UnityEngine;
using System.Collections;

public class CarHorn : MountedWeapon {

	public override bool ShouldHaveProjectilePrefab ()
	{
		return false;
	}
}
