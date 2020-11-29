using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CountermeasureStatusIndicator : MonoBehaviour {

    public Vehicle vehicle;

    public Text textIndicator;
    public string readyText = "";
    public string notReadyText = "";

    public Graphic[] tintTargets;
    public Color readyColor;
    public Color notReadyColor;

    public GameObject readyObject;
    public GameObject notReadyObject;

}
