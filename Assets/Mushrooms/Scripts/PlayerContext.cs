using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class PlayerContext : MonoBehaviour
{
    [Header("Insanity")]
    [SerializeField] private float _insanityPoints;
    [SerializeField] private float _startingInsanityPoints = -1f;
    [SerializeField] private float _pointsPerLevel = 100f;
    [SerializeField] private int _maxLevel = 6;
    [SerializeField] private bool _showNauseaOverlay = true;

    [Header("UI")]
    [SerializeField] private Texture2D[] _levelIcons = new Texture2D[6];
    [SerializeField] private float _iconSize = 220f;
    [SerializeField] private Vector2 _iconPadding = new Vector2(20f, 20f);

    [Header("Game Over")]
    [SerializeField] private VideoClip _gameOverVideo;

    private bool _dead;

    public int CurrentLevel => Mathf.Clamp(Mathf.FloorToInt(_insanityPoints / _pointsPerLevel), 0, _maxLevel);
    public float InsanityPoints => _insanityPoints;
    public int MaxLevel => _maxLevel;
    public bool IsDead => _dead;

    private void Awake()
    {
        if (_startingInsanityPoints >= 0f) _insanityPoints = _startingInsanityPoints;
    }

    public void InsanityChange(float value)
    {
        if (_dead == true) return;
        _insanityPoints = Mathf.Clamp(_insanityPoints + value, 0f, _pointsPerLevel * _maxLevel);
        if (CurrentLevel >= _maxLevel) Die();
    }

    private void Die()
    {
        if (_dead == true) return;
        _dead = true;
        Debug.Log($"{nameof(PlayerContext)} reached insanity level {_maxLevel} — player dies.");
        MioritzaGame.Game.GameOverScreen.Trigger(_gameOverVideo);
    }

    private void OnGUI()
    {
        if (MioritzaGame.Game.CutsceneManager.IsCutsceneActive == true) return;

        var level = CurrentLevel;
        if (level <= 0) return;

        var prev = GUI.color;

        // if (_showNauseaOverlay == true)
        // {
        //     var pulse = 0.5f + 0.5f * Mathf.Sin(Time.unscaledTime * 1.8f);
        //     var nauseaAlpha = Mathf.Lerp(0.08f, 0.32f, level / (float)_maxLevel) * (0.6f + 0.4f * pulse);
        //     GUI.color = new Color(0.55f, 0.0f, 0.05f, nauseaAlpha);
        //     GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), Texture2D.whiteTexture);

        //     var jitter = Mathf.Lerp(0f, 6f, level / (float)_maxLevel) * (pulse - 0.5f);
        //     var greenTint = new Color(0.0f, 0.4f, 0.15f, nauseaAlpha * 0.4f);
        //     GUI.color = greenTint;
        //     GUI.DrawTexture(new Rect(jitter, -jitter, Screen.width, Screen.height), Texture2D.whiteTexture);
        // }

        // var iconIndex = Mathf.Clamp(level - 1, 0, _levelIcons.Length - 1);
        // var icon = _levelIcons.Length > iconIndex ? _levelIcons[iconIndex] : null;
        // if (icon != null)
        // {
        //     GUI.color = Color.white;
        //     GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), icon, ScaleMode.StretchToFill, true);
        // }

        GUI.color = prev;
    }
}
