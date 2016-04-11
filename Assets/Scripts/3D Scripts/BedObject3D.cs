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
                    bedModel.transform.localPosition = new Vector3 (0.38f, -1f, 0.09f);
                    bedModel.transform.localRotation = Quaternion.Euler(new Vector3(0, 50, 0));
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
