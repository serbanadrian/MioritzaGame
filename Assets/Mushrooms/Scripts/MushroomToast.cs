using UnityEngine;

public class MushroomToast : MonoBehaviour
{
    public string Title;
    public string Body;
    public Color Tint = Color.white;
    public float Duration = 4f;

    private float _start;

    public static void Show(string title, string body, Color tint, float duration = 4f)
    {
        var go = new GameObject("MushroomToast");
        DontDestroyOnLoad(go);
        var t = go.AddComponent<MushroomToast>();
        t.Title = title;
        t.Body = body;
        t.Tint = tint;
        t.Duration = duration;
    }

    public static void Show(string title, Color tint, float duration = 2f)
        => Show(title, string.Empty, tint, duration);

    private float _activeElapsed;

    private void Awake() => _start = Time.unscaledTime;

    private void Update()
    {
        if (MioritzaGame.Game.CutsceneManager.IsCutsceneActive == true) return;
        _activeElapsed += Time.unscaledDeltaTime;
        if (_activeElapsed >= Duration) Destroy(gameObject);
    }

    private void OnGUI()
    {
        if (MioritzaGame.Game.CutsceneManager.IsCutsceneActive == true) return;

        var elapsed = _activeElapsed;
        var alpha = Mathf.Clamp01(1f - elapsed / Duration);
        var hasBody = string.IsNullOrEmpty(Body) == false;

        var width = Mathf.Min(Screen.width - 80f, 900f);
        var titleStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = 26,
            fontStyle = FontStyle.Bold,
            alignment = TextAnchor.MiddleCenter,
            wordWrap = true
        };
        titleStyle.normal.textColor = Tint;

        var bodyStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = 18,
            alignment = TextAnchor.MiddleCenter,
            wordWrap = true
        };

        var titleSize = titleStyle.CalcSize(new GUIContent(Title));
        var titleHeight = titleStyle.CalcHeight(new GUIContent(Title), width - 40f);
        var bodyHeight = hasBody ? bodyStyle.CalcHeight(new GUIContent(Body), width - 40f) : 0f;
        var totalHeight = titleHeight + bodyHeight + (hasBody ? 16f : 0f) + 32f;

        var x = (Screen.width - width) / 2f;
        var y = 60f;

        var prev = GUI.color;
        GUI.color = new Color(0f, 0f, 0f, alpha * 0.65f);
        GUI.DrawTexture(new Rect(x, y, width, totalHeight), Texture2D.whiteTexture);

        var titleColor = Tint; titleColor.a = alpha;
        titleStyle.normal.textColor = titleColor;
        GUI.color = Color.white;
        GUI.Label(new Rect(x + 20f, y + 16f, width - 40f, titleHeight), Title, titleStyle);

        if (hasBody == true)
        {
            var bodyColor = new Color(1f, 1f, 1f, alpha);
            bodyStyle.normal.textColor = bodyColor;
            GUI.Label(new Rect(x + 20f, y + 16f + titleHeight + 8f, width - 40f, bodyHeight), Body, bodyStyle);
        }

        GUI.color = prev;
    }
}
