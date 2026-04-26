using System;
using UnityEngine;

namespace MioritzaGame.Game
{
    public sealed class InsanityLevelEffects : MonoBehaviour
    {
        [Serializable]
        public struct LevelBucket
        {
            public MushroomSO _mushroom;
        }

        [SerializeField] private PlayerContext _player;
        [SerializeField] private ActiveEffects _activeEffects;

        [Tooltip("Index = insanity level (0..MaxLevel). Each entry references the MushroomSO whose effects fire when the player's insanity reaches that level. Same audio/visual as eating that mushroom.")]
        [SerializeField] private LevelBucket[] _byLevel = new LevelBucket[7];

        private int _lastLevel = -1;

        private void Awake()
        {
            if (_player == null) _player = GetComponent<PlayerContext>();
            if (_player == null) _player = UnityEngine.Object.FindAnyObjectByType<PlayerContext>();
            if (_activeEffects == null) _activeEffects = UnityEngine.Object.FindAnyObjectByType<ActiveEffects>(FindObjectsInactive.Include);
        }

        private void Update()
        {
            if (_player == null || _activeEffects == null) return;

            var level = _player.CurrentLevel;
            if (level == _lastLevel) return;
            _lastLevel = level;

            if (level < 0 || level >= _byLevel.Length) return;
            var mushroom = _byLevel[level]._mushroom;
            if (mushroom == null) return;

            _activeEffects.ApplyMushroomEffects(mushroom);
        }
    }
}
