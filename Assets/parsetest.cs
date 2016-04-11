using UnityEngine;
using System.Collections;
using Parse;

public class parsetest : MonoBehaviour {

	// Use this for initialization
	void Start () {
        ParseObject testObject = new ParseObject("TestObject");
        testObject["foo"] = "bar";
        testObject.SaveAsync();
        print("Hello");
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
