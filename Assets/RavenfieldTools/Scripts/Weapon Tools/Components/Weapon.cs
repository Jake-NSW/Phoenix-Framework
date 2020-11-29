using UnityEngine;
using UnityEngine.Audio;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(AudioSource))]
public class Weapon : MonoBehaviour {

	const float AIM_AMOUNT_SPEED = 6f;

	public enum HaltStrategy {Auto, Never, PreferredLongRange, PreferredAnyRange, AlwaysLongRange, Always};
	public enum Effectiveness {No, Yes, Preferred};
	public enum ReflectionSound {Auto, None, Handgun, RifleSmall, RifleLarge, Launcher, Tank};

	public Transform thirdPersonTransform;
	public Vector3 thirdPersonOffset = Vector3.zero;
	public Vector3 thirdPersonRotation = new Vector3(0f, 0f, -90f);
	public GameObject[] cullInThirdPerson;

	public float thirdPersonScale = 1f;
	public Configuration configuration;
	public AudioSource reloadAudio;
    public AudioSource changeFireModeAudio;
    public ReflectionSound reflectionSound = ReflectionSound.Auto;
	public float reflectionVolume = 0.35f;
	public float walkBobMultiplier = 1f;
    public float sprintBobMultiplier = 1f;
    public float proneBobMultiplier = 1f;

	public Sprite uiSprite;
	public SkinnedMeshRenderer arms;
	public bool allowArmMeshReplacement = true;

    public Weapon parentWeapon;
    public bool useParentWeaponSightModes;

    new protected AudioSource audio;

	protected Animator animator;
	protected List<Renderer> renderers;
	protected List<Renderer> nonScopeRenderers;
	int currentMuzzle = 0;
	Dictionary<Transform, ParticleSystem> muzzleFlash;

	float weaponVolume;
	float aimAmount = 0f;
	bool showScopeObject = false;
	Action stopFireLoop = new Action(0.12f);
	bool fireLoopPlaying = false;

	Action followupSpreadStayAction;
	float followupSpreadMagnitude = 0f;
	float followupSpreadDissipateRate = 1f;

	Action heatStayAction;
	float heat = 0f;
	bool isOverheating = false;
	[System.NonSerialized] public bool holdingFire = false;

	bool notifyAmmo = false;
	bool notifyRandom = false;
	bool notifyCharge = false;
    bool notifySmoothSightMode = false;

    bool aiming = false;
	[System.NonSerialized] public bool reloading = false;
	[System.NonSerialized] public bool unholstered = true;
    [System.NonSerialized] public bool switchedFromSubWeapon = false;
    [System.NonSerialized] public int ammo;
    [System.NonSerialized] public int activeSubWeaponIndex = -1;
    [System.NonSerialized] public int activeSightModeIndex = 0;
    float smoothSightMode = 0f;

    List<Weapon> alternativeWeapons = new List<Weapon>();

    protected float lastFiredTimestamp = 0f;

	int currentReloadMotion;

	Action chargeAction;

	bool hasFiredSingleRoundThisTrigger = false;

    public bool isLocked = false;
    Coroutine standardReload;


    protected virtual void Awake() {

		this.animator = this.GetComponent<Animator>();
		this.audio = GetComponent<AudioSource>();
		this.ammo = this.configuration.ammo;

		this.muzzleFlash = new Dictionary<Transform, ParticleSystem>(this.configuration.muzzles.Length);
		foreach(Transform muzzle in this.configuration.muzzles) {
			ParticleSystem muzzleFlashParticles = muzzle.GetComponent<ParticleSystem>();

			if(muzzleFlashParticles == null) {
				muzzleFlashParticles = muzzle.GetComponentInChildren<ParticleSystem>();
			}

			if(muzzleFlashParticles != null) {
				this.muzzleFlash.Add(muzzle, muzzleFlashParticles);
				muzzleFlashParticles.Stop();
			}
		}

		FindRenderers();

		this.audio.loop = this.configuration.auto;
		this.weaponVolume = this.audio.volume;

		this.chargeAction = new Action(this.configuration.chargeTime);

		this.followupSpreadStayAction = new Action(this.configuration.followupSpreadStayTime);
		this.followupSpreadDissipateRate = 1f/this.configuration.followupSpreadDissipateTime;
		this.heatStayAction = new Action(this.configuration.cooldown*1.1f);
    }

