using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponHolderPreview : MonoBehaviour {

	public Transform holdParent;

	public void HoldWeapon(Weapon weapon) {

		GetComponent<Animator>().SetFloat("pose type", (float) weapon.configuration.pose);

		weapon.transform.parent = holdParent;
		weapon.transform.localPosition = Vector3.zero;
		weapon.transform.localRotation = Quaternion.identity;
		weapon.transform.localScale = Vector3.one;

		weapon.thirdPersonTransform.localPosition = weapon.thirdPersonOffset;
		weapon.thirdPersonTransform.localEulerAngles = weapon.thirdPersonRotation;
		weapon.thirdPersonTransform.localScale = new Vector3(weapon.thirdPersonScale, weapon.thirdPersonScale, weapon.thirdPersonScale);
	}
}
