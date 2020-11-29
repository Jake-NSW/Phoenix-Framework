using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Vehicle : MonoBehaviour {

	public enum AiType {Capture, Roam, Transport};
	public enum ArmorRating {SmallArms, HeavyArms, AntiTank};
	public enum AiUseStrategy {Default, OnlyFromFrontlineSpawn, FromAnySpawn};
	public enum AiAttackStrategy {Default, Random, BehindEnemyLines};

	new public string name = "VEHICLE";

	public List<Seat> seats = new List<Seat>();
	public Animator animator;
	public Actor.TargetType targetType = Actor.TargetType.Unarmored;
	public ArmorRating armorDamagedBy = ArmorRating.HeavyArms;
	public float smallArmsMultiplier = 0.05f;
	public float heavyArmsMultiplier = 1f;

	public bool canBeRepairedAfterDeath = false;

	public float maxHealth = 1000f;
	public float crashDamageSpeedThrehshold = 2f;
	public float crashDamageMultiplier = 1f;
	public float spotChanceMultiplier = 3f;
	public float burnTime = 0f;
	public bool crashSkipsBurn = false;
	public bool directJavelinPath = false;
	public bool canCapturePoints = true;

	public Transform targetLockPoint;

	public AiType aiType;
	public AiUseStrategy aiUseStrategy = AiUseStrategy.Default;
	public bool aiUseToDefendPoint = true;
	public int minCrewCount = 0;
	public float roamCompleteDistance = 0f;

	public AudioSource interiorAudioSource;

	public ParticleSystem smokeParticles;
	public ParticleSystem fireParticles;
	public AudioSource fireAlarmSound;
	public ParticleSystem deathParticles;
	public AudioSource deathSound;

	public AudioSource impactAudio;
	public AudioSource heavyDamageAudio;

	public Transform blockSensor;

	public Texture blip;
	public float blipScale = 1f;

	public Vector2 avoidanceSize = Vector2.one;
	public float pathingRadius = 0f;

	public Vector3 ramSize = Vector3.one;
	public Vector3 ramOffset = Vector3.zero;

	public GameObject[] disableOnDeath;
	public GameObject[] activateOnDeath;
	public MaterialTarget[] teamColorMaterials;

	public Engine engine;

	public bool hasCountermeasures = false;
	public float countermeasuresActiveTime = 3f;
	public float countermeasuresCooldown = 20f;
	public ParticleSystem countermeasureParticles;
    public GameObject countermeasureSpawnPrefab;
    public AudioSource countermeasureAudio;

	[System.Serializable]
	public class Engine {
		public bool controlAudio = true;
		public float powerGainSpeed = 1f;
		public float pitchGainSpeed = 1f;
        public float throttleGainSpeed = 2f;
        public AudioSource throttleAudioSource;
        public AudioSource extraAudioSource;
        public AudioClip shiftForwardClip;
        public AudioClip shiftReverseClip;
        public AudioClip ignitionClip;
    }
}
