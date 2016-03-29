﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Poly2Tri;

public class WallGenerator : MonoBehaviour {

	public float height = 5;
	public float thickness = 0.2f;
	private Dictionary<Vector3, List<GameObject>> nodes = new Dictionary<Vector3, List<GameObject>>();
	private bool alternator = false;
	private Vector3[][] point_pairs_array = null;
	private GameObject[] walls = null;
	private List<Vector3> new_verts = new List<Vector3>();
	private List<int> new_tris = new List<int> ();
	private int new_vert_count = 0;
	private Dictionary<Vector3, int> vertexIndices = new Dictionary<Vector3, int> ();
    // Use this for initialization


    Vector3[][] initPointPairsFromNodes(List<GameObject> nodeList)
    {
        List<Vector3[]> point_pairs = new List<Vector3[]>();
        for (int i = 0; i < nodeList.Count; i++)
        {
            GameObject nodeObject = nodeList[i];
            Node nodeScript = nodeObject.GetComponent<Node>();
            for (int j = 0; j < nodeScript.adjacentNodes.Count; j++)
            {
                GameObject adjacentNodeObject = nodeScript.adjacentNodes[j];
                point_pairs.Add(new Vector3[] { swapVectorYZ(nodeObject.transform.position), swapVectorYZ(adjacentNodeObject.transform.position)});
            }
        }
        return point_pairs.ToArray();
    }


    Vector3[][] init_point_pairs() {
        //List of list
        List<Vector3[]> point_pairs = new List<Vector3[]>();
        point_pairs.Add (new Vector3[] {new Vector3(0, 0, 0), new Vector3(5, 0, 0) });
		point_pairs.Add (new Vector3[] {new Vector3(5, 0, 0), new Vector3(5, 0, 2) });
		point_pairs.Add (new Vector3[] {new Vector3(5, 0, 5), new Vector3(5, 0, 2) });
		point_pairs.Add (new Vector3[] {new Vector3(5, 0, 5), new Vector3(3, 0, 4) });
		point_pairs.Add (new Vector3[] {new Vector3(3, 0, 4), new Vector3(1, 0, 5) });
		point_pairs.Add (new Vector3[] {new Vector3(1, 0, 5), new Vector3(0, 0, 0) });
		point_pairs.Add (new Vector3[] {new Vector3(3, 0, 2), new Vector3(5, 0, 2) });
		point_pairs.Add (new Vector3[] {new Vector3(3, 0, 4), new Vector3(3, 0, 2) });

		return point_pairs.ToArray ();
	}

    public void generate3D(List<GameObject> nodeList, List<GameObject> windowList)
    {
        point_pairs_array = initPointPairsFromNodes(nodeList);
        generateWalls();
		addWindow (windowList[0]);
    }

