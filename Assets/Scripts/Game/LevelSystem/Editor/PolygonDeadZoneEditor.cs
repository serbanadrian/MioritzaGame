#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace MioritzaGame.Game.Editor
{
    [CustomEditor(typeof(PolygonDeadZone))]
    public sealed class PolygonDeadZoneEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            EditorGUILayout.Space();

            EditorGUILayout.HelpBox(
                "Walls block the player from leaving. Drag green handles in the Scene to move corners. " +
                "Use buttons below to add/remove corners.",
                MessageType.Info);

            var zone = (PolygonDeadZone)target;
            var points = zone.LocalPoints;
            var count = points?.Length ?? 0;

            EditorGUILayout.LabelField($"Points: {count}");

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("+ Add Point"))
            {
                Undo.RecordObject(zone, "Add Polygon Point");
                var list = new List<Vector3>(points ?? new Vector3[0]);
                Vector3 newPoint;
                if (list.Count == 0)
                    newPoint = Vector3.zero;
                else if (list.Count == 1)
                    newPoint = list[0] + new Vector3(0.1f, 0f, 0f);
                else
                    newPoint = (list[list.Count - 1] + list[0]) * 0.5f;
                list.Add(newPoint);
                zone.LocalPoints = list.ToArray();
                EditorUtility.SetDirty(zone);
            }
            if (GUILayout.Button("− Remove Last") && count > 0)
            {
                Undo.RecordObject(zone, "Remove Polygon Point");
                var list = new List<Vector3>(points);
                list.RemoveAt(list.Count - 1);
                zone.LocalPoints = list.ToArray();
                EditorUtility.SetDirty(zone);
            }
            EditorGUILayout.EndHorizontal();
        }

        private void OnSceneGUI()
        {
            var zone = (PolygonDeadZone)target;
            var points = zone.LocalPoints;
            if (points == null || points.Length == 0) return;

            var matrix = zone.transform.localToWorldMatrix;
            var changed = false;

            for (var i = 0; i < points.Length; i++)
            {
                var local = points[i];
                var world = matrix.MultiplyPoint3x4(new Vector3(local.x, 0f, local.z));

                Handles.color = new Color(0.4f, 1f, 0.4f, 1f);
                EditorGUI.BeginChangeCheck();
                var size = HandleUtility.GetHandleSize(world) * 0.15f;
                var newWorld = Handles.FreeMoveHandle(world, size, Vector3.zero, Handles.SphereHandleCap);
                if (EditorGUI.EndChangeCheck() == true)
                {
                    var newLocal = zone.transform.worldToLocalMatrix.MultiplyPoint3x4(newWorld);
                    points[i] = new Vector3(newLocal.x, local.y, newLocal.z);
                    changed = true;
                }

                Handles.color = Color.white;
                Handles.Label(world + Vector3.up * 0.8f, $"{i}");
            }

            if (changed == true)
            {
                Undo.RecordObject(zone, "Move Polygon Vertex");
                zone.LocalPoints = points;
                EditorUtility.SetDirty(zone);
            }
        }
    }
}
#endif
