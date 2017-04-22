using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FrostweepGames.SpeechRecognition.Google.Cloud;

public class Voice : MonoBehaviour {

	private ILowLevelSpeechRecognition _speechRecognition;
	public bool isRecording;
	private string topResult;
	public bool playersTurn;

	// Use this for initialization
	void Start () {
		playersTurn = false;
		isRecording = false;
		topResult = null;
		_speechRecognition = SpeechRecognitionModule.Instance;
		_speechRecognition.SpeechRecognizedSuccessEvent += SpeechRecognizedSuccessEventHandler;
		_speechRecognition.SpeechRecognizedFailedEvent += SpeechRecognizedFailedEventHandler;
	}

	// Update is called once per frame
	void Update () {
		//Debug.Log ("Update1");
		if (playersTurn == true) 
		{
			if (Input.GetKeyDown (KeyCode.Space) && !isRecording) 
			{
				Debug.Log ("Recording");
				isRecording = true;
				_speechRecognition.StartRecord();
			} 
			else if (Input.GetKeyUp (KeyCode.Space) && isRecording) 
			{
				isRecording = false;
				ApplySpeechContextPhrases();
				_speechRecognition.StopRecord();
			}	
		}
	}

	public string GetVoice()
	{
		return topResult;
	}

	public void ResetVoice()
	{
		topResult = null;
	}

	private void ApplySpeechContextPhrases()
	{
		string[] phrases = new string[10];
		_speechRecognition.SetSpeechContext(phrases);
	}

	private void SpeechRecognizedFailedEventHandler(string obj)
	{
		Debug.Log ("Speech Recognition failed with error: " + obj);
	}

	private void SpeechRecognizedSuccessEventHandler(RecognitionResponse obj)
	{
		if (obj != null && obj.results.Length > 0)
		{
			Debug.Log ("Speech Recognition succeeded! Detected Most useful: " + obj.results[0].alternatives[0].transcript);
			topResult = obj.results [0].alternatives [0].transcript;
			string other = "\nDetected alternative: ";

			foreach (var result in obj.results)
			{
				foreach (var alternative in result.alternatives)
				{
					if (obj.results[0].alternatives[0] != alternative)
						other += alternative.transcript + ", ";
				}
			}
			Debug.Log (other);
		}
		else
		{
			Debug.Log ("Speech Recognition succeeded! Words are no detected.");
		}
	}
}
