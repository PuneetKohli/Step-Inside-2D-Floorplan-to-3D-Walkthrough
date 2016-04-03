using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class MainMenuScrollView : MonoBehaviour {

	public UIAtlas appUIAtlas;
	public GameObject mainMenuItem;
    ClickManager clickManager;

	// Use this for initialization
	void Start () {
        clickManager = GameObject.Find("UI Root").GetComponent<ClickManager>();
        print("Items in click manager is " + clickManager.getItemNames().Length);
		foreach (string item in clickManager.getItemNames()) {
			GameObject go = NGUITools.AddChild(gameObject, mainMenuItem);
            go.name = item;
            print("Item is " + item);
			go.GetComponentInChildren<UILabel>().text = item.ToUpper() + "S";
			go.GetComponentInChildren<UISprite>().atlas = appUIAtlas;  
			go.transform.GetChild(1).GetComponent<UISprite>().spriteName = item; 
		}
		GetComponent<UIGrid> ().Reposition ();
	}

	// Update is called once per frame
	void Update () {
	
	}
}
