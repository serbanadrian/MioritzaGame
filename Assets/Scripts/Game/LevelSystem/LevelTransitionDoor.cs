using UnityEngine;
using UnityEngine.SceneManagement;
using MioritzaGame.Constants;

namespace MioritzaGame.Game
{
    public sealed class LevelTransitionDoor : MonoBehaviour
    {
        [SerializeField] private string _targetScene;
        [SerializeField, Min(0f)] private float _cooldown = 1f;
        private float _nextTriggerTime;
        private bool _showConfirm;

        public void SetTargetScene(string targetScene) => _targetScene = targetScene;

        private void OnTriggerEnter(Collider other)
        {
            if (string.IsNullOrEmpty(_targetScene) == true)
            {
                Debug.LogError($"{nameof(LevelTransitionDoor)} on '{name}' missing {nameof(_targetScene)}.");
                return;
            }
            if (Time.time < _nextTriggerTime) return;
            if (_showConfirm == true) return;

            var controller = other.GetComponentInParent<PlayerController>();
            if (controller == null) return;

            _nextTriggerTime = Time.time + _cooldown;

            if (HasUnpickedSheep() == true)
            {
                _showConfirm = true;
                Time.timeScale = 0f;
                return;
            }

            LoadTargetScene();
        }

        private void LoadTargetScene()
        {
            ScreenFader.Instance.FadeToBlack(0.4f);
            Invoke(nameof(DoLoad), 0.4f);
        }

        private void DoLoad() => SceneManager.LoadScene(_targetScene);

        private static bool HasUnpickedSheep()
        {
            var sheep = FindObjectsByType<SheepFollow>(FindObjectsSortMode.None);
            foreach (var s in sheep)
                if (s.isFollowing == false) return true;
            return false;
        }

        private void OnGUI()
        {
            if (_showConfirm == false) return;

            var w = 460f;
            var h = 160f;
            var rect = new Rect((Screen.width - w) / 2f, (Screen.height - h) / 2f, w, h);

            GUI.Box(rect, GUIContent.none);

            var labelStyle = new GUIStyle(GUI.skin.label);
            labelStyle.alignment = TextAnchor.MiddleCenter;
            labelStyle.fontSize = 14;
            labelStyle.fontStyle = FontStyle.Bold;
            labelStyle.wordWrap = true;
            labelStyle.normal.textColor = Color.white;
            GUI.Label(new Rect(rect.x + 20f, rect.y + 20f, w - 40f, 60f),
                Texts.LevelExit.SHEEP_REMINDER, labelStyle);

            if (GUI.Button(new Rect(rect.x + 60f, rect.y + 100f, 140f, 36f), Texts.LevelExit.OK) == true)
            {
                _showConfirm = false;
                Time.timeScale = 1f;
                LoadTargetScene();
            }
            if (GUI.Button(new Rect(rect.x + w - 200f, rect.y + 100f, 140f, 36f), Texts.LevelExit.CANCEL) == true)
            {
                _showConfirm = false;
                Time.timeScale = 1f;
            }
        }
    }
}
