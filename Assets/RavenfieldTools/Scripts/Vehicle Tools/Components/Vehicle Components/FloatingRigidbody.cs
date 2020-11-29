using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class FloatingRigidbody : MonoBehaviour {

    public Transform[] floatingSamplers;
    public float floatAcceleration = 10f;
    public float floatDepth = 0.5f;

    public float waterDrag = 2f;
    public float waterAngularDrag = 2f;

    
}
