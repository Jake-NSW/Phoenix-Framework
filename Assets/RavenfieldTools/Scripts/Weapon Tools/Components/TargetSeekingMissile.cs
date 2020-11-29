using UnityEngine;
using System.Collections;

public class TargetSeekingMissile : Rocket {

	public float ejectSpeed = 10f;

	public bool alwaysTakeDirectPath = false;
	public float damage = 800f;
	public float divingDamage = 1500f;
	public float correctionAcceleration = 200f;
	public float maxDrift = 0.5f;
	public float driftSpeed = 1f;

}
