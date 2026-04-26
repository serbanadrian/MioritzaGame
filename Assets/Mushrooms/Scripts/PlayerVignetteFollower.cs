using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace MioritzaGame.Game
{
    public sealed class PlayerVignetteFollower : MonoBehaviour
    {
        [SerializeField] private PlayerContext _player;
        [SerializeField] private Camera _camera;
        [SerializeField] private Volume _volume;

        [Header("Tuning")]
        [SerializeField, Range(0f, 1f)] private float _baseIntensity = 0.35f;
        [SerializeField, Range(0f, 1f)] private float _maxIntensity = 0.85f;
        [SerializeField, Range(0f, 1f)] private float _smoothness = 0.55f;
        [SerializeField] private Color _color = Color.black;

        private Vignette _vignette;
        private VolumeProfile _trackedProfile;

        private void Awake()
        {
            if (_player == null) _player = UnityEngine.Object.FindAnyObjectByType<PlayerContext>();
            if (_camera == null) _camera = Camera.main;
            if (_volume == null)
            {
                var tagged = GameObject.FindGameObjectWithTag("Post-Process");
                if (tagged != null) _volume = tagged.GetComponent<Volume>();
                if (_volume == null) _volume = UnityEngine.Object.FindAnyObjectByType<Volume>();
            }
        }

        private bool TryEnsureVignette()
        {
            if (_volume == null) return false;
            if (_volume.profile == null) return false;

            if (_vignette == null || _trackedProfile != _volume.profile)
            {
                _trackedProfile = _volume.profile;
                if (_trackedProfile.TryGet<Vignette>(out _vignette) == false)
                {
                    _vignette = _trackedProfile.Add<Vignette>(false);
                }
                _vignette.intensity.overrideState = true;
                _vignette.smoothness.overrideState = true;
                _vignette.color.overrideState = true;
                _vignette.center.overrideState = true;
                _vignette.smoothness.value = _smoothness;
                _vignette.color.value = _color;
            }

            _vignette.active = true;
            return true;
        }

        private void LateUpdate()
        {
            if (TryEnsureVignette() == false) return;
            if (_camera == null) _camera = Camera.main;
            if (_camera == null || _player == null) return;

            var viewport = _camera.WorldToViewportPoint(_player.transform.position);
            if (viewport.z < 0f)
            {
                _vignette.center.value = new Vector2(0.5f, 0.5f);
            }
            else
            {
                _vignette.center.value = new Vector2(Mathf.Clamp01(viewport.x), Mathf.Clamp01(viewport.y));
            }

            var levelRatio = _player.MaxLevel > 0
                ? Mathf.Clamp01(_player.CurrentLevel / (float)_player.MaxLevel)
                : 0f;
            _vignette.intensity.value = Mathf.Lerp(_baseIntensity, _maxIntensity, levelRatio);
        }
    }
}
