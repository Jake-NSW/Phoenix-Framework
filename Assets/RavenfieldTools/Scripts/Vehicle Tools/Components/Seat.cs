using UnityEngine;
using System.Collections;

public class Seat : MonoBehaviour {

	public enum SitAnimation {Chair = 0, Quad = 1, Standing = 2}
	public enum Type {FreelookUnarmed, LockedAllowFreelookUnarmed, AlwaysLockedUnarmed, FreelookArmed}

	public const int LAYER = 11;

	public Type type = Type.FreelookUnarmed;
	new public SitAnimation animation = SitAnimation.Chair;
	public bool enclosed = false;
	public bool enclosedDamagedByDirectFire = false;
	public bool allowLean = false;
	public bool allowUnderwater = false;
	public Vector3 exitOffset = Vector3.zero;
	public MountedWeapon[] weapons;
	public Transform handTargetL, handTargetR;
	new public Camera camera;
	public Camera thirdPersonCamera;

	public GameObject hud;

	public float maxOccupantBalance = 200f;
}
