using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Parse;
using UnityEngine.SceneManagement;

public class MobileManager : MonoBehaviour {

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
    GameObject player, isoCam, uiCam;
    GameObject walkthroughCam, vrCam;
    GameObject mainMenuScrollView, submenu;
    WallGenerator wallGenerator;
    bool didLoadAll = false;

    List<NodeConnection> connectionList = new List<NodeConnection>();
    List<GameObject> houseObjectList = new List<GameObject>(); 
    List<GameObject> windowList = new List<GameObject>(); 
    List<NodeParseObject> nodeList = new List<NodeParseObject>();

    // Use this for initialization
    void Awake () {

        didLoadAll = false;

        _3DRoot = GameObject.Find("3D Root");
        wallGenerator = _3DRoot.GetComponent<WallGenerator>();
        isoCam = _3DRoot.transform.Find("Isocam").gameObject as GameObject;
        player = _3DRoot.transform.Find("Player").gameObject as GameObject;
        uiCam = transform.Find("Camera").gameObject as GameObject;
        walkthroughCam = player.transform.Find("MainCamera").gameObject as GameObject;
        vrCam = player.transform.Find("CardboardMain").gameObject as GameObject;

        vrCam.SetActive(false);

        itemNames = new string[] { "windows & door", "table", "bed", "chair"};
        windowsanddoorNames = new string[]{ "door1", "door2", "window1", "window2"};
        tableNames = new string[] { "simple", "square", "long", "side", "oval", "stylish", "metallic", "coffeetop"};
        //cupboardNames = new string[] { "wooden", "metal", "glass", "circle" };
        bedNames = new string[] { "simple", "lowered", "side-table", "double-table"};
        chairNames = new string[] { "red", "wooden", "round"};

        _3DUIRoot = transform.Find("3D UI Root");

    }

    public void Start()
    {
        StartCoroutine(retrieve3DFromParse(PlayerPrefs.GetString("PlanID", "TLrgYc8Wro")));
    }

