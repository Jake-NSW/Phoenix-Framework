using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ActorSkin {

    public string name = "New Skin";
    public MeshSkin characterSkin;
    public MeshSkin armSkin;
    public MeshSkin kickLegSkin;

    [System.Serializable]
    public class MeshSkin {
        public Mesh mesh;
        public Material[] materials;
        public int teamMaterial = -1;
    }
}