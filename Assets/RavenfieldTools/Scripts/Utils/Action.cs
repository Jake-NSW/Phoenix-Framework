using UnityEngine;
using System.Collections;

public class Action {

	float lifetime, end;

	// Lie once so that stuff that uses ratio receives a 1.0f before done is called.
	bool lied;

	public Action(float lifetime) {
		this.lifetime = lifetime;
		this.end = 0f;
		this.lied = true;
	}

	public void Start() {
		this.end = Time.time+this.lifetime;
		this.lied = false;
	}

	public void StartLifetime(float lifetime) {
		this.lifetime = lifetime;
		Start();
	}

	public void Stop() {
		this.end = 0f;
		this.lied = true;
	}

	public float Remaining() {
		return this.end-Time.time;
	}

	public float Ratio() {
		return Mathf.Clamp01(1f-(this.end-Time.time)/this.lifetime);
	}

	public bool TrueDone() {
		return this.end <= Time.time;
	}

	public bool Done() {
		if(TrueDone()) {
			if(!this.lied) {
				this.lied = true;
				return false;
			}
			return true;
		}
		return false;
	}
}
