using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GearIndicator : MonoBehaviour {

    public ArcadeCar car;
    Text text;

    public string neutral = "-";
    public string forward = "FWD";
    public string reverse = "REV";

    void Awake() {
        this.text = GetComponent<Text>();
    }
}
