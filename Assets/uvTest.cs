using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class uvTest : MonoBehaviour {
	public float tilingfactor = 1;
	// Use this for initialization
	void Start () {
		GameObject wall = new GameObject ();
		wall.transform.parent = this.transform;
		wall.AddComponent<WallFunctions> ();
		wall.name = "wall";
		wall.GetComponent<WallFunctions> ().generateWall (new Vector3 (0, 0, 0), new Vector3 (5, 0, 0));

		Hole h = new Hole ();
		h.Hole_elevation = 2;
		h.Hole_length = 2;
		h.Hole_height = 2;
		h.Position = new Vector3 (2.5f, 0, 0);
		List<Hole> l = new List<Hole> ();
		l.Add (h);
		wall.GetComponent<WallFunctions> ().addHoles (l);
		Material newMat = new Material(Shader.Find("Standard"));//GetDefaultMaterial();

		newMat.mainTexture = (Texture2D) Resources.Load ("textures/tex1");
		//Debug.Log (newMat.mainTexture);
		//Debug.Log(Resources.Load("textures/tex1").GetType());
		newMat.color = Color.white;
		MeshRenderer renderer = wall.GetComponent<MeshRenderer>();
		renderer.material = newMat;
		newMat.mainTexture.wrapMode = TextureWrapMode.Repeat;


		Mesh m = wall.GetComponent<MeshFilter> ().mesh;
		Vector3[] verts = m.vertices;
		int ovl = wall.GetComponent<WallFunctions> ().Ovl;
		Hole[] holes = wall.GetComponent<WallFunctions> ().hole_list.ToArray();
		Vector2[] uvs = new Vector2[verts.Length];

		//faces 0 to 2 * OVL
		for (int i = 0; i < 2 * ovl; i++) {
			uvs [i] = new Vector2 (verts [i].z / tilingfactor, verts [i].y / tilingfactor);		
		}



		//loop for hexes -- 2 * OVL to hole start 

		for (int i = 2 * ovl; i < (2 * ovl + 6 * 4); i+=6) {
			//base points 
			uvs[i] = new Vector2 (verts [i].z / tilingfactor, verts [i].y / tilingfactor);
			uvs[i + 1] = new Vector2 (verts [i + 1].z / tilingfactor, verts [i + 1].y / tilingfactor);

			//mid and end
			if (verts [i].y == verts[i + 1].y)
				for (int k = 2; k < 6; k++) 
					uvs[i + k] = new Vector2 (verts [i + k].z / tilingfactor, (verts [i + k].y + verts [i + k].x - verts[i + k % 2].x) / tilingfactor);
			else 
				for (int k = 2; k < 6; k++) 
					uvs[i + k] = new Vector2 ((verts [i + k].z + verts [i + k].x - verts[i + k % 2].x) / tilingfactor, verts [i + k].y / tilingfactor);
		}

		//Loop for holes --- hole start to end

		for (int i = (2 * ovl + 6 * 4); i < verts.Length; i+=4) {
			//base points 
			uvs[i] = new Vector2 (verts [i].z / tilingfactor, verts [i].y / tilingfactor);
			uvs[i + 1] = new Vector2 (verts [i + 1].z / tilingfactor, verts [i + 1].y / tilingfactor);

			//mid and end
			if (verts [i].y == verts[i + 1].y)
				for (int k = 2; k < 4; k++) 
					uvs[i + k] = new Vector2 (verts [i + k].z / tilingfactor, (verts [i + k].y + verts [i + k].x - verts[i + k % 2].x) / tilingfactor);
			else 
				for (int k = 2; k < 4; k++) 
					uvs[i + k] = new Vector2 ((verts [i + k].z + verts [i + k].x - verts[i + k % 2].x) / tilingfactor, verts [i + k].y / tilingfactor);
		}

		m.uv = uvs;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
