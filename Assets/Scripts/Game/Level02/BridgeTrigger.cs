using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

namespace MioritzaGame.Game
{
    [RequireComponent(typeof(Collider))]
    public sealed class BridgeTrigger : MonoBehaviour
    {
        [SerializeField] private VideoClip _cutsceneClip;
        [SerializeField] private string _nextSceneOnCross = "Cutscene_Puzzle2";

        private bool _triggered;

        private void Reset()
        {
            var col = GetComponent<Collider>();
            if (col != null) col.isTrigger = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (_triggered == true) return;
            var player = other.GetComponentInParent<PlayerContext>();
            if (player == null) return;
            if (player.IsDead == true) return;

            _triggered = true;
            if (string.IsNullOrEmpty(_nextSceneOnCross) == true)
            {
                Debug.LogError($"{nameof(BridgeTrigger)} on '{name}' missing {nameof(_nextSceneOnCross)}.");
                return;
            }
            SceneManager.LoadScene(_nextSceneOnCross);
        }
    }
}
