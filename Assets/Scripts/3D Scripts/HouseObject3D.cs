using UnityEngine;
using System.Collections;

public abstract class HouseObject3D : MonoBehaviour {

	private BoxCollider boxCollider;
	private Rigidbody rigidBody;

	public bool canChange { get; set; } //We have multiple model for this type, ex many sofas
	public bool canInteract { get; set; } //For exmple - open a door

	// Use this for initialization
	void Start () {
        canChange = true;
        canInteract = false;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void didSelect()
	{
		print ("Did select");
		if (canChange) {
			didChange ();
		} else if (canInteract) {
			didInteract ();
		}
	}

	public abstract void didChange();

	public abstract void didInteract();

    public abstract void setModel(string name);

}
