using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MainMenuScrollView : MonoBehaviour {

	public UIAtlas appUIAtlas;
	public GameObject mainMenuItem;
	string[] itemNames = new string[] { "cupboard", "table", "bed", "chair", "sofa", "lamp", "sink" };
	// Use this for initialization
	void Start () {
		foreach (string item in itemNames) {
			GameObject go = NGUITools.AddChild(gameObject, mainMenuItem);
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
