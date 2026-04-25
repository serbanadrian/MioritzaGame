using UnityEngine;

namespace MioritzaGame.Game
{
    [CreateAssetMenu(
        fileName = "NewPlayerConfig",
        menuName = "MioritzaGame/Player Config",
        order = 0)]
    public sealed class PlayerConfiguration : ScriptableObject
    {
        [SerializeField, Min(0f)] private float _moveSpeed = 5f;
        [SerializeField, Min(1)] private int _maxHealth = 100;

        internal float MoveSpeed => _moveSpeed;
        internal int MaxHealth => _maxHealth;
    }
}
