using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TimeOfDay : MonoBehaviour {

	public static TimeOfDay instance;

	public Atmosphere nightAtmosphere;
	[System.NonSerialized] public Atmosphere atmosphere;

	public bool testNight = false;

	// Use this for initialization
	void Awake () {

		instance = this;

	}

	void Start() {
		if(this.testNight) {
			ApplyNight();
		}
		else {
			ApplyDay();
		}
	}

	void ApplyDay() {
		this.transform.Find("Day").gameObject.SetActive(true);
		this.transform.Find("Night").gameObject.SetActive(false);

		this.atmosphere = new Atmosphere();
		this.atmosphere.sky = RenderSettings.ambientSkyColor;
		this.atmosphere.equator = RenderSettings.ambientEquatorColor;
		this.atmosphere.ground = RenderSettings.ambientGroundColor;

		this.atmosphere.fog = RenderSettings.fogColor;
		this.atmosphere.fogDensity = RenderSettings.fogDensity;

		this.atmosphere.skyboxMaterial = RenderSettings.skybox;

		ApplyAtmosphere(this.atmosphere);
	}

	void ApplyNight() {
		this.transform.Find("Day").gameObject.SetActive(false);
		this.transform.Find("Night").gameObject.SetActive(true);

		ApplyAtmosphere(this.nightAtmosphere);
	}

	void ApplyAtmosphere(Atmosphere atmosphere) {

		this.atmosphere = atmosphere;

		RenderSettings.ambientSkyColor = atmosphere.sky;
		RenderSettings.ambientEquatorColor = atmosphere.equator;
		RenderSettings.ambientGroundColor = atmosphere.ground;

		RenderSettings.fogColor = atmosphere.fog;
		RenderSettings.fogDensity = atmosphere.fogDensity;

		RenderSettings.skybox = new Material(atmosphere.skyboxMaterial);
	}

	[System.Serializable]
	public class Atmosphere {
		public Color sky;
		public Color equator;
		public Color ground;

		public float fogDensity;
		public Color fog;

		public Material skyboxMaterial;
	}
}