    public void AddSubWeapon(Weapon weapon) {
        this.alternativeWeapons.Add(weapon);
        weapon.animator = this.animator;
    }

    public bool HasParentWeapon() {
        return this.parentWeapon != null;
    }

    public bool HasAltModes() {
        return this.alternativeWeapons.Count > 0;
    }

    protected virtual void Start() {
        if (HasParentWeapon()) {
            this.parentWeapon.AddSubWeapon(this);
        }

        if (this.animator != null) {
            this.animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;

            foreach (AnimatorControllerParameter parameter in this.animator.parameters) {
                if (parameter.nameHash == Animator.StringToHash("loaded ammo") && parameter.type == AnimatorControllerParameterType.Int) {
                    this.notifyAmmo = true;
                }
                else if (parameter.nameHash == Animator.StringToHash("random") && parameter.type == AnimatorControllerParameterType.Float) {
                    this.notifyRandom = true;
                }
                else if (parameter.nameHash == Animator.StringToHash("charging") && parameter.type == AnimatorControllerParameterType.Bool) {
                    this.notifyCharge = true;
                }
                else if (parameter.nameHash == Animator.StringToHash("smooth sight mode") && parameter.type == AnimatorControllerParameterType.Float) {
                    this.notifySmoothSightMode = true;
                }
            }

            if (this.notifyAmmo) {
                this.animator.SetInteger("loaded ammo", this.ammo);
            }
        }

        Unholster();
	}

	public virtual void FindRenderers() {
		this.renderers = new List<Renderer>(this.GetComponentsInChildren<Renderer>());

		foreach(Renderer renderer in this.renderers) {
			renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
			renderer.receiveShadows = false;
		}

		if(this.arms != null) {
			this.arms.updateWhenOffscreen = true;
		}

		if(HasScopeObject()) {
			this.nonScopeRenderers = new List<Renderer>(this.renderers);

			foreach(Renderer renderer in this.configuration.scopeAimObject.GetComponentsInChildren<Renderer>()) {
				this.nonScopeRenderers.Remove(renderer);
			}
		}
	}

	protected bool IsMuzzleTransform(Transform t) {
		for(int i = 0; i < this.configuration.muzzles.Length; i++) {
			if(this.configuration.muzzles[i] == t) return true;
		}
		return false;
	}

	public virtual void CullFpsObjects() {

		if(this.animator != null) {
			Destroy(this.animator);
			this.animator = null;
		}

		if(this.cullInThirdPerson != null) {
			foreach(GameObject go in this.cullInThirdPerson) {
				GameObject.Destroy(go);
			}
		}

		bool useAlternativeMuzzles = this.configuration.optionalThirdPersonMuzzles != null && this.configuration.optionalThirdPersonMuzzles.Length > 0;

		if(useAlternativeMuzzles) {
			this.configuration.muzzles = this.configuration.optionalThirdPersonMuzzles;
		}


		this.thirdPersonTransform.transform.parent = this.transform;

		for(int i = 0; i < this.transform.childCount; i++) {
			Transform child = this.transform.GetChild(i);
			if(child != this.thirdPersonTransform) {
				if(IsMuzzleTransform(child)) {
					child.transform.localPosition = this.thirdPersonTransform.localPosition;
					this.thirdPersonTransform.localRotation = Quaternion.identity;
				}
				else {
					GameObject.Destroy(child.gameObject);
				}
			}
		}
	}

    

    public void SwitchFireMode() {

        if (GetActiveSubWeapon().reloading) {
            return;
        }

        NextSubWeapon();
    }

    public virtual Weapon GetActiveSubWeapon() {

        if (HasParentWeapon()) {
            // An AltWeapon cannot have other subweapons, so return itself.
            return this;
        }

        return GetSubWeapon(this.activeSubWeaponIndex);
    }

    public Weapon GetSubWeapon(int index) {
        if (index == -1) {
            return this;
        }

        return this.alternativeWeapons[index];
    }

