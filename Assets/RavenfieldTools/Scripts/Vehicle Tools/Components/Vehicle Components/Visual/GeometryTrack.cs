using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(SkinnedMeshRenderer))]
public class GeometryTrack : MonoBehaviour {

#if UNITY_EDITOR

    [ContextMenu("Generate Blend Shapes")]
    public void GenerateBlendShapes() {
        var window = ScriptableObject.CreateInstance<GeometryTrackDataWindow>();
        window.target = this;
        window.position = new Rect(Screen.width / 2, Screen.height / 2, 250, 150);
        window.ShowPopup();
    }
#endif

    new SkinnedMeshRenderer renderer;
    public Transform speedSampleTransform;
    public float linkDistance = 0.3f;
    public Rigidbody vehicleRigidbody;

    float speed = 0f;
    float phase = 0f;

    public void Awake() {
        this.renderer = GetComponent<SkinnedMeshRenderer>();
        if(this.speedSampleTransform == null) {
            this.speedSampleTransform = this.transform;
        }
    }

    void Update() {
        //float phase = (10f * Time.time) % 1f;
        phase = (phase + 10000f + (this.speed * Time.deltaTime) / this.linkDistance) % 1f;

        this.renderer.SetBlendShapeWeight(0, phase);
    }

    private void FixedUpdate() {
        Vector3 velocity = vehicleRigidbody.GetPointVelocity(this.speedSampleTransform.position);
        this.speed = Vector3.Dot(velocity, this.speedSampleTransform.forward);
    }

}

#if UNITY_EDITOR
public class GeometryTrackDataWindow : EditorWindow {

    public GeometryTrack target;
    string text;
    int nLinks;

    void OnGUI() {
        EditorGUILayout.LabelField("Number of links:");
        this.text = EditorGUILayout.TextField(this.text);
        GUILayout.Space(70);
        if (GUILayout.Button("Generate")) {
            try {
                this.nLinks = int.Parse(this.text);
                Generate();
            }
            catch(System.Exception e) {
                Debug.LogException(e);
            }
            this.Close();
        }
    }

    void Generate() {

        Debug.Log("Generating shader data for " + this.target.gameObject.name + ", links: " + this.nLinks);

        Mesh oldMesh;

        var skinnedMeshRenderer = this.target.GetComponent<SkinnedMeshRenderer>();
        oldMesh = skinnedMeshRenderer.sharedMesh;

        if(oldMesh.vertices.Length % this.nLinks != 0) {
            throw new System.Exception("Number of links did not match the vertex count of the mesh, vertices: " + oldMesh.vertices.Length);
        }

        int nVertsPerLink = oldMesh.vertices.Length / this.nLinks;

        Mesh newMesh = new Mesh();
        newMesh.name = "Generated Track Mesh";

        newMesh.vertices = oldMesh.vertices;
        newMesh.normals = oldMesh.normals;
        newMesh.tangents = oldMesh.tangents;
        newMesh.uv = oldMesh.uv;
        newMesh.triangles = oldMesh.triangles;
        newMesh.bindposes = oldMesh.bindposes;
        newMesh.boneWeights = oldMesh.boneWeights;
        newMesh.bounds = oldMesh.bounds;

        AddBlendShape(newMesh, "next", 1, nVertsPerLink);

        string oldMeshPath = AssetDatabase.GetAssetPath(oldMesh);
        string newMeshPath = oldMeshPath + "_generated_tracks.mesh";

        Debug.Log("Saving new mesh to: " + newMeshPath);
        AssetDatabase.CreateAsset(newMesh, newMeshPath);
        AssetDatabase.SaveAssets();

        skinnedMeshRenderer.sharedMesh = newMesh;
    }

    void AddBlendShape(Mesh newMesh, string name, int linkStep, int nVertsPerLink) {
        Vector3[] deltaVertices = new Vector3[newMesh.vertices.Length];
        Vector3[] deltaNormals = new Vector3[newMesh.vertices.Length];
        Vector3[] deltaTangents = new Vector3[newMesh.vertices.Length];

        for (int link = 0; link < this.nLinks; link++) {
            int nextLink = (link + this.nLinks + 1) % this.nLinks;
            for (int v = 0; v < nVertsPerLink; v++) {
                int vertIndex = link * nVertsPerLink + v;
                int targetVertIndex = nextLink * nVertsPerLink + v;

                Vector3 offset = newMesh.vertices[targetVertIndex] - newMesh.vertices[vertIndex];

                deltaVertices[vertIndex] = newMesh.vertices[targetVertIndex] - newMesh.vertices[vertIndex];
                deltaNormals[vertIndex] = newMesh.normals[targetVertIndex] - newMesh.normals[vertIndex];
                deltaTangents[vertIndex] = newMesh.tangents[targetVertIndex] - newMesh.tangents[vertIndex];
            }
        }

        newMesh.AddBlendShapeFrame(name, 1f, deltaVertices, deltaNormals, deltaTangents);
    }
}

[CustomEditor(typeof(GeometryTrack))]
public class GeometryTrackEditor : Editor {

    public override void OnInspectorGUI() {
        base.OnInspectorGUI();

        if(GUILayout.Button("Generate Blend Shapes")) {
            var track = this.target as GeometryTrack;
            track.GenerateBlendShapes();
        }
    }
}
#endif