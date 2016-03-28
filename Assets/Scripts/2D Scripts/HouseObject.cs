using UnityEngine;
using System.Collections;

public class HouseObject : MonoBehaviour {

    Transform background;
    public bool isPlacable = true;
    public bool isWallAttachable = false;
    public Color placable, notPlacable;
    public LayerMask layerMask;


    void OnEnable()
    {
        background = transform.GetChild(0);
    }

    // Use this for initialization
    void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void MakeNotPlacable()
    {
        print("Objct is unplacable");
        background.GetComponent<Renderer>().material.color = notPlacable;
        isPlacable = false;
    }

    void MakePlacable()
    {
        print("Object is placable");
        background.GetComponent<Renderer>().material.color = placable;
        isPlacable = true;
        if (isWallAttachable)
        {
            RaycastHit[] hitList = Physics.BoxCastAll(GetComponent<Renderer>().bounds.center, GetComponent<Renderer>().bounds.extents * 1.1f, Vector3.forward, transform.rotation, float.PositiveInfinity, layerMask);
            if (hitList.Length > 0)
            {
                print("Hit some object after placement");
                adjustPosition(hitList[0].transform, hitList[0].point);
            }
        }
    }

    void PlaceObject()
    {
        background.GetComponent<Renderer>().material.color = Color.white;
        if (isWallAttachable)
        {
            RaycastHit[] hitList = Physics.BoxCastAll(GetComponent<Renderer>().bounds.center, GetComponent<Renderer>().bounds.extents * 1.1f, Vector3.forward, transform.rotation, float.PositiveInfinity, layerMask);
            if (hitList.Length > 0)
            {
                print("Hit some object after placement");
                adjustPosition(hitList[0].transform, hitList[0].point);
            }
        }
    }

    public void adjustPosition(Transform overlap, Vector3 point)
    {
        Vector p1 = new Vector(overlap.GetComponent<Wall>().startNode.transform.position.x, overlap.GetComponent<Wall>().startNode.transform.position.y);
        Vector p2 = new Vector(overlap.GetComponent<Wall>().endNode.transform.position.x, overlap.GetComponent<Wall>().endNode.transform.position.y);
        
        Vector q1 = new Vector(-20, transform.position.y);
        Vector q2 = new Vector(20, transform.position.y);

        Transform startNode = overlap.GetComponent<Wall>().startNode.transform;
        Transform endNode = overlap.GetComponent<Wall>().endNode.transform;
        print("The euler angles is " + overlap.transform.eulerAngles.z);
        if (overlap.transform.rotation.eulerAngles.z < 1 && overlap.transform.rotation.eulerAngles.z > -1)
        {
            q1 = new Vector(transform.position.x, -20);
            q2 = new Vector(transform.position.x, 20);
        }
        else if (overlap.transform.rotation.eulerAngles.z == 180)
        {
            q1 = new Vector(transform.position.x, -20);
            q2 = new Vector(transform.position.x, 20);
        }

        Vector intersectionPoint;
        if (LineSegementsIntersect(p1, p2, q1, q2, out intersectionPoint, true))
        {
            if(!double.IsNaN(intersectionPoint.X) && !double.IsNaN(intersectionPoint.Y))
            { 
                transform.position = new Vector3((float)intersectionPoint.X, (float)intersectionPoint.Y, transform.position.z);
                transform.rotation = overlap.rotation;
            }
        }

        //transform.localPosition = transform.TransformPoint(point);
        //transform.localPosition = new Vector3(transform.localPosition.x, transform.TransformPoint(overlap.localPosition).y, transform.localPosition.z);
        //transform.position = new Vector3(transform.TransformPoint(point).x * Mathf.Cos(overlap.rotation.z * Mathf.PI/180f), transform.TransformPoint(point).x * Mathf.Cos(overlap.rotation.z * Mathf.PI / 180f) / Mathf.Cos(overlap.rotation.z * Mathf.PI / 180f), transform.localPosition.z);
        //transform.rotation = overlap.rotation;
    }

    public void init(string category, string name, bool isWallAttachable)
    {
        GetComponent<Renderer>().material.mainTexture = Resources.Load("furniture/2D_Iso/" + category + "/" + name) as Texture2D;
        float height = GetComponent<Renderer>().material.mainTexture.height;
        float width = GetComponent<Renderer>().material.mainTexture.width;
        float aspect = height / width;
        transform.localScale = new Vector3(2f, 2 * aspect, 1f);
        this.isWallAttachable = isWallAttachable;
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

    public bool isLeft(Vector a, Vector b, Vector c)
    {
        return ((b.X - a.X) * (c.Y - a.Y) - (b.Y - a.Y) * (c.X - a.X)) >= 0;
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
