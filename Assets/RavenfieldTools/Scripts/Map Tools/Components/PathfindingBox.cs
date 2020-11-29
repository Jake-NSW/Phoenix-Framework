using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathfindingBox : MonoBehaviour {

	public enum Type {Infantry, Car, Boat}

	public Type type;
	public bool tiled = true;
	public bool automaticCellSize = true;
	public float cellSize = 1f;
	public float characterRadius = 2f;
	public float climbHeight = 0.5f;
	public float coverPointSpacing = 0.05f;
	public PathfindingBox[] blockers;
	[Range(0f, 90f)] public float maxSlope = 35f;
}
