using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Poly2Tri;
using System;
using System.Reflection;

public class WallFunctions : MonoBehaviour
{
    
    public float thickness = 0.2f;
    public float height = 5f;
    private Vector3 _start_pt;
    private Vector3 _end_pt;
    private float angle = 0;
    private float length = 0;
    private Dictionary<Vector3, int> vertexIndices = new Dictionary<Vector3, int>();
    private List<Vector3> new_verts = new List<Vector3>();
    private List<int> new_tris = new List<int>();
    private int new_vert_count = 0;
    public List<Hole> hole_list = new List<Hole> ();
    private int hole_start_index = 4;
    private int hole_end_index = 8;
    private int ovl;
    public float hole_length = 1;
    public float hole_height = 1;
    public float hole_elevation = 2;
    private Polygon latest_face = null;
	public float tilingfactor = 1;
    public int Ovl
    {
        get
        {
            return this.ovl;
        }
        set
        {
            ovl = value;
        }
    }
    
    public float Angle
    {
        get
        {
            return this.angle;
        }
        set
        {
            angle = value;
        }
    }
    
    public Vector3 Start_pt
    {
        get
        {
            return this._start_pt;
        }
        set
        {
            _start_pt = value;
        }
    }
    
    public Vector3 End_pt
    {
        get
        {
            return this._end_pt;
        }
        set
        {
            _end_pt = value;
        }
    }
    
    
    public void generateWall(Vector3 start, Vector3 end)
    {
        
        Start_pt = start;
        End_pt = end;
        // get angle between direction vector and x axis
        Vector3 wall_vector = end - start;
        
        float angle = Vector3.Angle(wall_vector, new Vector3(1, 0, 0));
        Vector3 cross = Vector3.Cross(wall_vector, new Vector3(1, 0, 0));
        if (cross.y > 0) angle = -angle;
        //this.Angle = angle;
        
        //get wall length
        length = wall_vector.magnitude;
        
        //create mesh
        gameObject.AddComponent<MeshFilter>();
        gameObject.AddComponent<MeshRenderer>();
        
        Mesh mesh = this.GetComponent<MeshFilter>().mesh;
        List<Vector3> verts = new List<Vector3>();
        verts.Add(new Vector3(-thickness / 2, 0f, 0f));
        verts.Add(new Vector3(-thickness / 2, height, 0f));
        verts.Add(new Vector3(-thickness / 2, height, length));
        verts.Add(new Vector3(-thickness / 2, 0f, length));
        
        mesh.vertices = verts.ToArray(); ;
        mesh.triangles = new int[] { 0, 2, 1, 0, 3, 2 };
        
        
        //extrude mesh
        mesh = this.extrudeWall(mesh);
        mesh.RecalculateNormals();

		//adjust wall parameters
        
        
        //assign material
        Material newMat = new Material(Shader.Find("Standard"));//GetDefaultMaterial();
        newMat.color = Color.white;

		//apply texture
		newMat.mainTexture = (Texture2D) Resources.Load ("textures/tex1");
		newMat.mainTexture.wrapMode = TextureWrapMode.Repeat;

        //Resources.Load("meshgen material", typeof(Material)) as Material;
        //newMat.EnableKeyword("_EMISSION");
        //newMat.SetColor ("_EmissionColor", Color.white);
        MeshRenderer renderer = this.GetComponent<MeshRenderer>();
        renderer.material = newMat;

        //transform mesh to position and angle
        this.transform.RotateAround(new Vector3(0, 0, 0), new Vector3(0, 1, 0), 90 + angle);
        this.transform.position = start;
        
        //PUNEET -> Added Meshcollider to wall
        gameObject.AddMissingComponent<MeshCollider>().sharedMesh = mesh;
    }
    
    
    public void addHoles(List<Hole> holes)
    {
        Mesh mesh = GetComponent<MeshFilter>().mesh;
        List<Vector2> rectanglePoints = new List<Vector2> ();
        rectanglePoints.Add (new Vector2 (0, 0));
        rectanglePoints.Add (new Vector2 (0, height));
        rectanglePoints.Add (new Vector2 (length, height));
        rectanglePoints.Add (new Vector2 (length, 0));
        latest_face = createPoly (rectanglePoints.ToArray ());
        
        for (int l = 0; l < holes.Count; l++)
            Debug.Log (holes[l].Position);
        
        for (int k = 0; k < holes.Count; k++) {
            float distance = (holes[k].Position - this.Start_pt).magnitude;
            hole_length = holes [k].Hole_length;
            hole_height = holes [k].Hole_height;
            hole_elevation = holes [k].Hole_elevation;
            //distance = 1;
            
            if (distance > (length - hole_length / 2) || distance < hole_length / 2) {
                Debug.Log ("Hole exceeds " + this.name + " by " + distance);
            } else {
                List<Vector2> holePoints = new List<Vector2> ();
                
                holePoints.Add (new Vector2 (distance - hole_length / 2, hole_elevation - hole_height / 2));
                holePoints.Add (new Vector2 (distance - hole_length / 2, hole_elevation + hole_height / 2));
                holePoints.Add (new Vector2 (distance + hole_length / 2, hole_elevation + hole_height / 2));
                holePoints.Add (new Vector2 (distance + hole_length / 2, hole_elevation - hole_height / 2));
                
                
                Polygon Hole = createPoly (holePoints.ToArray ());
                Debug.Log ("Created hole");
                
                
                latest_face.AddHole (Hole);
                
                holes [k].Hole_start_index = (k - 1) >= 0 ? holes [k - 1].Hole_end_index : 4;
                //Debug.Log ("here");
                hole_list.Add (holes [k]);
            }
            
            
        } // holes loop end
        print(latest_face);
        P2T.Triangulate (latest_face);
        for (int i = 0; i < latest_face.Triangles.Count; i++)
        for (int j = 0; j < 3; j++) {
            TriangulationPoint tpt = latest_face.Triangles [i].Points [j];
            Vector3 pt = new Vector3 (-thickness / 2, (float)tpt.Y, (float)tpt.X);
            new_tris.Add (vertexIndices [pt]);
        }
        mesh.Clear ();
        mesh.vertices = new_verts.ToArray ();
        mesh.triangles = new_tris.ToArray ();
        mesh = this.extrudeWall (mesh);
        mesh.RecalculateNormals ();
        //PUNEET -> ReAdded Meshcollider to wall in the case of hole
        gameObject.AddMissingComponent<MeshCollider>().sharedMesh = mesh;

		//change uvs according to hole
        
    }
    
