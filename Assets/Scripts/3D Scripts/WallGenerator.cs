using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Poly2Tri;

public class WallGenerator : MonoBehaviour
{
    
    public float height = 5;
    public float thickness = 0.2f;
    private Dictionary<Vector3, List<GameObject>> nodes = new Dictionary<Vector3, List<GameObject>>();
    private bool alternator = false;
    private Vector3[][] point_pairs_array = null;
    private GameObject[] walls = null;
    private List<Vector3> new_verts = new List<Vector3>();
    private List<int> new_tris = new List<int>();
    private int new_vert_count = 0;
    private Dictionary<Vector3, int> vertexIndices = new Dictionary<Vector3, int>();
    private Dictionary<GameObject, List<Hole>> wall_holes = new Dictionary<GameObject, List<Hole>>();
    public LayerMask layerMask3D;
    public GameObject _3DContainer;

    public void Refresh()
    {
        nodes.Clear();
        point_pairs_array = null;
        walls = null;
        new_verts.Clear();
        new_tris.Clear();
        new_vert_count = 0;
        vertexIndices.Clear();
        wall_holes.Clear();
    }

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
                point_pairs.Add(new Vector3[] { swapVectorYZ(nodeObject.transform.position), swapVectorYZ(adjacentNodeObject.transform.position) });
            }
        }
        return point_pairs.ToArray();
    }

    public void generate3D(List<GameObject> nodeList, List<GameObject> windowList, List<GameObject> houseObjectList)
    {
        point_pairs_array = initPointPairsFromNodes(nodeList);
        generateWalls();
        
        if(windowList.Count > 0)
        {
            addWindows(windowList);
        }
		adjustWalls ();
		generateFloor ();
        if (houseObjectList.Count > 0)
        {
            addHouseObjects(houseObjectList);
        }

        print("House object list size is : " + houseObjectList.Count);
    }

    private void generateHouseObjects()
    {
         //Resources.Load("furniture/3D/" + category + "/" + name) as Texture2D;
    }

    private void generateWalls()
    {
        GameObject[] walls = new GameObject[point_pairs_array.Length];
        for (int i = 0; i < point_pairs_array.Length; i++)
        {
            GameObject wall_object = new GameObject();
            wall_object.layer = 10;
            wall_object.name = "Wall " + i;
            wall_object.transform.parent = _3DContainer.transform;
            walls[i] = wall_object;
            WallFunctions wall_script = wall_object.AddComponent<WallFunctions>();
            wall_script.generateWall(point_pairs_array[i][0], point_pairs_array[i][1]);
            
        }
        this.walls = walls;

        //Create a dictionary of all nodes and coincident walls 
		//To be used for adjusting walls and floor generation
		foreach (GameObject wall_object in walls)
		{
			nodeAddOrUpdate(wall_object.GetComponent<WallFunctions>().Start_pt, wall_object);
			nodeAddOrUpdate(wall_object.GetComponent<WallFunctions>().End_pt, wall_object);
		}
        
    }
	private void adjustWalls() {
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
	}
	private void generateFloor() {
		Vector3[] node_points = nodes.Keys.ToArray();
		if (point_pairs_array.Length > 2) {
			List<Point> p = new List<Point> ();
			for (int i = 0; i < node_points.Length; i++) {
				Point s = new Point ();
				s.X = node_points [i].x;
				s.Y = node_points [i].z;
				p.Add (s);
			}

			Point[] ch = ConvexHull.CH2 (p).ToArray ();
			Vector2[] floor_vertices = new Vector2[ch.Length - 1];

			for (int i = 0; i < ch.Length - 1; i++) {
				floor_vertices [i] = new Vector2 (ch [i].X, ch [i].Y);
				Debug.Log (floor_vertices [i]);
			}

			GameObject floor = new GameObject ();
			floor.name = "Floor";
			floor.transform.parent = _3DContainer.transform;
			floor.AddComponent<MeshFilter> ();
			floor.AddComponent<MeshRenderer> ();

			Mesh floor_m = floor.GetComponent<MeshFilter> ().mesh;

			Polygon floor_poly = createPoly (floor_vertices);
			P2T.Triangulate (floor_poly);

			for (int i = 0; i < floor_poly.Triangles.Count; i++)
				for (int j = 0; j < 3; j++) {
					TriangulationPoint tpt = floor_poly.Triangles [i].Points [j];
					Vector3 pt = new Vector3 ((float)tpt.X, 0, (float)tpt.Y);
					new_tris.Add (vertexIndices [pt]);
				}

			floor_m.vertices = new_verts.ToArray ();
			int[] tris = new_tris.ToArray ();
			for (int i = 0; i < tris.Length; i += 3) {
				int temp = tris [i + 1];
				tris [i + 1] = tris [i + 2];
				tris [i + 2] = temp;
			}
			floor_m.triangles = tris;
			floor_m.RecalculateNormals ();

			//assign material
			/*Material newMat = new Material(Shader.Find("Standard"));
            //Resources.Load("meshgen material", typeof(Material)) as Material;
            MeshRenderer renderer = floor.GetComponent<MeshRenderer>();
            renderer.sharedMaterial = newMat;*/
			Material newMat = new Material(Shader.Find("Standard"));//GetDefaultMaterial();
			newMat.color = Color.gray;
			//newMat.mainTexture.wrapMode = TextureWrapMode.Repeat;
			floor.GetComponent<MeshRenderer>().material = newMat;

			//PUNEET -> Added Mesh Collider to floor
			floor.AddComponent<MeshCollider> ();
			floor.GetComponent<MeshCollider> ().sharedMesh = floor_m;
			floor.layer = 10;
		}
	}
    private Polygon createPoly(Vector2[] points)
    {
        List<PolygonPoint> polyPoints = new List<PolygonPoint>();
        for (int i = 0; i < points.Length; i++)
        {
            polyPoints.Add(new PolygonPoint(points[i].x, points[i].y));
            Vector3 pt = new Vector3(points[i].x, 0, points[i].y);
            new_verts.Add(pt);
            vertexIndices.Add(pt, new_vert_count);
            new_vert_count++;
        }
        Polygon P = new Polygon(polyPoints);
        return P;
    }

    public void addHouseObjects(List<GameObject> houseObjects)
    {
        foreach (GameObject houseObject in houseObjects)
        {
            string category = houseObject.GetComponent<HouseObject>().category;
            GameObject container = Instantiate(Resources.Load("furniture/3D_Models/" + category + "Container")) as GameObject;
            container.transform.position = swapVectorYZ(houseObject.transform.position);
            container.transform.parent = _3DContainer.transform;
            Vector3 newYVector = container.transform.position;
            newYVector.y += 1;
            container.transform.position = newYVector;
            container.GetComponent<HouseObject3D>().setModel(houseObject.name);
        }
    }

    public void addWindows(List<GameObject> windows)
    {
        foreach (GameObject window in windows) {
            
            Hole h = new Hole ();
            h.Position = window.transform.position;
            h.Hole_length = window.GetComponent<WallAttachableObject>().length;
            h.Hole_height = window.GetComponent<WallAttachableObject>().height;
            h.Hole_elevation = window.GetComponent<WallAttachableObject>().elevation;
            //Vector3 startNode = swapVectorYZ(window.GetComponent<WallAttachableObject>().startNode.transform.position);
            //Vector3 endNode = swapVectorYZ(window.GetComponent<WallAttachableObject>().endNode.transform.position);
            GameObject w = liesOn (h);
            h.Position = swapVectorYZ(window.transform.position);
            
            if (w != null) {
                holeAddOrUpdate (w, h);
            }
            else 
                Debug.Log("Not Found");
        }
        if (wall_holes.Count > 0) {
            foreach (KeyValuePair<GameObject, List<Hole>> entry in wall_holes) {
                Debug.Log ("Sent holes : " + entry.Value.Count );
                entry.Key.GetComponent<WallFunctions> ().addHoles (entry.Value);
            }
        }  
    }
    private bool contains(Vector3[] array, Vector3 vect)
    {
        for (int i = 0; i < array.Length; i++)
            if (array[i].Equals(vect))
                return true;
        return false;
    }
    private GameObject liesOn (Hole h) {
        Vector3 relativePos = new Vector3(h.Position.x, 2.5f, h.Position.y);
        //RaycastHit[] hitList = Physics.BoxCastAll (relativePos, new Vector3 (h.Hole_length / 2, h.Hole_height / 2, thickness), Vector3.down, layerMask3D);
        //print ("Hit list count is " + hitList.Length);
        print("Box pos is  " + relativePos + " " + new Vector3 (thickness, h.Hole_height / 2, thickness));
        
        Collider[] colliderList = Physics.OverlapBox (relativePos, new Vector3 (thickness, h.Hole_height / 2, thickness), Quaternion.identity, layerMask3D);
        print ("Size of collider list " + colliderList.Length);
        foreach (Collider hit in colliderList) {
            if (hit.name.ToLower ().Contains ("wall")) {
                print ("Hit with Wall " + hit.gameObject);
                return hit.gameObject;
            }
        }
        /*
        for (int i = 0; i < walls.Length; i++)
        {
            //print ("Point pair array " + point_pairs_array [i][0] + " " + point_pairs_array[i][1]);
            //if (contains(point_pairs_array[i], startNode) && contains(point_pairs_array[i], endNode))
            print("Absolute position is " + h.Position);
            print("Wall Renderer is " + walls[i].GetComponent<Renderer>().bounds);

            //PUNEET -> Changed your method of checking for window. Instead, just check if the wall overlaps that position.
            //Slight issue and doesn't work in some odd cases when the 2D position is a little off and not exactly centered. 
            //Solution could be -> If "NOT FOUND" perform the startNode and endNode check which you were previously doing
            if(walls[i].GetComponent<Renderer>().bounds.Contains(relativePos))
            {
                return walls [i];
            }
        }*/
        return null;
    }
    
    private void holeAddOrUpdate(GameObject wall, Hole hole)
    {
        print ("Inside add or update and wall is " + wall + " hole is " + hole);
        print ("Wall holes dictionairy is size" + wall_holes.Count);
        if (wall_holes.ContainsKey(wall))
        {
            List<Hole> l = wall_holes[wall];
            l.Add(hole);
            wall_holes[wall] = l;
        }
        else
        {
            List<Hole> l = new List<Hole>();
            l.Add(hole);
            wall_holes.Add(wall, l);
        }
    }

    private void nodeAddOrUpdate(Vector3 corner, GameObject wall)
    {
        if (nodes.ContainsKey(corner))
        {
            List<GameObject> l = nodes[corner];
            l.Add(wall);
            nodes[corner] = l;
        }
        else
        {
            List<GameObject> l = new List<GameObject>();
            l.Add(wall);
            nodes.Add(corner, l);
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
        //  ext = -ext;
        
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
    private bool isStart(GameObject a, Vector3 point)
    {
        //when coincident edges arent all start edges
        return (a.GetComponent<WallFunctions>().Start_pt == point);
    }
    private Vector3 getStart(GameObject a)
    {
        return a.GetComponent<WallFunctions>().Start_pt;
    }
    
    private Vector3 getEnd(GameObject a)
    {
        return a.GetComponent<WallFunctions>().End_pt;
    }
    void Update()
    {
        
    }
    
    Vector3 swapVectorYZ(Vector3 vectorToSwap)
    {
        float z = vectorToSwap.z;
        vectorToSwap.z = vectorToSwap.y;
        vectorToSwap.y = z;
        return vectorToSwap;
    }
}
struct Point
{
    public float X, Y;
    public static bool operator ==(Point u1, Point u2)
    {
        return u1.Equals(u2);  // use ValueType.Equals() which compares field-by-field.
    }
    public static bool operator !=(Point u1, Point u2)
    {
        return !u1.Equals(u2);  // use ValueType.Equals() which compares field-by-field.
    }
    public Point(float x, float y)
    {
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
public class Hole {
    private int hole_start_index;
    private int hole_end_index;
    private float hole_length = 1;
    private float hole_height = 1;
    private float hole_elevation = 2;
    private Vector3 position;
    public int Hole_end_index {
        get {
            return this.hole_end_index;
        }
        set {
            hole_end_index = value;
        }
    }
    public int Hole_start_index {
        get {
            return this.hole_start_index;
        }
        set {
            hole_start_index = value;
            hole_end_index = value + 4;
        }
    }
    
    public float Hole_length {
        get {
            return this.hole_length;
        }
        set {
            hole_length = value;
        }
    }
    
    public float Hole_height {
        get {
            return this.hole_height;
        }
        set {
            hole_height = value;
        }
    }
    
    public float Hole_elevation {
        get {
            return this.hole_elevation;
        }
        set {
            hole_elevation = value;
        }
    }
    public Vector3 Position {
        get {
            return this.position;
        }
        set {
            position = value;
        }
    }
}