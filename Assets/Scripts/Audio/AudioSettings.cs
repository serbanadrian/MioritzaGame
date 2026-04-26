using System;
using UnityEngine;

namespace MioritzaGame.Audio
{
    // Central bridge for audio settings. Stores PlayerPrefs and emits events when values change.
    public static class AudioSettings
    {
        public static event Action<float> MusicVolumeChanged;
        public static event Action<float> MasterVolumeChanged;

        private const string MusicKey = "MusicVolume";
        private const string MasterKey = "MasterVolume";

        public static float GetMusicVolume() => PlayerPrefs.GetFloat(MusicKey, 1f);
        public static float GetMasterVolume() => PlayerPrefs.GetFloat(MasterKey, 1f);

        public static void SetMusicVolume(float value)
        {
            PlayerPrefs.SetFloat(MusicKey, value);
            PlayerPrefs.Save();
            MusicVolumeChanged?.Invoke(value);
        }

        public static void SetMasterVolume(float value)
        {
            PlayerPrefs.SetFloat(MasterKey, value);
            PlayerPrefs.Save();
            MasterVolumeChanged?.Invoke(value);
        }
    }
}
