using UnityEngine;

namespace MioritzaGame.Game
{
    public sealed class EntryDoor : MonoBehaviour
    {
        private EntryDoor _targetDoor;
        private Vector3 _targetSpawn;
        private EntryFacing _targetFacing;
        private Vector3 _targetCameraPosition;
        private bool _hasTarget;
        private float _nextTriggerTime;

        public void SetTarget(EntryDoor targetDoor, Vector3 spawnPosition, EntryFacing facing, Vector3 cameraPosition)
        {
            _targetDoor = targetDoor;
            _targetSpawn = spawnPosition;
            _targetFacing = facing;
            _targetCameraPosition = cameraPosition;
            _hasTarget = true;
        }

        public void SuppressUntil(float time)
        {
            if (time > _nextTriggerTime) _nextTriggerTime = time;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (_hasTarget == false) return;
            if (Time.time < _nextTriggerTime) return;

            var controller = other.GetComponentInParent<PlayerController>();
            if (controller == null) return;

            var cooldown = Time.time + 0.75f;
            _nextTriggerTime = cooldown;
            if (_targetDoor != null) _targetDoor.SuppressUntil(cooldown);

            ScreenFader.Instance.TransitionTo(() =>
            {
                controller.Spawn(_targetSpawn, _targetFacing);
                var cam = Camera.main;
                if (cam != null)
                {
                    var current = cam.transform.position;
                    cam.transform.position = new Vector3(_targetCameraPosition.x, current.y, _targetCameraPosition.z);
                }
            });
        }
    }
}
