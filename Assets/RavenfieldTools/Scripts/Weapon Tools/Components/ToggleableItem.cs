using UnityEngine;
using System.Collections;

public class ToggleableItem : Weapon {

	public GameObject activatedObject;
	public float cooldown = 0.5f;

	public override bool ShouldHaveProjectilePrefab ()
	{
		return false;
	}
}
