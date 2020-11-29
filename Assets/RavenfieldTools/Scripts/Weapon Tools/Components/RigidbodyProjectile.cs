using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RigidbodyProjectile : Projectile {

	// Use this for initialization
	protected virtual void Awake () {
		Rigidbody rigidbody = GetComponent<Rigidbody>();
		rigidbody.velocity = this.transform.forward*this.configuration.speed;
	}

	protected override void Update ()
	{
		if(Time.time > this.expireTime) {
			Destroy(this.gameObject);
			return;
		}
	}
}
