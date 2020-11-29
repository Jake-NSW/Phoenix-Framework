using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PileTexturer : MonoBehaviour {

	public Layer[] layers;
	public DetailLayer[] detailLayers;

	[System.Serializable]
	public class Layer {
		public enum Type {Keep, Set, Slope, Height}
		//public enum Type {Keep, Set, Slope, Height, Convex, Concave}
		public Type type;
		public float inputGain = 1f;
		public AnimationCurve curve;
		public float outputGain = 1f;
		public float outputNoise = 0f;
		public float noiseFrequency = 10f;
		public int noiseSeed = 0;
	}

	[System.Serializable]
	public class DetailLayer {
		public int layerIndex = 0;
		public AnimationCurve densityCurve;
		public float densityMultiplier = 1f;
	}
}
