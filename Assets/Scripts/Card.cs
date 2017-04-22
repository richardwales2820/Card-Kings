using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Card : MonoBehaviour, IComparable
{

    // Card will refer to a graphic representing its value
    // as well as a string for its name
	public int suit;
    public int value;

    public Card(int s, int v)
    {
		suit = s;
        value = v;
    }

	// Use this for initialization
	void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    // Compare card objects together
	public int CompareTo(object ob)
	{
		Card c = (Card)ob;

		return this.value - c.value;
	}

    public override string ToString()
    {
        return "Suit: " + suit + " Value: " + value;
    }
}