    private void generateWalls()
    {
        GameObject[] walls = new GameObject[point_pairs_array.Length];
        for (int i = 0; i < point_pairs_array.Length; i++)
        {
            GameObject wall_object = new GameObject();
            wall_object.name = "Wall " + i;
            wall_object.transform.parent = this.transform;
            walls[i] = wall_object;
            WallFunctions wall_script = wall_object.AddComponent<WallFunctions>();
            wall_script.generateWall(point_pairs_array[i][0], point_pairs_array[i][1]);

        }
		this.walls = walls;
        //Debug.Log (-3 % walls.Length);
        //adjust all walls
        foreach (GameObject wall_object in walls)
        {
            addOrUpdate(wall_object.GetComponent<WallFunctions>().Start_pt, wall_object);
            addOrUpdate(wall_object.GetComponent<WallFunctions>().End_pt, wall_object);
        }

        List<GameObject>[] node_values = nodes.Values.ToArray();
        Vector3[] node_points = nodes.Keys.ToArray();
        //walls[2].GetComponent<WallFunctions>().addHole();
        for (int i = 0; i < node_values.Count(); i++)
        {
            GameObject[] coincident_walls = node_values[i].ToArray();
            for (int j = 0; j < coincident_walls.Length; j++)
            {
                Vector3 start = getStart(coincident_walls[j]);
                Vector3 end = getEnd(coincident_walls[j]);
                Vector3 otherPoint = start == node_points[i] ? end : start;

                float angle = Vector3.Angle(otherPoint - node_points[i], new Vector3(1, 0, 0));
                Vector3 cross = Vector3.Cross(otherPoint - node_points[i], new Vector3(1, 0, 0));
                if (cross.y < 0) angle = -angle;


                coincident_walls[j].GetComponent<WallFunctions>().Angle = angle;
            }
            if (coincident_walls.Length < 3)
                coincident_walls = coincident_walls.OrderBy(w => w.GetComponent<WallFunctions>().Angle).ToArray();
            else
                coincident_walls = coincident_walls.OrderByDescending(w => w.GetComponent<WallFunctions>().Angle).ToArray();

            alternator = false;
            Debug.Log("Point : " + node_points[i]);
            if (coincident_walls.Length > 1)
            {
                for (int j = 0; j < coincident_walls.Length; j++)
                {
                    Debug.Log(coincident_walls[j].name + " : " + coincident_walls[j].GetComponent<WallFunctions>().Angle + ", " + coincident_walls[(j + 1) % coincident_walls.Length].name + " : " + coincident_walls[(j + 1) % coincident_walls.Length].GetComponent<WallFunctions>().Angle);
                    alternator = !alternator;
                    adjustShape(coincident_walls[j], coincident_walls[(j + 1) % coincident_walls.Length], node_points[i]);
                }
            }
        }
		//Floor
		List<Point> p = new List<Point>();
		for (int i = 0; i < node_points.Length; i++) {
			Point s = new Point ();
			s.X = node_points [i].x;
			s.Y = node_points [i].z;
			p.Add (s);
		}

		Point[] ch = ConvexHull.CH2 (p).ToArray();
		Vector2[] floor_vertices = new Vector2[ch.Length - 1];

		for (int i = 0; i < ch.Length - 1; i++) {
			floor_vertices [i] = new Vector2 (ch[i].X, ch[i].Y);
			Debug.Log (floor_vertices [i]);
		}

		GameObject floor = new GameObject ();
		floor.name = "Floor";
		floor.transform.parent = this.transform;
		floor.AddComponent<MeshFilter> ();
		floor.AddComponent<MeshRenderer> ();

		Mesh floor_m = floor.GetComponent<MeshFilter> ().mesh;

		Polygon floor_poly = createPoly (floor_vertices);
		P2T.Triangulate (floor_poly);

		for (int i = 0; i < floor_poly.Triangles.Count; i++)
			for (int j = 0; j < 3; j++) {
				TriangulationPoint tpt = floor_poly.Triangles [i].Points[j];
				Vector3 pt = new Vector3 ( (float) tpt.X, 0, (float) tpt.Y);
				new_tris.Add (vertexIndices [pt]);
			}

		floor_m.vertices = new_verts.ToArray ();
		int[] tris =  new_tris.ToArray ();
		for (int i = 0; i < tris.Length; i+=3) {
			int temp = tris [i + 1];
			tris [i + 1] = tris [i + 2];
			tris [i + 2] = temp;
		}
		floor_m.triangles = tris;
		floor_m.RecalculateNormals ();
		Material newMat = Resources.Load("floorMaterial.mat", typeof(Material)) as Material;
		Debug.Log (newMat);
		MeshRenderer renderer = floor.GetComponent<MeshRenderer> ();
		renderer.materials[0] = newMat;
		//floor.AddComponent<Material> = new Material ();

	}
	private Polygon createPoly(Vector2[] points) {
		List<PolygonPoint> polyPoints = new List<PolygonPoint> ();
		for (int i = 0; i < points.Length; i++) {
			polyPoints.Add (new PolygonPoint (points [i].x, points [i].y));
			Vector3 pt = new Vector3 ( points [i].x, 0,  points [i].y );
			new_verts.Add (pt);
			vertexIndices.Add (pt, new_vert_count);
			new_vert_count++;
		}
		Polygon P = new Polygon (polyPoints);
		return P;
	}

	public void addWindow(GameObject window) {
		Vector3 abs_position = window.transform.position;
		Vector3 startNode = swapVectorYZ(window.GetComponent<WallAttachableObject>().startNode.transform.position);
		Vector3 endNode = swapVectorYZ(window.GetComponent<WallAttachableObject>().endNode.transform.position);
		float holeLength = window.GetComponent<WallAttachableObject>().length;
		float holeHeight = window.GetComponent<WallAttachableObject>().height;
		float holeElevation = window.GetComponent<WallAttachableObject>().elevation;
		for (int i = 0; i < walls.Length; i++) {
			//print ("Point pair array " + point_pairs_array [i][0] + " " + point_pairs_array[i][1]);
			if (contains(point_pairs_array[i], startNode) && contains(point_pairs_array[i], endNode)) {
				Vector3 wall_position = walls [i].GetComponent<WallFunctions> ().Start_pt;
				walls [i].GetComponent<WallFunctions> ().addHole (abs_position - wall_position, holeLength, holeHeight, holeElevation);
				//Debug.Log ("Found " + walls[i].name);
				return;
			}
		}
		Debug.Log ("Not Found");
	}
	private bool contains ( Vector3[] array, Vector3 vect ) {
		for (int i = 0; i < array.Length; i++)
			if (array [i].Equals (vect))
				return true;
		return false;
	}
			

