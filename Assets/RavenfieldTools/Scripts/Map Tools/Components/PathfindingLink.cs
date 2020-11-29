using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pathfinding {
	public class PathfindingLink : GraphModifier {

		public Transform end;
		public float costFactor = 1.0f;
		public bool oneWay = false;
		public PathfindingBox.Type type;
	}
}
