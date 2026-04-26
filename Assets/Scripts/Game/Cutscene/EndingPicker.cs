using UnityEngine;
using UnityEngine.Video;

namespace MioritzaGame.Game
{
    public sealed class EndingPicker : MonoBehaviour
    {
        [SerializeField] private VideoClip _badEnding;
        [SerializeField] private VideoClip _neutralEnding1;
        [SerializeField] private VideoClip _neutralEnding2;
        [SerializeField] private VideoClip _goodEnding;

        private void Start()
        {
            var sheepCount = CountFollowingSheep();
            var clip = PickClip(sheepCount);
            if (clip == null)
            {
                Debug.LogError($"{nameof(EndingPicker)} missing clip for {sheepCount} sheep — falling back to bad ending.");
                clip = _badEnding;
            }
            MioritzaGame.GameManager.OpenCreditsOnLoad = true;
            GameOverScreen.Trigger(clip);
        }

        private static int CountFollowingSheep()
        {
            var sheep = Object.FindObjectsByType<SheepFollow>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            var count = 0;
            for (var i = 0; i < sheep.Length; i++)
                if (sheep[i] != null && sheep[i].isFollowing == true) count++;
            return count;
        }

        private VideoClip PickClip(int sheepCount)
        {
            switch (sheepCount)
            {
                case 0: return _badEnding;
                case 1: return _neutralEnding1;
                case 2: return _neutralEnding2;
                default: return _goodEnding;
            }
        }
    }
}
