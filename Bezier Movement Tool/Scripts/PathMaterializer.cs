using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(PathEditorBehaviour))]
public class PathMaterializer : MonoBehaviour {

    public float Resolution;

    public Mesh GeneratedMesh;

    public Sprite GeneratedImage;

    public Triangulation2D triangulation;

    public List<Vector3> Points;

    public float AreaEffector;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
