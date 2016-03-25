using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class SubmenuDragDropItem : UIDragDropItem
{
    public GameObject prefab;
    public LayerMask layerMask;
    GameObject realWorldItem = null;
    bool isDragging = false;

    protected override void OnDragDropStart()
    {
        print("Drag drop started");
        this.isDragging = true;
        base.OnDragDropStart();
        this.realWorldItem = GameObject.Instantiate(prefab);

    }

    protected override void OnClone(GameObject original)
    {
        print("clone created");
        base.OnClone(original);
    }

    protected override void OnDragStart()
    {
        print("Drag started");
        this.isDragging = true;
        base.OnDragStart();
    }
    
    protected override void OnDragEnd()
    {
        print("Ended");
        this.enabled = false;
        this.isDragging = false;
        base.OnDragEnd();
    }

    protected override void OnDragDropEnd()
    {
        print("Drop ended");
        this.isDragging = false;
        base.OnDragDropEnd();
    }

    protected override void OnDrag(Vector2 delta)
    {
        handleDrag();
    }

    /*protected override void OnDrag(Vector2 delta)
    {
        print("Dragging with current mouse position " + GetCurrentMousePosition(Input.mousePosition).GetValueOrDefault());
        print("But dragged object position is " + transform.position + " With extents " + transform.GetComponent<BoxCollider>().size);
        print("REal world item is " + realWorldItem);
        if (realWorldItem != null)
        {
            print("I have the item!");
            realWorldItem.transform.position = GetCurrentMousePosition(Input.mousePosition).GetValueOrDefault();
        }
        RaycastHit hit = new RaycastHit();

        //Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit[] hit2 = Physics.BoxCastAll(GetCurrentMousePosition(Input.mousePosition).GetValueOrDefault(), transform.GetComponent<BoxCollider>().bounds.extents, Vector3.forward);
        if (hit2.Length > 0)
        {
            print("Box has Hit " + hit2[0]);
           // hit.transform.gameObject.SendMessage("HandleInput");
        }
        base.OnDrag(delta);
    }
   */

    void handleDrag()
    {
        print("IS DRAGGING??? " + isDragging);
        if (this.isDragging && realWorldItem != null)
        {
            print(realWorldItem);
            realWorldItem.transform.position = GetCurrentMousePosition(Input.mousePosition).GetValueOrDefault();
            RaycastHit[] hitList = Physics.BoxCastAll(GetCurrentMousePosition(Input.mousePosition).GetValueOrDefault(), realWorldItem.GetComponent<Renderer>().bounds.extents * 1.1f, Vector3.forward, transform.rotation, float.PositiveInfinity, layerMask);
            if (hitList.Length > 0)
            {
                realWorldItem.SendMessage("makeNotPlacable");
                for (int i = 0; i < hitList.Length; i++)
                {
                    print("Hit with object " + hitList[i].transform.name);
                }
            }
            else
            {
                realWorldItem.SendMessage("makePlacable");
            }
        }
    }
    protected override void Update()
    {
        if(this.isDragging)
        { 
            if (Input.GetKeyDown(KeyCode.R) && realWorldItem != null)
            {
                print("Hit key R" + realWorldItem);
                realWorldItem.transform.Rotate(Vector3.forward, 90f);
            }
        }
        //if(GetCurrentMousePosition(Input.mousePosition).GetValueOrDefault())
        base.Update();
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
