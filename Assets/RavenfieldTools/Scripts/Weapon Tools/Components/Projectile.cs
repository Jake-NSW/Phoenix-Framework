using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour {

	public Configuration configuration;

	public bool autoAssignArmorDamage = true;
	public Vehicle.ArmorRating armorDamage = Vehicle.ArmorRating.SmallArms;
	
	protected Vector3 velocity = Vector3.zero;
	protected float expireTime = 0f;

	protected float travelDistance = 0f;

	int hitMask;

	protected bool travelling = true;

	protected virtual void Start() {
		this.velocity = this.transform.forward*this.configuration.speed;
		this.expireTime = Time.time + this.configuration.lifetime;
	}

	// Update is called once per frame
	protected virtual void Update () {

		if(Time.time > this.expireTime) {
			Destroy(this.gameObject);
			return;
		}

		UpdatePosition();
	}

	protected virtual void UpdatePosition() {
		this.travelDistance += this.configuration.speed*Time.deltaTime;

		this.velocity += Physics.gravity*this.configuration.gravityMultiplier*Time.deltaTime;
		Vector3 delta = this.velocity*Time.deltaTime;
		Travel(delta);
	}

	protected virtual void Travel(Vector3 delta) {

		Vector3 nextPosition = this.transform.position+delta;

		RaycastHit hitInfo;
		if(Physics.Linecast(this.transform.position, nextPosition, out hitInfo, 1)) {
			Hit(hitInfo.point, hitInfo.normal);
		}

		if(travelling) {
			this.transform.position += delta;
			this.transform.rotation = Quaternion.LookRotation(delta);
		}
	}

	protected virtual void Hit(Vector3 point, Vector3 normal) {
		Destroy(this.gameObject);
		WeaponUser.RegisterHit(point);
	}

	[System.Serializable]
	public class Configuration {
		public float speed = 300f;
		public float impactForce = 200f;
		public float lifetime = 2f;
		public float damage = 70f;
		public float balanceDamage = 60f;
		public float impactDecalSize = 0.2f;
		public bool passThroughPenetrateLayer = true;
		public bool piercing = false;
		public bool makesFlybySound = false;
		public float flybyPitch = 1f;
		public float dropoffEnd = 300f;
		public float gravityMultiplier = 1f;
		public AnimationCurve damageDropOff;
		public bool inheritVelocity = false;
	}
}
