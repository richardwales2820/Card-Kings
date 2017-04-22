namespace FrostweepGames.SpeechRecognition.Google.Cloud
{
    [System.Serializable]
    public class RecognitionResponse
    {
        public SpeechRecognitionResult[] results;

        public RecognitionResponse() { }
    }

    [System.Serializable]
    public class SpeechRecognitionAlternative
    {
        public string transcript;
        public double confidence;

        public SpeechRecognitionAlternative() { }
    }

    [System.Serializable]
    public class SpeechRecognitionResult
    {
        public SpeechRecognitionAlternative[] alternatives;

        public SpeechRecognitionResult() { }
    }
}
