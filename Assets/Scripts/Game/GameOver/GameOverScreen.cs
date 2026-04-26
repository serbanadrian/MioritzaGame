using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;
using MioritzaGame.Constants;

namespace MioritzaGame.Game
{
    public sealed class GameOverScreen : MonoBehaviour
    {
        private static GameOverScreen _instance;

        public static bool IsActive => _instance != null && _instance._isPlaying == true;

        private VideoPlayer _videoPlayer;
        private AudioSource _audioSource;
        private Canvas _canvas;
        private RawImage _videoImage;
        private AspectRatioFitter _videoFitter;
        private RenderTexture _videoTarget;
        private Text _hintText;

        private bool _isPlaying;
        private bool _videoStarted;
        private float _startedTime;
        private float _previousTimeScale;

        private const string MAIN_MENU_SCENE = "MainMenu";

        public static void Trigger(VideoClip clip)
        {
            if (clip == null)
            {
                Debug.LogError($"{nameof(GameOverScreen)}.{nameof(Trigger)} called with null clip — loading main menu immediately.");
                SceneManager.LoadScene(MAIN_MENU_SCENE);
                return;
            }

            if (_instance == null)
            {
                var go = new GameObject(nameof(GameOverScreen));
                DontDestroyOnLoad(go);
                _instance = go.AddComponent<GameOverScreen>();
                _instance.BuildUI();
            }

            _instance.Begin(clip);
        }

        private void BuildUI()
        {
            Application.runInBackground = true;

            var canvasGo = new GameObject("GameOverCanvas");
            canvasGo.transform.SetParent(transform, false);
            _canvas = canvasGo.AddComponent<Canvas>();
            _canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            _canvas.sortingOrder = 33000;
            canvasGo.AddComponent<CanvasScaler>();
            canvasGo.AddComponent<GraphicRaycaster>();
            _canvas.gameObject.SetActive(false);

            var backdrop = CreateChild(canvasGo.transform, "Backdrop").AddComponent<RawImage>();
            backdrop.texture = Texture2D.whiteTexture;
            backdrop.color = Color.black;
            Stretch(backdrop.rectTransform);

            _videoImage = CreateChild(canvasGo.transform, "VideoImage").AddComponent<RawImage>();
            _videoImage.uvRect = new Rect(0f, 0f, 1f, 1f);
            _videoFitter = _videoImage.gameObject.AddComponent<AspectRatioFitter>();
            _videoFitter.aspectMode = AspectRatioFitter.AspectMode.FitInParent;
            _videoFitter.aspectRatio = 16f / 9f;
            Stretch(_videoImage.rectTransform);
            _videoImage.transform.localScale = Vector3.one;

            _hintText = CreateChild(canvasGo.transform, "Hint").AddComponent<Text>();
            _hintText.alignment = TextAnchor.LowerCenter;
            _hintText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            _hintText.fontSize = 14;
            _hintText.color = new Color(1f, 1f, 1f, 0.6f);
            _hintText.text = Texts.Cutscene.CONTINUE_HINT;
            var hintRect = _hintText.rectTransform;
            hintRect.anchorMin = new Vector2(0f, 0f);
            hintRect.anchorMax = new Vector2(1f, 0f);
            hintRect.pivot = new Vector2(0.5f, 0f);
            hintRect.anchoredPosition = new Vector2(0f, 14f);
            hintRect.sizeDelta = new Vector2(0f, 30f);

            _audioSource = gameObject.AddComponent<AudioSource>();
            _audioSource.playOnAwake = false;
            _audioSource.spatialBlend = 0f;
            _audioSource.ignoreListenerPause = true;

            _videoPlayer = gameObject.AddComponent<VideoPlayer>();
            _videoPlayer.playOnAwake = false;
            _videoPlayer.waitForFirstFrame = false;
            _videoPlayer.renderMode = VideoRenderMode.RenderTexture;
            _videoPlayer.audioOutputMode = VideoAudioOutputMode.Direct;

            EnsureRenderTexture(1920, 1080);
            _videoPlayer.targetTexture = _videoTarget;
            _videoImage.texture = _videoTarget;

            _videoPlayer.prepareCompleted += OnVideoPrepared;
            _videoPlayer.errorReceived += OnVideoError;
        }

        private static GameObject CreateChild(Transform parent, string name)
        {
            var go = new GameObject(name, typeof(RectTransform));
            go.transform.SetParent(parent, false);
            return go;
        }

        private static void Stretch(RectTransform rect)
        {
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
        }

        private void EnsureRenderTexture(int w, int h)
        {
            if (_videoTarget != null && _videoTarget.width == w && _videoTarget.height == h) return;
            if (_videoTarget != null) _videoTarget.Release();
            _videoTarget = new RenderTexture(w, h, 0);
            _videoTarget.Create();
        }

        private void OnVideoError(VideoPlayer source, string message)
        {
            Debug.LogError($"{nameof(GameOverScreen)} VideoPlayer error: {message}");
        }

        private void OnVideoPrepared(VideoPlayer source)
        {
            var w = (int)source.width;
            var h = (int)source.height;
            if (w <= 0 || h <= 0) return;

            if (_videoTarget == null || _videoTarget.width != w || _videoTarget.height != h)
            {
                EnsureRenderTexture(w, h);
                source.targetTexture = _videoTarget;
                _videoImage.texture = _videoTarget;
            }
            _videoFitter.aspectRatio = (float)w / h;
        }

        private void Begin(VideoClip clip)
        {
            if (_isPlaying == true) return;

            _isPlaying = true;
            _videoStarted = false;
            _startedTime = Time.unscaledTime;

            _previousTimeScale = Time.timeScale;
            Time.timeScale = 0f;
            AudioListener.pause = true;

            _canvas.gameObject.SetActive(true);
            _videoPlayer.clip = clip;
            _videoPlayer.time = 0;
            _videoFitter.aspectRatio = (float)clip.width / Mathf.Max(1, (int)clip.height);
            _videoPlayer.Play();
        }

        private void End()
        {
            if (_isPlaying == false) return;

            if (_videoPlayer != null && _videoPlayer.isPlaying == true) _videoPlayer.Stop();
            if (_audioSource != null) _audioSource.Stop();
            _canvas.gameObject.SetActive(false);
            _isPlaying = false;

            Time.timeScale = _previousTimeScale <= 0f ? 1f : _previousTimeScale;
            AudioListener.pause = false;

            SceneManager.LoadScene(MAIN_MENU_SCENE);
        }

        private void Update()
        {
            if (_isPlaying == false) return;

            var elapsed = Time.unscaledTime - _startedTime;
            var advance = false;

            if (_videoPlayer.isPlaying == true) _videoStarted = true;
            if (_videoStarted == false && _videoPlayer.isPlaying == false && elapsed < 2f)
            {
                _videoPlayer.time = 0;
                _videoPlayer.Play();
            }
            if (_videoStarted == true && _videoPlayer.isPlaying == false && elapsed > 0.5f) advance = true;

            if (Input.GetKeyDown(KeyCode.Space) == true) advance = true;
            if (Input.GetKeyDown(KeyCode.Return) == true) advance = true;
            if (Input.GetKeyDown(KeyCode.Escape) == true) advance = true;
            if (Input.GetMouseButtonDown(0) == true) advance = true;

            if (advance == true) End();
        }

        private void OnDestroy()
        {
            if (_videoPlayer != null)
            {
                _videoPlayer.prepareCompleted -= OnVideoPrepared;
                _videoPlayer.errorReceived -= OnVideoError;
            }
            if (_videoTarget != null) _videoTarget.Release();
        }
    }
}
