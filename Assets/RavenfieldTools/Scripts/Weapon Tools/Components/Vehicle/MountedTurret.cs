using UnityEngine;
using System.Collections;

public class MountedTurret : MountedWeapon {

	public Transform bearingTransform, pitchTransform;

	public Clamp clampX;
	public Clamp clampY;

    public float sensitivityX = 1f;
    public float sensitivityY = 1f;

    [System.Serializable]
	public class Clamp {
		public bool enabled;
		public float min, max;

		public float ClampAngle(float a) {
			return Mathf.Clamp(Mathf.DeltaAngle(0f, a), this.min, this.max);
		}
	}
}
