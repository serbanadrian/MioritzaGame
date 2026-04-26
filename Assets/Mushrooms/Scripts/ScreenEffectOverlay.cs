using UnityEngine;

public class ScreenEffectOverlay : MonoBehaviour
{
    public Color TintColor = new Color(0.6f, 0.2f, 0.6f, 0.35f);
    public bool ShowVignette = true;
    public bool ShowGrain;
    public float Duration = 6f;

    private float _start;

    public static ScreenEffectOverlay Show(Color tint, float duration, bool vignette, bool grain)
    {
        var go = new GameObject("ScreenEffectOverlay");
        var o = go.AddComponent<ScreenEffectOverlay>();
        o.TintColor = tint;
        o.Duration = duration;
        o.ShowVignette = false;
        o.ShowGrain = grain;
        return o;
    }

    private void Awake() => _start = Time.unscaledTime;

    private void Update()
    {
        if (Time.unscaledTime - _start >= Duration) Destroy(gameObject);
    }

    private void OnGUI()
    {
        if (MioritzaGame.Game.CutsceneManager.IsCutsceneActive == true) return;

        var elapsed = Time.unscaledTime - _start;
        var fade = Mathf.Clamp01(1f - elapsed / Duration);

        var prev = GUI.color;
        var tint = TintColor;
        tint.a *= fade;
        GUI.color = tint;
        GUI.depth = -1000;
        GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), Texture2D.whiteTexture, ScaleMode.StretchToFill, true);

        if (ShowVignette == true)
        {
            var vColor = new Color(0f, 0f, 0f, 0.35f * fade);
            GUI.color = vColor;
            var thickness = Mathf.Min(Screen.width, Screen.height) * 0.08f;
            GUI.DrawTexture(new Rect(0, 0, Screen.width, thickness), Texture2D.whiteTexture);
            GUI.DrawTexture(new Rect(0, Screen.height - thickness, Screen.width, thickness), Texture2D.whiteTexture);
            GUI.DrawTexture(new Rect(0, 0, thickness, Screen.height), Texture2D.whiteTexture);
            GUI.DrawTexture(new Rect(Screen.width - thickness, 0, thickness, Screen.height), Texture2D.whiteTexture);
        }

        if (ShowGrain == true)
        {
            var grainColor = new Color(1f, 1f, 1f, 0.06f * fade);
            GUI.color = grainColor;
            for (var i = 0; i < 200; i++)
            {
                var x = Random.Range(0, Screen.width);
                var y = Random.Range(0, Screen.height);
                GUI.DrawTexture(new Rect(x, y, 2, 2), Texture2D.whiteTexture);
            }
        }

        GUI.color = prev;
    }
}
