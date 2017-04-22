using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class User : MonoBehaviour
{

	public int idNumber;
	public string password;
	public int wins;
	public int losses;
	public double largestPotWin;

	User(int idNumber, string password, int wins, int losses, double largestPotWin)
    {

    }

	// Use this for initialization
	void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    public int getUserID()
    {
        return idNumber;
    }

    public void setUserID(string name)
    {

    }

}
