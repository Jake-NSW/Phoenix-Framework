using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Speedometer : MonoBehaviour {

    public Rigidbody target;
    public float multiplier = 3.6f; // From MPS to KPH
    Text text;

    void Awake() {
        this.text = GetComponent<Text>();
    }

	// Update is called once per frame
	void Update () {
        float speed = target.transform.worldToLocalMatrix.MultiplyVector(target.velocity).z * this.multiplier;
        this.text.text = Mathf.Abs((int)speed).ToString();
    }
}
