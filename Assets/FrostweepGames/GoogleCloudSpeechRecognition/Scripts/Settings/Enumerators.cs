namespace FrostweepGames.SpeechRecognition.Utilites
{
    public class Enumerators
    {
        public enum MicrophoneStateType : int
        {
            START_RECORD,
            STOP_RECORD,
            MIC_AUTH
        }

        public enum SpeechDetectionVersion
        {
            VERSION_1,
            VERSION_2
        }

        public enum ReleaseType
        {
            STABLE,
            BETA,
            ALPHA
        }

        public enum AudioEncoding
        {
            ENCODING_UNSPECIFIED,
            LINEAR16,
            FLAC,
            MULAW,
            AMR,
            AMR_WB
        }

        public enum Model
        {
            QUERIES,
            MAPS,
            DATES,
            NAMES,
            NUMBERS,
            MUSIC,
            BUYING
        }

        public enum Language
        {
            RU_RU,
            EN_GB,
            DE,
            FR,
            UK,
            EN_US,
            AF,
            AR_SA,
            EU,
            BE,
            BG,
            HR,
            ZH_TW,
            CS,
            DA,
            HL,
            EN,
            ET,
            FO,
            FI,
            EL,
            HE,
            HU,
            IS,
            ID,
            IT,
            JA,
            KO,
            LV,
            LT,
            NO,
            PL,
            PT,
            RO,
            SK,
            SL,
            ES,
            SV,
            TH,
            TR,
            VI,
            ZH_CHS,
            ZH_CHT
        }

    }
}