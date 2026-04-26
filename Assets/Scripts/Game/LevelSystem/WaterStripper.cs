using UnityEngine;

namespace MioritzaGame.Game
{
    public sealed class WaterStripper : MonoBehaviour
    {
        private void Start()
        {
            var roots = gameObject.scene.GetRootGameObjects();
            foreach (var root in roots) StripIn(root.transform);
        }

        private static void StripIn(Transform t)
        {
            for (var i = t.childCount - 1; i >= 0; i--)
            {
                var child = t.GetChild(i);
                StripIn(child);
            }
            if (t.CompareTag("Water") == true) Destroy(t.gameObject);
        }
    }
}
