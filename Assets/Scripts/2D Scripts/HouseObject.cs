using UnityEngine;
using System.Collections;

public class HouseObject : MonoBehaviour {

    Transform background;
    public bool isPlacable = true;
    public bool isWallAttachable = false;
    public Color placable, notPlacable;
    public LayerMask layerMask;
    protected WallManager wallManager;
    public string category;

    void OnEnable()
    {
        background = transform.GetChild(0);
    }

    // Use this for initialization
    protected virtual void Start () {
        wallManager = GameObject.Find("2DManager").GetComponent<WallManager>();
        print("Wall manager is " + wallManager);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    protected virtual void MakeNotPlacable()
    {
        print("Objct is unplacable");
        background.GetComponent<Renderer>().material.color = notPlacable;
        isPlacable = false;
    }

    protected virtual void MakePlacable()
    {
        print("Object is placable");
        background.GetComponent<Renderer>().material.color = placable;
        isPlacable = true;
    }

    protected virtual void PlaceObject()
    {
        background.GetComponent<Renderer>().material.color = Color.white;
        GetComponent<BoxCollider>().enabled = true;
    }

    public virtual void init(string category, string name, bool isWallAttachable)
    {
        GetComponent<Renderer>().material.mainTexture = Resources.Load("furniture/2D_Top/" + category + "/" + name) as Texture2D;
        float height = GetComponent<Renderer>().material.mainTexture.height;
        float width = GetComponent<Renderer>().material.mainTexture.width;
        float aspect = width / height; //2
        float multiplier = 2;
        this.category = category;
        if (aspect > 1)
        {
            multiplier = scaleDown(aspect);
            print("Multiplier is " + multiplier);
            if (multiplier < 0.5f)
            {
                multiplier *= scaleUp(multiplier);
            }
        }

        transform.localScale = new Vector3(multiplier *  aspect, multiplier, 1f);
        transform.name = name;
        this.isWallAttachable = isWallAttachable;
    }

    float scaleDown(float f)
    {
        return 1/f;
    }

    float scaleUp(float f)
    {
        return 0.5f/f;
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
