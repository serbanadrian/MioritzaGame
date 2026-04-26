using UnityEngine;

public class Mushroom : MonoBehaviour
{
    public static event System.Action<MushroomSO> OnConsumed;

    [SerializeField] MushroomSO data;
    [SerializeField] ActiveEffects effects;

    [Header("Pickup")]
    [SerializeField] private float _pickupRadius = 4f;

    private SpriteRenderer sprite;
    private bool _playerInRange;
    private Transform _player;

    void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();
        if (data != null && sprite != null) sprite.sprite = data.sprite;

        var col = GetComponent<Collider>();
        if (col != null) col.isTrigger = true;
    }

    public void Initialize(MushroomSO newData, ActiveEffects sceneEffects = null)
    {
        data = newData;
        if (sprite == null) sprite = GetComponent<SpriteRenderer>();
        if (data != null && sprite != null) sprite.sprite = data.sprite;

        if (sceneEffects != null)
        {
            effects = sceneEffects;
        }
        else if (effects == null)
        {
            effects = GetComponentInChildren<ActiveEffects>();
            if (effects == null)
            {
                var go = GameObject.FindGameObjectWithTag("ActiveEffects");
                if (go != null && go.TryGetComponent<ActiveEffects>(out var ae))
                    effects = ae;
            }
        }
    }

    void Update()
    {
        if (MioritzaGame.Game.CutsceneManager.IsCutsceneActive == true) { _playerInRange = false; return; }

        if (_player == null) TryFindPlayer();
        if (_player == null) return;

        var delta = transform.position - _player.position;
        delta.y = 0f;
        var distance = delta.magnitude;
        _playerInRange = distance <= _pickupRadius;

        if (_playerInRange == true && Input.GetKeyDown(KeyCode.E) == true) Consume();
    }

    private void TryFindPlayer()
    {
        var pc = Object.FindAnyObjectByType<MioritzaGame.Game.PlayerController>();
        if (pc == null) return;
        _player = pc.transform;
    }

    private void Consume()
    {
        if (effects == null) effects = Object.FindAnyObjectByType<ActiveEffects>(FindObjectsInactive.Include);
        if (effects != null && effects.gameObject.activeInHierarchy == false)
        {
            Debug.LogWarning($"{nameof(Mushroom)} located inactive {nameof(ActiveEffects)} — activating it.");
            effects.gameObject.SetActive(true);
        }
        if (effects != null && data != null) effects.ConsumeMushroom(data);

        if (data != null) OnConsumed?.Invoke(data);

        var label = data != null && string.IsNullOrEmpty(data.mushroomName) == false
            ? $"TOOK {data.mushroomName.ToUpper()}"
            : "TOOK MUSHROOM";
        var tint = data != null && data.type == MushroomType.Bad ? new Color(1f, 0.4f, 0.4f) : new Color(0.6f, 1f, 0.6f);
        var description = BuildFlavorLine(data);
        MushroomToast.Show(label, description, tint);

        var overlayTint = data != null && data.type == MushroomType.Bad
            ? new Color(0.8f, 0.1f, 0.1f, 0.18f)
            : new Color(0.2f, 0.7f, 0.3f, 0.15f);
        ScreenEffectOverlay.Show(overlayTint, 4f, vignette: true, grain: data != null && data.type == MushroomType.Bad);

        Destroy(gameObject);
    }

    private static string BuildFlavorLine(MushroomSO mushroom)
    {
        if (mushroom == null) return string.Empty;

        if (mushroom.type == MushroomType.Good)
            return mushroom.InsanityPoints < 0
                ? "You took a good mushroom — your sanity returns a little."
                : "You took a good mushroom — it steadies you.";

        if (mushroom.type == MushroomType.Bad)
            return mushroom.InsanityPoints > 0
                ? "Bad mushroom — your head spins with nausea."
                : "Bad mushroom — your stomach churns.";

        return "You took a mushroom — its taste lingers.";
    }

    void OnGUI()
    {
        if (_playerInRange == false) return;
        var cam = Camera.main;
        if (cam == null) return;

        var screenPos = cam.WorldToScreenPoint(transform.position);
        if (screenPos.z < 0f) return;

        var label = data != null && string.IsNullOrEmpty(data.mushroomName) == false
            ? $"PRESS E TO TAKE {data.mushroomName.ToUpper()}"
            : "PRESS E TO TAKE";

        var rect = new Rect(screenPos.x - 140f, Screen.height - screenPos.y - 80f, 280f, 28f);
        var style = new GUIStyle(GUI.skin.box);
        style.fontSize = 16;
        style.fontStyle = FontStyle.Bold;
        style.normal.textColor = Color.white;
        style.alignment = TextAnchor.MiddleCenter;
        GUI.Label(rect, label, style);
    }
}
