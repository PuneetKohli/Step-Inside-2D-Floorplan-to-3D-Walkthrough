using UnityEngine;
using System.Collections.Generic;

public class SofaObject3D : HouseObject3D {

	public List<GameObject> SofaObjects; //List of all Sofas -> Set this in the inspector
	GameObject SofaModel; //The current model of the Sofa, which is child of the main Sofa object
	public int indexOfModel = 0;
	// Use this for initialization
	void Start () {
		canChange = true;
		canInteract = false;
		SofaModel = GameObject.Instantiate (SofaObjects [indexOfModel]);
		SofaModel.transform.parent = transform;
		SofaModel.transform.localScale *= 2f;
		SofaModel.transform.localPosition = new Vector3 (0f, SofaModel.transform.localPosition.y, 0f);
	}

	// Update is called once per frame
	void Update () {

	}

	public override void didChange()
	{
		print ("Did change");

		//Write the logic for giving option of Sofa
		//Get index of current Sofa
		print("Sofa model is " + SofaModel);
		print ("Model index is " + indexOfModel);

		//in cyclic order -> if last Sofa, then go to 0 index

		if (indexOfModel != SofaObjects.Count - 1) {
			indexOfModel++;
		} else {
			indexOfModel = 0;
		}

		//Change the Sofa to the next Sofa
		GameObject newSofaModel = GameObject.Instantiate(SofaObjects[indexOfModel]);
		newSofaModel.transform.parent = transform;
		newSofaModel.transform.localScale *= 2f;
		SofaModel.transform.localPosition = new Vector3 (0f, SofaModel.transform.localPosition.y, 0f);
		GameObject.Destroy (SofaModel);
		SofaModel = newSofaModel;

	}

	public override void didInteract()
	{
		return;
	}

    public override void setModel(string name)
    {}
}
