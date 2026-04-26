using UnityEngine;
using UnityEngine.Audio;

namespace MioritzaGame.Audio
{
    [RequireComponent(typeof(AudioSource))]
    public class AudioMixerController : MonoBehaviour
    {
        [Header("Mixer")]
        [SerializeField] private AudioMixer audioMixer;
        [Tooltip("Exposed parameter name for music (in dB)")]
        [SerializeField] private string musicParam = "MusicVol";
        [Tooltip("Exposed parameter name for master (in dB)")]
        [SerializeField] private string masterParam = "MasterVol";

        private void OnEnable()
        {
            AudioSettings.MusicVolumeChanged += OnMusicVolumeChanged;
            AudioSettings.MasterVolumeChanged += OnMasterVolumeChanged;
        }

        private void OnDisable()
        {
            AudioSettings.MusicVolumeChanged -= OnMusicVolumeChanged;
            AudioSettings.MasterVolumeChanged -= OnMasterVolumeChanged;
        }

        private void Start()
        {
            DontDestroyOnLoad(this);
            // Initialize mixer values from PlayerPrefs
            OnMusicVolumeChanged(AudioSettings.GetMusicVolume());
            OnMasterVolumeChanged(AudioSettings.GetMasterVolume());
        }

        private void OnMusicVolumeChanged(float linear)
        {
            if (audioMixer == null) return;
            float dB = LinearToDecibel(linear);
            audioMixer.SetFloat(musicParam, dB);
        }

        private void OnMasterVolumeChanged(float linear)
        {
            if (audioMixer == null) return;
            float dB = LinearToDecibel(linear);
            audioMixer.SetFloat(masterParam, dB);
        }

        private float LinearToDecibel(float linear)
        {
            linear = Mathf.Clamp(linear, 0.0001f, 1f);
            return Mathf.Log10(linear) * 20f;
        }
    }
}
