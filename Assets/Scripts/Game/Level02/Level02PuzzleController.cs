using UnityEngine;
using UnityEngine.SceneManagement;
using MioritzaGame.Constants;

namespace MioritzaGame.Game
{
    public sealed class Level02PuzzleController : MonoBehaviour
    {
        [SerializeField] private GameObject _apa;
        [SerializeField] private GameObject _bridge;
        [SerializeField] private string _nextSceneName = "Cutscene_Puzzle2";

        private static bool s_triggered;

        private void OnEnable()
        {
            s_triggered = false;
            Mushroom.OnConsumed += HandleMushroomConsumed;
            if (_bridge != null) _bridge.SetActive(false);
        }

        private void OnDisable()
        {
            Mushroom.OnConsumed -= HandleMushroomConsumed;
        }

        private void HandleMushroomConsumed(MushroomSO data)
        {
            if (data == null) return;
            if (data.cleansToxicWater == false) return;
            if (s_triggered == true) return;

            if (string.IsNullOrEmpty(_nextSceneName) == true)
            {
                Debug.LogError($"{nameof(Level02PuzzleController)} missing {nameof(_nextSceneName)}.");
                return;
            }

            s_triggered = true;

            if (_apa != null) _apa.SetActive(false);
            MushroomToast.Show(Texts.Bridge.EVAPORATED_TITLE, Texts.Bridge.EVAPORATED_BODY, new Color(0.7f, 1f, 0.7f), 2.2f);
            SceneManager.LoadScene(_nextSceneName);
        }
    }
}
