using UnityEngine;
using System.Collections;

public class MountedWeapon : Weapon {

	public Camera overrideCamera;
	public Camera aimCamera;
	public float aimChangeSpeed = 10f;
    public float vehicleRigidbodyRecoilForce = 0f;


    protected override void Update ()
	{
		
	}
}
