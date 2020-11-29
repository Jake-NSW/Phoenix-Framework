using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiElementTracker : MonoBehaviour {

	public Transform target;

	RectTransform rectTransform;

	void Awake() {
		this.rectTransform = (RectTransform) this.transform;
	}

	void LateUpdate() {
		try {
			this.rectTransform.anchoredPosition = Camera.main.WorldToScreenPoint(this.target.position);
		}
		catch(System.Exception) {

		}
	}


}
