using FrostweepGames.SpeechRecognition.Utilites;
using UnityEngine;
using UnityEngine.UI;

namespace FrostweepGames.SpeechRecognition.Google.Cloud.Examples
{
    public class Example : MonoBehaviour
    {
        private ILowLevelSpeechRecognition _speechRecognition;

        private Button _startRecordButton,
                       _stopRecordButton,
                       _startRuntimeDetection,
                       _stopRuntimeDetection;

        private Image _speechRecognitionState;

        private Text _speechRecognitionResult;

        private Toggle _isRuntimeDetectionToggle;

        private Dropdown _languageDropdown;

        private InputField _contextPhrases;

        private void Start()
        {
            _speechRecognition = SpeechRecognitionModule.Instance;
            _speechRecognition.SpeechRecognizedSuccessEvent += SpeechRecognizedSuccessEventHandler;
            _speechRecognition.SpeechRecognizedFailedEvent += SpeechRecognizedFailedEventHandler;

            _startRecordButton = transform.Find("Canvas/Button_StartRecord").GetComponent<Button>();
            _stopRecordButton = transform.Find("Canvas/Button_StopRecord").GetComponent<Button>();
            _startRuntimeDetection = transform.Find("Canvas/Button_StartRuntimeDetection").GetComponent<Button>();
            _stopRuntimeDetection = transform.Find("Canvas/Button_StopRuntimeDetection").GetComponent<Button>();

            _speechRecognitionState = transform.Find("Canvas/Image_RecordState").GetComponent<Image>();

            _speechRecognitionResult = transform.Find("Canvas/Text_Result").GetComponent<Text>();

            _isRuntimeDetectionToggle = transform.Find("Canvas/Toggle_IsRuntime").GetComponent<Toggle>();

            _languageDropdown = transform.Find("Canvas/Dropdown_Language").GetComponent<Dropdown>();

            _contextPhrases = transform.Find("Canvas/InputField_SpeechContext").GetComponent<InputField>();

            _startRecordButton.onClick.AddListener(StartRecordButtonOnClickHandler);
            _stopRecordButton.onClick.AddListener(StopRecordButtonOnClickHandler);
            _startRuntimeDetection.onClick.AddListener(StartRuntimeDetectionButtonOnClickHandler);
            _stopRuntimeDetection.onClick.AddListener(StopRuntimeDetectionButtonOnClickHandler);
            _isRuntimeDetectionToggle.onValueChanged.AddListener(IsRuntimeDetectionOnValueChangedHandler);

            _speechRecognitionState.color = Color.white;
            _startRecordButton.interactable = true;
            _stopRecordButton.interactable = false;
            _startRuntimeDetection.interactable = true;
            _stopRuntimeDetection.interactable = false;

            _languageDropdown.ClearOptions();

            for (int i = 0; i < 43; i++)
            {
                _languageDropdown.options.Add(new Dropdown.OptionData(((Enumerators.Language)i).ToString()));
            }

            _languageDropdown.onValueChanged.AddListener(LanguageDropdownOnValueChanged);

            _languageDropdown.value = 1;

        }

        private void OnDestroy()
        {
            _speechRecognition.SpeechRecognizedSuccessEvent -= SpeechRecognizedSuccessEventHandler;
            _speechRecognition.SpeechRecognizedFailedEvent -= SpeechRecognizedFailedEventHandler;
        }

        private void StartRuntimeDetectionButtonOnClickHandler()
        {
            ApplySpeechContextPhrases();

            _startRuntimeDetection.interactable = false;
            _stopRuntimeDetection.interactable = true;
            _speechRecognitionState.color = Color.green;
            _speechRecognitionResult.text = "";
            _speechRecognition.StartRuntimeRecord();
        }

        private void StopRuntimeDetectionButtonOnClickHandler()
        {
            _stopRuntimeDetection.interactable = false;
            _startRuntimeDetection.interactable = true;
            _speechRecognitionState.color = Color.green;
            _speechRecognition.StopRuntimeRecord();
            _speechRecognitionResult.text = "";
        }

        private void StartRecordButtonOnClickHandler()
        {
            _startRecordButton.interactable = false;
            _stopRecordButton.interactable = true;
            _speechRecognitionState.color = Color.red;
            _speechRecognitionResult.text = "";
            _speechRecognition.StartRecord();
        }

        private void StopRecordButtonOnClickHandler()
        {
            ApplySpeechContextPhrases();

            _stopRecordButton.interactable = false;
            _speechRecognitionState.color = Color.yellow;
            _speechRecognition.StopRecord();
        }

        private void LanguageDropdownOnValueChanged(int value)
        {
            _speechRecognition.SetLanguage((Enumerators.Language)value);
        }

        private void IsRuntimeDetectionOnValueChangedHandler(bool value)
        {
            StopRuntimeDetectionButtonOnClickHandler();

            (_speechRecognition as SpeechRecognitionModule).isRuntimeDetection = value;
        }

        private void ApplySpeechContextPhrases()
        {
            string[] phrases = _contextPhrases.text.Trim().Split(","[0]);

            if (phrases.Length > 0)
                _speechRecognition.SetSpeechContext(phrases);
        }

        private void SpeechRecognizedFailedEventHandler(string obj)
        {
            _speechRecognitionState.color = Color.green;
            _speechRecognitionResult.text = "Speech Recognition failed with error: " + obj;

            _startRecordButton.interactable = true;
            _stopRecordButton.interactable = false;
        }

        private void SpeechRecognizedSuccessEventHandler(RecognitionResponse obj)
        {
            _startRecordButton.interactable = true;

            _speechRecognitionState.color = Color.green;

            if (obj != null && obj.results.Length > 0)
            {
                _speechRecognitionResult.text = "Speech Recognition succeeded! Detected Most useful: " + obj.results[0].alternatives[0].transcript;

                string other = "\nDetected alternative: ";

                foreach (var result in obj.results)
                {
                    foreach (var alternative in result.alternatives)
                    {
                        if (obj.results[0].alternatives[0] != alternative)
                            other += alternative.transcript + ", ";
                    }
                }

                _speechRecognitionResult.text += other;
            }
            else
            {
                _speechRecognitionResult.text = "Speech Recognition succeeded! Words are no detected.";

            }
        }
    }
}