    protected virtual void Update() {
		if(!this.stopFireLoop.Done()) {
			float ratio = (1f-this.stopFireLoop.Ratio());
			this.audio.volume = ratio*this.weaponVolume;

			if(this.stopFireLoop.TrueDone()) {
				this.audio.Stop();
			}
		}

		if(HasScopeObject()) {
			this.aimAmount = Mathf.MoveTowards(this.aimAmount, aiming ? 1f : 0f, Time.deltaTime*AIM_AMOUNT_SPEED);

			bool wasShowingScopeObject = this.showScopeObject;
			this.showScopeObject = this.aimAmount >= 0.95f;

			this.configuration.scopeAimObject.SetActive(this.showScopeObject);

			if(!wasShowingScopeObject && this.showScopeObject) {
				foreach(Renderer renderer in this.nonScopeRenderers) {
					renderer.enabled = false;
				}
			}
			else if(wasShowingScopeObject && !this.showScopeObject) {
				foreach(Renderer renderer in this.nonScopeRenderers) {
					renderer.enabled = true;
				}
			}
		}

		if(this.followupSpreadStayAction.TrueDone()) {
			this.followupSpreadMagnitude = Mathf.MoveTowards(this.followupSpreadMagnitude, 0f, this.followupSpreadDissipateRate*Time.deltaTime);
		}

		if(this.configuration.applyHeat) {
			UpdateHeat();
		}

        if (HasActiveAnimator() && this.unholstered) {
            this.animator.SetBool("tuck", Input.GetKey(KeyCode.LeftShift));

            if (HasActiveAnimator() && this.notifyRandom) {
                this.animator.SetFloat("random", Random.Range(0f, 100f));
            }

            if (HasActiveAnimator() && this.notifyCharge) {
                this.animator.SetBool("charging", !this.chargeAction.TrueDone());
            }

            UpdateSightModeAnimation();
        }
    }

    void UpdateSightModeAnimation() {

        if (HasParentWeapon() && this.useParentWeaponSightModes) {
            this.parentWeapon.UpdateSightModeAnimation();
            return;
        }

        this.animator.SetInteger("sight mode", this.activeSightModeIndex);

        if (this.notifySmoothSightMode) {
            this.smoothSightMode = Mathf.MoveTowards(Mathf.Lerp(this.smoothSightMode, this.activeSightModeIndex, 15f * Time.deltaTime), this.activeSightModeIndex, Time.deltaTime);
            this.animator.SetFloat("smooth sight mode", this.smoothSightMode);
        }
    }

    void UpdateHeat() {
		if(this.heatStayAction.TrueDone()) {
			this.heat = Mathf.Clamp01(this.heat - this.configuration.heatDrainRate*Time.deltaTime);
		}

		if(this.isOverheating) {
			this.isOverheating = this.heat > 0f;
		}

		this.animator.SetBool("overheat", this.isOverheating);

		if(this.configuration.heatMaterial.HasTarget()) {
			this.configuration.heatMaterial.Get().SetColor("_EmissionColor", Color.Lerp(Color.black, this.configuration.heatColor, this.configuration.heatColorGain.Evaluate(this.heat)));
		}
	}

	public virtual void Fire(Vector3 direction, bool useMuzzleDirection) {

        var subWeapon = this.GetActiveSubWeapon();

        if (subWeapon != this) {
            subWeapon.Fire(direction, useMuzzleDirection);
            return;
        }

        if (!this.unholstered || this.isOverheating) return;

		if(CanFire()) {
			if(this.configuration.auto && (!this.audio.isPlaying || !this.stopFireLoop.Done())) {
				StartFireLoop();
			}
			Shoot(direction, useMuzzleDirection);

			if(this.configuration.applyHeat) {
				this.heat = Mathf.Clamp01(this.heat+this.configuration.heatGainPerShot);
				this.heatStayAction.Start();

				this.isOverheating = this.heat == 1f;

				if(this.isOverheating) {
					StopFire();
					if(this.configuration.overheatParticles != null) {
						this.configuration.overheatParticles.Play();
					}
					if(this.configuration.overheatSound != null) {
						this.configuration.overheatSound.Play();
					}

				}
			}

			if(this.ammo == 0) {
				StopFire();
			}

		}

		if(this.ammo > 0 && !this.reloading) {
			if(!this.holdingFire) {
				if(this.configuration.useChargeTime) {
					this.chargeAction.Start();
				}

				if(this.configuration.chargeSound != null) {
					this.configuration.chargeSound.Play();
				}
			}

			this.holdingFire = true;
		}
	}

