using UnityEngine;
using System.Collections;

public class SoundBank : MonoBehaviour {

	public AudioClip[] clips;
	public AudioSource audioSource;

	int lastIndex = 0;

	public virtual void Start() {
		if(this.audioSource == null) {
			this.audioSource = GetComponent<AudioSource>();
		}

		this.lastIndex = Random.Range(0, this.clips.Length);

		if(this.audioSource.playOnAwake) {
			this.audioSource.Stop();
			PlayRandom();
		}
	}

	public void PlayRandom() {
		this.lastIndex = (this.lastIndex+Random.Range(1, this.clips.Length))%(this.clips.Length);
		PlaySoundBank(this.lastIndex);
	}

	public void PlaySoundBank(int index) {
		this.audioSource.PlayOneShot(this.clips[index]);
	}
}
