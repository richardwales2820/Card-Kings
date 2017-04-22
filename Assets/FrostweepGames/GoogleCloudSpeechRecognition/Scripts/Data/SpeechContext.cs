namespace FrostweepGames.SpeechRecognition.Google.Cloud
{
    [System.Serializable]
    public class SpeechContext
    {
        public string[] phrases;

        public SpeechContext()
        {
            phrases = new string[0];
        }
    }
}