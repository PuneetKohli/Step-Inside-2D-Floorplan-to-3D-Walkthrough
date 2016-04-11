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
                    DiningtableModel.transform.localPosition = new Vector3 (0.38f, -1f, 0.09f);
                    DiningtableModel.transform.localRotation = Quaternion.Euler(new Vector3(0, 50, 0));
                }
                else if (name.Equals("xyz"))
                {
                    //and so on..
                }
                break;
            }
        }
    }
}


