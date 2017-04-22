using UnityEngine;
using System.Linq;
using System;
using System.Collections;
using System.Collections.Generic;

namespace FrostweepGames.SpeechRecognition.Google.Cloud
{
    public class SpeechDetection
    {
        public event Action UserTalkingStartedEvent;
        public event Action UserTalkingFinishedEvent;
        public event Action UserTalkingFailedEvent;

        private int _recordLength,
                    _sampleRate;

        private float _treshold = 0.0035f,
                      _currentAverageVolume;

        private float _timeForDisableRecording = 2f,
                      _timerForDisableRecording;

        private bool _isSpeaking = false,
                     _isStarted = false;

        private string _microphoneDevice;

        private AudioClip _micRecordClip;

        private AudioSource _workingSource;

        private float[] _clipSampleData;

        private List<float> _recordedClipAudio;
        

        public bool IsCanWork { get; private set; }


        public SpeechDetection() { }
        public SpeechDetection(AudioSource workingSource, int recordLength = 15, int sampleRate = 16000, float treshold = 0.001f)
        {
            _recordLength = recordLength;
            _sampleRate = sampleRate;
            _treshold = treshold;
            _workingSource = workingSource;

#if UNITY_WEBPLAYER
            SpeechRecognitionModule.Instance.StartCoroutine(MicrophoneAuthorization());
#else
            CheckMicrophones();
#endif
        }

        public void Update()
        {
            if (!_isStarted || SpeechRecognitionModule.Instance.isRecognitionProcessing)
                return;

            _workingSource.GetSpectrumData(_clipSampleData, 0, FFTWindow.Rectangular);
            _currentAverageVolume = _clipSampleData.Average();

            if (_currentAverageVolume > _treshold)
            {
                if (!_isSpeaking)
                {
                    if (UserTalkingStartedEvent != null)
                        UserTalkingStartedEvent();

                    _isSpeaking = true;
                    _timerForDisableRecording = _timeForDisableRecording;
                }
            }
            else if (_isSpeaking)
            {
                _timerForDisableRecording -= Time.deltaTime;

                if (_timerForDisableRecording <= 0)
                {
                    _isSpeaking = false;

                    StopDetection();

                    _micRecordClip = AudioClip.Create("TEST", _recordedClipAudio.Count, 1, 16000, false);
                    _micRecordClip.SetData(_recordedClipAudio.ToArray(), 0);

                    _workingSource.outputAudioMixerGroup = null;
                    _workingSource.clip = _micRecordClip;
                    _workingSource.Play();

                    if (UserTalkingFinishedEvent != null)
                        UserTalkingFinishedEvent();
                }
                else
                {
                    WorkingOnRecordingAudio();
                }
            }
        }

        public void StartDetection()
        {
            if (!IsCanWork)
            {
                if (UserTalkingFailedEvent != null)
                    UserTalkingFailedEvent();
                return;
            }

            if (_micRecordClip != null)
                MonoBehaviour.Destroy(_micRecordClip);

            if (_recordedClipAudio == null)
                _recordedClipAudio = new List<float>();
            else
                _recordedClipAudio.Clear();

            _clipSampleData = new float[1024];


#if !UNITY_WEBGL
            _micRecordClip = Microphone.Start(_microphoneDevice, true, _recordLength, _sampleRate);
#endif
            _workingSource.loop = true;
            _workingSource.clip = _micRecordClip;
            _workingSource.Play();

            _isStarted = true;
        }

        public void StopDetection()
        {
            _workingSource.Stop();
            _workingSource.clip = null;
            _clipSampleData = null;
            _isStarted = false;

#if !UNITY_WEBGL
            Microphone.End(_microphoneDevice);
#endif
        }

        public AudioClip GetRecordedAudio()
        {
            return _micRecordClip;
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
#if !UNITY_WEBGL
            if (Microphone.devices.Length > 0)
            {
                _microphoneDevice = Microphone.devices[0];
                IsCanWork = true;
            }
            else
                IsCanWork = false;
#endif
        }

        private void WorkingOnRecordingAudio()
        {
            float[] data = new float[1024];
            _micRecordClip.GetData(data, 0);

            _recordedClipAudio.AddRange(data);
        }
    }
}