using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace FrostweepGames.SpeechRecognition.Google.Cloud
{
    public class RuntimeSpeechDetection
    {
        public event Action StartedRecordEvent;
        public event Action FinishedRecordEvent;
        public event Action RecordFailedEvent;

        private int _recordLength,
                    _sampleRate,
                    _positionOfSamplesToPlay = 0,
                    _positionOfLastSample = 0,
                    _loopWithoutRecording = 0,
                    _maxLoopWithoutRecord = 15;

        private float _initialTimeToListen = 0.5f,
                      _timeToListenAgain = 3f,
                      _noiseLevel = 0.005f,
                      _timeToStartRecording,
                      _endTimeToTalking;

        private string _microphoneDevice;

        private AudioClip _micRecordClip,
                          _createdClip;

        private bool _isRecord = false,
                     _isPlaying = false,
                     _isReadyToRecord = true,
                     _isStarted = false;


        private List<float> _workingData = new List<float>();
        private float[] _checkingSamples;

        public bool IsCanWork { get; private set; }

        #region ctors
        public RuntimeSpeechDetection() { }

        public RuntimeSpeechDetection(int recordLength = 15, int sampleRate = 16000, float treshold = 0.005f)
        {
            _recordLength = recordLength;
            _sampleRate = sampleRate;
            _noiseLevel = treshold;

#if UNITY_WEBPLAYER
            SpeechRecognitionModule.Instance.StartCoroutine(MicrophoneAuthorization());
#else
            CheckMicrophones();
#endif
        }
        #endregion ctors

        public void Update()
        {
            if (!_isStarted || SpeechRecognitionModule.Instance.isRecognitionProcessing)
                return;

#if !UNITY_WEBGL
            _positionOfLastSample = Microphone.GetPosition(null);
            _micRecordClip.GetData(_checkingSamples, 0);

            Listen();

            if (_isRecord) RecordSound();

            if (_isPlaying)
            {
                _timeToStartRecording = Time.time + _timeToListenAgain;
                _isPlaying = false;
            }

            if (Time.time > _timeToStartRecording)
            {
                _isReadyToRecord = true;
            }
#endif
        }

        public void StartRuntimeDetection()
        {
            if (!IsCanWork)
            {
                if (RecordFailedEvent != null)
                    RecordFailedEvent();

                return;
            }

            if (_createdClip != null)
                MonoBehaviour.Destroy(_createdClip);
#if !UNITY_WEBGL
            _micRecordClip = Microphone.Start(_microphoneDevice, true, 1, _sampleRate);
#endif

            _checkingSamples = new float[_micRecordClip.samples * _micRecordClip.channels];

            _isStarted = true;
            _isRecord = false;
            _isPlaying = false;
            _isReadyToRecord = false;
            _loopWithoutRecording = 0;
            _positionOfSamplesToPlay = 0;
            _positionOfLastSample = 0;
            _endTimeToTalking = 0f;

            _timeToStartRecording = Time.time + _initialTimeToListen;
        }

        public void StopRuntimeDetection()
        {
            _checkingSamples = null;
            _workingData = null;
            _isStarted = false;

#if !UNITY_WEBGL
            Microphone.End(_microphoneDevice);
#endif

            if (_micRecordClip != null)
                MonoBehaviour.Destroy(_micRecordClip);
        }

        public AudioClip GetRecordedAudio()
        {
            return _createdClip;
        }

        private int CheckingSpeech()
        {
            int value, length = _checkingSamples.Length;

            if (Time.time >= _endTimeToTalking && _endTimeToTalking != 0)
                return -1;

            if (_positionOfSamplesToPlay > _positionOfLastSample)
            {
                value = LoopCheckingSpeech(_positionOfSamplesToPlay, length);

                if (value > 0)
                    return value;
            }

            value = LoopCheckingSpeech(_positionOfSamplesToPlay, _positionOfLastSample);

            return value;
        }

        private int LoopCheckingSpeech(int point, int maxPoint)
        {
            float value;
            int count = 0;

            for (int i = point; i < maxPoint; i++)
            {
                value = Mathf.Abs(_checkingSamples[i]);

                if (value >= _noiseLevel)
                {
                    count++;
                    if (count > 5)
                        return i;
                }
                else count = 0;
            }

            return -1;
        }

        private void Listen()
        {
            if (_isReadyToRecord)
            {
                int positionOfSample = CheckingSpeech();

                if (!_isRecord)
                {
                    if (positionOfSample >= 0)
                    {
                        _endTimeToTalking = Time.time + _recordLength;
                        _workingData = new List<float>();

                        _positionOfSamplesToPlay = positionOfSample;

                        //if (_positionOfSamplesToPlay - _sampleRate >= 0)
                        //{
                        //    _positionOfSamplesToPlay -= _sampleRate;
                        //}
                        //else
                        //{
                        //    _positionOfSamplesToPlay = _checkingSamples.Length - Mathf.Abs(_positionOfSamplesToPlay - _sampleRate);
                        //}

                        _isRecord = true;
                        if (StartedRecordEvent != null)
                            StartedRecordEvent();
                        return;
                    }
                }
                else if (positionOfSample < 0)
                {
                    if (!_isRecord)
                    {
                        _positionOfSamplesToPlay = _positionOfLastSample;
                    }
                    else
                    {
                        _loopWithoutRecording++;

                        if (_loopWithoutRecording >= _maxLoopWithoutRecord)
                        {
                            _endTimeToTalking = 0;
                            _isRecord = false;
                            _isPlaying = true;
                            _isReadyToRecord = false;
                            _loopWithoutRecording = 0;

                            RecordSound();

                            if (_createdClip != null)
                               MonoBehaviour.Destroy(_createdClip);

                            _createdClip = AudioClip.Create("Speech", _workingData.Count, 1, _sampleRate, false);
                            _createdClip.SetData(_workingData.ToArray(), 0);

                            if (FinishedRecordEvent != null)
                                FinishedRecordEvent();
                        }
                    }
                }
                else _loopWithoutRecording = 0;
            }
        }

        private void RecordSound()
        {
            int length = _checkingSamples.Length;

            if (_positionOfSamplesToPlay > _positionOfLastSample)
            {
                for (int i = _positionOfSamplesToPlay; i < length; i++)
                {
                    _workingData.Add(_checkingSamples[i]);
                }

                _positionOfSamplesToPlay = 0;
            }

            for (int i = _positionOfSamplesToPlay; i < _positionOfLastSample; i++)
            {
                _workingData.Add(_checkingSamples[i]);
            }

            _positionOfSamplesToPlay = _positionOfLastSample;
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
    }
}