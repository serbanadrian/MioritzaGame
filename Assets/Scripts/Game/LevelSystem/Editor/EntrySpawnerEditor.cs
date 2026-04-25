#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace MioritzaGame.Game.Editor
{
    [CustomEditor(typeof(EntrySpawner))]
    public sealed class EntrySpawnerEditor : UnityEditor.Editor
    {
        private static GameObject _entry;

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            EditorGUILayout.Space();
            EditorGUILayout.HelpBox(
                "Play Mode workflow:\n" +
                "1. Drag a spawned entry GameObject (e.g. 'intrari_3') from the hierarchy into the field below.\n" +
                "2. Move the Player to the desired spawn spot.\n" +
                "3. Click the button — the offset is computed and written into the matching placement.",
                MessageType.Info);

            _entry = (GameObject)EditorGUILayout.ObjectField("Entry to capture", _entry, typeof(GameObject), allowSceneObjects: true);

            if (GUILayout.Button("Capture Spawn Offset → Apply To Placement") == true)
                CaptureAndApply((EntrySpawner)target);
        }

        private static void CaptureAndApply(EntrySpawner spawner)
        {
            if (_entry == null)
            {
                Debug.LogError($"{nameof(EntrySpawnerEditor)}: drag a spawned entry GameObject into the 'Entry to capture' field first.");
                return;
            }

            var player = GameObject.Find("Player");
            if (player == null)
            {
                Debug.LogError($"{nameof(EntrySpawnerEditor)}: Player not found.");
                return;
            }

            var map = FindNearestRoom(_entry.transform.position);
            if (map == null)
            {
                Debug.LogError($"{nameof(EntrySpawnerEditor)}: could not find a Room_*(Clone) in the scene.");
                return;
            }

            var entryLocal = map.InverseTransformPoint(_entry.transform.position);
            var playerLocal = map.InverseTransformPoint(player.transform.position);
            var offset = playerLocal - entryLocal;

            var config = new SerializedObject(spawner).FindProperty("_configuration").objectReferenceValue as EntryConfiguration;
            if (config == null)
            {
                Debug.LogError($"{nameof(EntrySpawnerEditor)}: spawner has no {nameof(EntryConfiguration)} assigned.");
                return;
            }

            var so = new SerializedObject(config);
            var placements = so.FindProperty("_placements");
            for (var i = 0; i < placements.arraySize; i++)
            {
                var placement = placements.GetArrayElementAtIndex(i);
                var sprite = placement.FindPropertyRelative("_sprite").objectReferenceValue as Sprite;
                if (sprite == null) continue;
                if (sprite.name != _entry.name) continue;

                placement.FindPropertyRelative("_spawnOffset").vector3Value = offset;
                so.ApplyModifiedProperties();
                EditorUtility.SetDirty(config);
                AssetDatabase.SaveAssets();

                Debug.Log($"[EntrySpawner] {_entry.name} _spawnOffset = ({offset.x:F4}, {offset.y:F4}, {offset.z:F4}) — written to {config.name}");
                return;
            }

            Debug.LogError($"{nameof(EntrySpawnerEditor)}: no placement matches sprite name '{_entry.name}'.");
        }

        private static Transform FindNearestRoom(Vector3 worldPosition)
        {
            Transform best = null;
            var bestDistance = float.MaxValue;
            var rooms = Object.FindObjectsByType<Transform>(FindObjectsSortMode.None);
            foreach (var t in rooms)
            {
                if (t.name.StartsWith("Room_") == false) continue;
                var distance = (t.position - worldPosition).sqrMagnitude;
                if (distance < bestDistance)
                {
                    bestDistance = distance;
                    best = t;
                }
            }
            return best;
        }
    }
}
#endif
