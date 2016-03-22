using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class DrawWall : MonoBehaviour {

    public GameObject wallSprite;
    public GameObject nodeSprite;
    public Transform wallContainer, nodeContainer;
    GameObject newWall;
    GameObject initialNode, currentNode;

    public List<GameObject> nodeList = new List<GameObject>();
    public List<GameObject> wallList = new List<GameObject>();
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
            Debug.Log("tap recognizer fired: " + r);

            if (!isDrawing)
            {
                didDraw = false;
                isDrawing = true;
                _initialPos = GetCurrentMousePosition(r.startTouchLocation()).GetValueOrDefault();
                instantiateNode(_initialPos);
            }
            else
            {
                didDraw = true;
                float newXpos = wallList.Last().transform.position.x + wallList.Last().transform.localScale.x * Mathf.Cos(_xRotation * Mathf.PI / 180f);
                float newYpos = wallList.Last().transform.position.y + wallList.Last().transform.localScale.x * Mathf.Sin(_xRotation * Mathf.PI / 180f);

                newXpos = Mathf.Round(newXpos * 100) / 100f;
                newYpos = Mathf.Round(newYpos * 100) / 100f;

                Vector3 newPos = new Vector3(newXpos, newYpos, 0);
                _initialPos = newPos;
                setPreviousWallEndNode();
                handleOverlap(r.startTouchLocation());
            }

            instantiateWall(_initialPos);

        };
        TouchKit.addGestureRecognizer(tapRecognizer);
    }

    void handleOverlap(Vector3 position)
    {
        Ray ray = Camera.main.ScreenPointToRay(position);
        RaycastHit hit;
		wallList.Last ().gameObject.GetComponent<BoxCollider> ().enabled = false;
        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.gameObject.tag == "Wall")
            {
                print("Now split wall for wall" + hit.collider.gameObject.name);
                splitWall(hit.collider.gameObject);
            }
            // raycast hit this gameobject
        }
		wallList.Last().gameObject.GetComponent<BoxCollider> ().enabled = true;
    }

    void splitWall(GameObject wall)
    {
        int wallIndex = wallList.IndexOf(wall);
        GameObject startNode = wall.GetComponent<Wall>().startNode;
        GameObject endNode = wall.GetComponent<Wall>().endNode;

		Vector3 scale = wall.transform.localScale;
		int multiplier = 1;
		if (scale.x < 0) {
			multiplier = -1;
		}

        instantiateWall(currentNode, endNode, wall.transform.rotation, multiplier);

        wall.GetComponent<Wall>().endNode = currentNode;

		wall.transform.localScale = new Vector3 (multiplier * Vector3.Distance(startNode.transform.position, wall.GetComponent<Wall> ().endNode.transform.position), scale.y, scale.z);
	}

    void instantiateWall(Vector3 position)
    {
        newWall = GameObject.Instantiate(wallSprite);
        newWall.name = "Wall" + wallList.Count();
        newWall.transform.parent = wallContainer;
        newWall.transform.position = _initialPos;
        newWall.GetComponent<BoxCollider>().enabled = false;
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
		print ("Multiplier is " + multiplier);
		newWall.transform.localScale = new Vector3(multiplier * Vector3.Distance(startNode.transform.position, endNode.transform.position), 0.2f, 1);
		newWall.transform.rotation = rotation;
        Wall w = newWall.GetComponent<Wall>();
        w.startNode = startNode;
        w.endNode = endNode;
        wallList.Add(newWall);

    }

    GameObject instantiateNode(Vector3 position)
    {
        GameObject newNode = GameObject.Instantiate(nodeSprite);
        newNode.transform.position = position;
        newNode.transform.parent = nodeContainer;
        newNode.name = "Node " + nodeList.Count();
        if (currentNode != null)
        {
            currentNode.GetComponent<Node>().adjacentNodes.Add(newNode);
        }
        nodeList.Add(newNode);
        if (!didDraw)
        {
            initialNode = newNode;
        }
        currentNode = newNode;
        return newNode;
    }

    void setPreviousWallEndNode()
    {
        wallList.Last().GetComponent<Wall>().endNode = instantiateNode(_initialPos);
        wallList.Last().GetComponent<BoxCollider>().enabled = true;

    }

    // Update is called once per frame
    void Update () {

        detectRightClick();

        if (newWall != null && isDrawing)
        {
            _currentPos = GetCurrentMousePosition(Input.mousePosition).GetValueOrDefault();

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

    private void detectRightClick()
    {
        if (Input.GetMouseButtonDown(1))
        {
            wallList.Remove(newWall);
            GameObject.DestroyImmediate(newWall);
            
            if (currentNode.GetComponent<Node>().adjacentNodes.Count == 0 && !didDraw)
            {
                nodeList.Remove(currentNode);
                GameObject.Destroy(currentNode);
            }
            isDrawing = false;
            currentNode = null;
        }
    }

    private Vector3? GetCurrentMousePosition(Vector3 screenPosition)
    {
        var ray = Camera.main.ScreenPointToRay(screenPosition);
        var plane = new Plane(Vector3.forward, Vector3.zero);

        float rayDistance;
        if (plane.Raycast(ray, out rayDistance))
        {
            return ray.GetPoint(rayDistance);
        }

        return null;
    }

	float SignedAngleBetween(Vector3 a, Vector3 b, Vector3 n){
		// angle in [0,180]
		float angle = Vector3.Angle(a,b);
		float sign = Mathf.Sign(Vector3.Dot(n,Vector3.Cross(a,b)));
		
		// angle in [-179,180]
		float signed_angle = angle * sign;
		
		// angle in [0,360] (not used but included here for completeness)
		float angle360 =  (signed_angle + 360) % 360;

		return angle360;
	}

	float AngleDir(Vector3 a, Vector3 b, Vector3 forward) {
		Vector3 perp = Vector3.Cross(a, b);
		float dir = Vector3.Dot(perp, forward);

		print ("DIR IS " + dir);
		if (dir > 0f) {
			return 1f;
		} else if (dir < 0f) {
			return -1f;
		} else {
			return 0f;
		}
	}

	int directionSign(Vector3 a, Vector3 b)
	{
		int sign = Vector3.Cross(a, b).z < 0 ? -1 : 1;
		print ("Direction sign is , " + sign);
		return sign;
	}
}
