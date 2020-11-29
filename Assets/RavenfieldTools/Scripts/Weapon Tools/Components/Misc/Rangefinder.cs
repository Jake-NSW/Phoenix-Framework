using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Rangefinder : MonoBehaviour {

    const int MASK = 1;

    public Text rangeText;
    public string noReadingText = "-";
    public float maxDistance = 1000f;
    public float samplesPerSecond = 2f;
    Action sampleAction = new Action(1f);

	public void Sample() {
        RaycastHit hitInfo;
        if (Physics.Raycast(new Ray(this.transform.position, this.transform.forward), out hitInfo, maxDistance, MASK)) {
            this.rangeText.text = ((int)hitInfo.distance).ToString();
        }
        else {
            this.rangeText.text = this.noReadingText;
        }

        this.sampleAction.Start();
    }

    private void Update() {
        if(this.sampleAction.TrueDone()) {
            Sample();
        }
    }

    private void Awake() {
        float sampleInterval = 1f / samplesPerSecond;
        this.sampleAction = new Action(sampleInterval);
        Sample();
    }
}
