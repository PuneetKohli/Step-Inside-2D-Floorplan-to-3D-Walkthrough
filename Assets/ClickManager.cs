using UnityEngine;
using System.Collections;

public class ClickManager : MonoBehaviour {

    WallManager wallManager;
    GameObject _3DRoot, _2DRoot;
    WallGenerator wallGenerator;


	// Use this for initialization
	void Start () {
        wallManager = GetComponent<WallManager>();
        _2DRoot = GameObject.Find("2D Root");
        _3DRoot = GameObject.Find("3D Root");
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
}
