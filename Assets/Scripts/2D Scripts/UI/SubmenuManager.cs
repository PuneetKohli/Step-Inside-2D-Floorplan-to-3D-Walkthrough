using UnityEngine;
using System.Collections;

public class SubmenuManager : MonoBehaviour {
    ClickManager clickManager;
    public GameObject submenuItemPrefab;
    GameObject submenuTitle;
    GameObject submenuScrollview;

    // Use this for initialization
    void Start () {
        clickManager = GameObject.Find("2DManager").GetComponent<ClickManager>();
        submenuTitle = GameObject.Find("Sub Menu Title");
        submenuScrollview = GameObject.Find("Sub Menu Scroll View");
        ReloadSubmenu(0);
    }

    // Update is called once per frame
    void Update () {
	
	}

    public void ReloadSubmenu(int index)
    {
        submenuTitle.GetComponent<UILabel>().text = clickManager.itemNames[index] + "s";
        NGUITools.DestroyChildren(submenuScrollview.transform);


        if (clickManager.GetType().GetField(clickManager.itemNames[index] + "Names") != null)
        {
            string[] submenuItems = (string[])clickManager.GetType().GetField(clickManager.itemNames[index] + "Names").GetValue(clickManager);
            for (int i = 0; i < submenuItems.Length; i++)
            {
                GameObject submenuItem = NGUITools.AddChild(submenuScrollview, submenuItemPrefab);
                submenuItem.transform.GetComponent<SubmenuItem>().init(clickManager.itemNames[index], submenuItems[i]);
            }
            submenuScrollview.GetComponent<UIGrid>().Reposition();
        }
    }
}
