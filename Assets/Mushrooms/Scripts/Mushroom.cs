using System;
using UnityEngine;
using MioritzaGame.Constants;

public class Mushroom : MonoBehaviour
{
    public static event System.Action<MushroomSO> OnConsumed;

    [SerializeField] MushroomSO data;
    [SerializeField] ActiveEffects effects;

    [Header("Pickup")]
    [SerializeField] private float _pickupRadius = 4f;

    private static Mushroom s_closestPromptOwner;
    private static float s_closestPromptDistanceSq = float.MaxValue;
    private static int s_closestPromptFrame = -1;

    private SpriteRenderer sprite;
    private bool _playerInRange;
    private Transform _player;
    private float _lastDistanceSq;

    // Fired when a good mushroom is eaten by water. Passes the world position where it happened.
    public static event Action<Vector3> OnGoodEatenByWater;

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
        _lastDistanceSq = delta.sqrMagnitude;
        var radiusSq = _pickupRadius * _pickupRadius;
        _playerInRange = _lastDistanceSq <= radiusSq;

        if (_playerInRange == true)
        {
            if (s_closestPromptFrame != Time.frameCount)
            {
                s_closestPromptFrame = Time.frameCount;
                s_closestPromptOwner = this;
                s_closestPromptDistanceSq = _lastDistanceSq;
            }
            else if (_lastDistanceSq < s_closestPromptDistanceSq)
            {
                s_closestPromptOwner = this;
                s_closestPromptDistanceSq = _lastDistanceSq;
            }
        }

        if (_playerInRange == true && Input.GetKeyDown(KeyCode.E) == true) Consume();
    }

    private void TryFindPlayer()
    {
        var pc = UnityEngine.Object.FindAnyObjectByType<MioritzaGame.Game.PlayerController>();
        if (pc == null) return;
        _player = pc.transform;
    }

    private void Consume()
    {
        if (effects == null) effects = UnityEngine.Object.FindAnyObjectByType<ActiveEffects>(FindObjectsInactive.Include);
        if (effects != null && effects.gameObject.activeInHierarchy == false)
        {
            Debug.LogWarning($"{nameof(Mushroom)} located inactive {nameof(ActiveEffects)} — activating it.");
            effects.gameObject.SetActive(true);
        }
        if (effects != null && data != null) effects.ConsumeMushroom(data);

        if (data != null) OnConsumed?.Invoke(data);

        var label = data != null && string.IsNullOrEmpty(data.mushroomName) == false
            ? Texts.Mushroom.TOOK_PREFIX + data.mushroomName.ToUpper()
            : Texts.Mushroom.TOOK_GENERIC;
        var tint = data != null && data.type == MushroomType.Bad ? new Color(1f, 0.4f, 0.4f) : new Color(0.6f, 1f, 0.6f);
        var description = BuildFlavorLine(data);
        MushroomToast.Show(label, description, tint);

        var overlayTint = data != null && data.type == MushroomType.Bad
            ? new Color(0.8f, 0.1f, 0.1f, 0.18f)
            : new Color(0.2f, 0.7f, 0.3f, 0.15f);
        ScreenEffectOverlay.Show(overlayTint, 4f, vignette: true, grain: data != null && data.type == MushroomType.Bad);

        if (RevealsExit(data) == true)
            MushroomToast.Show(Texts.Mushroom.EXIT_REVEALED_TITLE, Texts.Mushroom.EXIT_REVEALED_BODY, new Color(1f, 0.85f, 0.4f));

        Destroy(gameObject);
    }

    private static bool RevealsExit(MushroomSO mushroom)
    {
        if (mushroom == null) return false;
        if (mushroom.goodEffects == null) return false;
        foreach (var effect in mushroom.goodEffects)
            if (effect is CorrectExitHintSO) return true;
        return false;
    }

    private static string BuildFlavorLine(MushroomSO mushroom)
    {
        if (mushroom == null) return string.Empty;

        if (mushroom.type == MushroomType.Good)
            return mushroom.InsanityPoints < 0
                ? Texts.Mushroom.GOOD_HEAL
                : Texts.Mushroom.GOOD_STEADY;

        if (mushroom.type == MushroomType.Bad)
            return mushroom.InsanityPoints > 0
                ? Texts.Mushroom.BAD_NAUSEA
                : Texts.Mushroom.BAD_CHURN;

        return Texts.Mushroom.NEUTRAL;
    }

    void OnGUI()
    {
        if (_playerInRange == false) return;
        if (s_closestPromptOwner != this) return;
        var cam = Camera.main;
        if (cam == null) return;

        var screenPos = cam.WorldToScreenPoint(transform.position);
        if (screenPos.z < 0f) return;

        string label;
        if (data != null && string.IsNullOrEmpty(data.pickupHint) == false)
        {
            label = data.pickupHint;
        }
        else
        {
            var name = data != null && string.IsNullOrEmpty(data.mushroomName) == false
                ? data.mushroomName
                : Texts.Mushroom.PRESS_E_GENERIC_NAME;
            label = string.Format(Texts.Mushroom.PRESS_E_FORMAT, name);
        }

        var style = new GUIStyle(GUI.skin.box);
        style.fontSize = 16;
        style.fontStyle = FontStyle.Bold;
        style.normal.textColor = Color.white;
        style.alignment = TextAnchor.MiddleCenter;
        style.wordWrap = true;
        style.padding = new RectOffset(12, 12, 8, 8);

        var maxWidth = Mathf.Min(720f, Screen.width - 40f);
        var content = new GUIContent(label);
        var height = style.CalcHeight(content, maxWidth);
        var width = Mathf.Min(maxWidth, style.CalcSize(content).x + 24f);

        var rect = new Rect(
            Mathf.Clamp(screenPos.x - width * 0.5f, 10f, Screen.width - width - 10f),
            Mathf.Clamp(Screen.height - screenPos.y - 80f, 10f, Screen.height - height - 10f),
            width,
            height);
        GUI.Label(rect, content, style);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other == null) return;

        if (other.CompareTag("Water"))
        {
            if (data != null && data.cleansToxicWater == true)
            {
                if (effects == null) effects = UnityEngine.Object.FindAnyObjectByType<ActiveEffects>(FindObjectsInactive.Include);
                if (effects != null) effects.ConsumeMushroom(data);
                Destroy(gameObject);
                Destroy(other.gameObject);
                OnGoodEatenByWater?.Invoke(transform.position);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other == null) return;
        var go = other.gameObject;
        if (go.CompareTag("Water"))
        {
            if (data != null && data.cleansToxicWater == true)
            {
                if (effects == null) effects = UnityEngine.Object.FindAnyObjectByType<ActiveEffects>(FindObjectsInactive.Include);
                if (effects != null) effects.ConsumeMushroom(data);
                Destroy(gameObject);
                Destroy(go);
                OnGoodEatenByWater?.Invoke(transform.position);
            }
        }
    }
}
