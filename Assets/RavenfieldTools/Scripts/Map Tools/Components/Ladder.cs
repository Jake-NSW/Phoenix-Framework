using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ladder : Pathfinding.PathfindingLink {

	public float height = 10f;
	public Vector3 bottomExitDirection = new Vector3(0f, 0f, -1f);
	public Vector3 topExitDirection = Vector3.forward;

	public Vector3 GetBottomPosition() {
		return this.transform.position;
	}

	public Vector3 GetTopPosition() {
		return this.transform.position + this.transform.up*this.height;
	}

	public Vector3 GetBottomExitPosition() {
		return this.transform.localToWorldMatrix.MultiplyPoint(this.bottomExitDirection);
	}

	public Vector3 GetTopExitPosition() {
		return GetTopPosition() + this.transform.localToWorldMatrix.MultiplyVector(this.topExitDirection);
	}
}
