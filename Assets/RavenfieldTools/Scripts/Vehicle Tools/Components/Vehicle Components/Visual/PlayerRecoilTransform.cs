using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRecoilTransform : MonoBehaviour {

    public Seat activePlayerSeat;

    public Vector3 positionWeight = Vector3.one;
    public Vector3 rotationWeight = Vector3.one;
    public bool applyCameraKick = false;
    
}
