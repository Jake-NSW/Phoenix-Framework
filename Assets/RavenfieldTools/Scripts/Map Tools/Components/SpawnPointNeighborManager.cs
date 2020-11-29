using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpawnPointNeighborManager : MonoBehaviour {

	public SpawnPointNeighbor[] neighbors;

	[System.Serializable]
	public class SpawnPointNeighbor {
		public SpawnPoint a, b;
		public bool landConnection = true;
		public bool waterConnection = true;
		public bool oneWay = false;
	}
}
