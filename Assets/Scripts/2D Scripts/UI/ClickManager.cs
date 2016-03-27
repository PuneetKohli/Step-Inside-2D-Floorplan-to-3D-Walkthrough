using UnityEngine;
using System.Linq;

public class ClickManager : MonoBehaviour {

    public string[] itemNames = new string[] { "cupboard", "table", "bed", "chair", "sofa", "lamp", "sink" };
    [HideInInspector]
    public string[] tableNames = new string[] { "simple", "wooden", "metal", "glass", "circle" };
    [HideInInspector]
    public string[] cupboardNames = new string[] {"wooden", "metal", "glass", "circle" };
    [HideInInspector]
    public string[] bedNames = new string[] { "bed1", "bed2", "bed3", "bed4", "bed5" };
    [HideInInspector]
    public string[] chairNames = new string[] { "red", "wooden", "metal", "stylish", "plastic" };

    WallManager wallManager;
    GameObject _3DRoot, _2DRoot, mainMenuScrollView, submenu;
    WallGenerator wallGenerator;


	// Use this for initialization
	void Start () {
        wallManager = GetComponent<WallManager>();
        _2DRoot = GameObject.Find("2D Root");
        _3DRoot = GameObject.Find("3D Root");
        mainMenuScrollView = GameObject.Find("Main Menu Scroll View");
        submenu = GameObject.Find("Sub Menu");
        _3DRoot.SetActive(false);
        wallGenerator = _3DRoot.GetComponent<WallGenerator>();
    }

    public void clicked3DView()
    {
        print("3d view!");
        print(wallManager.exportNodes().Count);
        _3DRoot.SetActive(true);
        wallGenerator.generate3D(wallManager.exportNodes());
        _2DRoot.SetActive(false);
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
}
