using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class ParticlesAmbientColor : MonoBehaviour {

    public float weight = 1f;
    public float ambientIntensity = 1f;
    public float sunlightIntensity = 0.5f;
    public bool useSunlightRaycast = false;

    Color sceneAmbientColor;
    Color sceneSunlightColor;
    Light sceneSunlight;

    // Use this for initialization
    void Start () {

        SetupColors();

        var particleSystem = GetComponent<ParticleSystem>();
        var main = particleSystem.main;
        var startColor = main.startColor;

        float sunlightMultiplier = this.sunlightIntensity;

        if(this.useSunlightRaycast && !IsInSunlight()) {
            sunlightMultiplier = 0f;
        }

        startColor.color = BlendColor(startColor.color, sunlightMultiplier);
        startColor.colorMax = BlendColor(startColor.colorMax, sunlightMultiplier);
        startColor.colorMin = BlendColor(startColor.colorMin, sunlightMultiplier);

        main.startColor = startColor;
    }

    void SetupColors() {
        if (RenderSettings.ambientMode == UnityEngine.Rendering.AmbientMode.Trilight) {
            this.sceneAmbientColor = RenderSettings.ambientSkyColor * 0.333f + RenderSettings.ambientEquatorColor * 0.333f + RenderSettings.ambientGroundColor * 0.333f;
        }
        else if (RenderSettings.ambientMode == UnityEngine.Rendering.AmbientMode.Flat) {
            this.sceneAmbientColor = RenderSettings.ambientLight;
        }
        else if (RenderSettings.ambientMode == UnityEngine.Rendering.AmbientMode.Skybox || RenderSettings.ambientMode == UnityEngine.Rendering.AmbientMode.Custom) {
            Vector3[] directions = new Vector3[] { Vector3.up, Vector3.forward, Vector3.down };
            Color[] colors = new Color[directions.Length];
            RenderSettings.ambientProbe.Evaluate(directions, colors);

            this.sceneAmbientColor = Color.black;
            for (int i = 0; i < colors.Length; i++) {
                this.sceneAmbientColor += colors[i] / colors.Length;
            }

            this.sceneAmbientColor *= RenderSettings.ambientIntensity;
        }
        this.sceneAmbientColor.a = 1f;

        this.sceneSunlight = null;

        Light[] lights = FindObjectsOfType<Light>();
        float maxIntensity = 0f;
        foreach (Light light in lights) {
            if (light.type == LightType.Directional) {
                if (light.intensity > maxIntensity) {
                    this.sceneSunlight = light;
                    maxIntensity = light.intensity;
                }
            }
        }

        if (this.sceneSunlight != null) {
            this.sceneSunlightColor = this.sceneSunlight.color * this.sceneSunlight.intensity;
            this.sceneSunlightColor.a = 1f;
        }
    }

    bool IsInSunlight() {
        if(this.sceneSunlight == null) {
            return true;
        }
        Ray ray = new Ray(this.transform.position, -this.sceneSunlight.transform.forward);
        return !Physics.Raycast(ray, 9999f, 1);
    }


    Color BlendColor(Color c, float sunlightMultiplier) {
        Color targetColor = c * (this.ambientIntensity * this.sceneAmbientColor + sunlightMultiplier * this.sceneSunlightColor);
        return Color.Lerp(c, targetColor, this.weight);
    }
}
