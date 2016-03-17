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
        print("Handling overlap position = " + position);
        Ray ray = Camera.main.ScreenPointToRay(position);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            print("Hit something -> "+ hit.collider.gameObject.name);
            if (hit.collider.gameObject.tag == "Wall")
            {
                print("Now split wall for wall" + hit.collider.gameObject.name);
                splitWall(hit.collider.gameObject);
            }
            // raycast hit this gameobject
        }
    }

    void splitWall(GameObject wall)
    {
        int wallIndex = wallList.IndexOf(wall);
        GameObject startNode = wall.GetComponent<Wall>().startNode;
        GameObject endNode = wall.GetComponent<Wall>().endNode;
        instantiateWall(currentNode, endNode);
        wall.GetComponent<Wall>().endNode = currentNode;
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

    void instantiateWall(GameObject startNode, GameObject endNode)
    {
        newWall = GameObject.Instantiate(wallSprite);
        newWall.name = "Wall" + wallList.Count();
        newWall.transform.parent = wallContainer;
        newWall.transform.position = startNode.transform.position;
        newWall.transform.localScale = endNode.transform.position - startNode.transform.position;
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
                Debug.Log("Pressed right click. New Wall is " + newWall);
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
}
