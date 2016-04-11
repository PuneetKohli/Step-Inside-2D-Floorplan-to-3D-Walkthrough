using UnityEngine;
using System.Collections;

public class MainMenuScrollItem : MonoBehaviour {

    // Use this for initialization
    ClickManager clickManager;

	void Start () {
        clickManager = GameObject.Find("UI Root").GetComponent<ClickManager>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void SelectedMainmenuItem()
    {
        clickManager.ClickedMainMenuItem(name);
    }
}
