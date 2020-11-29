using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.ImageEffects;

[ExecuteInEditMode]
public class GenericImageEffect : ImageEffectBase {

    public TextureMap[] textures;
    public FloatMap[] floats;
    public IntMap[] integers;
    public ColorMap[] colors;
    public VectorMap[] vectors;

    public bool updateParametersEveryFrame = false;

    protected override void Start() {
        base.Start();
        ApplyParameters();
    }

    void ApplyParameters() {
        if (this.textures != null) {
            foreach (var t in this.textures) {
                this.material.SetTexture(t.name, t.value);
            }
        }

        if (this.floats != null) {
            foreach (var t in this.floats) {
                this.material.SetFloat(t.name, t.value);
            }
        }

        if (this.integers != null) {
            foreach (var t in this.integers) {
                this.material.SetInt(t.name, t.value);
            }
        }

        if (this.colors != null) {
            foreach (var t in this.colors) {
                this.material.SetColor(t.name, t.value);
            }
        }

        if(this.vectors != null) {
            foreach(var t in this.vectors) {
                this.material.SetVector(t.name, t.value);
            }
        }
    }

    void OnRenderImage(RenderTexture source, RenderTexture destination) {

#if UNITY_EDITOR
        if (!Application.isPlaying) {
            // Always update in realtime inside the editor.
            ApplyParameters();
        }
        else
#endif
        if (this.updateParametersEveryFrame) {
            ApplyParameters();
        }

        Graphics.Blit(source, destination, material);
    }

    [System.Serializable]
    public struct FloatMap {
        public string name;
        public float value;
    }

    [System.Serializable]
    public struct IntMap {
        public string name;
        public int value;
    }

    [System.Serializable]
    public struct TextureMap {
        public string name;
        public Texture value;
    }

    [System.Serializable]
    public struct ColorMap {
        public string name;
        public Color value;
    }

    [System.Serializable]
    public struct VectorMap {
        public string name;
        public Vector4 value;
    }
}