using UnityEngine;
using System.Collections;

public class MeleeWeapon : Weapon {

	public float radius = 0.4f;
	public float range = 2f;
	public float swingTime = 0.15f;
	public float damage = 55f;
	public float balanceDamage = 150f;
	public float force = 100f;

	public AudioClip hitSound;

	protected override Projectile SpawnProjectile (Vector3 direction, Vector3 position)
	{
		return null;
	}

	public override bool ShouldHaveProjectilePrefab ()
	{
		return false;
	}
}
