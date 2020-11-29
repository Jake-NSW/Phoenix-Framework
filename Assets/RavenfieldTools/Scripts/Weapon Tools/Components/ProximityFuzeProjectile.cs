using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ProximityFuzeProjectile : ExplodingProjectile {

	public float detonationDistance = 15f;
	public float distanceWobbleGain = 1f;
	public float armDistance = 10f;
	public float autoExplodeDistance = 300f;
	public bool allowAllTargets = false;
	public List<Actor.TargetType> allowedTargetTypes;


}
