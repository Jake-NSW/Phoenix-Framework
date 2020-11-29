using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomActorModel : MonoBehaviour {

	public SpawnPoint.Team team = SpawnPoint.Team.Blue;

	public Mesh actorMesh;

	public Material[] materials;
	public int actorMaterial = -1;


	public Mesh armMesh;

	public Material[] armMaterials;
	public int armActorMaterial;


	public Mesh kickLegMesh;

	public Material[] kickLegMaterials;
	public int kickLegActorMaterial;
}
