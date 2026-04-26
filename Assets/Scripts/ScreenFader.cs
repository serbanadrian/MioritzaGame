using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScreenFader : MonoBehaviour
{
    private static ScreenFader _instance;

    public static ScreenFader Instance
    {
        get
        {
            EnsureExists();
            return _instance;
        }
    }

    private float _alpha = 1f;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Init()
    {
        EnsureExists();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private static void EnsureExists()
    {
        if (_instance != null) return;
        var go = new GameObject("ScreenFader");
        DontDestroyOnLoad(go);
        _instance = go.AddComponent<ScreenFader>();
        _instance._alpha = 1f;
    }

    private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (_instance == null) return;
        _instance._alpha = 1f;
        _instance.StopAllCoroutines();
        _instance.StartCoroutine(_instance.WaitForCutsceneAndFadeIn(0.6f));
    }

    private IEnumerator WaitForCutsceneAndFadeIn(float duration)
    {
        yield return null;
        yield return null;
        if (MioritzaGame.Game.CutsceneManager.IsCutsceneActive == true) yield break;
        yield return FadeRoutine(_alpha, 0f, duration);
    }

    public void FadeFromBlack(float duration)
    {
        StopAllCoroutines();
        StartCoroutine(FadeRoutine(_alpha, 0f, duration));
    }

    public void FadeToBlack(float duration)
    {
        StopAllCoroutines();
        StartCoroutine(FadeRoutine(_alpha, 1f, duration));
    }

    public void TransitionTo(System.Action atBlack, float halfDuration = 0.18f)
    {
        StopAllCoroutines();
        StartCoroutine(TransitionRoutine(atBlack, halfDuration));
    }

    private IEnumerator TransitionRoutine(System.Action atBlack, float halfDuration)
    {
        yield return FadeRoutine(_alpha, 1f, halfDuration);
        atBlack?.Invoke();
        yield return FadeRoutine(1f, 0f, halfDuration);
    }

    private IEnumerator FadeRoutine(float from, float to, float duration)
    {
        if (duration <= 0f)
        {
            _alpha = to;
            yield break;
        }
        var t = 0f;
        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            _alpha = Mathf.Lerp(from, to, t / duration);
            yield return null;
        }
        _alpha = to;
    }

    private void OnGUI()
    {
        if (_alpha <= 0f) return;

        var prev = GUI.color;
        GUI.color = new Color(0f, 0f, 0f, Mathf.Clamp01(_alpha));
        GUI.depth = -2000;
        GUI.DrawTexture(new Rect(0f, 0f, Screen.width, Screen.height), Texture2D.whiteTexture);
        GUI.color = prev;
    }
}
