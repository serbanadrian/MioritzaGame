using UnityEngine;
using MioritzaGame.Constants;

namespace MioritzaGame.Game
{
    public sealed class Level02PuzzleController : MonoBehaviour
    {
        [SerializeField] private GameObject _apa;
        [SerializeField] private GameObject _bridge;

        private void OnEnable()
        {
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
            if (data.type != MushroomType.Good) return;

            if (_apa != null) _apa.SetActive(false);
            if (_bridge != null) _bridge.SetActive(true);

            MushroomToast.Show(Texts.Bridge.EVAPORATED_TITLE, Texts.Bridge.EVAPORATED_BODY, new Color(0.7f, 1f, 0.7f), 2.2f);
        }
    }
}
