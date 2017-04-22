using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cj : MonoBehaviour {

    public Animator ani;
	// Use this for initialization
	void Start () {
        ani = GetComponent<Animator>();
	}
	
    void playcardspin()
    {
        ani.Play("card spin fall");
    }
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown("1"))
        {
            ani.Play("card spin fall");
        }
	}
}
