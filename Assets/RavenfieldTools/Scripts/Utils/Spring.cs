using UnityEngine;
using System.Collections;

public class Spring {

	public Spring(float spring, float drag, Vector3 min, Vector3 max, int iterations) {
		this.spring = spring;
		this.drag = drag;
		this.position = Vector3.zero;
		this.velocity = Vector3.zero;
		this.min = min;
		this.max = max;
		this.iterations = iterations;
	}

	public void AddVelocity(Vector3 delta) {
		this.velocity += delta;
	}

	public void Update() {
		float dt = Time.deltaTime/this.iterations;
		for(int i = 0; i < iterations; i++) {
			this.velocity -= (this.position*this.spring+this.velocity*this.drag)*dt;
			this.position = Vector3.Min(Vector3.Max(this.position+this.velocity*dt, this.min), this.max);
		}
	}

	float spring, drag;
	public Vector3 position, velocity;
	Vector3 min, max;
	int iterations;
}
