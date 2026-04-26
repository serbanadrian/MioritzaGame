using System.Collections.Generic;
using UnityEngine;

namespace MioritzaGame.Game
{
    [DisallowMultipleComponent]
    public sealed class ExitGlow : MonoBehaviour
    {
        private static readonly List<ExitGlow> s_all = new List<ExitGlow>();

        [SerializeField] private bool _alwaysOn = false;
        [SerializeField] private Color _glowColor = new Color(1.6f, 1.4f, 0.6f, 1f);
        [SerializeField] private float _pulseHz = 1.6f;
        [SerializeField] private float _lightIntensity = 8f;
        [SerializeField] private float _lightRange = 8f;

        private SpriteRenderer[] _renderers;
        private Color[] _originalColors;
        private Light _light;
        private float _glowUntilUnscaled;

        public static void GlowAll(float duration)
        {
            var until = Time.unscaledTime + Mathf.Max(0f, duration);
            for (var i = 0; i < s_all.Count; i++)
            {
                if (s_all[i] != null) s_all[i]._glowUntilUnscaled = until;
            }
        }

        public static void ClearAll()
        {
            for (var i = 0; i < s_all.Count; i++)
            {
                if (s_all[i] != null) s_all[i]._glowUntilUnscaled = 0f;
            }
        }

        private void Awake()
        {
            CacheRenderers();
            EnsureLight();
        }

        private void CacheRenderers()
        {
            _renderers = GetComponentsInChildren<SpriteRenderer>(true);
            _originalColors = new Color[_renderers.Length];
            for (var i = 0; i < _renderers.Length; i++) _originalColors[i] = _renderers[i].color;
        }

        private void EnsureLight()
        {
            if (_light != null) return;
            var lightGo = new GameObject("ExitGlowLight");
            lightGo.transform.SetParent(transform, false);
            lightGo.transform.localPosition = new Vector3(0f, 1.5f, 0f);
            _light = lightGo.AddComponent<Light>();
            _light.type = LightType.Point;
            _light.color = _glowColor;
            _light.range = _lightRange;
            _light.intensity = 0f;
            _light.enabled = false;
        }

        private void OnEnable() => s_all.Add(this);
        private void OnDisable()
        {
            s_all.Remove(this);
            RestoreColors();
            if (_light != null) _light.enabled = false;
        }

        private void RestoreColors()
        {
            if (_renderers == null) return;
            for (var i = 0; i < _renderers.Length; i++)
            {
                if (_renderers[i] != null && i < _originalColors.Length) _renderers[i].color = _originalColors[i];
            }
        }

        private void LateUpdate()
        {
            EnsureLight();
            if (_alwaysOn == false && Time.unscaledTime >= _glowUntilUnscaled)
            {
                RestoreColors();
                _light.enabled = false;
                return;
            }

            var pulse = 0.5f + 0.5f * Mathf.Sin(Time.unscaledTime * _pulseHz * Mathf.PI * 2f);

            if (_renderers != null)
            {
                for (var i = 0; i < _renderers.Length; i++)
                {
                    if (_renderers[i] == null) continue;
                    _renderers[i].color = Color.Lerp(_originalColors[i], _glowColor, pulse);
                }
            }

            _light.enabled = true;
            _light.intensity = Mathf.Lerp(_lightIntensity * 0.4f, _lightIntensity, pulse);
            _light.color = _glowColor;
        }
    }
}
