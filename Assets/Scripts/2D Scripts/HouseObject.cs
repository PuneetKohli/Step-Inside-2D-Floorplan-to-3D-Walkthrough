using UnityEngine;
using System.Collections;

public class HouseObject : MonoBehaviour {

    Transform background;
    public bool isPlacable = true;
    public Color placable, notPlacable;

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
    }

    void PlaceObject()
    {
        background.GetComponent<Renderer>().material.color = Color.white;
    }
}
