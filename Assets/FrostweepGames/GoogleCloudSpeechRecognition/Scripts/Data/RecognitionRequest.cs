namespace FrostweepGames.SpeechRecognition.Google.Cloud
{
    [System.Serializable]
    public class RecognitionRequest
    {
        public RecognitionConfig config;
        public RecognitionAudio audio;

        public RecognitionRequest()
        {
            config = new RecognitionConfig();
            audio = new RecognitionAudio();
        }
    }
}