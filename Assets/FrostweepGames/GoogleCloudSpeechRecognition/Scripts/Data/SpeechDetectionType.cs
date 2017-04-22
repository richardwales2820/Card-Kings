using System;
using FrostweepGames.SpeechRecognition.Utilites;
using UnityEngine;

[Serializable]
public class SpeechDetectionTypeSettings
{
    public Enumerators.SpeechDetectionVersion speechDetectionVersion;

    public float treshold;

    public int recordLength,
               frequency,
               audioChannels;

    public AudioSource workingSource;

    public bool isAvailable;
}