    private Polygon createPoly(Vector2[] points)
    {
        List<PolygonPoint> polyPoints = new List<PolygonPoint>();
        for (int i = 0; i < points.Length; i++)
        {
            polyPoints.Add(new PolygonPoint(points[i].x, points[i].y));
            Vector3 pt = new Vector3(-thickness / 2, points[i].y, points[i].x);
            new_verts.Add(pt);
            vertexIndices.Add(pt, new_vert_count);
            new_vert_count++;
        }
        Polygon P = new Polygon(polyPoints);
        return P;
    }
    
    private Mesh extrudeWall(Mesh mesh)
    {
        //duplicate vertices
        Vector3[] orignal_vertices = mesh.vertices;
        Vector3[] back_vertices = new Vector3[orignal_vertices.Length];
        Vector3[] mid_vertices = new Vector3[4];
        
        Vector3 thickness_vector = new Vector3(thickness, 0, 0);
        for (int i = 0; i < orignal_vertices.Length; i++)
            back_vertices[i] = orignal_vertices[i] + thickness_vector;
        
        Vector3 mid_vector = new Vector3(thickness / 2, 0, 0);
        for (int i = 0; i < mid_vertices.Length; i++)
            mid_vertices[i] = orignal_vertices[i] + mid_vector;
        
        //combine arrays 
        Vector3[] first_vertices = new Vector3[orignal_vertices.Length + back_vertices.Length];
        orignal_vertices.CopyTo(first_vertices, 0);
        back_vertices.CopyTo(first_vertices, orignal_vertices.Length);
        
        int ovl = orignal_vertices.Length;
        this.ovl = ovl;
        int last = first_vertices.Length;
        
        //generate inverted back triangles
        int[] orignal_triangles = mesh.triangles;
        
        int[] back_triangles = new int[orignal_triangles.Length];
        for (int i = 0; i < orignal_triangles.Length; i += 3)
        {
            back_triangles[i] = orignal_triangles[i] + ovl;
            back_triangles[i + 1] = orignal_triangles[i + 2] + ovl;
            back_triangles[i + 2] = orignal_triangles[i + 1] + ovl;
        }
        
        //hole triangles
        
        
        
        
        // triangles and vertices for hex faces
        int[] hex_triangles = new int[8 * 2 * 3];//8 quads with 2 triangles with 3 values each
        int count = 0;
        Vector3[] hex_vertices = new Vector3[6 * 4];
        for (int i = 0; i < 4; i++)
        {
            
            int first = i;
            int second = (i + 1) % 4;
            
            Vector3[] vertices_new = new Vector3[6];
            vertices_new[0] = orignal_vertices[first];
            vertices_new[1] = orignal_vertices[second];
            vertices_new[2] = mid_vertices[first];
            vertices_new[3] = mid_vertices[second];
            vertices_new[4] = back_vertices[first];
            vertices_new[5] = back_vertices[second];
            vertices_new.CopyTo(hex_vertices, count);
            
            int[] quad_tri = quadTriangles(last + count + 1, last + count + 0, last + count + 2, last + count + 3);
            quad_tri.CopyTo(hex_triangles, 2 * 2 * 3 * i);
            
            quad_tri = quadTriangles(last + count + 4, last + count + 5, last + count + 3, last + count + 2);
            quad_tri.CopyTo(hex_triangles, 2 * 2 * 3 * i + 2 * 3);
            count += 6;
        }
        last = first_vertices.Length + hex_vertices.Length;
        
        int[] hole_triangles = new int[hole_list.Count * 4 * 2 * 3]; //two triangles for each edge with 3 values each
        Vector3[] hole_vertices = new Vector3[hole_list.Count * 4 * 4]; // 4 new points for each edge
        //triangles and vertices for Hole faces
        
        for (int k = 0; k < hole_list.Count; k++) {
            Debug.Log ("Hole " + k);
            count = 0;
            hole_start_index = hole_list [k].Hole_start_index;
            hole_end_index = hole_list [k].Hole_end_index;
            Debug.Log (hole_start_index);
            for (int i = hole_start_index; i < hole_end_index; i++) {
                
                
                int first = i;
                int second = hole_start_index + (((i - hole_start_index) + 1) % (hole_end_index - hole_start_index));
                
                Vector3[] vertices_hole_new = new Vector3[4];
                vertices_hole_new [0] = orignal_vertices [first];
                vertices_hole_new [1] = orignal_vertices [second];
                vertices_hole_new [2] = back_vertices [first];
                vertices_hole_new [3] = back_vertices [second];
                Debug.Log ("Wound " + vertices_hole_new[0]);
                vertices_hole_new.CopyTo (hole_vertices, k*16 + count);
                
                int[] quad_tri = quadTriangles (last + count + 0, last + count + 1, last + count + 3, last + count + 2);
                quad_tri.CopyTo (hole_triangles,(6 * 4 * k) + (6 * (i - hole_start_index)));
                for (int m = 0; m < quad_tri.Length; m++) {
                    Debug.Log(quad_tri [m]);
                }
                count += 4;
            }
            last += 16;
        }
        for (int m = 0; m < hole_vertices.Length; m++) {
            Debug.Log(hole_vertices [m]);
        }
        Vector3[] vertices = new Vector3[first_vertices.Length + hex_vertices.Length + hole_vertices.Length];
        first_vertices.CopyTo(vertices, 0);
        hex_vertices.CopyTo(vertices, first_vertices.Length);
        hole_vertices.CopyTo(vertices, first_vertices.Length + hex_vertices.Length);
        
        
        //combine triangles
        int[] triangles = new int[orignal_triangles.Length + back_triangles.Length + hex_triangles.Length + hole_triangles.Length];
        orignal_triangles.CopyTo(triangles, 0);
        back_triangles.CopyTo(triangles, orignal_triangles.Length);
        hex_triangles.CopyTo(triangles, orignal_triangles.Length + back_triangles.Length);
        hole_triangles.CopyTo(triangles, orignal_triangles.Length + back_triangles.Length + hex_triangles.Length);
        /*Debug.Log (triangles.Length);
        for (int i = 0; i < triangles.Length; i ++)
            Debug.Log (triangles[i]);*/
        


        //add to mesh and return
        mesh.vertices = vertices;
        mesh.triangles = triangles;

		//assign UVs
		assignUV();

        return mesh;
    }
    
    int[] quadTriangles(int a, int b, int c, int d)
    {
        int[] triangles = { a, c, b, a, d, c };
        return triangles;
    }
    
	private void assignUV() {
		Mesh m = GetComponent<MeshFilter> ().mesh;
		Vector3[] verts = m.vertices;
		int ovl = GetComponent<WallFunctions> ().Ovl;
		Vector2[] uvs = new Vector2[verts.Length];

		//faces ---- 0 to 2 * OVL
		for (int i = 0; i < 2 * ovl; i++) {
			uvs [i] = new Vector2 (verts [i].z / tilingfactor, verts [i].y / tilingfactor);		
		}



		//loop for hexes ---- 2 * OVL to hole start 

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
    // Use this for initialization
    void Start()
    {
        
    }
    
    // Update is called once per frame
    void Update()
    {
        
    }
}