	void StartFireLoop() {
		this.audio.volume = this.weaponVolume;
		this.audio.Play();
		this.stopFireLoop.Stop();
		this.fireLoopPlaying = true;
	}

	void StopFireLoop() {
		if(this.fireLoopPlaying) {
			this.stopFireLoop.Start();
			this.fireLoopPlaying = false;
		}
	}

	public void StopFire() {

        var subWeapon = this.GetActiveSubWeapon();

        if (subWeapon != this) {
            subWeapon.StopFire();
            return;
        }

        if (this.configuration.auto) {
			StopFireLoop();
		}
		this.holdingFire = false;
		this.chargeAction.Stop();
		this.hasFiredSingleRoundThisTrigger = false;

		if(this.configuration.chargeSound != null) {
			this.configuration.chargeSound.Stop();
		}
	}

	public virtual void SetAiming(bool aiming) {

        var subWeapon = GetActiveSubWeapon();

        if (subWeapon != this) {
            subWeapon.SetAiming(aiming);
            return;
        }

        this.aiming = aiming;
		if(HasActiveAnimator()) {
            this.animator.SetBool("aim", aiming);
		}
	}

	public virtual void Reload(bool overrideHolstered = false) {

        var subWeapon = GetActiveSubWeapon();

        if (subWeapon != this) {
            subWeapon.Reload(overrideHolstered);
            return;
        }

        if (this.reloading || this.isLocked || this.configuration.spareAmmo < 0) return;

		if(this.fireLoopPlaying) {
			StopFireLoop();
		}

        if (HasActiveAnimator()) {
			this.animator.SetTrigger("reload");
		}

		DisableOverrideLayer();

		if(this.reloadAudio != null) {
			this.reloadAudio.Play();
		}
		this.holdingFire = false;
		this.chargeAction.Stop();
		this.reloading = true;

		if(this.configuration.dropAmmoWhenReloading) {
			int targetAmmo = Mathf.Min(this.ammo, this.configuration.maxRemainingAmmoAfterDrop);
			int ammoToReturn = Mathf.Max(0, this.ammo - targetAmmo);

			this.ammo = targetAmmo;
		}

		if(this.configuration.advancedReload) {
			StartAdvancedReload();
		}
		else {
            this.standardReload = StartCoroutine(StandardReloadRoutine());
        }
	}

    void NextSubWeapon() {
        int prevSubWeaponIndex = this.activeSubWeaponIndex;
        this.activeSubWeaponIndex++;
        if (this.activeSubWeaponIndex >= this.alternativeWeapons.Count) {
            this.activeSubWeaponIndex = -1;
        }

        if (this.activeSubWeaponIndex != prevSubWeaponIndex) {
            GetSubWeapon(prevSubWeaponIndex).HolsterSubWeapon();
            GetActiveSubWeapon().UnholsterSubWeapon(true);

            if (this.animator != null) {
                this.animator.SetInteger("alt weapon", this.activeSubWeaponIndex + 1);
            }
        }
    }

    public virtual bool NextSightMode() {

        if (HasParentWeapon() && this.useParentWeaponSightModes) {
            return this.parentWeapon.NextSightMode();
        }

        if (this.configuration.sightModes != null && this.configuration.sightModes.Length > 0) {
            this.activeSightModeIndex = (activeSightModeIndex + 1) % this.configuration.sightModes.Length;
            return true;
        }
        return false;
    }

    public virtual bool PreviousSightMode() {

        if (HasParentWeapon() && this.useParentWeaponSightModes) {
            return this.parentWeapon.PreviousSightMode();
        }

        if (this.configuration.sightModes != null && this.configuration.sightModes.Length > 0) {
            this.activeSightModeIndex = (activeSightModeIndex - 1 + this.configuration.sightModes.Length) % this.configuration.sightModes.Length;
            return true;
        }
        return false;
    }

    public float GetAimFov() {
        if (HasParentWeapon() && this.useParentWeaponSightModes) {
            return this.parentWeapon.GetAimFov();
        }

        if (!(this.configuration.sightModes != null && this.configuration.sightModes.Length > 0)) {
            return this.configuration.aimFov;
        }

        return this.configuration.sightModes[this.activeSightModeIndex].overrideFov ? this.configuration.sightModes[this.activeSightModeIndex].fov : this.configuration.aimFov;
    }

