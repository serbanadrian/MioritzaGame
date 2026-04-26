#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace MioritzaGame.Game.Editor
{
    [CustomEditor(typeof(ApaTrigger))]
    public sealed class ApaTriggerEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            EditorGUILayout.Space();

            EditorGUILayout.HelpBox(
                "Drag the blue handles in the Scene to shape the toxic-substance area on the ground. " +
                "Use the buttons to add/remove corners.",
                MessageType.Info);

            var trigger = (ApaTrigger)target;
            var points = trigger.LocalPoints;
            var count = points?.Length ?? 0;

            EditorGUILayout.LabelField($"Points: {count}");

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("+ Add Point"))
            {
                Undo.RecordObject(trigger, "Add Apa Point");
                var list = new List<Vector3>(points ?? new Vector3[0]);
                Vector3 newPoint;
                if (list.Count == 0) newPoint = Vector3.zero;
                else if (list.Count == 1) newPoint = list[0] + new Vector3(1f, 0f, 0f);
                else newPoint = (list[list.Count - 1] + list[0]) * 0.5f;
                list.Add(newPoint);
                trigger.LocalPoints = list.ToArray();
                EditorUtility.SetDirty(trigger);
            }
            if (GUILayout.Button("− Remove Last") && count > 0)
            {
                Undo.RecordObject(trigger, "Remove Apa Point");
                var list = new List<Vector3>(points);
                list.RemoveAt(list.Count - 1);
                trigger.LocalPoints = list.ToArray();
                EditorUtility.SetDirty(trigger);
            }
            EditorGUILayout.EndHorizontal();
        }

        private void OnSceneGUI()
        {
            var trigger = (ApaTrigger)target;
            var points = trigger.LocalPoints;
            if (points == null || points.Length == 0) return;

            var origin = trigger.transform.position;
            var changed = false;

            for (var i = 0; i < points.Length; i++)
            {
                var local = points[i];
                var world = origin + new Vector3(local.x, 0f, local.z);

                Handles.color = new Color(0.3f, 0.7f, 1f, 1f);
                EditorGUI.BeginChangeCheck();
                var size = HandleUtility.GetHandleSize(world) * 0.2f;
                var newWorld = Handles.Slider2D(
                    world,
                    Vector3.up,
                    Vector3.right,
                    Vector3.forward,
                    size,
                    Handles.SphereHandleCap,
                    Vector2.zero);
                if (EditorGUI.EndChangeCheck() == true)
                {
                    var delta = newWorld - origin;
                    points[i] = new Vector3(delta.x, 0f, delta.z);
                    changed = true;
                }

                Handles.color = Color.white;
                Handles.Label(world + Vector3.up * 0.8f, $"{i}");
            }

            if (changed == true)
            {
                Undo.RecordObject(trigger, "Move Apa Vertex");
                trigger.LocalPoints = points;
                EditorUtility.SetDirty(trigger);
            }
        }
    }
}
#endif
