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
		ChairModel = GameObject.Instantiate (ChairObjects [indexOfModel]);
		ChairModel.transform.parent = transform;
		ChairModel.transform.localScale *= 2f;
		ChairModel.transform.localPosition = Vector3.zero;
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
    {}

}