    public int GetSpareAmmo() {
		return this.configuration.spareAmmo;
	}

	void StartAdvancedReload() {
		this.animator.SetBool("reloading", true);
		AdvancedReloadNextMotion();
	}

	void AdvancedReloadNextMotion() {
		int remainingAmmo = this.configuration.ammo-this.ammo;

		if(remainingAmmo == 0) {
			EndAdvancedReload();
			return;
		}

		this.currentReloadMotion = 0;

		foreach(int availableMotion in this.configuration.allowedReloads) {
			if(availableMotion <= remainingAmmo && availableMotion >= this.currentReloadMotion) {
				this.currentReloadMotion = availableMotion;
			}
		}

		if(this.currentReloadMotion == 0) {
			EndAdvancedReload();
			return;
		}

		this.animator.SetInteger("reload motion", this.currentReloadMotion);
	}

	void EndAdvancedReload() {
		this.animator.SetBool("reloading", false);
	}

	public void MotionDone() {

        var subweapon = GetActiveSubWeapon();

        if(subweapon != this) {
            subweapon.MotionDone();
            return;
        }

		int loadedAmmo = this.currentReloadMotion;
		this.ammo = Mathf.Min(this.ammo + loadedAmmo, this.configuration.ammo);

		if(this.notifyAmmo) {
			this.animator.SetInteger("loaded ammo", this.ammo);
		}

		AdvancedReloadNextMotion();
	}

    IEnumerator StandardReloadRoutine() {
        yield return new WaitForSeconds(this.configuration.reloadTime);
        ReloadDone();
    }

    public void ReloadDone() {

        var subweapon = GetActiveSubWeapon();

        if (subweapon != this) {
            subweapon.ReloadDone();
            return;
        }

        if (this.standardReload != null) {
            StopCoroutine(this.standardReload);
            this.standardReload = null;
        }

        EnableOverrideLayer();

		if(this.configuration.useMaxAmmoPerReload) {
			this.ammo = Mathf.Min(this.configuration.ammo, this.ammo + this.configuration.maxAmmoPerReload);
		}
		else {
			this.ammo = this.configuration.ammo;
		}

		if(this.notifyAmmo) {
			this.animator.SetInteger("loaded ammo", this.ammo);
		}

		this.reloading = false;
	}

	void DisableOverrideLayer() {
		if(HasActiveAnimator() && this.animator.layerCount > 1) {
			this.animator.SetLayerWeight(1, 0f);
		}
	}

	void EnableOverrideLayer() {
		if(HasActiveAnimator() && this.animator.layerCount > 1) {
			this.animator.SetLayerWeight(1, 1f);
		}
	}

	public virtual bool CanFire() {
		return this.ammo != 0 && !this.reloading && !this.isLocked && (this.configuration.auto || !this.hasFiredSingleRoundThisTrigger) && !CoolingDown() && (!this.isOverheating || !this.configuration.applyHeat) && (!this.configuration.useChargeTime || (this.chargeAction.TrueDone() && this.holdingFire));
	}

	public bool CoolingDown() {
		return Time.time-this.lastFiredTimestamp < this.configuration.cooldown;
	}

