using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinWhenHoldingFire : MonoBehaviour {

	public Weapon weapon;
	public Vector3 rotation = new Vector3(0f, 0f, 1000f);
	public float spinUpRate = 1f;

	float spinSpeed = 0f;
	
	// Update is called once per frame
	void Update () {

		this.spinSpeed = Mathf.MoveTowards(this.spinSpeed, (this.weapon.holdingFire && this.weapon.unholstered) ? 1f : 0f, this.spinUpRate*Time.deltaTime);

		Vector3 eulerAngle = this.transform.localEulerAngles;
		eulerAngle += this.rotation*this.spinSpeed*Time.deltaTime;

		this.transform.localEulerAngles = eulerAngle;
	}
}
