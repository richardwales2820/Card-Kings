using UnityEngine;
using System;
using System.Collections;
#if UNITY_WEBGL
using FrostweepGames.UniversalMicrophoneLibrary;
#endif

namespace FrostweepGames.SpeechRecognition.Google.Cloud
{
    public class MicrophoneWorker
    {
        public event Action StartedRecordEvent;
        public event Action FinishedRecordEvent;
        public event Action RecordFailedEvent;

        private AudioClip _audioClip;
        private string _microphoneDevice;
        private bool _isLoop;
        private int _recordLength;
        private int _sampleRate;

#if UNITY_WEBGL
        public bool IsCanWork
        {
            get
            {
                return MicrophoneWebGL.IsAvailable();
            }
        }
#else
        public bool IsCanWork { get; private set; }
#endif

        public bool IsRecording { get; private set; }

#region ctors
        public MicrophoneWorker(int recordLength, bool isLoop, int sampleRate)
        {
            _recordLength = recordLength;
            _isLoop = isLoop;
            _sampleRate = sampleRate;

#if UNITY_WEBPLAYER
            SpeechRecognitionModule.Instance.StartCoroutine(MicrophoneAuthorization());
#else
            CheckMicrophones();
#endif
        }
#endregion

        public void StartRecord()
        {
            if (!IsCanWork)
            {
                if (RecordFailedEvent != null)
                    RecordFailedEvent();

                return;
            }

            if (_audioClip != null)
                MonoBehaviour.Destroy(_audioClip);

#if UNITY_WEBGL
            MicrophoneWebGL.Start();
#else
            _audioClip = Microphone.Start(_microphoneDevice, _isLoop, _recordLength, _sampleRate);
#endif
            IsRecording = true;

            if (StartedRecordEvent != null)
                StartedRecordEvent();
        }

        public void StopRecord()
        {
            if (!IsRecording)
                return;

#if UNITY_WEBGL
            MicrophoneWebGL.End();
#else
            Microphone.End(_microphoneDevice);
#endif

            IsRecording = false;

            if (FinishedRecordEvent != null)
                FinishedRecordEvent();
        }

        public AudioClip GetRecordedAudio()
        {
#if UNITY_WEBGL
           return MicrophoneWebGL.GetAudioClip();
#else
            return _audioClip;
#endif
        }

        private IEnumerator MicrophoneAuthorization()
        {
            var auth = Application.RequestUserAuthorization(UserAuthorization.Microphone);
            while (!auth.isDone)
            {
                yield return auth;
            }

            CheckMicrophones();
        }

        private void CheckMicrophones()
        {
#if UNITY_WEBGL
            MicrophoneWebGL.Init();
#else
            if (Microphone.devices.Length > 0)
            {
                _microphoneDevice = Microphone.devices[0];
                IsCanWork = true;
            }
            else
            {
                Debug.Log("Microphone devices not found!");
                IsCanWork = false;
            }
#endif
        }
    }
}