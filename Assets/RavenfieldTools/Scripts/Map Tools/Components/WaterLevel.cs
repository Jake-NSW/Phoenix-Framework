using UnityEngine;
using System.Collections;

public class WaterLevel : MonoBehaviour {

	public static float height = 0f;

	public static bool InWater(Vector3 position) {
		return position.y <= height;
	}

	public static float Depth(Vector3 position) {
		return height-position.y;
	}

	// Use this for initialization
	void Awake () {
		height = this.transform.position.y;
	}
}