    bool didGenerate = false;
    public void Update()
    {
        if (didLoadAll && !didGenerate)
        {
            didGenerate = true;
            wallGenerator.generate3DFromParse(connectionList, houseObjectList, windowList);
        }    

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (vrCam.activeInHierarchy)
            {
                ClickedBackFromVR();
            }
        }

    }

    public void ClickedWalkthrough(GameObject g)
    {
        if (g.GetComponent<UILabel>().text.Equals("Walkthrough"))
        {
            g.GetComponent<UILabel>().text = "Top View";
            EnablePlayerCam();
            player.GetComponent<CharacterController>().enabled = true;
        } else
        {
            g.GetComponent<UILabel>().text = "Walkthrough";
            EnableIsoCam();
        }
    }

    public void ClickedBack()
    {
        SceneManager.LoadScene(0);
    }

    public void ClickedVR()
    {
        EnablePlayerCam();
        player.GetComponent<CharacterController>().enabled = false;
        walkthroughCam.SetActive(false);
        vrCam.SetActive(true);
        player.GetComponent<VRClickScript>().enabled = true;
        uiCam.SetActive(false);
    }

    public void ClickedBackFromVR()
    {
        EnablePlayerCam();
        player.GetComponent<CharacterController>().enabled = true;
        walkthroughCam.SetActive(true);
        vrCam.SetActive(false);
        player.GetComponent<VRClickScript>().enabled = false;
        uiCam.SetActive(true);
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

    IEnumerator retrieve3DFromParse(string planID)  //Input is objectID
    {

        print("Plan ID is " + planID);
        Plan currentPlan = ParseObject.CreateWithoutData<Plan>(planID);

        var connectionQuery = new ParseQuery<NodeConnection>();
        //connectionQuery = connectionQuery.WhereDoesNotExist("plan_id");
        connectionQuery = connectionQuery.Include("start_node");
        connectionQuery = connectionQuery.Include("end_node");
        connectionQuery = connectionQuery.WhereEqualTo("plan_id", currentPlan);
        var connectionQueryTask = connectionQuery.FindAsync();
        while (!connectionQueryTask.IsCompleted)
        {
            yield return null;
        }

        IEnumerable<NodeConnection> nodeConnections = connectionQueryTask.Result;
        saveConnections(nodeConnections);

        var houseobjectQuery = new ParseQuery<HouseParseObject>();
        houseobjectQuery = houseobjectQuery.WhereEqualTo("plan_id", currentPlan);
        //connectionQuery = connectionQuery.WhereDoesNotExist("plan_id");
        var houseQueryTask = houseobjectQuery.FindAsync();

        while (!houseQueryTask.IsCompleted)
        {
            yield return null;
        }

        IEnumerable<HouseParseObject> houseObjects = houseQueryTask.Result;
        saveHouseObjects(houseObjects);  
   }
        
    void saveNodes(IEnumerable<NodeParseObject> nodes)
    {
        print("NODE NODE NODE");
        foreach (NodeParseObject node in nodes)
        {
            nodeList.Add(node);
            print("Added node to local " + nodeList.Last().Xpos);
        }
    }
        
    void saveConnections(IEnumerable<NodeConnection> nodeConnections)
    {
        print("How many connections? " + nodeConnections.Count());

        foreach (NodeConnection nodeConnection in nodeConnections)
        {
            connectionList.Add(nodeConnection);
            print("Added connection to local " + nodeConnection.StartNode.ObjectId + " " + nodeConnection.StartNode.Xpos);
        }
    }

    void saveHouseObjects(IEnumerable<HouseParseObject> houseObjects)
    {
        foreach (HouseParseObject houseObject in houseObjects)
        {
            if (houseObject.Isattached)
            {
                print("Added window to local");
                GameObject g = new GameObject();
                g.AddMissingComponent<WallAttachableObject>();
                g.GetComponent<WallAttachableObject>().length = houseObject.Length;
                g.GetComponent<WallAttachableObject>().height = houseObject.Height;
                g.GetComponent<WallAttachableObject>().elevation = houseObject.Elevation;
                g.transform.position = new Vector3(houseObject.Xpos, houseObject.Ypos, 0f);
                windowList.Add(g);
            } else
            { 
                print("Added house object to local " + houseObject.Xpos);
                GameObject g = new GameObject();
                g.AddMissingComponent<PlacableHouseObject>();
                g.transform.position = new Vector3(houseObject.Xpos, houseObject.Ypos, 0f);
                houseObjectList.Add(g);
            }
        }
        didLoadAll = true;
    }

    IEnumerator save3DToParse(List<HouseParseObject> objectList, Plan currentPlan)
    {
        yield return null;
    }

    IEnumerator saveToParse(List<GameObject> nodeList, List<GameObject> windowList, List<GameObject> objectList, Plan currentPlan)
    {
        List<NodeParseObject> curNodes = new List<NodeParseObject>(); 

        foreach (GameObject node in nodeList)
        {

            var curNode = new NodeParseObject
            { 
                Xpos = node.transform.position.x,
                Ypos = node.transform.position.y,
                PlanId = currentPlan
            };

            curNodes.Add(curNode);
        }

        List<Node> nodeComponentList = new List<Node>();
        for (int i = 0; i < nodeList.Count; i++)
        {
            nodeComponentList.Add(nodeList [i].GetComponent<Node>());
        }

        var saveTask = ParseObject.SaveAllAsync(curNodes);
        while (!saveTask.IsCompleted)
            yield return null;

        //Save completed
        print("Entered save all");    
        for (int i = 0; i < nodeList.Count; i++)
        {
            foreach (GameObject adjacentNode in nodeComponentList[i].adjacentNodes) //<--- for NodeConnection
            {
                print("Entered its adjacent nodes");
                foreach (NodeParseObject tempnode in curNodes)
                {
                    print("Checking cur node tempnodes");
                    if (adjacentNode.transform.position.x == tempnode.Xpos && adjacentNode.transform.position.y == tempnode.Ypos)
                    {
                        var nodeConnection = new NodeConnection();
                        nodeConnection.StartNode = curNodes [i];
                        nodeConnection.EndNode = tempnode;
                        nodeConnection.PlanId = null;
                        nodeConnection.SaveAsync();
                        break;
                    }
                }
            }
        }       

        foreach (GameObject window in windowList)
        {
            var windowObject = new HouseParseObject();
            windowObject.Name = window.name;
            windowObject.Rotation = window.transform.rotation.z;
            windowObject.Xpos = window.transform.position.x;
            windowObject.Ypos = window.transform.position.y;
            windowObject.Category = window.GetComponent<HouseObject>().category;
            windowObject.PlanId = currentPlan;
            windowObject.Isattached = true;
            windowObject.SaveAsync();
        }

        foreach (GameObject houseObject in objectList)
        {
            var houseParseObject = new HouseParseObject();
            houseParseObject.Name = houseObject.name;
            houseParseObject.Rotation = houseObject.transform.rotation.z;
            houseParseObject.Xpos = houseObject.transform.position.x;
            houseParseObject.Ypos = houseObject.transform.position.y;
            houseParseObject.Category = houseObject.GetComponent<HouseObject>().category;
            houseParseObject.PlanId = currentPlan;
            houseParseObject.Isattached = false;
            houseParseObject.SaveAsync();
        }
    }
       
}