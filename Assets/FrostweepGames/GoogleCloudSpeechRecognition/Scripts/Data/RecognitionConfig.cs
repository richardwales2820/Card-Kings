using UnityEngine;
using System.Collections;
using FrostweepGames.SpeechRecognition.Utilites;

namespace FrostweepGames.SpeechRecognition.Google.Cloud
{
    [System.Serializable]
    public class RecognitionConfig
    {
        public string encoding; //[Required] Encoding of audio data sent in all RecognitionAudio messages. 
        public int sampleRate; //[Required] Sample rate in Hertz of the audio data sent in all RecognitionAudio messages. 
        public string languageCode; //[Optional] The language of the supplied audio as a BCP-47 language tag.
        public int maxAlternatives; //[Optional] Maximum number of recognition hypotheses to be returned. valid 0-30
        public bool profanityFilter; //[Optional] If set to true, the server will attempt to filter out profanities, replacing all but the initial character in each filtered word with asterisks, e.g. "f***". If set to false or omitted, profanities won't be filtered out. 
        public SpeechContext speechContext; //[Optional] A means to provide context to assist the speech recognition. 

        public RecognitionConfig()
        {
            speechContext = new SpeechContext();
        }
    }
}