using UnityEngine;
using System.Collections;

public class VRClickScript : MonoBehaviour {

    bool isMoving = false;
    public GameObject head;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if (Cardboard.SDK.Triggered) {     
            //something here;   
            toggleMovement();
        }
        if (isMoving)
        {
            Vector3 newTransform = head.transform.forward;
            newTransform.y = 0;
            transform.Translate(newTransform * 0.07f);
        }
	}

    public void clickedInVR()
    {
        print("Clicked in vr");
        toggleMovement();
    }

    void toggleMovement()
    {
        isMoving = !isMoving;
    }
     
}
