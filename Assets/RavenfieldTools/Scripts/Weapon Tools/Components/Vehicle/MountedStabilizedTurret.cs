using UnityEngine;
using System.Collections;

public class MountedStabilizedTurret : MountedWeapon {

	const float MAX_TURN_DELTA = 10f;

	public Transform bearingTransform, pitchTransform;

    public float sensitivityX = 1f;
    public float sensitivityY = 1f;

	public Clamp clampX;
	public Clamp clampY;

    public bool useMaxTurnSpeed = false;
    public float maxTurnSpeed = 100f;

    public bool useSpring = false;
    public float springAmount = 0.002f;
    public float springForce = 30f;
    public float springDrag = 5f;
    public Vector2 springMaxOffset = new Vector2(0.1f, 0.1f);

    [System.Serializable]
	public class Clamp {
		public bool enabled;
		public float min, max;

		public float ClampAngle(float a) {
			return Mathf.Clamp(Mathf.DeltaAngle(0f, a), this.min, this.max);
		}
	}
}
