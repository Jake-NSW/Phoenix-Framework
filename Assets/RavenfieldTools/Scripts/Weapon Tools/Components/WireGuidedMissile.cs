using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WireGuidedMissile : Rocket {

	public float correctionGain = 0.002f;
	public float maxCorrection = 0.05f;
	public float controlLossDistance = 400f;
}
