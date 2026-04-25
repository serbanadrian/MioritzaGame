using System;
using UnityEngine;

namespace MioritzaGame.Game
{
    [CreateAssetMenu(
        fileName = "NewEntryConfig",
        menuName = "MioritzaGame/Entry Config",
        order = 1)]
    public sealed class EntryConfiguration : ScriptableObject
    {
        [SerializeField] private EntryPlacement[] _placements = Array.Empty<EntryPlacement>();
        [SerializeField, Min(0)] private int _minCount = 2;
        [SerializeField, Min(0)] private int _maxCount = 4;
        [SerializeField, Min(0f)] private float _scale = 3f;
        [SerializeField] private float _yOffset = 0.05f;

        internal EntryPlacement[] Placements => _placements;
        internal int MinCount => _minCount;
        internal int MaxCount => _maxCount;
        internal float Scale => _scale;
        internal float YOffset => _yOffset;

        private void OnValidate()
        {
            if (_maxCount < _minCount) _maxCount = _minCount;
        }
    }
}
