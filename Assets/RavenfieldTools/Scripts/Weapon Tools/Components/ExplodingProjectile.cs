using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ExplodingProjectile : Projectile {

	public ExplosionConfiguration explosionConfiguration;
	public float smokeTime = 8f;

    const float CLEANUP_TIME = 10f;

	public Renderer[] renderers;
	public ParticleSystem trailParticles;
	public ParticleSystem impactParticles;

    public GameObject activateOnExplosion;
    public float deactivateAgainTime = -1f;

	protected override void Hit (Vector3 point, Vector3 normal)
	{
		this.transform.position = point;
		this.transform.rotation = Quaternion.LookRotation(normal);

		foreach(Renderer renderer in this.renderers) {
			renderer.enabled = false;
		}

		if(this.trailParticles != null) {
			this.trailParticles.Stop();
		}

        var audio = GetComponent<AudioSource>();

        if(audio != null) {
            audio.Play();
        }

        if (this.activateOnExplosion != null) {
            this.activateOnExplosion.SetActive(true);

            if (this.deactivateAgainTime > 0) {
                Invoke("Deactivate", this.deactivateAgainTime);
            }
        }

        this.impactParticles.Play();

		Invoke("StopSmoke", this.smokeTime);

		this.travelling = false;
		this.enabled = false;

		WeaponUser.RegisterHit(point);
	}

    void StopSmoke() {
        this.impactParticles.Stop();

        Invoke("Cleanup", CLEANUP_TIME);
    }

    void Deactivate() {
        this.activateOnExplosion.SetActive(false);
    }

    void Cleanup() {
		Destroy(this.gameObject);
	}

	[System.Serializable]
	public class ExplosionConfiguration {
		public float damage = 300f;
		public float balanceDamage = 300f;
		public float force = 500f;
		public float damageRange = 6f;
		public AnimationCurve damageFalloff;
		public float balanceRange = 9f;
		public AnimationCurve balanceFalloff;
	}
}
