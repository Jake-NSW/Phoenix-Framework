using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvoidanceBox : MonoBehaviour {

	public bool applyToAllTypes = true;
	public PathfindingBox.Type type;

	public uint penalty = 10000;
	public bool unwalkable = false;
}
