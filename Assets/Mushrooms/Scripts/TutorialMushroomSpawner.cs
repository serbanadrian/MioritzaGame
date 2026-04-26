using UnityEngine;

public class TutorialMushroomSpawner : MonoBehaviour
{
    [SerializeField] private Mushroom _mushroomPrefab;
    [SerializeField] private MushroomSO _mushroomData;
    [SerializeField] private float _spawnDelay = 1f;
    [SerializeField] private Vector3 _offsetFromPlayer = new Vector3(3f, 0f, 0f);

    private bool _spawned;

    private void Start() => Invoke(nameof(Spawn), _spawnDelay);

    private void Spawn()
    {
        if (_spawned == true) return;
        if (_mushroomPrefab == null || _mushroomData == null)
        {
            Debug.LogError($"{nameof(TutorialMushroomSpawner)} missing prefab or data.");
            return;
        }

        var pc = Object.FindAnyObjectByType<MioritzaGame.Game.PlayerController>();
        if (pc == null)
        {
            Invoke(nameof(Spawn), 0.5f);
            return;
        }

        var pos = pc.transform.position + _offsetFromPlayer;
        pos.y = 0.5f;
        var instance = Instantiate(_mushroomPrefab, pos, Quaternion.Euler(90f, 0f, 0f));
        instance.transform.localScale = Vector3.one * 2.5f;
        instance.Initialize(_mushroomData);
        _spawned = true;
    }
}