	void Start (){
		//Vector3[][] point_pairs_array = init_point_pairs ();
	}

	//draw corner points using gizmos 
	/*void OnDrawGizmos() {
		this.point_pairs_array = init_point_pairs();
		//init_corners ();
		Gizmos.color = Color.black;
		for (int i = 0; i < point_pairs_array.Length; i++) {
			Gizmos.DrawSphere(point_pairs_array[i][0], 0.1f);
			Gizmos.DrawSphere(point_pairs_array[i][1], 0.1f);
		}
			
	}*/
	void Awake() {
		this.point_pairs_array = init_point_pairs();
	}
	private void addOrUpdate(Vector3 corner, GameObject wall) {
		if (nodes.ContainsKey (corner)) {
			List<GameObject> l = nodes [corner];
			l.Add (wall);
			nodes [corner] = l;
		} else {
			List<GameObject> l = new List<GameObject>();
			l.Add(wall);
			nodes.Add (corner, l);
		}
	}

    private void adjustShape(GameObject a, GameObject b, Vector3 point)
    {
        float angle = a.GetComponent<WallFunctions>().Angle - b.GetComponent<WallFunctions>().Angle;
        int baseA, baseB, dupli_baseA, dupli_baseB;

        //angle adjustments
        if (angle > 180)
        {
            angle = -(angle - 180);
        }

        if (angle < -180)
        {
            angle = (angle + 360);
        }
        //Debug.Log (angle);

        Mesh meshA = a.GetComponent<MeshFilter>().mesh;
        Mesh meshB = b.GetComponent<MeshFilter>().mesh;

        Vector3[] vertsA = meshA.vertices;
        int ovlA = a.GetComponent<WallFunctions>().Ovl;
        Vector3[] vertsB = meshB.vertices;
        int ovlB = b.GetComponent<WallFunctions>().Ovl;

        float ext = (thickness / 2) / Mathf.Tan(angle * Mathf.Deg2Rad / 2);

        //Debug.Log (ext);
        //if (Mathf.Abs (angle) > 90)
        //	ext = -ext;

        //Debug.Log (a.name + " " + b.name + " : " + ext);
        bool isStartA = isStart(a, point);
        bool isStartB = isStart(b, point);

        if (isStartA)
        {
            baseA = ovlA;
            dupli_baseA = 2 * ovlA + 4;
        }
        else
        {
            baseA = 0;
            dupli_baseA = 2 * ovlA;
        }

        if (!isStartB)
        {
            baseB = ovlB;
            dupli_baseB = 2 * ovlB + 4;
        }
        else
        {
            baseB = 0;
            dupli_baseB = 2 * ovlB;
        }

        //subtract positive z direction vector from close-to-angle edge of A
        Vector3 ext_vector = new Vector3(0, 0, ext);

        if (isStartA)
        {
            vertsA[baseA + 0] += ext_vector;
            vertsA[dupli_baseA + 0] += ext_vector;
            vertsA[dupli_baseA + 19] += ext_vector;

            vertsA[baseA + 1] += ext_vector;
            vertsA[dupli_baseA + 1] += ext_vector;
            vertsA[dupli_baseA + 6] += ext_vector;
        }
        else
        {
            vertsA[baseA + 2] -= ext_vector;
            vertsA[dupli_baseA + 7] -= ext_vector;
            vertsA[dupli_baseA + 12] -= ext_vector;

            vertsA[baseA + 3] -= ext_vector;
            vertsA[dupli_baseA + 13] -= ext_vector;
            vertsA[dupli_baseA + 18] -= ext_vector;

        }

        if (isStartB)
        {
            vertsB[baseB + 0] += ext_vector;
            vertsB[dupli_baseB + 0] += ext_vector;
            vertsB[dupli_baseB + 19] += ext_vector;

            vertsB[baseB + 1] += ext_vector;
            vertsB[dupli_baseB + 1] += ext_vector;
            vertsB[dupli_baseB + 6] += ext_vector;
        }
        else
        {
            vertsB[baseB + 2] -= ext_vector;
            vertsB[dupli_baseB + 7] -= ext_vector;
            vertsB[dupli_baseB + 12] -= ext_vector;

            vertsB[baseB + 3] -= ext_vector;
            vertsB[dupli_baseB + 13] -= ext_vector;
            vertsB[dupli_baseB + 18] -= ext_vector;
        }
        meshA.vertices = vertsA;
        meshB.vertices = vertsB;




    }
    private bool isStart(GameObject a, Vector3 point) {
		//when coincident edges arent all start edges
		return (a.GetComponent<WallFunctions> ().Start_pt == point);
	}
	private Vector3 getStart(GameObject a) {
		return a.GetComponent<WallFunctions> ().Start_pt;
	}

