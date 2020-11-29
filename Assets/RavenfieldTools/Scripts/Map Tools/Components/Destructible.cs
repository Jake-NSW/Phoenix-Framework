using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destructible : MonoBehaviour {

    public float smallArmsMultiplier = 1f;
    public float heavyArmsMultiplier = 1f;
    public float antiTankMultiplier = 1f;

    public Vehicle.ArmorRating armorDamagedBy = Vehicle.ArmorRating.HeavyArms;

    public bool canInstantShatter = false;
    public Vehicle.ArmorRating armorInstantShatteredBy = Vehicle.ArmorRating.HeavyArms;

    public GameObject[] activateOnDeath;
    public GameObject[] disableOnDeath;

    public bool showHitIndicator = true;

    public float health = 300f;
    
}
