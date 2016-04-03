using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class WallManager : MonoBehaviour {

    public GameObject wallSprite;
    public GameObject nodeSprite;
	public Transform wallContainer, nodeContainer, houseObjectcontainer, windowContainer;
    GameObject newWall;
    GameObject initialNode, currentNode;

    public List<GameObject> nodeList = new List<GameObject>();
    public List<GameObject> wallList = new List<GameObject>();
    public List<GameObject> windowList = new List<GameObject>();
    public List<GameObject> houseObjectList = new List<GameObject>();

    private Vector3 _initialPos, _currentPos;
    private float _xRotation;
    public bool isDrawing = false; //This is used to determine whether the user has stopped drawing (right click) and perform necessary action
                                    // Use this for initialization
    public bool didDraw = false;

	void Start () {
        addTapGesture();
	}

    void addTapGesture()
    {
        TKTapRecognizer tapRecognizer = new TKTapRecognizer();

        tapRecognizer.gestureRecognizedEvent += (r) =>
		{
			//Debug.Log ("tap recognizer fired: " + r);
            if(gameObject.activeInHierarchy)
            {
			if (gameObject.GetComponent<BoxCollider> ().bounds.Contains (transform.TransformPoint(GetCurrentMousePosition (r.startTouchLocation ()).GetValueOrDefault ()))) {
				if (!isDrawing) {
					didDraw = false;
					isDrawing = true;
					_initialPos = GetCurrentMousePosition (r.startTouchLocation ()).GetValueOrDefault ();
					instantiateNode (_initialPos);
				} else {
					didDraw = true;
					float newXpos = wallList.Last ().transform.position.x + wallList.Last ().transform.localScale.x * Mathf.Cos (_xRotation * Mathf.PI / 180f);
					float newYpos = wallList.Last ().transform.position.y + wallList.Last ().transform.localScale.x * Mathf.Sin (_xRotation * Mathf.PI / 180f);

					newXpos = Mathf.Round (newXpos * 100) / 100f;
					newYpos = Mathf.Round (newYpos * 100) / 100f;

					Vector3 newPos = new Vector3 (newXpos, newYpos, 0);
					_initialPos = newPos;
					setPreviousWallEndNode ();
                    handleOverlap(wallList.Last());
				}

				instantiateWall (_initialPos);

			}
			else
			{
				removeDrawingWall();
			}
            }
		};


        TouchKit.addGestureRecognizer(tapRecognizer);
    }

	void handleOverlap(GameObject wall)
	{
        int count = wallList.Count - 1;

        Dictionary<GameObject, Vector> wallsToSplit = new Dictionary<GameObject, Vector>();

        for (int i = 0; i < count; i++)
        {
            if(wall.GetComponent<Wall>().startNode != wallList[i].GetComponent<Wall>().endNode)
            { 
                Vector intersectionPoint = new Vector();
                Vector p1 = new Vector(wall.GetComponent<Wall>().startNode.transform.position.x, wall.GetComponent<Wall>().startNode.transform.position.y);
                Vector p2 = new Vector(wall.GetComponent<Wall>().endNode.transform.position.x, wall.GetComponent<Wall>().endNode.transform.position.y);

                Vector q1 = new Vector(wallList[i].GetComponent<Wall>().startNode.transform.position.x, wallList[i].GetComponent<Wall>().startNode.transform.position.y);
                Vector q2 = new Vector(wallList[i].GetComponent<Wall>().endNode.transform.position.x, wallList[i].GetComponent<Wall>().endNode.transform.position.y);
                if (LineSegementsIntersect(p1, p2, q1, q2, out intersectionPoint, true))
                {
                    print("Wall " + wall.name + " Overlaps with " + wallList[i] + " at point " + intersectionPoint.X + " " + intersectionPoint.Y);
                    if(!double.IsNaN(intersectionPoint.X) || !double.IsNaN(intersectionPoint.Y))
                    {
                        wallsToSplit.Add(wallList[i], intersectionPoint);
                    }
                }
            }
        }

        for (int index = 0; index < wallsToSplit.Count; index++)
        {
            var item = wallsToSplit.ElementAt(index);
            print("splitting wall " + item.Key + " At position " + (float) item.Value.X + " " + (float) item.Value.Y);
            splitWall(item.Key, new Vector3((float)item.Value.X, (float)item.Value.Y, 0));
        }
    }

    void splitWall(GameObject wall, Vector3 position)
    {
        GameObject newNode = instantiateIntersectionNode(position);
        GameObject startNode = wall.GetComponent<Wall>().startNode;
        GameObject endNode = wall.GetComponent<Wall>().endNode;

        /*startNode.GetComponent<Node>().adjacentNodes.Remove(endNode);
        startNode.GetComponent<Node>().adjacentNodes.Add(newNode);
        newNode.GetComponent<Node>().adjacentNodes.Add(endNode);*/

        Vector3 scale = wall.transform.localScale;
        int multiplier = 1;
        if (scale.x < 0)
        {
            multiplier = -1;
        }

        instantiateWall(newNode, endNode, wall.transform.rotation, multiplier);

        wall.GetComponent<Wall>().endNode = newNode;

        wall.transform.localScale = new Vector3(multiplier * Vector3.Distance(startNode.transform.position, wall.GetComponent<Wall>().endNode.transform.position), scale.y, scale.z);

    }



    void instantiateWall(Vector3 position)
    {
        newWall = GameObject.Instantiate(wallSprite);
        newWall.name = "Wall" + wallList.Count();
        newWall.transform.parent = wallContainer;
        newWall.transform.position = _initialPos;
        //newWall.GetComponent<BoxCollider>().enabled = false;
        Wall w = newWall.GetComponent<Wall>();
        if (currentNode == null)
        {
            w.startNode = initialNode;
        }
        else
        {
            w.startNode = currentNode;
        }
        wallList.Add(newWall);
    }

    void instantiateWall(GameObject startNode, GameObject endNode, Quaternion rotation, int multiplier)
    {
        newWall = GameObject.Instantiate(wallSprite);
        newWall.name = "Wall" + wallList.Count();
        newWall.transform.parent = wallContainer;
        newWall.transform.position = startNode.transform.position;

        newWall.transform.localScale = new Vector3(multiplier * Vector3.Distance(startNode.transform.position, endNode.transform.position), 0.2f, 1);
		newWall.transform.rotation = rotation;
        Wall w = newWall.GetComponent<Wall>();
        w.startNode = startNode;
        w.endNode = endNode;
        wallList.Add(newWall);
    }

    GameObject instantiateNode(Vector3 position)
    {
        GameObject newNode = NormalizeNodeAtPoint(position);
        if (newNode == null)
        {
            newNode = GameObject.Instantiate(nodeSprite);
            newNode.transform.position = position;
            newNode.transform.parent = nodeContainer;
            newNode.name = "Node " + nodeList.Count();
            nodeList.Add(newNode);
        }
        if (!didDraw)
        {
            initialNode = newNode;
        }
        if (currentNode != null)
        {
            if (newNode != currentNode)
            {
                currentNode.GetComponent<Node>().adjacentNodes.Add(newNode);
            }
        }
        currentNode = newNode;
        return newNode;
    }

    GameObject instantiateIntersectionNode(Vector3 position)
    {
        GameObject newNode = NormalizeNodeAtPoint(position);
        if(newNode == null)
        { 
            newNode = GameObject.Instantiate(nodeSprite);
            newNode.transform.position = position;
            newNode.transform.parent = nodeContainer;
            newNode.name = "Node " + nodeList.Count();
        
            nodeList.Add(newNode);
        }
        print("Instantiating intersection node with name " + newNode.name);

        return newNode;
    }

    GameObject NormalizeNodeAtPoint(Vector3 position)
    {
        print("Normalizing");
        for (int i = 0; i < nodeList.Count; i++)
        {
            if (nodeList[i].GetComponent<Renderer>().bounds.Contains(position))
            {
                print("Overlap with node " + nodeList[i]);
                return nodeList[i];
            }
        }
        return null;
    }


    void setPreviousWallEndNode()
    {
        wallList.Last().GetComponent<Wall>().endNode = instantiateNode(_initialPos);
        //wallList.Last().GetComponent<BoxCollider>().enabled = true;
    }

    // Update is called once per frame
    void Update () {

        detectRightClick();

        if (newWall != null && isDrawing)
        {
            _currentPos = GetCurrentMousePosition(Input.mousePosition).GetValueOrDefault();
			if(gameObject.GetComponent<BoxCollider>().bounds.Contains(transform.TransformPoint(_currentPos)))
			{
            	Vector3 difference = _currentPos - _initialPos;

            	float newX = difference.magnitude; //The new X scale for the 

            	if (difference.x < 0)
            	{
           	    	newX *= -1;
				}

            	//Need to give new value of rotation for the wall script
            	Quaternion newRotation = Quaternion.LookRotation(_initialPos - _currentPos, Vector3.up);
            	newRotation.x = 0.0f;
            	newRotation.y = 0.0f;
            	_xRotation = Mathf.Round(newRotation.eulerAngles.z / 15) * 15;
            	newRotation = Quaternion.Euler(newRotation.x, newRotation.y, _xRotation);

            	//wallList.Last().transform.rotation = newRotation;
            	newWall.transform.rotation = newRotation;

            	//wallList.Last().transform.localScale = new Vector3(newX, wallList.Last().transform.localScale.y, wallList.Last().transform.localScale.z);
            	//newX = Mathf.Round(newX * 0.5f) / 0.5f;
            	Vector3 newScale = new Vector3(newX, newWall.transform.localScale.y, newWall.transform.localScale.z);
            	newWall.transform.localScale = newScale;
			}
        }
    }

    private void detectRightClick()
    {
        if (Input.GetMouseButtonDown(1))
        {
			removeDrawingWall();
        }
    }

	private void removeDrawingWall()
	{
		if (currentNode != null) {
			wallList.Remove (newWall);
			GameObject.DestroyImmediate (newWall);
		
			if (currentNode.GetComponent<Node> ().adjacentNodes.Count == 0 && !didDraw) {
				nodeList.Remove (currentNode);
				GameObject.Destroy (currentNode);
			}
			isDrawing = false;
			currentNode = null;
		}
	}

    private Vector3? GetCurrentMousePosition(Vector3 screenPosition)
    {
        if(Camera.main != null)
        { 
            var ray = Camera.main.ScreenPointToRay(screenPosition);
            var plane = new Plane(Vector3.forward, Vector3.zero);

            float rayDistance;
            if (plane.Raycast(ray, out rayDistance))
            {
                return ray.GetPoint(rayDistance);
            }
        }
        return null;
    }

	public List<GameObject> exportNodes()
	{
		return nodeList;		
	}

    public List<GameObject> exportObjects()
    {
        return houseObjectList;
    }

    public List<GameObject> exportWindows()
    {

            //(GetComponent<Renderer>().bounds.center, GetComponent<Renderer>().bounds.extents * 1.1f, Vector3.forward, transform.rotation, float.PositiveInfinity, layerMask);

        for (int i = 0; i < windowList.Count; i++)
        {
            RaycastHit[] hitList = Physics.RaycastAll(transform.TransformPoint(windowList[i].transform.position), Vector3.forward);

            int correctWallIndex = -1;

            for (int j = 0; j < hitList.Length; j++)
            {
                print("The ray hit" + hitList[j].transform.name);
                if (hitList[j].transform.name.Contains("Wall"))
                {
                    if(Mathf.Approximately(hitList[j].transform.rotation.eulerAngles.z, windowList[i].transform.rotation.eulerAngles.z))
                    { 
                        correctWallIndex = j;
                    }
                    break;
                }
            }
            
            WallAttachableObject w = windowList[i].GetComponent<WallAttachableObject>();
            if(correctWallIndex < hitList.Length && correctWallIndex != -1)
            { 
                w.startNode = hitList[correctWallIndex].transform.GetComponent<Wall>().startNode;
                w.endNode = hitList[correctWallIndex].transform.GetComponent<Wall>().endNode;
            }
        }
        return windowList;
    }

    public void Refresh()
    {
        nodeList.Clear();
        nodeContainer.DestroyChildren();

        wallList.Clear();
        wallContainer.DestroyChildren();

        windowList.Clear();
        windowContainer.DestroyChildren();

        houseObjectList.Clear();
        houseObjectcontainer.DestroyChildren();

        isDrawing = false;
        didDraw = false;
       
    }

    /// <summary>
    /// Test whether two line segments intersect. If so, calculate the intersection point.
    /// <see cref="http://stackoverflow.com/a/14143738/292237"/>
    /// </summary>
    /// <param name="p">Vector to the start point of p.</param>
    /// <param name="p2">Vector to the end point of p.</param>
    /// <param name="q">Vector to the start point of q.</param>
    /// <param name="q2">Vector to the end point of q.</param>
    /// <param name="intersection">The point of intersection, if any.</param>
    /// <param name="considerOverlapAsIntersect">Do we consider overlapping lines as intersecting?
    /// </param>
    /// <returns>True if an intersection point was found.</returns>
    public static bool LineSegementsIntersect(Vector p, Vector p2, Vector q, Vector q2,
        out Vector intersection, bool considerCollinearOverlapAsIntersect = false)
    {
        intersection = new Vector();

        var r = p2 - p;
        var s = q2 - q;
        var rxs = r.Cross(s);
        var qpxr = (q - p).Cross(r);

        // If r x s = 0 and (q - p) x r = 0, then the two lines are collinear.
        if (rxs.IsZero() && qpxr.IsZero())
        {
            // 1. If either  0 <= (q - p) * r <= r * r or 0 <= (p - q) * s <= * s
            // then the two lines are overlapping,
            if (considerCollinearOverlapAsIntersect)
                if ((0 <= (q - p) * r && (q - p) * r <= r * r) || (0 <= (p - q) * s && (p - q) * s <= s * s))
                    return true;

            // 2. If neither 0 <= (q - p) * r = r * r nor 0 <= (p - q) * s <= s * s
            // then the two lines are collinear but disjoint.
            // No need to implement this expression, as it follows from the expression above.
            return false;
        }

        // 3. If r x s = 0 and (q - p) x r != 0, then the two lines are parallel and non-intersecting.
        if (rxs.IsZero() && !qpxr.IsZero())
            return false;

        // t = (q - p) x s / (r x s)
        var t = (q - p).Cross(s) / rxs;

        // u = (q - p) x r / (r x s)

        var u = (q - p).Cross(r) / rxs;

        // 4. If r x s != 0 and 0 <= t <= 1 and 0 <= u <= 1
        // the two line segments meet at the point p + t r = q + u s.
        if (!rxs.IsZero() && (0 <= t && t <= 1) && (0 <= u && u <= 1))
        {
            // We can calculate the intersection point using either t or u.
            intersection = p + t * r;

            // An intersection was found.
            return true;
        }

        // 5. Otherwise, the two line segments are not parallel but do not intersect.
        return false;
    }
}
