using UnityEngine;
using System.Collections.Generic;


public class ChairObject3D : HouseObject3D {

    public List<GameObject> ChairObjects; //List of all Chairs -> Set this in the inspector
    GameObject ChairModel; //The current model of the Chair, which is child of the main Chair object
    public int indexOfModel = 0;
    // Use this for initialization
    void Start () {
        canChange = true;
        canInteract = false;
    }

    // Update is called once per frame
    void Update () {

    }

    public override void didChange()
    {
        print ("Did change");

        //Write the logic for giving option of Chair
        //Get index of current Chair
        print("Chair model is " + ChairModel);
        print ("Model index is " + indexOfModel);

        //in cyclic order -> if last Chair, then go to 0 index

        if (indexOfModel != ChairObjects.Count - 1) {
            indexOfModel++;
        } else {
            indexOfModel = 0;
        }

        //Change the Chair to the next Chair
        GameObject newChairModel = GameObject.Instantiate(ChairObjects[indexOfModel]);
        newChairModel.transform.parent = transform;
        newChairModel.transform.localScale *= 2f;
        newChairModel.transform.localPosition = Vector3.zero;
        GameObject.Destroy (ChairModel);
        ChairModel = newChairModel;

    }

    public override void didInteract()
    {
        return;
    }

    public override void setModel(string name)
    { print("Name is " + name);
        Object[] gos = Resources.LoadAll("furniture/3D_Models/chair", typeof(GameObject));
        foreach (GameObject go in gos)
        {
            ChairObjects.Add(go);
        }
        for (int i = 0; i < ChairObjects.Count; i++)
        {
            print(ChairObjects[i].name);
            if(ChairObjects[i].name.Equals(name))
            {
                ChairModel = GameObject.Instantiate (ChairObjects [i]);
                ChairModel.transform.parent = transform;
                ChairModel.layer = gameObject.layer;
                //ChairModel.transform.localScale *= 2f;
                if(name.Equals("red"))
                {
                    ChairModel.transform.localPosition = new Vector3 (-0.01f, -1f, -0.33f);
                    ChairModel.transform.localRotation = Quaternion.Euler(new Vector3(0f, -0.76f, 0f));
                    ChairModel.transform.localScale  = new Vector3(0.7f, 0.7f, 0.7f);

                }
                else if (name.Equals("wooden"))
                {
                    ChairModel.transform.localPosition = new Vector3 (0.1f, -0.89f, 0.09f);
                    ChairModel.transform.localRotation = Quaternion.Euler(new Vector3(0f, -0.2f, 0f));
                    ChairModel.transform.localScale  = new Vector3(1f, 1f, 1f);

                }

                else if (name.Equals("round"))
                {
                    ChairModel.transform.localPosition = new Vector3 (0.09f, -0.95f, -0.22f);
                    ChairModel.transform.localRotation = Quaternion.Euler(new Vector3(0f, -1.17f, 0f));
                    ChairModel.transform.localScale  = new Vector3(0.6f, 0.6f, 0.6f);

                }
                break;
            }
        }
    }
}


