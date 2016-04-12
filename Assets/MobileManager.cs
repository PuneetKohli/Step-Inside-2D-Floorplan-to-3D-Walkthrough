using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Parse;

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
    GameObject player, isoCam;
    GameObject mainMenuScrollView, submenu;
    WallGenerator wallGenerator;
    bool didLoadAll = false;

    List<NodeConnection> connectionList = new List<NodeConnection>();
    List<HouseParseObject> houseObjectList = new List<HouseParseObject>(); 
    List<HouseParseObject> windowList = new List<HouseParseObject>(); 
    List<NodeParseObject> nodeList = new List<NodeParseObject>();

    // Use this for initialization
    void Awake () {

        didLoadAll = false;

        _3DRoot = GameObject.Find("3D Root");
        wallGenerator = _3DRoot.GetComponent<WallGenerator>();
        isoCam = _3DRoot.transform.Find("Isocam").gameObject as GameObject;
        player = _3DRoot.transform.Find("Player").gameObject as GameObject;

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
        retrieve3DFromParse("xyz");
    }

    bool didGenerate = false;
    public void Update()
    {
        if (didLoadAll && !didGenerate)
        {
            didGenerate = true;
            wallGenerator.generate3DFromParse(connectionList, houseObjectList, windowList);
        }    
    }

    public void clicked3DView()
    {
        List<GameObject> nodeList = wallManager.exportNodes();
        List<GameObject> windowList = wallManager.exportWindows();
        List<GameObject> objectList = wallManager.exportObjects();
        StartCoroutine(saveToParse(nodeList, windowList, objectList,  null));
        print("3d view!");
        _3DRoot.SetActive(true);
        print("Wall generator is" + wallGenerator + " Wall manager is " + wallManager);
        wallGenerator.generate3D(nodeList, windowList, objectList);
        EnableIsoCam();
    }

    public void ClickedWalkthrough()
    {
        EnablePlayerCam();
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

    void retrieve3DFromParse(string planID)  //Input is objectID
    {
        Plan currentPlan = ParseObject.CreateWithoutData<Plan>(planID);

        var query = new ParseQuery<NodeParseObject>();
        //query = query.WhereDoesNotExist("plan_id");
        query.FindAsync().ContinueWith(t =>
        {
            IEnumerable<NodeParseObject> nodes = t.Result;
            saveNodes(nodes);
            var connectionQuery = new ParseQuery<NodeConnection>();
            //connectionQuery = connectionQuery.WhereDoesNotExist("plan_id");
            connectionQuery = connectionQuery.Include("start_node");
            connectionQuery = connectionQuery.Include("end_node");
            connectionQuery.FindAsync().ContinueWith(t2 =>
            {
                IEnumerable<NodeConnection> nodeConnections = t2.Result;
                saveConnections(nodeConnections);

                var houseobjectQuery = new ParseQuery<HouseParseObject>();
                //connectionQuery = connectionQuery.WhereDoesNotExist("plan_id");
                houseobjectQuery.FindAsync().ContinueWith(t3 =>
                {
                    IEnumerable<HouseParseObject> houseObjects = t3.Result;
                    saveHouseObjects(houseObjects);
                });

            });
        });
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
                windowList.Add(houseObject);
            } else
            { 
                print("Added house object to local " + houseObject.Xpos);
                houseObjectList.Add(houseObject);
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