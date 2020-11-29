using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotor : MonoBehaviour {
	public Vector3 rotationSpeed = new Vector3(0f, 0f, 1000f);

	public Renderer solidRotorRenderer;
	public Renderer blurredRotorRenderer;
	public bool renderSolidRendererShadow = true;
}
