using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class clickSubmit : MonoBehaviour {

    UILabel label;

	// Use this for initialization
	void Start () {
        label = GameObject.Find("InputBox").GetComponentInChildren<UILabel>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void submit()
    {
        if (label.text.Equals("") || label.text == null)
        {
            PlayerPrefs.SetString("PlanID", "TLrgYc8Wro");
        }
        else
            PlayerPrefs.SetString("PlanID", label.text);
        SceneManager.LoadScene(1);
    }
}
