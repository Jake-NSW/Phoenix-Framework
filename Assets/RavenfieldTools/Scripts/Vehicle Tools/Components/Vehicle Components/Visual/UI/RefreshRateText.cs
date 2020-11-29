using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RefreshRateText : MonoBehaviour {

	void Awake() {
        GetComponent<Text>().text = Screen.currentResolution.refreshRate + " Hz";
    }
}
