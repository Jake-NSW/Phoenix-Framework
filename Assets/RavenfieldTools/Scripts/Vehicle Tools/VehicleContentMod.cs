using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleContentMod : MonoBehaviour {

	public GameObject jeep;
	public GameObject jeepMachineGun;
	public GameObject quad;
	public GameObject tank;
    public GameObject apc;
    public GameObject attackBoat;
	public GameObject rhib;
	public GameObject attackHelicopter;
	public GameObject transportHelicopter;
	public GameObject attackPlane;
	public GameObject bombPlane;

	public GameObject turretMachineGun;
	public GameObject turretAntiTank;
	public GameObject turretAntiAir;

	public GameObject[] AllEntries() {
		return new GameObject[] {
			this.jeep,
			this.jeepMachineGun,
			this.quad,
			this.tank,
            this.apc,
            this.attackBoat,
			this.rhib,
			this.attackHelicopter,
			this.transportHelicopter,
			this.attackPlane,
			this.bombPlane,
			this.turretMachineGun,
			this.turretAntiTank,
			this.turretAntiAir,
		};
	}
}
