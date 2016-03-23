using UnityEngine;
using System.Collections;

public class wallScaler : MonoBehaviour 
{
    TKPanRecognizer panRecognizer;

	// Use this for initialization
	void Start () {
        //addPanGestureRecognizer();
	}

    void addPanGestureRecognizer()
    {
        panRecognizer = new TKPanRecognizer();
        panRecognizer.gestureRecognizedEvent += (r) =>
        {

            //print("Touch delta is " + r.deltaTranslation.y);
            //print("Touch delta cm is " + r.deltaTranslationCm);
            print("Scale" + transform.localScale.y + 0.1f * r.deltaTranslation.y);
            float yScale = Mathf.Max(transform.localScale.y + 0.1f * r.deltaTranslation.y, 1.0f);
            transform.localScale = new Vector3(transform.localScale.x, yScale, 0);
            //Debug.Log(r);
        };

        panRecognizer.gestureCompleteEvent += r =>
        {
            Debug.Log("pan gesture complete");
        };

        TouchKit.addGestureRecognizer(panRecognizer);
    }

	
	// Update is called once per frame
	void Update () {
	
	}
}
