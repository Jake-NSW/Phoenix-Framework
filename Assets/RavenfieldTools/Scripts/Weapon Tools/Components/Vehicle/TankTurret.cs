using UnityEngine;
using System.Collections;

public class TankTurret : MountedWeapon {

	public ConfigurableJoint towerJoint;
	public HingeJoint cannonJoint;
	public Renderer cannonRenderer;

    public float sensitivityX = 1f;
    public float sensitivityY = 1f;
}
