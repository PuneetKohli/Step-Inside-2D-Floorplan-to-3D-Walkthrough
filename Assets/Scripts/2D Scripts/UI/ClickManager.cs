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
    GameObject _3DRoot, _2DRoot;
    Transform _3DUIRoot, _2DUIRoot;
    GameObject player, isoCam;
    GameObject mainMenuScrollView, submenu;
    WallGenerator wallGenerator;


	// Use this for initialization
	void Awake () {
        _3DRoot = GameObject.Find("3D Root");
        wallGenerator = _3DRoot.GetComponent<WallGenerator>();
        isoCam = _3DRoot.transform.Find("Isocam").gameObject as GameObject;
        player = _3DRoot.transform.Find("Player").gameObject as GameObject;
        _3DRoot.SetActive(false);
        _2DRoot = GameObject.Find("2D Root");
        wallManager = GameObject.Find("2DManager").GetComponent<WallManager>();
        //_2DRoot = GameObject.Find("2D Root");
        //_3DRoot = GameObject.Find("3D Root");
        mainMenuScrollView = GameObject.Find("Main Menu Scroll View");
        submenu = GameObject.Find("Sub Menu");

        itemNames = new string[] { "windows & door", "table", "bed", "chair"};
        windowsanddoorNames = new string[]{ "door1", "door2", "window1", "window2"};
        tableNames = new string[] { "simple", "square", "long", "side", "oval", "stylish", "metallic", "coffeetop"};
        //cupboardNames = new string[] { "wooden", "metal", "glass", "circle" };
        bedNames = new string[] { "simple", "lowered", "side-table", "double-table"};
        chairNames = new string[] { "red", "wooden", "round"};

        _3DUIRoot = transform.Find("3D UI Root");
        _2DUIRoot = transform.Find("2D UI Root");

        _3DUIRoot.gameObject.SetActive(false);

    }

    public void clicked3DView()
    {
        print("3d view!");
        _3DRoot.SetActive(true);
        print("Wall generator is" + wallGenerator + " Wall manager is " + wallManager);
        wallGenerator.generate3D(wallManager.exportNodes(), wallManager.exportWindows(), wallManager.exportObjects());
        _2DRoot.SetActive(false);
        EnableIsoCam();
        SwapUIMode();
    }

    public void clicked2DView()
    {
        _3DRoot.transform.Find("3DContainer").DestroyChildren();
        wallGenerator.Refresh();
        _3DRoot.SetActive(false);
        _2DRoot.SetActive(true);
        SwapUIMode();
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

    public void ClickedClear()
    {
        wallManager.Refresh();
    }

    public void ClickedWalkthrough()
    {
        EnablePlayerCam();
    }
    public int IndexOfItem(string itemName)
    {
        return itemNames.ToList().IndexOf(itemName);
    }

    public string[] getItemNames()
    {
        return itemNames;
    }

    void SwapUIMode()
    {
        bool swapTo2D = !_2DUIRoot.gameObject.activeInHierarchy;

        _2DUIRoot.gameObject.SetActive(swapTo2D);
        _3DUIRoot.gameObject.SetActive(!swapTo2D);
    }

    void EnableIsoCam()
    {
        isoCam.SetActive(true);
        player.SetActive(false);
    }

    void EnablePlayerCam()
    {        
        player.SetActive(true);
        isoCam.SetActive(false);
    }
}
