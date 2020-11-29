using Ravenfield.Mutator.Configuration;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MutatorEntry {

    public string name = "New Mutator";
    public string description = "";
    public Texture2D menuImage;
    public GameObject mutatorPrefab;
	public MutatorConfiguration configuration;

}
