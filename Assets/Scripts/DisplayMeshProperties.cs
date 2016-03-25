using UnityEngine;
using System.Collections;

public class DisplayMeshProperties : MonoBehaviour {

	// Use this for initialization
	void Start () {
		Mesh m = GetComponent<MeshFilter> ().mesh;
		Vector3[] verts = m.vertices;
		Vector3[] normals = m.normals;

		Vector3[] looper = normals;

		for (int i = 0; i < looper.Length; i++) {
			Debug.Log (looper [i]);
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
