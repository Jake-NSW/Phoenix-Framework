using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponUser : MonoBehaviour {

	const float REFLECTION_BASE_VOLUME = 0.7f;

	public static WeaponUser instance;
	public Text text;
    public Text targetText;

    public Transform bullseyeWorldObject;
	public RectTransform bullseye;
	public GameObject hitIndicatorPrefab;
	public GameObject hitPanel;
	public GameObject hitPanelText;

    public Transform targetCube;

	public GameObject canvasObject;

	public AudioSource reflectionAudioSource;
	public AudioClip[] reflectionAudioClips;

	public AnimationCurve reflectionDuckCurve;

    int targetDistance = 50;

	Weapon weapon;

	AudioSource[] audioSources;
	Dictionary<AudioSource, float> defaultPitch;

	Action reflectionDuckAction = new Action(0.25f);

    WeaponHitIndicator lastHit;

	// Use this for initialization
	void Start () {
		canvasObject.SetActive(true);

        var weapons = FindObjectsOfType<Weapon>();

        foreach(var weapon in weapons) {
            if(!weapon.HasParentWeapon()) {
                SetWeapon(weapon);
            }
        }

		this.audioSources = FindObjectsOfType<AudioSource>();
		this.defaultPitch = new Dictionary<AudioSource, float>(this.audioSources.Length);

		foreach(AudioSource audioSource in this.audioSources) {
			this.defaultPitch.Add(audioSource, audioSource.pitch);
		}
	}

	void SetWeapon(Weapon weapon) {

        Debug.Log("Setting weapon: " + weapon.gameObject.name);

		WeaponHolderPreview previewHolder = FindObjectOfType<WeaponHolderPreview>();

		Weapon thirdPersonWeapon = null;

		if(previewHolder != null) {
			thirdPersonWeapon = GameObject.Instantiate(weapon.gameObject).GetComponent<Weapon>();
		}

		this.weapon = weapon;
		this.weapon.transform.parent = this.weaponParent;
		this.weapon.transform.localPosition = Vector3.zero;
		this.weapon.transform.localRotation = Quaternion.identity;

		SetAimFov(weapon.GetAimFov());

		if(previewHolder != null) {
			thirdPersonWeapon.CullFpsObjects();
			thirdPersonWeapon.enabled = false;
			previewHolder.HoldWeapon(thirdPersonWeapon);
		}
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetMouseButton(0)) {
			this.weapon.Fire(this.transform.forward, true);
		}
		else {
			this.weapon.StopFire();
		}

        var subWeapon = weapon.GetActiveSubWeapon();

		this.aiming = Input.GetMouseButton(1) && !subWeapon.reloading && subWeapon.unholstered;
		this.weapon.SetAiming(this.aiming);

		if(Input.GetKeyDown(KeyCode.R)) {
			this.weapon.Reload();
		}

        if (Input.GetKeyDown(KeyCode.X)) {
            this.weapon.SwitchFireMode();
        }

        if(Input.GetKey(KeyCode.T)) {
            if(Input.mouseScrollDelta.y > 0f) {
                subWeapon.NextSightMode();
            }
            else if(Input.mouseScrollDelta.y < 0f) {
                subWeapon.PreviousSightMode();
            }
        }

        if(Input.GetMouseButtonDown(4)) {
            subWeapon.NextSightMode();
        }

        if (Input.GetMouseButtonDown(3)) {
            subWeapon.PreviousSightMode();
        }

        if (Input.GetKeyDown(KeyCode.Alpha1)) {
			this.weapon.Unholster();
		}

		if(Input.GetKeyDown(KeyCode.CapsLock)) {
			if(Time.timeScale > 0.9f) {
				Time.timeScale = 0.2f;
			}
			else {
				Time.timeScale = 1f;
			}

			foreach(AudioSource audioSource in this.audioSources) {
				if(audioSource != null) {
					audioSource.pitch = Time.timeScale*this.defaultPitch[audioSource];
				}
			}
		}

        if(Input.GetKeyDown(KeyCode.Plus) ||Input.GetKeyDown(KeyCode.KeypadPlus)) {
            IncrementTargetDistance(50);
        }

        if (Input.GetKeyDown(KeyCode.Minus) || Input.GetKeyDown(KeyCode.KeypadMinus)) {
            IncrementTargetDistance(-50);
        }

        if (Input.GetKeyDown(KeyCode.T)) {
			this.hitPanel.SetActive(!this.hitPanel.activeSelf);
			this.hitPanelText.SetActive(!this.hitPanel.activeSelf);
		}

        SetAimFov(weapon.GetAimFov());

        ApplyMotion();

        var sightWeapon = subWeapon;

        if(subWeapon.HasParentWeapon() && subWeapon.useParentWeaponSightModes) {
            sightWeapon = subWeapon.parentWeapon;
        }

        string infoText = "";
        if(subWeapon.isLocked) {
            infoText += "WEAPON MANUALLY LOCKED!";
        }
        if (sightWeapon.configuration.sightModes != null && sightWeapon.configuration.sightModes.Length > 0) {
            infoText += "\nSIGHT: " + sightWeapon.configuration.sightModes[sightWeapon.activeSightModeIndex].name + " ("+sightWeapon.activeSightModeIndex+")";
        }
        if(this.weapon.HasAltModes()) {
            infoText += "\nMODE: " + subWeapon.gameObject.name + " (" + this.weapon.activeSubWeaponIndex + ")";
        }
        infoText += subWeapon.ammo.ToString();

        this.text.text = infoText;
    }

    void IncrementTargetDistance(int delta) {
        this.targetDistance = Mathf.Max(50, this.targetDistance + delta);

        this.targetCube.transform.position = new Vector3(0f, 0f, this.targetDistance);
        this.targetText.text = this.targetDistance + " meters";
    }


    void LateUpdate() {
		if(this.reflectionDuckAction.TrueDone()) {
			this.reflectionAudioSource.volume = REFLECTION_BASE_VOLUME;
		}
		else {
			this.reflectionAudioSource.volume = this.reflectionDuckCurve.Evaluate(this.reflectionDuckAction.Ratio()) * REFLECTION_BASE_VOLUME;
		}
	}

	const float CAMERA_HIP_FOV = 60f;
	const float CAMERA_AIM_FOV = 45f;
	const float FOV_SPEED = 150f;

	const float POSITION_SPRING = 150f;
	const float POSITION_DRAG = 10f;
	const float MAX_POSITION_OFFSET = 0.2f;
	const int POSITION_SPRING_ITERAIONS = 8;

	const float ROTATION_SPRING = 70f;
	const float ROTATION_DRAG = 6f;
	const float MAX_ROTATION_OFFSET = 15f;
	const float ROTATION_IMPULSE_GAIN = 100f;
	const int ROTATION_SPRING_ITERAIONS = 8;

	const float CAMERA_KICK_MULTIPLIER = 0.7f;
	const float WEAPON_EXTRA_PITCH = 0.1f;

	const float LEAN_MAX_HEAD_OFFSET = 0.4f;
	const float LEAN_MAX_WEAPON_OFFSET_LEFT = 0.23f;
	const float LEAN_MAX_WEAPON_OFFSET_RIGHT = 0.1f;
	const float LEAN_MAX_CAMERA_ROLL_ANGLE = 3f;
	const float LEAN_MAX_ROLL_ANGLE = 12f;
	const float LEAN_HEAD_SPHERECAST_RADIUS = 0.3f;

	const float SCREENSHAKE_WEAPON_SHAKE = 0.1f;
	const float SCREENSHAKE_ITERATION_TIME = 0.17f;

	const float WALK_OFFSET_MAGNITUDE_AIMING = 0.003f;
	const float WALK_OFFSET_MAGNITUDE = 0.06f;
	const float WALK_OFFSET_MAGNITUDE_CHANGE_SPEED = 0.06f;

	Camera fpCamera;
	//Transform fpCameraParent;

	Vector3 localOrigin = Vector3.zero;
	Spring positionSpring;
	Spring rotationSpring;
	Transform movementProbe;
	Vector3 lastPosition = Vector3.zero;
	Vector3 lastRotation = Vector3.zero;

	Transform weaponParent;
	//Vector3 weaponParentOrigin;

	//Vector3 kickEuler = Vector3.zero;
	Action fpKickAction = new Action(0.2f);
	Action weaponSnapAction = new Action(0.5f);
	float weaponSnapMagnitude = 0.5f;
	float weaponSnapFrequency = 5f;

	[System.NonSerialized] public float lean;

	float fovRatio;
	float fovSpeed = 5f;
	bool aiming = false;

	bool screenshake = false;
	Coroutine screenshakeCoroutine;

	float verticalFov = CAMERA_HIP_FOV;

	float normalFov = CAMERA_HIP_FOV;
	float zoomFov = CAMERA_AIM_FOV;

	//float walkOffsetMagnitude = 0f;

	//float extraPitch = 0f;

	void Awake() {

		instance = this;

		this.fpCamera = Camera.main;

		this.fpCamera.fieldOfView = CAMERA_HIP_FOV;
		this.movementProbe = this.transform.parent;
		this.positionSpring = new Spring(POSITION_SPRING, POSITION_DRAG, -Vector3.one*MAX_POSITION_OFFSET, Vector3.one*MAX_POSITION_OFFSET, POSITION_SPRING_ITERAIONS);
		this.rotationSpring = new Spring(ROTATION_SPRING, ROTATION_DRAG, -Vector3.one*MAX_ROTATION_OFFSET, Vector3.one*MAX_ROTATION_OFFSET, ROTATION_SPRING_ITERAIONS);
		this.weaponParent = this.transform;
		//this.weaponParentOrigin = this.weaponParent.transform.localPosition;
		//this.fpCameraParent = this.fpCamera.transform.parent;

		this.movementProbe = this.transform;

		this.localOrigin = this.transform.localPosition;
		this.lastPosition = this.movementProbe.position;
		this.lastRotation = this.movementProbe.eulerAngles;

		SetupHorizontalFov(90f);
		SetAimFov(CAMERA_AIM_FOV);
	}

	void ApplyMotion() {

		/*float stepPhase = this.fpsController.StepCycle()*Mathf.PI;

		float targetWalkOffsetMagnitude = 0f;

		if(!this.actor.IsSeated()) {
			targetWalkOffsetMagnitude = Mathf.Clamp01(this.actor.Velocity().sqrMagnitude/60f)*(this.aiming ? WALK_OFFSET_MAGNITUDE_AIMING : WALK_OFFSET_MAGNITUDE);
		}
		this.walkOffsetMagnitude = Mathf.MoveTowards(this.walkOffsetMagnitude, targetWalkOffsetMagnitude, Time.deltaTime*WALK_OFFSET_MAGNITUDE_CHANGE_SPEED);
		Vector3 walkOffset = new Vector3(Mathf.Cos(stepPhase)*this.walkOffsetMagnitude, Mathf.Sin(stepPhase*2f)*this.walkOffsetMagnitude*0.7f, 0f);*/

		Vector3 walkOffset = Vector3.zero;

		this.positionSpring.Update();
		this.rotationSpring.Update();

		Vector3 localDeltaMovement = this.transform.worldToLocalMatrix.MultiplyVector(this.movementProbe.position - this.lastPosition);
		Vector2 deltaRotation = new Vector2(Mathf.DeltaAngle(lastRotation.x, this.movementProbe.eulerAngles.x), Mathf.DeltaAngle(lastRotation.y, this.movementProbe.eulerAngles.y));
		this.lastPosition = this.movementProbe.position;
		this.lastRotation = this.movementProbe.eulerAngles;

		float weaponSnap = 0f;

		if(!weaponSnapAction.TrueDone()) {
			//float falloff = Mathf.Pow(1f-this.weaponSnapAction.Ratio(), 2);
			float falloff = 1f-this.weaponSnapAction.Ratio();
			weaponSnap = falloff*Mathf.Sin(this.weaponSnapAction.Ratio()*(0.1f+falloff)*this.weaponSnapFrequency)*this.weaponSnapMagnitude;
		}

		this.transform.localPosition = this.localOrigin + this.positionSpring.position + Vector3.down*weaponSnap*0.1f + walkOffset;
		this.transform.localEulerAngles = this.rotationSpring.position + Vector3.left*weaponSnap*20f;

		//this.positionSpring.position -= 0.03f*localDeltaMovement;
		this.rotationSpring.position += new Vector3(-0.1f*deltaRotation.x+localDeltaMovement.y*5f, -0.1f*deltaRotation.y, 0f);
		this.positionSpring.position += new Vector3(-0.0001f*deltaRotation.y, 0.0001f*deltaRotation.x, 0f);

		this.fovRatio = Mathf.MoveTowards(this.fovRatio, this.aiming ? 1f : 0f, Time.deltaTime*this.fovSpeed);
		this.fpCamera.fieldOfView = Mathf.Lerp(this.normalFov, this.zoomFov, this.fovRatio);
	}

	/*void LateUpdate() {
		Vector3 localDeltaPosition = Vector3.right*lean*LEAN_MAX_HEAD_OFFSET;
		Vector3 targetDeltaPosition = this.fpCamera.transform.localToWorldMatrix.MultiplyVector(localDeltaPosition);
		Ray ray = new Ray(this.fpCamera.transform.parent.position, targetDeltaPosition.normalized);

		RaycastHit hitInfo;
		if(Physics.SphereCast(ray, LEAN_HEAD_SPHERECAST_RADIUS, out hitInfo, targetDeltaPosition.magnitude, 1)) {
			localDeltaPosition = localDeltaPosition.normalized*hitInfo.distance;
		}

		this.fpCamera.transform.localPosition = localDeltaPosition;


		Vector3 fpCameraLocalEuler = Vector3.back*lean*LEAN_MAX_CAMERA_ROLL_ANGLE;
		Vector3 localTarget = this.weaponParentOrigin;

		if(!this.fpKickAction.Done()) {
			float offset = Mathf.Sin(this.fpKickAction.Ratio()*Mathf.PI);
			fpCameraLocalEuler += offset * this.kickEuler;
		}

		this.fpCamera.transform.localEulerAngles = fpCameraLocalEuler;

		if(!this.aiming) {
			if(this.lean > 0f) {
				localTarget += new Vector3(1f, -1f, 0f)*lean*LEAN_MAX_WEAPON_OFFSET_RIGHT;
			}
			else {
				localTarget += new Vector3(1f, 0.3f, 0f)*lean*LEAN_MAX_WEAPON_OFFSET_LEFT;
			}
		}

		Vector3 newLocalPosition = Vector3.MoveTowards(this.weaponParent.transform.localPosition, localTarget, 2f*Time.deltaTime);
		// Do ray cast?

		this.weaponParent.transform.localPosition = newLocalPosition;

		this.extraPitch = Mathf.MoveTowards(this.extraPitch, this.aiming ? 0f : WEAPON_EXTRA_PITCH, Time.deltaTime);

		this.weaponParent.transform.localEulerAngles = Vector3.right*Mathf.DeltaAngle(0f, this.fpCameraParent.localEulerAngles.x)*extraPitch + Vector3.back*lean*LEAN_MAX_ROLL_ANGLE;
	}*/

	public void ApplyScreenshake(float magnitude, int iterations) {
		if(this.screenshake) {
			this.StopCoroutine(this.screenshakeCoroutine);
		}
		this.screenshakeCoroutine = StartCoroutine(Screenshake(magnitude, iterations));
	}

	IEnumerator Screenshake(float magnitude, int iterations) {
		this.screenshake = true;
		for(int i = 0; i < iterations; i++) {
			float iterationMagnitude = magnitude*(((float)(iterations-i))/iterations);
			this.positionSpring.AddVelocity(Random.insideUnitSphere*iterationMagnitude*SCREENSHAKE_WEAPON_SHAKE);
			KickCamera(Random.insideUnitSphere*iterationMagnitude);
			yield return new WaitForSeconds(SCREENSHAKE_ITERATION_TIME);
		}

		this.screenshake = false;
	}

	public void ApplyRecoil(Vector3 impulse) {
		this.positionSpring.AddVelocity(impulse);

		Vector3 kickEuler = new Vector3(impulse.z, impulse.x, -impulse.x);

		float rotationMultiplier = 0.1f+this.positionSpring.position.magnitude/MAX_POSITION_OFFSET;
		this.rotationSpring.AddVelocity(kickEuler*rotationMultiplier*ROTATION_IMPULSE_GAIN);

		if(!this.screenshake) {
			KickCamera(kickEuler);
		}
	}

	public void ApplyWeaponSnap(float magnitude, float duration, float frequency) {
		weaponSnapAction.StartLifetime(duration);
		this.weaponSnapFrequency = frequency;
		this.weaponSnapMagnitude = magnitude;
	}

	public void SetupHorizontalFov(float hFov) {
		float aspectRatio = ((float) Screen.width)/Screen.height;
		this.verticalFov = 2f*Mathf.Rad2Deg*Mathf.Atan(Mathf.Tan((hFov/2)*Mathf.Deg2Rad)/aspectRatio);

		this.fpCamera.fieldOfView = this.verticalFov;
		this.normalFov = this.verticalFov;
	}

	public void SetAimFov(float zoom) {
		SetFov(this.verticalFov, zoom);
	}

	public void SetFov(float normal, float zoom) {
		this.normalFov = normal;
		this.zoomFov = zoom;
	}

	public void KickCamera(Vector3 kick) {
		this.fpKickAction.Start();
		//this.kickEuler = kick*CAMERA_KICK_MULTIPLIER;
	}

	public void PlayReflectionSound(Weapon.ReflectionSound sound, float volume) {
		int soundIndex = ((int) sound)-2;

		if(soundIndex < 0) return;

		this.reflectionDuckAction.Start();

		this.reflectionAudioSource.Stop();
		this.reflectionAudioSource.PlayOneShot(this.reflectionAudioClips[soundIndex], volume);
	}

	public static void RegisterHit(Vector3 point) {

		Vector3 localBullseye = instance.bullseyeWorldObject.worldToLocalMatrix.MultiplyPoint(point);

		if(Mathf.Abs(localBullseye.z) > 0.5f) return;

		RectTransform rt = (RectTransform) GameObject.Instantiate(instance.hitIndicatorPrefab, instance.bullseye).transform;
		Vector2 anchor = new Vector2(localBullseye.x, localBullseye.y);
		rt.anchorMin = anchor;
		rt.anchorMax = anchor;

        if(instance.lastHit != null) {
            instance.lastHit.SetColor(Color.black);
        }
        instance.lastHit = rt.GetComponent<WeaponHitIndicator>();
        instance.lastHit.SetColor(new Color(1f, 0.2f, 0.2f));
    }
}
