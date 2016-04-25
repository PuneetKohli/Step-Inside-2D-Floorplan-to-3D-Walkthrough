using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class DiningtableObject3D : HouseObject3D {

    List<GameObject> DiningtableObjects = new List<GameObject>(); //List of all Diningtables -> Set this in the inspector
    GameObject DiningtableModel; //The current model of the Diningtable, which is child of the main Diningtable object
    public int indexOfModel = 0;
    // Use this for initialization
    void Start () {
        canChange = true;
        canInteract = false;

        print(DiningtableObjects.Count);
    }

    // Update is called once per frame
    void Update () {

    }

    public override void didChange()
    {
        //Write the logic for giving option of Diningtable
        //Get index of current Diningtable
        print("Diningtable model is " + DiningtableModel);
        print ("Model index is " + indexOfModel);

        //in cyclic order -> if last Diningtable, then go to 0 index

        if (indexOfModel != DiningtableObjects.Count - 1) {
            indexOfModel++;
        } else {
            indexOfModel = 0;
        }

        //Change the Diningtable to the next Diningtable
        GameObject newDiningtableModel = GameObject.Instantiate(DiningtableObjects[indexOfModel]);
        newDiningtableModel.transform.parent = transform;
        newDiningtableModel.transform.localScale *= 2f;
        newDiningtableModel.transform.localPosition = new Vector3 (0.38f, -1f, 0.09f);
        GameObject.Destroy (DiningtableModel);
        DiningtableModel = newDiningtableModel;
        string name = DiningtableModel.name;
            if(name.Equals("simple"))
            {
                DiningtableModel.transform.localPosition = new Vector3 (-0.01f, -1.29f, -0.33f);
                DiningtableModel.transform.localRotation = Quaternion.Euler(new Vector3(0f, 31.18f, 0f));
                DiningtableModel.transform.localScale  = new Vector3(0.55f, 0.55f, 0.55f);

            }
            else if (name.Equals("square"))
            {
                DiningtableModel.transform.localPosition = new Vector3 (0.1f, -1.26f, 0.09f);
                DiningtableModel.transform.localRotation = Quaternion.Euler(new Vector3(0f, -11.37f, 0f));
                DiningtableModel.transform.localScale  = new Vector3(0.35f, 0.35f, 0.35f);

            }
            else if (name.Equals("long"))
            {
                DiningtableModel.transform.localPosition = new Vector3 (0.03f, -1f, 0.09f);
                DiningtableModel.transform.localRotation = Quaternion.Euler(new Vector3(0f, -8.33f, 0f));
                DiningtableModel.transform.localScale  = new Vector3(0.45f, 0.45f, 0.45f);

            }
            else if (name.Equals("side"))
            {
                DiningtableModel.transform.localPosition = new Vector3 (1.52f, -1.49f, -1.23f);
                DiningtableModel.transform.localRotation = Quaternion.Euler(new Vector3(0f, 19.31f, 0f));
                DiningtableModel.transform.localScale  = new Vector3(1.2f, 1.2f, 1.2f);

            }
            else if (name.Equals("oval"))
            {
                DiningtableModel.transform.localPosition = new Vector3 (0.38f, -1f, 0.67f);
                DiningtableModel.transform.localRotation = Quaternion.Euler(new Vector3(0f, 51.35f, 0f));
                DiningtableModel.transform.localScale  = new Vector3(1.2f, 1.2f, 1.2f);

            }
            else if (name.Equals("stylish"))
            {
                DiningtableModel.transform.localPosition = new Vector3 (1.38f, -0.96f, 3f);
                DiningtableModel.transform.localRotation = Quaternion.Euler(new Vector3(0f, 53.65f, 0f));
                DiningtableModel.transform.localScale  = new Vector3(1f, 1f, 1f);

            }
            else if (name.Equals("metallic"))
            {
                DiningtableModel.transform.localPosition = new Vector3 (-0.01f, -2.5f, 0.2f);
                DiningtableModel.transform.localRotation = Quaternion.Euler(new Vector3(0f, -0.90f, 0f));
                DiningtableModel.transform.localScale  = new Vector3(1.4f, 1.4f, 1.4f);

            }
            else if (name.Equals("coffeetop"))
            {
                DiningtableModel.transform.localPosition = new Vector3 (-1.41f, -1f, -1.7f);
                DiningtableModel.transform.localRotation = Quaternion.Euler(new Vector3(0f, 48.64f, 0f));
                DiningtableModel.transform.localScale  = new Vector3(0.9f, 0.9f, 0.9f);

            }

    }

    public override void didInteract()
    {
        return;
    }

    public override void setModel(string name)
    {
        print("Name is " + name);
        Object[] gos = Resources.LoadAll("furniture/3D_Models/table", typeof(GameObject));
        foreach (GameObject go in gos)
        {
            DiningtableObjects.Add(go);
        }
        for (int i = 0; i < DiningtableObjects.Count; i++)
        {
            print(DiningtableObjects[i].name);
            if(DiningtableObjects[i].name.Equals(name))
            {
                DiningtableModel = GameObject.Instantiate (DiningtableObjects [i]);
                DiningtableModel.transform.parent = transform;
                DiningtableModel.layer = gameObject.layer;
                //DiningtableModel.transform.localScale *= 2f;
                if(name.Equals("simple"))
                {
                    DiningtableModel.transform.localPosition = new Vector3 (-0.01f, -1.29f, -0.33f);
                    DiningtableModel.transform.localRotation = Quaternion.Euler(new Vector3(0f, 31.18f, 0f));
                    DiningtableModel.transform.localScale  = new Vector3(0.55f, 0.55f, 0.55f);

                }
                else if (name.Equals("square"))
                {
                    DiningtableModel.transform.localPosition = new Vector3 (0.1f, -1.26f, 0.09f);
                    DiningtableModel.transform.localRotation = Quaternion.Euler(new Vector3(0f, -11.37f, 0f));
                    DiningtableModel.transform.localScale  = new Vector3(0.35f, 0.35f, 0.35f);

                }
                else if (name.Equals("long"))
                {
                    DiningtableModel.transform.localPosition = new Vector3 (0.03f, -1f, 0.09f);
                    DiningtableModel.transform.localRotation = Quaternion.Euler(new Vector3(0f, -8.33f, 0f));
                    DiningtableModel.transform.localScale  = new Vector3(0.45f, 0.45f, 0.45f);

                }
                else if (name.Equals("side"))
                {
                    DiningtableModel.transform.localPosition = new Vector3 (1.52f, -1.49f, -1.23f);
                    DiningtableModel.transform.localRotation = Quaternion.Euler(new Vector3(0f, 19.31f, 0f));
                    DiningtableModel.transform.localScale  = new Vector3(1.2f, 1.2f, 1.2f);

                }
                else if (name.Equals("oval"))
                {
                    DiningtableModel.transform.localPosition = new Vector3 (0.38f, -1f, 0.67f);
                    DiningtableModel.transform.localRotation = Quaternion.Euler(new Vector3(0f, 51.35f, 0f));
                    DiningtableModel.transform.localScale  = new Vector3(1.2f, 1.2f, 1.2f);

                }
                else if (name.Equals("stylish"))
                {
                    DiningtableModel.transform.localPosition = new Vector3 (1.38f, -0.96f, 3f);
                    DiningtableModel.transform.localRotation = Quaternion.Euler(new Vector3(0f, 53.65f, 0f));
                    DiningtableModel.transform.localScale  = new Vector3(1f, 1f, 1f);

                }
                else if (name.Equals("metallic"))
                {
                    DiningtableModel.transform.localPosition = new Vector3 (-0.01f, -2.5f, 0.2f);
                    DiningtableModel.transform.localRotation = Quaternion.Euler(new Vector3(0f, -0.90f, 0f));
                    DiningtableModel.transform.localScale  = new Vector3(1.4f, 1.4f, 1.4f);

                }
                else if (name.Equals("coffeetop"))
                {
                    DiningtableModel.transform.localPosition = new Vector3 (-1.41f, -1f, -1.7f);
                    DiningtableModel.transform.localRotation = Quaternion.Euler(new Vector3(0f, 48.64f, 0f));
                    DiningtableModel.transform.localScale  = new Vector3(0.9f, 0.9f, 0.9f);

                }
                break;
            }
        }
    }
}


