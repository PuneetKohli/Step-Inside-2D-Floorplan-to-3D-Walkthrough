using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class DrawWall : MonoBehaviour {

    public GameObject wallSprite;
    public GameObject nodeSprite;

    public GameObject newWall;

    public List<GameObject> nodeList = new List<GameObject>();
    public List<GameObject> wallList = new List<GameObject>();
    private Vector3 _initialPos, _currentPos;
    private float _xRotation;
    private bool isDrawing = false; //This is used to determine whether the user has stopped drawing (right click) and perform necessary action
	// Use this for initialization
	void Start () {
        addTapGesture();
	}

    void addTapGesture()
    {
        TKTapRecognizer tapRecognizer = new TKTapRecognizer();

        tapRecognizer.gestureRecognizedEvent += (r) =>
        {
            Debug.Log("tap recognizer fired: " + r);
            if (newWall != null)
            {
                wallList.Add(newWall);
            }

            newWall = GameObject.Instantiate(wallSprite);
            newWall.name = "Yolo wall";
            if (!isDrawing)
            {
                _initialPos = GetCurrentMousePosition(r.startTouchLocation()).GetValueOrDefault();
                isDrawing = true;
            }
            else
            {
                print("Cos is " + Mathf.Cos(_xRotation * Mathf.PI / 180f));
                float newXpos = wallList.Last().transform.position.x + wallList.Last().transform.localScale.x * Mathf.Cos(_xRotation * Mathf.PI / 180f);
                float newYpos = wallList.Last().transform.position.y + wallList.Last().transform.localScale.x * Mathf.Sin(_xRotation * Mathf.PI / 180f);

                newXpos = Mathf.Round(newXpos * 100) / 100f;
                newYpos = Mathf.Round(newYpos * 100) / 100f;

                Vector3 newPos = new Vector3(newXpos, newYpos, 0);
                //Vector3 newPos = new Vector3(wallList.Last().transform.position.x + wallList.Last().transform.localScale.x, wallList.Last().transform.position.y + wallList.Last().transform.localScale.x, 0);
                _initialPos = newPos;
            }
            newWall.transform.position = _initialPos;

            instantiateNode(_initialPos);

        };
        TouchKit.addGestureRecognizer(tapRecognizer);
    }

    void instantiateNode(Vector3 position)
    {
        GameObject newNode = GameObject.Instantiate(nodeSprite);
        newNode.transform.position = position;
        nodeList.Add(newNode);
        
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
            print("Z rotation is " + newRotation.eulerAngles.z);
            print("X Rotation is " + _xRotation);
            print("New rotation is " + newRotation);

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
            Debug.Log("Pressed right click.");
            GameObject.Destroy(newWall);
            //wallList.Remove(wallList.Last());
            isDrawing = false;
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
