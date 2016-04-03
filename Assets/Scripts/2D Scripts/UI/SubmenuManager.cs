using UnityEngine;
using System.Collections;

public class SubmenuManager : MonoBehaviour {
    ClickManager clickManager;
    public GameObject submenuItemPrefab;
    GameObject submenuTitle;
    GameObject submenuScrollview;

    // Use this for initialization
    void Start () {
        clickManager = GameObject.Find("UI Root").GetComponent<ClickManager>();
        submenuTitle = GameObject.Find("Sub Menu Title");
        submenuScrollview = GameObject.Find("Sub Menu Scroll View");
        ReloadSubmenu(0);
    }

    // Update is called once per frame
    void Update () {
	
	}

    public void ReloadSubmenu(int index)
    {
        submenuTitle.GetComponent<UILabel>().text = clickManager.getItemNames()[index] + "s";
        NGUITools.DestroyChildren(submenuScrollview.transform);

        string name =  clickManager.getItemNames()[index].Replace(" ", "").Replace("&","and");
        print("name is " + name);

        if (clickManager.GetType().GetField(name + "Names") != null)
        {
            string[] submenuItems = (string[])clickManager.GetType().GetField(name + "Names").GetValue(clickManager);
            for (int i = 0; i < submenuItems.Length; i++)
            {
                GameObject submenuItem = NGUITools.AddChild(submenuScrollview, submenuItemPrefab);
                submenuItem.transform.GetComponent<SubmenuItem>().init(name, submenuItems[i]);
            }
            submenuScrollview.GetComponent<UIGrid>().Reposition();
        }

        
    }
   
}
