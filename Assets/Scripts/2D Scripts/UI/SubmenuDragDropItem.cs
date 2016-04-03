using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class SubmenuDragDropItem : UIDragDropItem
{
    public GameObject houseObject, attachableHouseObject;
    Transform houseObjectContainer, attachableObjectContainer;
    public LayerMask layerMask;
    GameObject realWorldItem = null;
    GameObject gameContainer;
    bool isDragging = false;

    Transform originalParent = null;

    protected override void OnDragDropStart()
    {
        gameContainer = GameObject.Find("2DManager");
        houseObjectContainer = GameObject.Find("HouseObjectContainer").transform;
        attachableObjectContainer = GameObject.Find("AttachableObjectContainer").transform;

        this.isDragging = true;
        if (originalParent.GetComponent<SubmenuItem>().category.Equals("windowsanddoor"))
        {
            this.realWorldItem = GameObject.Instantiate(attachableHouseObject);
            this.realWorldItem.GetComponent<HouseObject>().init(originalParent.GetComponent<SubmenuItem>().category, originalParent.name, true);
            this.realWorldItem.transform.parent = attachableObjectContainer;
        }
        else
        {
            this.realWorldItem = GameObject.Instantiate(houseObject);
            this.realWorldItem.GetComponent<HouseObject>().init(originalParent.GetComponent<SubmenuItem>().category, originalParent.name, false);
            this.realWorldItem.transform.parent = houseObjectContainer;
        }
        base.OnDragDropStart();

    }

    protected override void OnClone(GameObject original)
    {
        originalParent = original.transform.parent;
        this.transform.localScale = Vector3.zero;
        base.OnClone(original);
    }

    protected override void OnDragStart()
    {
        this.isDragging = true;
        base.OnDragStart();
    }

    protected override void OnDragEnd()
    {
        this.enabled = false;
        this.isDragging = false;
        base.OnDragEnd();
    }

    protected override void OnDragDropEnd()
    {
        this.isDragging = false;
        if (realWorldItem != null)
        {
            if (!realWorldItem.GetComponent<HouseObject>().isPlacable)
            {
                GameObject.Destroy(realWorldItem);
            }
            else
            {
                realWorldItem.transform.position = new Vector3(realWorldItem.transform.position.x, realWorldItem.transform.position.y, -10);

                if (!gameContainer.GetComponent<BoxCollider>().bounds.Intersects(realWorldItem.GetComponent<Renderer>().bounds))
                {
                    GameObject.Destroy(realWorldItem);
                }
                else
                {
                    realWorldItem.transform.position = new Vector3(realWorldItem.transform.position.x, realWorldItem.transform.position.y, 0);
                    realWorldItem.SendMessage("PlaceObject");
                }
            }
        }
        base.OnDragDropEnd();
    }

    protected override void OnDrag(Vector2 delta)
    {
        handleDrag();
        base.OnDrag(delta);
    }

    void handleDrag()
    {
        if (this.isDragging && realWorldItem != null)
        {
            print(realWorldItem);
            realWorldItem.transform.position = GetCurrentMousePosition(Input.mousePosition).GetValueOrDefault();
            RaycastHit[] hitList = Physics.BoxCastAll(GetCurrentMousePosition(Input.mousePosition).GetValueOrDefault(), realWorldItem.GetComponent<Renderer>().bounds.extents * 1.1f, Vector3.forward, transform.rotation, float.PositiveInfinity, layerMask);
            if (hitList.Length > 0)
            {
                if (!realWorldItem.GetComponent<HouseObject>().isWallAttachable)
                {
                    realWorldItem.SendMessage("MakeNotPlacable");
                }
                else
                {
                    realWorldItem.SendMessage("MakePlacable");
                }

                /*for (int i = 0; i < hitList.Length; i++)
                {
                    print("Hit with object " + hitList[i].transform.name);
                }*/
            }
            else
            {
                if (!realWorldItem.GetComponent<HouseObject>().isWallAttachable)
                {
                    realWorldItem.SendMessage("MakePlacable");
                }
                else
                {
                    realWorldItem.SendMessage("MakeNotPlacable");
                }
            }
        }
    }

    protected override void Update()
    {
        if (this.isDragging)
        {
            if (Input.GetKeyDown(KeyCode.R) && realWorldItem != null)
            {
                print("Hit key R" + realWorldItem);
                realWorldItem.transform.Rotate(Vector3.forward, 90f);
            }
        }
        detectRightClick();

        base.Update();
    }

    void detectRightClick()
    {
        if (Input.GetMouseButtonDown(1))
        {
            if (realWorldItem != null)
            {
                Destroy(realWorldItem);
            }
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
