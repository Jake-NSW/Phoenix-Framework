using UnityEngine;
using System.Collections;

public class ThrowableWeapon : Weapon {

	public override void Unholster ()
	{
		base.Unholster ();

		// Force reload throwables.
		if(this.ammo == 0) ReloadDone();
	}

	public override void Fire (Vector3 direction, bool useMuzzleDirection)
	{
		if(CanFire()) {
			this.lastFiredTimestamp = Time.time;
			if(this.animator != null) {
				this.animator.SetTrigger("throw");
			}
			else {
				Shoot(direction, useMuzzleDirection);
			}
		}
		
		this.holdingFire = true;
	}

	public void SpawnThrowable() {
		Shoot (Vector3.zero, true);
		Reload();
	}

	public override bool CanBeAimed ()
	{
		return base.CanBeAimed () && this.ammo > 0;
	}
}
