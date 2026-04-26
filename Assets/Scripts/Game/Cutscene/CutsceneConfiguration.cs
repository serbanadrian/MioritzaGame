using UnityEngine;

namespace MioritzaGame.Game
{
    [CreateAssetMenu(
        fileName = "NewCutscene",
        menuName = "MioritzaGame/Cutscene Config",
        order = 2)]
    public sealed class CutsceneConfiguration : ScriptableObject
    {
        [SerializeField] private bool _enabled = true;
        [SerializeField] private CutsceneStep[] _steps;

        internal bool Enabled => _enabled;
        internal CutsceneStep[] Steps => _steps;
    }
}
