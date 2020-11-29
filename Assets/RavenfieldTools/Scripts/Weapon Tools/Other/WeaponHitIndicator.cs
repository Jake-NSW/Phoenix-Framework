using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponHitIndicator : MonoBehaviour {

	const float LIFETIME = 10f;

	// Use this for initialization
	void Start () {
		Invoke("Cleanup", LIFETIME);
	}

	void Cleanup() {
		Destroy(this.gameObject);
	}

    public void SetColor(Color c) {
        GetComponent<Image>().color = c;
    }
}
