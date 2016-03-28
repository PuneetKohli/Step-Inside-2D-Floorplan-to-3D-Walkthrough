using UnityEngine;
using System.Collections;

public class SubmenuItem : MonoBehaviour {

    UILabel submenuItemLabel;
    UITexture submenuItemSprite;
    public string category;

    public void init(string category, string name)
    {
        submenuItemLabel = transform.Find("SubMenuItemLabel").GetComponent<UILabel>();
        submenuItemSprite = transform.Find("SubMenuItemSprite").GetComponent<UITexture>();
        submenuItemLabel.text = name;
        this.transform.name = name;
        this.category = category;
        submenuItemSprite.mainTexture = Resources.Load("furniture/2D_Iso/" + category + "/" + name) as Texture2D;
    }
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
