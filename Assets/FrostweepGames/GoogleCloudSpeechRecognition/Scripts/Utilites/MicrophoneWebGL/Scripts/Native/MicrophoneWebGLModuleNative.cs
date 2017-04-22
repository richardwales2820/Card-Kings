using UnityEngine;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System;
using FrostweepGames.SpeechRecognition.Utilites;

namespace FrostweepGames.UniversalMicrophoneLibrary.Native.Utilites.WebGL
{
#if UNITY_WEBGL
    public class MicrophoneWebGLModuleNative : MonoBehaviour
    {
        #region __Internal

        [DllImport("__Internal")]
        private static extern void DoMicrophoneLibByState(int param);

        #endregion

        private static MicrophoneWebGLModuleNative _Instance;
        public static MicrophoneWebGLModuleNative Instance
        {
            get
            {
                if (_Instance == null)
                    _Instance = new GameObject("MicrophoneWebGLModuleNative").AddComponent<MicrophoneWebGLModuleNative>();

                return _Instance;
            }
        }

        public event Action RecordSuccessEvent;
        public event Action<string> RecordFailedEvent;
        public event Action RecordFinishedEvent;
        public event Action<string> RecordInfoEvent;

        public bool isRecording;

        private List<float> _microphoneBuffer;   
        private bool _isModuleAvailable;
        private AudioClip _madeAudioClip;
        private int _sampleRate = 44100;

        public void StartRecordNative()
        {
            if(!_isModuleAvailable)
            {
                if (RecordFailedEvent != null)
                    RecordFailedEvent("Microphone Module isn't available");

                return;
            }
            else if (isRecording)
            {
                if (RecordFailedEvent != null)
                    RecordFailedEvent("Record is already started");

                return;
            }

            CheckNClearCache();

            DoMicrophoneLibByState((int)Enumerators.MicrophoneStateType.START_RECORD);

            isRecording = true;

            if (RecordSuccessEvent != null)
                RecordSuccessEvent();
        }

        public void StopRecordNative()
        {
            if (!isRecording)
                return;

            DoMicrophoneLibByState((int)Enumerators.MicrophoneStateType.STOP_RECORD);

            isRecording = false;

            if (RecordFinishedEvent != null)
                RecordFinishedEvent();
        }

        public void Initialize()
        {
            DoMicrophoneLibByState((int)Enumerators.MicrophoneStateType.MIC_AUTH);
        }

        public List<float> GetRecordedBytes()
        {
            return _microphoneBuffer;
        }

        public AudioClip GetAudioClipFromRecordedAudio()
        {
            if (_madeAudioClip != null)
                return _madeAudioClip;

            _madeAudioClip = AudioClip.Create("BufferedClip", _microphoneBuffer.Count, 1, _sampleRate, false);
            _madeAudioClip.SetData(_microphoneBuffer.ToArray(), 0);

            return _madeAudioClip;
        }

        public bool IsModuleAvailable()
        {
            return _isModuleAvailable;
        }

        public void Dispose()
        {
            _Instance = null;
            CheckNClearCache();

            Destroy(gameObject);
        }

        private void AddByteToBufferFromMicrophoneHandler(float value)
        {
            _microphoneBuffer.Add(value);
        }

        private void CheckMicrophoneAuthorizationHandler(int value)
        {
            _isModuleAvailable = value == 1 ? true : false;
        }

        private void CheckNClearCache()
        {
            if (_madeAudioClip != null)
                Destroy(_madeAudioClip);

            if (_microphoneBuffer == null)
                _microphoneBuffer = new List<float>();
            else
                _microphoneBuffer.Clear();
        }
    }
#endif
}