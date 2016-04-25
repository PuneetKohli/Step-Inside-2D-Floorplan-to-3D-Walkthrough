using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetMouseButtonDown(0))
		{
			print("Hello");
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			if (Physics.Raycast (ray, out hit)) {
				if (hit.transform.GetComponent<HouseObject3D> () != null) {
					print ("IT is a house object");
					hit.transform.GetComponent<HouseObject3D> ().didSelect ();
				}
			}
		}
	}
}
