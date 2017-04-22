using UnityEngine;
using FrostweepGames.UniversalMicrophoneLibrary.Native.Utilites.WebGL;
using System.Collections.Generic;
using System;

namespace FrostweepGames.UniversalMicrophoneLibrary
{
    public class MicrophoneWebGL
    {
        public MicrophoneWebGL() { }

        #if UNITY_WEBGL
        public static void End()
        {
            MicrophoneWebGLModuleNative.Instance.StopRecordNative();
        }

        public static void Start()
        {
            MicrophoneWebGLModuleNative.Instance.StartRecordNative();
        }

        public static void Init()
        {
            MicrophoneWebGLModuleNative.Instance.Initialize();
        }

        public static void Dispose()
        {
            MicrophoneWebGLModuleNative.Instance.Dispose();
        }

        public static bool IsRecording()
        {
            return MicrophoneWebGLModuleNative.Instance.isRecording;
        }

        public static AudioClip GetAudioClip()
        {
            return MicrophoneWebGLModuleNative.Instance.GetAudioClipFromRecordedAudio();
        }

        public static List<float> GetRawAudioClipData()
        {
            return MicrophoneWebGLModuleNative.Instance.GetRecordedBytes();
        }

        public static bool IsAvailable()
        {
            return MicrophoneWebGLModuleNative.Instance.IsModuleAvailable();
        }

        #region REGISTER CALLBACKS
        public static void RegisterRecordSuccessCallback(Action callback)
        {
            MicrophoneWebGLModuleNative.Instance.RecordSuccessEvent += callback;
        }

        public static void RegisterRecordFinishedCallback(Action callback)
        {
            MicrophoneWebGLModuleNative.Instance.RecordFinishedEvent += callback;
        }

        public static void RegisterRecordFailedCallback(Action<string> callback)
        {
            MicrophoneWebGLModuleNative.Instance.RecordFailedEvent += callback;
        }

        public static void RegisterRecordInfoCallback(Action<string> callback)
        {
            MicrophoneWebGLModuleNative.Instance.RecordInfoEvent += callback;
        }
#endregion

        #region UNREGISTER CALLBACKS
        public static void UnregisterRecordSuccessCallback(Action callback)
        {
            MicrophoneWebGLModuleNative.Instance.RecordSuccessEvent -= callback;
        }

        public static void UnregisterRecordFinishedCallback(Action callback)
        {
            MicrophoneWebGLModuleNative.Instance.RecordFinishedEvent -= callback;
        }

        public static void UnregisterRecordFailedCallback(Action<string> callback)
        {
            MicrophoneWebGLModuleNative.Instance.RecordFailedEvent -= callback;
        }

        public static void UnregisterRecordInfoCallback(Action<string> callback)
        {
            MicrophoneWebGLModuleNative.Instance.RecordInfoEvent -= callback;
        }
#endregion
#endif
    }
}