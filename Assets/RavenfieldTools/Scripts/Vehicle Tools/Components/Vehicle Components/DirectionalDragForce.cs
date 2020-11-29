using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirectionalDragForce : MonoBehaviour {

	public float forwardDrag = 0.1f;
	public float sideDrag = 0.5f;
	public float upwardsDrag = 0.1f;

	public Transform forceApplyPoint;
}
