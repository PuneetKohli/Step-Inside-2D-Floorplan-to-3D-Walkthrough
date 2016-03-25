using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class WallGenerator : MonoBehaviour {

	public float height = 5;
	List<Vector3[]> point_pairs = new List<Vector3[]>();
	public float thickness = 0.2f;
	private Dictionary<Vector3, List<GameObject>> nodes = new Dictionary<Vector3, List<GameObject>>();
	private bool alternator = false;
	private Vector3[][] point_pairs_array = null; 
	// Use this for initialization



	Vector3[][] init_point_pairs() {
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
	void Start (){
		//Vector3[][] point_pairs_array = init_point_pairs ();
		GameObject[] walls = new GameObject[point_pairs_array.Length];
		for (int i = 0; i < point_pairs_array.Length; i++) {
			GameObject wall_object = new GameObject ();
			wall_object.name = "Wall " + i;
			wall_object.transform.parent = this.transform;
			walls[i] = wall_object;
			WallFunctions wall_script = wall_object.AddComponent <WallFunctions>();
			wall_script.generateWall (point_pairs_array [i][0], point_pairs_array [i][1]);

		}

		//Debug.Log (-3 % walls.Length);
		//adjust all walls
		foreach(GameObject wall_object in walls) {
			addOrUpdate(wall_object.GetComponent<WallFunctions>().Start_pt, wall_object);
			addOrUpdate(wall_object.GetComponent<WallFunctions>().End_pt, wall_object);
		}

		List<GameObject>[] node_values = nodes.Values.ToArray ();
		Vector3[] node_points = nodes.Keys.ToArray ();
		walls [2].GetComponent<WallFunctions> ().addHole ();

		Vector3[] m = walls [1].GetComponent<MeshFilter> ().mesh.normals;
		for (int i = 0; i < m.Length; i++)
			Debug.Log (m[i]);
		
		for (int i = 0; i < node_values.Count(); i++) {
			GameObject[] coincident_walls = node_values [i].ToArray (); 
			for (int j = 0; j < coincident_walls.Length; j++) {
				Vector3 start = getStart (coincident_walls [j]);
				Vector3 end = getEnd (coincident_walls [j]);
				Vector3 otherPoint = start == node_points[i] ? end : start;

				float angle = Vector3.Angle (otherPoint - node_points[i], new Vector3(1, 0, 0));
				Vector3 cross = Vector3.Cross (otherPoint - node_points[i], new Vector3 (1, 0, 0));
				if (cross.y < 0) angle = - angle;


				coincident_walls [j].GetComponent<WallFunctions> ().Angle = angle;
			}
			if (coincident_walls.Length < 3)
				coincident_walls = coincident_walls.OrderBy (w => w.GetComponent<WallFunctions> ().Angle).ToArray();
			else 
				coincident_walls = coincident_walls.OrderByDescending (w => w.GetComponent<WallFunctions> ().Angle).ToArray();

			alternator = false;
			//Debug.Log ("Point : " + node_points [i]);
			if (coincident_walls.Length > 1) {
				for (int j = 0; j < coincident_walls.Length; j++) {
					//Debug.Log (coincident_walls [j].name + " : " + coincident_walls [j].GetComponent<WallFunctions>().Angle + ", " + coincident_walls [(j + 1)  % coincident_walls.Length].name + " : " + coincident_walls [(j + 1)  % coincident_walls.Length].GetComponent<WallFunctions>().Angle);
					alternator = !alternator;
					adjustShape (coincident_walls [j], coincident_walls [(j + 1) % coincident_walls.Length], node_points [i]);
				}
			}
		}

	
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

	private void adjustShape(GameObject a, GameObject b, Vector3 point) {
		float angle = a.GetComponent<WallFunctions> ().Angle - b.GetComponent<WallFunctions> ().Angle;
		int baseA, baseB;

		//angle adjustments
		if (angle > 180) {
			angle = - (angle - 180);
		}

		if (angle < -180) {
			angle = - (angle + 180);
		}
		//Debug.Log (angle);

		Mesh meshA = a.GetComponent<MeshFilter> ().mesh;
		Mesh meshB = b.GetComponent<MeshFilter> ().mesh;

		Vector3[] vertsA = meshA.vertices;
		int ovlA = (vertsA.Length - 4) / 2;
		Vector3[] vertsB = meshB.vertices;
		int ovlB = (vertsB.Length - 4) / 2;

		float ext = (thickness / 2) / Mathf.Tan (angle * Mathf.Deg2Rad/ 2);

		//Debug.Log (ext);
		//if (Mathf.Abs (angle) > 90)
		//	ext = -ext;
	
		//Debug.Log (a.name + " " + b.name + " : " + ext);
		bool isStartA = isStart (a, point);
		bool isStartB = isStart (b, point);

		if (isStartA) {
			baseA = ovlA;
		}
		else 
			baseA = 0;

		if (!isStartB) {
			baseB = ovlB;
		}
		else 
			baseB = 0;
		
		//subtract positive z direction vector from close-to-angle edge of A
		Vector3 ext_vector = new Vector3(0, 0, ext);

		if (isStartA) {
			vertsA [baseA + 0] += ext_vector;
			vertsA [baseA + 1] += ext_vector;
		} else {
			vertsA [baseA + 2] -= ext_vector;
			vertsA [baseA + 3] -= ext_vector;
		}

		if (isStartB) {
			vertsB [baseB + 0] += ext_vector;
			vertsB [baseB + 1] += ext_vector;
		} else {
			vertsB [baseB + 2] -= ext_vector;
			vertsB [baseB + 3] -= ext_vector;
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
}