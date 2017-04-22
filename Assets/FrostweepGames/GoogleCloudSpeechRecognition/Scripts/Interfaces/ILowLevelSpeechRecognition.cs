using System;
using UnityEngine;

namespace FrostweepGames.SpeechRecognition.Google.Cloud
{
    public interface ILowLevelSpeechRecognition
    {
        event Action<RecognitionResponse> SpeechRecognizedSuccessEvent;
        event Action<string> SpeechRecognizedFailedEvent;

        void SetLanguage(Utilites.Enumerators.Language language);
        void SetSpeechContext(string[] phrases);
        void Recognize(AudioClip clip);
        void StartRecord();
        void StopRecord();
        void StartRuntimeRecord();
        void StopRuntimeRecord();
    }
}