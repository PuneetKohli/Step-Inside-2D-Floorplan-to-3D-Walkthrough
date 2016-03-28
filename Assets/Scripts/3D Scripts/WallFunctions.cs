using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Poly2Tri;
using System;

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
    private List<int[]> holes = new List<int[]>();
    private int hole_start_index = 4;
    private int hole_end_index = 8;
    private int ovl;
    private bool has_hole = false;
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
        Material newMat = Resources.Load("meshgen material", typeof(Material)) as Material;
        MeshRenderer renderer = this.GetComponent<MeshRenderer>();
        renderer.materials[0] = newMat;

        //transform mesh to position and angle
        this.transform.RotateAround(new Vector3(0, 0, 0), new Vector3(0, 1, 0), 90 + angle);
        this.transform.position = start;

    }

    public void addHole()
    {
        Mesh mesh = GetComponent<MeshFilter>().mesh;

        List<Vector2> rectanglePoints = new List<Vector2>();
        List<Vector2> holePoints = new List<Vector2>();

        rectanglePoints.Add(new Vector2(0, 0));
        rectanglePoints.Add(new Vector2(0, height));
        rectanglePoints.Add(new Vector2(length, height));
        rectanglePoints.Add(new Vector2(length, 0));

        holePoints.Add(new Vector2(length / 3, height / 3));
        holePoints.Add(new Vector2(length / 3, 2 * height / 3));
        holePoints.Add(new Vector2(2 * length / 3, 2 * height / 3));
        holePoints.Add(new Vector2(2 * length / 3, height / 3));

        Polygon Rectangle = createPoly(rectanglePoints.ToArray());
        Polygon Hole = createPoly(holePoints.ToArray());


        Rectangle.AddHole(Hole);
        P2T.Triangulate(Rectangle);
        //Debug.Log ("here");
        for (int i = 0; i < Rectangle.Triangles.Count; i++)
            for (int j = 0; j < 3; j++)
            {
                TriangulationPoint tpt = Rectangle.Triangles[i].Points[j];
                Vector3 pt = new Vector3(-thickness / 2, (float)tpt.Y, (float)tpt.X);
                new_tris.Add(vertexIndices[pt]);
            }
        mesh.Clear();
        this.has_hole = true;
        hole_start_index = 4;
        hole_end_index = 8;
        mesh.vertices = new_verts.ToArray();
        mesh.triangles = new_tris.ToArray();

        //Debug.Log ("First vertex " + mesh.vertices [0]);

        mesh = this.extrudeWall(mesh);
        mesh.RecalculateNormals();

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

        int[] hole_triangles = new int[(hole_end_index - hole_start_index) * 2 * 3]; //two triangles for each edge with 3 values each
        Vector3[] hole_vertices = new Vector3[(hole_end_index - hole_start_index) * 4];
        //triangles and vertices for Hole faces
        if (!this.has_hole)
            hole_start_index = hole_end_index = 0;

        count = 0;
        for (int i = hole_start_index; i < hole_end_index; i++)
        {
            int first = i;
            int second = hole_start_index + (((i - hole_start_index) + 1) % (hole_end_index - hole_start_index));

            Vector3[] vertices_hole_new = new Vector3[4];
            vertices_hole_new[0] = orignal_vertices[first];
            vertices_hole_new[1] = orignal_vertices[second];
            vertices_hole_new[2] = back_vertices[first];
            vertices_hole_new[3] = back_vertices[second];
            vertices_hole_new.CopyTo(hole_vertices, count);
            int[] quad_tri = quadTriangles(last + count + 0, last + count + 1, last + count + 3, last + count + 2);
            quad_tri.CopyTo(hole_triangles, 6 * (i - hole_start_index));
            count += 4;
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
        return mesh;
    }

    int[] quadTriangles(int a, int b, int c, int d)
    {
        int[] triangles = { a, c, b, a, d, c };
        return triangles;
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