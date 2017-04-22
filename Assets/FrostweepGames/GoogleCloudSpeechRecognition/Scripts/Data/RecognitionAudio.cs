namespace FrostweepGames.SpeechRecognition.Google.Cloud
{
    [System.Serializable]
    public class RecognitionAudio
    {
        public string content;
     //   public string uri;

        public RecognitionAudio()
        {
            content = string.Empty;
          //  uri = string.Empty;
        }
    }
}