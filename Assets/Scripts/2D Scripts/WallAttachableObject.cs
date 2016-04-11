using UnityEngine;
using System.Collections;

public class WallAttachableObject : HouseObject
{
    public GameObject startNode, endNode;
    public float length;
    public float height;
    public float elevation;

    // Use this for initialization
    protected override void Start()
    {
        isWallAttachable = true;
        base.Start();
    }

    public override void init(string category, string name, bool isWallAttachable)
    {
        base.init(category, name, isWallAttachable);
        if (name.Contains("window"))
        {
            length = 2f;
            height = 2f;
            elevation = 2.5f;
        }
        else if (name.Contains("door"))
        {
            length = 2f;
            height = 4f;
            elevation = height * 0.5f + 0.001f;
        }
    }

    // Update is called once per frame
    void Update()
    {
    }

    protected override void MakePlacable()
    {
        base.MakePlacable();
        if (isWallAttachable)
        {
            RaycastHit[] hitList = Physics.BoxCastAll(GetComponent<Renderer>().bounds.center, GetComponent<Renderer>().bounds.extents * 1.1f, Vector3.forward, transform.rotation, float.PositiveInfinity, layerMask);
            if (hitList.Length > 0)
            {
                for (int i = 0; i < hitList.Length; i++)
                {
                    if (hitList[i].transform.name.Contains("Wall"))
                    {
                        adjustPosition(hitList[i].transform);
                        break;
                    }
                    else
                    {
                        MakeNotPlacable();
                    }
                }
            }
        }
    }

    protected override void PlaceObject()
    {

        RaycastHit[] hitList = Physics.BoxCastAll(GetComponent<Renderer>().bounds.center, GetComponent<Renderer>().bounds.extents * 1.1f, Vector3.forward, transform.rotation, float.PositiveInfinity, layerMask);
        int firstWallPos = hitList.Length;
        if (hitList.Length > 0)
        {
            for (int i = 0; i < hitList.Length; i++)
            {
                if (!hitList[i].transform.name.Contains("Wall"))
                {
                    Destroy(gameObject);
                }
                else if (i < firstWallPos)
                {
                    firstWallPos = i;
                }
            }
            adjustPosition(hitList[firstWallPos].transform);
        }
        print(wallManager + " is wall manager");
        wallManager.windowList.Add(gameObject);
        gameObject.name += (wallManager.windowList.Count - 1);
        base.PlaceObject();
    }

    public void adjustPosition(Transform overlap)
    {
        Vector p1 = new Vector(overlap.GetComponent<Wall>().startNode.transform.position.x, overlap.GetComponent<Wall>().startNode.transform.position.y);
        Vector p2 = new Vector(overlap.GetComponent<Wall>().endNode.transform.position.x, overlap.GetComponent<Wall>().endNode.transform.position.y);

        Vector q1 = new Vector(-20, transform.position.y);
        Vector q2 = new Vector(20, transform.position.y);

        Transform startNode = overlap.GetComponent<Wall>().startNode.transform;
        Transform endNode = overlap.GetComponent<Wall>().endNode.transform;

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
            if (!double.IsNaN(intersectionPoint.X) && !double.IsNaN(intersectionPoint.Y))
            {
                transform.position = new Vector3((float)intersectionPoint.X, (float)intersectionPoint.Y, transform.position.z);
                transform.rotation = overlap.rotation;
            }
        }
    }

}