	protected virtual void Shoot(Vector3 direction, bool useMuzzleDirection) {

		this.followupSpreadMagnitude = Mathf.Clamp(this.followupSpreadMagnitude, 0f, this.aiming ? this.configuration.followupMaxSpreadAim : this.configuration.followupMaxSpreadHip);

		this.lastFiredTimestamp = Time.time;

		if(HasActiveAnimator()) {
			if(!this.configuration.fireFromAllMuzzles && this.configuration.muzzles.Length > 1) {
				this.animator.SetInteger("muzzle", this.currentMuzzle);
			}

			this.animator.SetTrigger("fire");
		}

		if(this.configuration.fireFromAllMuzzles) {
			for(int i = 0; i < this.configuration.muzzles.Length; i++) {
				FireFromMuzzle(i, direction, useMuzzleDirection);
			}
		}
		else {
			FireFromMuzzle(this.currentMuzzle, direction, useMuzzleDirection);
		}

		WeaponUser.instance.ApplyRecoil(this.configuration.kickback*Vector3.back+Random.insideUnitSphere*this.configuration.randomKick);
		WeaponUser.instance.ApplyWeaponSnap(this.configuration.snapMagnitude, this.configuration.snapDuration, this.configuration.snapFrequency);

		if(this.reflectionVolume > 0f && this.reflectionSound != ReflectionSound.None) {
			WeaponUser.instance.PlayReflectionSound(this.reflectionSound, this.reflectionVolume);
		}

		if(!this.configuration.auto) {
			this.audio.Play();
			this.hasFiredSingleRoundThisTrigger = true;
		}
		else if(this.ammo == 0) {
			StopFireLoop();
		}

		this.followupSpreadMagnitude = Mathf.Clamp(this.followupSpreadMagnitude+this.configuration.followupSpreadGain, 0f, this.aiming ? this.configuration.followupMaxSpreadAim : this.configuration.followupMaxSpreadHip);
		this.followupSpreadStayAction.StartLifetime(this.configuration.followupSpreadStayTime);
		this.currentMuzzle = (this.currentMuzzle+1)%this.configuration.muzzles.Length;

		if(this.ammo > 0) {
			this.ammo = Mathf.Max(this.ammo-1, 0);
		}

		if(this.notifyAmmo) {
			this.animator.SetInteger("loaded ammo", this.ammo);
		}
	}

	void FireFromMuzzle(int muzzleIndex, Vector3 direction, bool useMuzzleDirection) {

		Transform muzzle = this.configuration.muzzles[muzzleIndex];

		if(useMuzzleDirection) {
			direction = muzzle.forward;
		}

		for(int i = 0; i < this.configuration.projectilesPerShot; i++) {
			SpawnProjectile(direction, muzzle.position);
		}

		if(this.muzzleFlash.ContainsKey(muzzle)) {
			this.muzzleFlash[muzzle].Play(true);
		}

		if(this.configuration.casingParticles.Length > 0) {
			this.configuration.casingParticles[muzzleIndex%this.configuration.casingParticles.Length].Play(false);
		}
	}

	public Transform CurrentMuzzle() {
		return this.configuration.muzzles[this.currentMuzzle];
	}

	protected bool HasActiveAnimator() {
		return this.animator != null && this.animator.isActiveAndEnabled;
	}

	protected virtual Projectile SpawnProjectile(Vector3 direction, Vector3 position) {
		float spread = this.configuration.spread+this.followupSpreadMagnitude;
		Quaternion rotation = Quaternion.LookRotation(direction+Random.insideUnitSphere*spread);
		Projectile projectile = ((GameObject) GameObject.Instantiate(this.configuration.projectilePrefab, position, rotation)).GetComponent<Projectile>();

		return projectile;
	}

    // Holster subweapon, but don't activate game objects.
    public virtual void UnholsterSubWeapon(bool switchedFromSubWeapon) {
        this.unholstered = false;
        this.holdingFire = false;
        this.hasFiredSingleRoundThisTrigger = false;
        this.switchedFromSubWeapon = switchedFromSubWeapon;
        this.isLocked = false;

        if (switchedFromSubWeapon && this.changeFireModeAudio != null) {
            this.changeFireModeAudio.Play();
        }

        DisableOverrideLayer();
        Invoke("UnholsterDone", switchedFromSubWeapon ? this.configuration.changeFireModeTime : this.configuration.unholsterTime);
    }

    // Unholster entire weapon, including activating game objects.
    public virtual void Unholster() {

        UnholsterSubWeapon(false);

        if (this.configuration.unholsterIsReload) {
            ReloadDone();
        }

        this.unholstered = false;
		this.aiming = false;
		if(HasActiveAnimator()) {
			this.animator.SetTrigger("unholster");
		}
	}

	public void UnholsterDone() {
		EnableOverrideLayer();
		this.unholstered = true;

        if (this.switchedFromSubWeapon && this.configuration.changeFireModeIsReload) {
            ReloadDone();
        }
    }

