using UnityEngine;
using System.Collections;

public class UIGridRenderer : MonoBehaviour {

	public GameObject gridLine;
	Vector3 bottomLeft;
	Vector3 bottomRight;
	Vector3 topRight;

	void Start () {
		Bounds bounds = NGUIMath.CalculateRelativeWidgetBounds(transform);
		print (bounds);
		float yExtent = bounds.extents.y;
		float xExtent = bounds.extents.x;
		bottomLeft = new Vector3(bounds.center.x - bounds.extents.x, -bounds.extents.y);
		bottomRight = new Vector3(bounds.center.x + bounds.extents.x, -bounds.extents.y);
		topRight = new Vector3(bounds.center.x + bounds.extents.x, bounds.extents.y);
		RenderLines (new Vector3 (xExtent * 2, yExtent * 2), 2);
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
			GameObject line = NGUITools.AddChild(gameObject, gridLine);
			line.transform.position = new Vector3(bottomLeft.x + i + adjustX, bottomLeft.y, 1);
			//lineRenderer.SetWidth(thickness, thickness);
			//lineRenderer.SetPosition(0, new Vector3(bottomLeft.x + i + adjustX, bottomLeft.y, 1));
			//lineRenderer.SetPosition(1, new Vector3(bottomLeft.x + i + adjustX, topRight.y, 1));
		}
		
		//Generate line from left to right
		/*for (int j = 0; j < numberOfLines.y; j++)
		{
			GameObject go = GameObject.Instantiate(gridLine);
			go.transform.parent = transform; //Make this the parent
			LineRenderer lineRenderer = go.GetComponent<LineRenderer>();
			lineRenderer.SetVertexCount(2);
			lineRenderer.SetWidth(thickness, thickness);
			lineRenderer.SetPosition(0, new Vector3(bottomLeft.x, bottomLeft.y + adjustY + j, 1));
			lineRenderer.SetPosition(1, new Vector3(bottomRight.x , bottomLeft.y + adjustY + j, 1));
		}*/
		
	}

}
