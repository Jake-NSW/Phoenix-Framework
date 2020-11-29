using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetTracker : MonoBehaviour {

	public float fieldOfView = 20f;
	public float scanFrequency = 5f;
	public float lockTime = 2f;
	public bool requireAim = false;
	public bool requireLockToFire = false;
	public bool onlyLockWhenWeaponIsEquipped = true;
	public Transform trackingPositionIndicator;
	public GameObject activateWhenLocking;
	public GameObject activateWhenLocked;

	void Awake() {
		this.activateWhenLocked.SetActive(false);
		this.activateWhenLocking.SetActive(false);
	}
}