	private Vector3 getEnd(GameObject a) {
		return a.GetComponent<WallFunctions> ().End_pt;
	}
	void Update () {
		
	}

    Vector3 swapVectorYZ(Vector3 vectorToSwap)
    {
        float z = vectorToSwap.z;
        vectorToSwap.z = vectorToSwap.y;
        vectorToSwap.y = z;
        return vectorToSwap;
    }
}
struct Point {
	public float X, Y;
	public static bool operator == (Point u1, Point u2) 
	{
		return u1.Equals(u2);  // use ValueType.Equals() which compares field-by-field.
	}
	public static bool operator != (Point u1, Point u2) 
	{
		return !u1.Equals(u2);  // use ValueType.Equals() which compares field-by-field.
	}
	public Point (float x, float y) {
		X = x;
		Y = y;
	}
}

class ConvexHull
{
	public static List<Point> CH2(List<Point> points)
	{
		return CH2(points, false);
	}

	public static List<Point> CH2(List<Point> points, bool removeFirst)
	{
		List<Point> vertices = new List<Point>();

		if (points.Count == 0)
			return null;
		else if (points.Count == 1)
		{
			// If it's a single point, return it
			vertices.Add(points[0]);
			return vertices;
		}


		Point leftMost = CH2Init(points);
		vertices.Add(leftMost);

		Point prev = leftMost;
		Point? next;
		double rot = 0;
		do
		{
			next = CH2Step(prev, points, ref rot);

			// If it's not the first vertex (leftmost) or we want spiral (instead of CH2)
			// remove it
			if (prev != leftMost || removeFirst)
				points.Remove(prev);

			// If this isn't the last vertex, save it
			if (next.HasValue)
			{
				vertices.Add(next.Value);
				prev = next.Value;
			}

		} while (points.Count > 0 && next.HasValue && next.Value != leftMost);
		points.Remove(leftMost);

		return vertices;

	}

	private static Point CH2Init(List<Point> points)
	{
		// Initialization - Find the leftmost point
		Point leftMost = points[0];
		double leftX = leftMost.X;

		foreach (Point p in points)
		{
			if (p.X < leftX)
			{
				leftMost = p;
				leftX = p.X;
			}
		}
		return leftMost;
	}

	private static Point? CH2Step(Point currentPoint, List<Point> points, ref double rot)
	{
		double angle, angleRel, smallestAngle = 2 * Mathf.PI, smallestAngleRel = 4 * Mathf.PI;
		Point? chosen = null;
		float xDiff, yDiff;

		foreach (Point candidate in points)
		{
			if (candidate == currentPoint)
				continue;

			xDiff = candidate.X - currentPoint.X;
			yDiff = -(candidate.Y - currentPoint.Y); //Y-axis starts on top
			angle = ComputeAngle(new Point(xDiff, yDiff));

			// angleRel is the angle between the line and the rotated y-axis
			// y-axis has the direction of the last computed supporting line
			// given by variable rot.
			angleRel = 2 * Mathf.PI - (rot - angle);

			if (angleRel >= 2 * Mathf.PI)
				angleRel -= 2 * Mathf.PI;
			if (angleRel < smallestAngleRel)
			{
				smallestAngleRel = angleRel;
				smallestAngle = angle;
				chosen = candidate;
			}

		}

		// Save the smallest angle as the rotation of the y-axis for the
		// computation of the next supporting line.
		rot = smallestAngle;

		return chosen;
	}


	private static double ComputeAngle(Point p)
	{
		if (p.X > 0 && p.Y > 0)
			return Mathf.Atan(p.X / p.Y);
		else if (p.X > 0 && p.Y == 0)
			return (Mathf.PI / 2);
		else if (p.X > 0 && p.Y < 0)
			return (Mathf.PI + Mathf.Atan(p.X / p.Y));
		else if (p.X == 0 && p.Y >= 0)
			return 0;
		else if (p.X == 0 && p.Y < 0)
			return Mathf.PI;
		else if (p.X < 0 && p.Y > 0)
			return (2 * Mathf.PI + Mathf.Atan(p.X / p.Y));
		else if (p.X < 0 && p.Y == 0)
			return (3 * Mathf.PI / 2);
		else if (p.X < 0 && p.Y < 0)
			return (Mathf.PI + Mathf.Atan(p.X / p.Y));
		else
			return 0;
	}



}