using Ravenfield.Mutator.Configuration;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[System.Serializable]
public class MutatorEntry {

    [FoldoutGroup("$name")]
    [FoldoutGroup("$name/Basic Info")]
    [HorizontalGroup("$name/Basic Info/top", LabelWidth = 20)]
    [TextArea]
    public string name = "New Mutator";

    [HorizontalGroup("$name/Basic Info/top", LabelWidth = 20)]
    [TextArea]
    public string description = "";

    [Title("Menu Image", horizontalLine: false, bold: false)]
    [HorizontalGroup("$name/Basic Info/bottom")] [HideLabel]
    public Texture2D menuImage;
    
    [Title("Mutator Prefab", horizontalLine: false, bold: false)]
    [HorizontalGroup("$name/Basic Info/bottom")] [HideLabel]
    public GameObject mutatorPrefab;

    [FoldoutGroup("$name/Configuration")] [HideLabel]
	public MutatorConfiguration configuration;

}
