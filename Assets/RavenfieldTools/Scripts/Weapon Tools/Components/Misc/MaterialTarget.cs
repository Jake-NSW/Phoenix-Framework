using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MaterialTarget {

	public Renderer targetRenderer;
	public int materialSlot;

	public bool HasTarget() {
		return targetRenderer != null;
	}

	public Material Get() {
		return this.targetRenderer.materials[this.materialSlot];
	}
}
