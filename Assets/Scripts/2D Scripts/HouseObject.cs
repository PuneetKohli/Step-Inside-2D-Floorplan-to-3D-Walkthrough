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

    public void init(string category, string name)
    {
        GetComponent<Renderer>().material.mainTexture = Resources.Load("furniture/2D_Iso/" + category + "/" + name) as Texture2D;
        float height = GetComponent<Renderer>().material.mainTexture.height;
        float width = GetComponent<Renderer>().material.mainTexture.width;
        float aspect = height / width;
        transform.localScale = new Vector3(2f, 2 * aspect, 1f);


    }
}
