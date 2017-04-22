using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadMenu : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void LoadMenuScene()
	{
		string user = GameObject.Find ("UserField").GetComponent<InputField>().text;
		string pass = GameObject.Find ("PasswordField").GetComponent<InputField> ().text;

		Debug.Log (user);
		Debug.Log (pass);
		if (user.Equals ("SafaBestTA") && pass.Equals ("stache420")) {
			Debug.Log ("Log in success");
			Text infoText = GameObject.Find("Bad Login").GetComponent<Text>();
			infoText.text = "Login successful";
			SceneManager.LoadScene ("Casino");	
		} else {
			// Show to the user that it is wrong
			Text infoText = GameObject.Find("Bad Login").GetComponent<Text>();
			infoText.text = "Incorrect credentials, try again";
		}
	}
}
