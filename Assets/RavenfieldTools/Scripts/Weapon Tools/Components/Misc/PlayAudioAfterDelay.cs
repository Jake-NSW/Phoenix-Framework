using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayAudioAfterDelay : MonoBehaviour {

	public float delay;

	void OnEnable () {
		Invoke("Play", this.delay);
	}
	
	void OnDisable () {
		CancelInvoke();
	}

	void Play() {
		GetComponent<AudioSource>().Play();
	}
}
