using UnityEngine;

namespace MioritzaGame.Audio
{
    [RequireComponent(typeof(AudioSource))]
    public class AudioSourcePrefVolume : MonoBehaviour
    {
        public enum VolumeType { Music, Master }

        [SerializeField] private VolumeType volumeType = VolumeType.Music;
        [SerializeField][Range(0f, 1f)] private float multiplier = 1f;

        private AudioSource _source;

        private void Awake()
        {
            _source = GetComponent<AudioSource>();
        }

        private void OnEnable()
        {
            AudioSettings.MusicVolumeChanged += HandleMusic;
            AudioSettings.MasterVolumeChanged += HandleMaster;
        }

        private void OnDisable()
        {
            AudioSettings.MusicVolumeChanged -= HandleMusic;
            AudioSettings.MasterVolumeChanged -= HandleMaster;
        }

        private void Start()
        {
            // Initialize volume from PlayerPrefs
            float music = AudioSettings.GetMusicVolume();
            float master = AudioSettings.GetMasterVolume();
            ApplyVolumes(music, master);
        }

        private void HandleMusic(float v)
        {
            if (volumeType == VolumeType.Music) ApplyVolumes(v, AudioSettings.GetMasterVolume());
        }

        private void HandleMaster(float v)
        {
            if (volumeType == VolumeType.Master) ApplyVolumes(AudioSettings.GetMusicVolume(), v);
        }

        private void ApplyVolumes(float music, float master)
        {
            if (_source == null) return;
            // Combine master and type-specific volume, then apply multiplier
            float combined = Mathf.Clamp01(master * (volumeType == VolumeType.Music ? music : 1f));
            _source.volume = combined * multiplier;
        }
    }
}
