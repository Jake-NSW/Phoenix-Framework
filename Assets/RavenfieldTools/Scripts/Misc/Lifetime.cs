using UnityEngine;
using System.Collections;

public class Lifetime : MonoBehaviour {

	public float lifetime = 1f;
	Action lifetimeAction;

	// Use this for initialization
	void Start () {
		this.lifetimeAction = new Action(this.lifetime);
		this.lifetimeAction.Start();
	}
	
	// Update is called once per frame
	void Update () {
		if(this.lifetimeAction.TrueDone()) {
			Destroy(this.gameObject);
		}
	}
}
