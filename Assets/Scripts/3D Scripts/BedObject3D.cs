using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BedObject3D : HouseObject3D {

    public List<GameObject> bedObjects; //List of all beds -> Set this in the inspector
    GameObject bedModel; //The current model of the bed, which is child of the main bed object
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

        //Write the logic for giving option of bed
        //Get index of current bed
        print("Bed model is " + bedModel);
        print ("Model index is " + indexOfModel);

        //in cyclic order -> if last bed, then go to 0 index

        if (indexOfModel != bedObjects.Count - 1) {
            indexOfModel++;
        } else {
            indexOfModel = 0;
        }

        //Change the bed to the next bed
        GameObject newBedModel = GameObject.Instantiate(bedObjects[indexOfModel]);
        newBedModel.transform.parent = transform;
        newBedModel.transform.localScale *= 2f;
        newBedModel.transform.localPosition = Vector3.zero;
        GameObject.Destroy (bedModel);
        bedModel = newBedModel;

    }

    public override void didInteract()
    {
        return;
    }

    public override void setModel(string name)
    {
        print("Name is " + name);
        Object[] gos = Resources.LoadAll("furniture/3D_Models/bed", typeof(GameObject));
        foreach (GameObject go in gos)
        {
            bedObjects.Add(go);
        }
        for (int i = 0; i < bedObjects.Count; i++)
        {
            print(bedObjects[i].name);
            if(bedObjects[i].name.Equals(name))
            {
                bedModel = GameObject.Instantiate (bedObjects [i]);
                bedModel.transform.parent = transform;
                bedModel.layer = gameObject.layer;
                //DiningtableModel.transform.localScale *= 2f;
                if(name.Equals("simple"))
                {
                    bedModel.transform.localPosition = new Vector3 (-0.5f, -0.53f, 0.09f);
                    bedModel.transform.localRotation = Quaternion.Euler(new Vector3(0f, 80.94f, 0f));
                    bedModel.transform.localScale  = new Vector3(0.4f, 0.4f, 0.4f);
                }
                else if (name.Equals("lowered"))
                {
                    bedModel.transform.localPosition = new Vector3 (-0.14f, -0.61f, 0.09f);
                    bedModel.transform.localRotation = Quaternion.Euler(new Vector3(0f, 80.94f, 0f));
                    bedModel.transform.localScale  = new Vector3(1.2f, 1.2f, 1.2f);
                }
                else if (name.Equals("side-table"))
                {
                    bedModel.transform.localPosition = new Vector3 (0.22f, -0.61f, 0.57f);
                    bedModel.transform.localRotation = Quaternion.Euler(new Vector3(0f, 90.18f, 0f));
                    bedModel.transform.localScale  = new Vector3(0.5f, 0.5f, 0.5f);

                }
                else if (name.Equals("double-table"))
                {
                    bedModel.transform.localPosition = new Vector3 (0.01f, -0.48f, 0.06f);
                    bedModel.transform.localRotation = Quaternion.Euler(new Vector3(0f, 89.94f, 0f));
                    bedModel.transform.localScale  = new Vector3(0.3f, 0.3f, 0.3f);
                }
                break;
            }
        }
    }
}