    public virtual void HolsterSubWeapon() {
        this.unholstered = false;
        this.reloading = false;
        this.holdingFire = false;
        this.chargeAction.Stop();

        if (this.fireLoopPlaying) {
            StopFireLoop();
        }

        if (this.reloadAudio != null && this.reloadAudio.isPlaying) {
            this.reloadAudio.Stop();
        }

        CancelInvoke();
    }

    public void LockWeapon() {

        var subweapon = GetActiveSubWeapon();

        if (subweapon != this) {
            subweapon.LockWeapon();
            return;
        }

        this.isLocked = true;
    }

    public void UnlockWeapon() {

        var subweapon = GetActiveSubWeapon();

        if (subweapon != this) {
            subweapon.UnlockWeapon();
            return;
        }

        this.isLocked = false;
    }

    public virtual bool CanBeAimed() {
		return (this.configuration.canBeAimedWhileReloading || (!this.reloading && !this.isLocked)) && this.unholstered;
	}

	bool HasScopeObject() {
		return this.configuration.scopeAimObject != null;
	}

	public virtual bool ShouldHaveProjectilePrefab() {
		return true;
	}

	[System.Serializable]
	public class Configuration {
		public bool auto = false;
		public int ammo = 10;
		public int spareAmmo = 50;
		public int resupplyNumber = 10;
		public float reloadTime = 2f;
		public float cooldown = 0.2f;
		public float unholsterTime = 1.2f;
		public bool unholsterIsReload = false;
        public float changeFireModeTime = 0.3f;
        public bool changeFireModeIsReload = false;
        public float aimFov = 50f;
		public bool forceSniperAimSensitivity = false;
		public float aimSensitivityMultiplier = 1f;
		public float autoReloadDelay = 0f;

		public bool canBeAimedWhileReloading = false;
		public bool forceAutoReload = false;
		public bool loud = true;
		public bool forceWorldAudioOutput = false;

		public Transform[] muzzles;
		public Transform[] optionalThirdPersonMuzzles;
		public ParticleSystem[] casingParticles;

		public bool fireFromAllMuzzles = false;
		public int projectilesPerShot = 1;
		public GameObject projectilePrefab;

		public GameObject scopeAimObject;

		public float kickback = 0.5f;
		public float randomKick = 0.05f;
		public float spread = 0.001f;

		public float followupSpreadGain = 0.005f;
		public float followupMaxSpreadHip = 0.05f;
		public float followupMaxSpreadAim = 0.02f;
		public float followupSpreadStayTime = 0.2f;
		public float followupSpreadDissipateTime = 1f;

		public float snapMagnitude = 0.3f;
		public float snapDuration = 0.4f;
		public float snapFrequency = 4f;

        public float kickbackProneMultiplier = 0.6f;
        public float spreadProneMultiplier = 1f;
        public float followupSpreadProneMultiplier = 0.5f;
        public float snapProneMultiplier = 0.5f;

        public float aiAllowedAimSpread = 1f;
		public float aiAimSwing = 0f;
		public Effectiveness effInfantry = Effectiveness.Yes;
		public Effectiveness effInfantryGroup = Effectiveness.No;
		public Effectiveness effUnarmored = Effectiveness.Yes;
		public Effectiveness effArmored = Effectiveness.No;
		public Effectiveness effAir = Effectiveness.No;
		public Effectiveness effAirFastMover = Effectiveness.No;
		public float effectiveRange = 100f;
		public HaltStrategy haltStrategy = HaltStrategy.Auto;

		public int pose = 0;

		public bool applyHeat = false;
		public MaterialTarget heatMaterial;
		public float heatGainPerShot = 0f;
		public float heatDrainRate = 0.25f;
		public Color heatColor;
		public AnimationCurve heatColorGain;
		public ParticleSystem overheatParticles;
		public AudioSource overheatSound;

		public bool useChargeTime = false;
		public float chargeTime = 0.5f;
		public AudioSource chargeSound;

		public bool dropAmmoWhenReloading = false;
		public int maxRemainingAmmoAfterDrop = 0;

		public bool useMaxAmmoPerReload = false;
		public int maxAmmoPerReload = 30;

		public bool advancedReload = false;
		public int[] allowedReloads;

        public SightMode[] sightModes;
    }

    [System.Serializable]
    public struct SightMode {
        public bool overrideFov;
        public float fov;
        public string name;
    }
}
