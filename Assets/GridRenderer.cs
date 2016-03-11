using UnityEngine;
using System.Collections;

public class GridRenderer : MonoBehaviour {

    public GameObject gridLine;
    Vector3 bottomLeft;
    Vector3 bottomRight;
    Vector3 topRight;

int gridSize = 1;

	// Use this for initialization
	void Start () {
        Vector3 screenDimensions = CalculateScreenSizeInWorldCoords();
        print("Screen height" + Screen.height);
        float pxThickness = 1 / (Screen.height / (Camera.main.orthographicSize * 2));
        print(pxThickness);
        RenderLines(screenDimensions, pxThickness);
    }
	
	// Update is called once per frame
	void Update () {
	
	}
    void RenderLines(Vector2 dimensions, float thickness)
    {
        Vector2 numberOfLines = new Vector2(Mathf.CeilToInt(dimensions.x), Mathf.CeilToInt(dimensions.y));
        print("height" + dimensions.y);
        float adjustY = Mathf.Abs((int)dimensions.y - dimensions.y) / 2 + 0.5f;
        print("Adjust y" + adjustY);
        float adjustX = Mathf.Abs((int)dimensions.x - dimensions.x) / 2;

        //Generate line from top to bottom
        for (int i = 0; i < numberOfLines.x; i++)
        {
            GameObject go = GameObject.Instantiate(gridLine);
            go.transform.parent = transform; //Make this the parent
            LineRenderer lineRenderer = go.GetComponent<LineRenderer>();
            lineRenderer.SetVertexCount(2);
            lineRenderer.SetWidth(thickness, thickness);
            lineRenderer.SetPosition(0, new Vector3(bottomLeft.x + i + adjustX, bottomLeft.y, 1));
            lineRenderer.SetPosition(1, new Vector3(bottomLeft.x + i + adjustX, topRight.y, 1));
        }

        //Generate line from left to right
        for (int j = 0; j < numberOfLines.y; j++)
        {
            GameObject go = GameObject.Instantiate(gridLine);
            go.transform.parent = transform; //Make this the parent
            LineRenderer lineRenderer = go.GetComponent<LineRenderer>();
            lineRenderer.SetVertexCount(2);
            lineRenderer.SetWidth(thickness, thickness);
            lineRenderer.SetPosition(0, new Vector3(bottomLeft.x, bottomLeft.y + adjustY + j, 1));
            lineRenderer.SetPosition(1, new Vector3(bottomRight.x , bottomLeft.y + adjustY + j, 1));
        }

    }
    Vector2 CalculateScreenSizeInWorldCoords()
    {
        Camera cam = Camera.main;
        bottomLeft = cam.ViewportToWorldPoint(new Vector3(0, 0, cam.nearClipPlane)); //Bottom Left Point (0,0)
        bottomRight = cam.ViewportToWorldPoint(new Vector3(1, 0, cam.nearClipPlane)); //Bottom Right Point (0,1)
        topRight = cam.ViewportToWorldPoint(new Vector3(1, 1, cam.nearClipPlane)); //Top Right Point (1,1)
        float width  = (bottomRight - bottomLeft).magnitude;
        float height  = (topRight - bottomRight).magnitude;
 
        Vector2 dimensions = new Vector2(width, height);
 
        return dimensions;
 }
}
