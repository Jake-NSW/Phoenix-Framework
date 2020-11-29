using UnityEngine;
using System.Collections;

public class ReflectionProber : MonoBehaviour {
	public ReflectionProbe normalProbe;
	public ReflectionProbe nightVisionProbe;

	void Awake() {
		normalProbe.RenderProbe();
	}
}
