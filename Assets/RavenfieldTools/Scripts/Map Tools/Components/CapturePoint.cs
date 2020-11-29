using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CapturePoint : SpawnPoint {

	public Transform contestedSpawnpointContainer;

	public float captureRange = 25f;
	public float captureFloor = 5f;
	public float captureCeiling = 20f;

	public Transform flagParent;
}
