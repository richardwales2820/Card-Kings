using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioSource source;
    // Use this for initialization
    void Start ()
    {
        source = GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    // Each method plays the relevant sound file when called
    public void PlayLoss()
    {
        source.PlayOneShot(Factory.lossAudio);
    }

	public void PlayWin()
    {
        source.PlayOneShot(Factory.winAudio);
    }

	public void PlayChip()
    {
		Debug.Log ("Play");
        source.PlayOneShot(Factory.chipAudio);
    }

	public void PlayCard()
    {
        source.PlayOneShot(Factory.flipAudio);
    }
}
