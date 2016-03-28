using UnityEngine;
using System.Linq;

public class ClickManager : MonoBehaviour {

    private string[] itemNames; 
    [HideInInspector]
    public string[] tableNames;
    [HideInInspector]
    public string[] cupboardNames;
    [HideInInspector]
    public string[] bedNames;
    [HideInInspector]
    public string[] chairNames;
    [HideInInspector]
    public string[] windowsanddoorNames;

WallManager wallManager;
    GameObject _3DRoot, _2DRoot, mainMenuScrollView, submenu;
    WallGenerator wallGenerator;


	// Use this for initialization
	void Awake () {
        wallManager = GetComponent<WallManager>();
        _2DRoot = GameObject.Find("2D Root");
        _3DRoot = GameObject.Find("3D Root");
        mainMenuScrollView = GameObject.Find("Main Menu Scroll View");
        submenu = GameObject.Find("Sub Menu");
        _3DRoot.SetActive(false);
        wallGenerator = _3DRoot.GetComponent<WallGenerator>();

        itemNames = new string[] { "windows & door", "cupboard", "table", "bed", "chair", "sofa", "lamp", "sink" };
        windowsanddoorNames = new string[]{ "window", "door"};
        tableNames = new string[] { "simple", "wooden", "metal", "glass", "circle" };
        cupboardNames = new string[] { "wooden", "metal", "glass", "circle" };
        bedNames = new string[] { "bed1", "bed2", "bed3", "bed4", "bed5" };
        chairNames = new string[] { "red", "wooden", "metal", "stylish", "plastic" };
    }

    public void clicked3DView()
    {
        print("3d view!");
        _3DRoot.SetActive(true);
        wallGenerator.generate3D(wallManager.exportNodes(), wallManager.exportWindows());
        _2DRoot.SetActive(false);
    }

    public void ClickedSave()
    {
        wallManager.exportWindows();
    }
    public void ClickedMainMenuItem(string itemName)
    {
        int index = IndexOfItem(itemName);
        submenu.GetComponent<SubmenuManager>().ReloadSubmenu(index);
    }

    public int IndexOfItem(string itemName)
    {
        return itemNames.ToList().IndexOf(itemName);
    }

    public string[] getItemNames()
    {
        return itemNames;
    }
}
