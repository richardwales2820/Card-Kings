using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class move : MonoBehaviour {
    public Animator anim;
    private float H;
    private float V;
    public Rigidbody rbody;
	// Use this for initialization
	void Start () {
        anim = GetComponent<Animator>();
        rbody = GetComponent<Rigidbody>();

        
	}
	
	// Update is called once per frame
	void Update () {
        rbody.velocity = new Vector3(0f , 0f, 1f);
    }
}
