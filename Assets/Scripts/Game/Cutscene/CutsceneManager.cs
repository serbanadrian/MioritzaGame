using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;
using MioritzaGame.Constants;

namespace MioritzaGame.Game
{
    public sealed class CutsceneManager : MonoBehaviour
    {
        [SerializeField] private CutsceneConfiguration _configuration;
        [SerializeField] private bool _playOnStart = true;
        [SerializeField] private string _nextSceneOnEnd;

        public static bool IsCutsceneActive { get; private set; }

        private int _currentStep;
        private bool _isPlaying;
        private bool _videoStarted;
        private float _stepStartedTime;
        private VideoPlayer _videoPlayer;
        private AudioSource _audioSource;
        private Canvas _canvas;
        private RawImage _videoImage;
        private RawImage _imageImage;
        private Text _captionText;
        private Text _hintText;
        private AspectRatioFitter _videoFitter;
        private AspectRatioFitter _imageFitter;
        private RenderTexture _videoTarget;

        private void Awake()
        {
            Application.runInBackground = true;
            BuildUI();
            BuildVideoPlayer();
        }

        private void BuildUI()
        {
            var canvasGo = new GameObject("CutsceneCanvas");
            canvasGo.transform.SetParent(transform, false);
            _canvas = canvasGo.AddComponent<Canvas>();
            _canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            _canvas.sortingOrder = 32000;
            canvasGo.AddComponent<CanvasScaler>();
            canvasGo.AddComponent<GraphicRaycaster>();
            _canvas.gameObject.SetActive(false);

            var backdrop = CreateChild(canvasGo.transform, "Backdrop").AddComponent<RawImage>();
            backdrop.texture = Texture2D.whiteTexture;
            backdrop.color = Color.black;
            Stretch(backdrop.rectTransform);
            backdrop.transform.localScale = new Vector3(1.05f, 1.05f, 1f);

            _videoImage = CreateChild(canvasGo.transform, "VideoImage").AddComponent<RawImage>();
            _videoImage.uvRect = new Rect(0f, 0f, 1f, 1f);
            _videoFitter = _videoImage.gameObject.AddComponent<AspectRatioFitter>();
            _videoFitter.aspectMode = AspectRatioFitter.AspectMode.FitInParent;
            _videoFitter.aspectRatio = 16f / 9f;
            Stretch(_videoImage.rectTransform);
            _videoImage.transform.localScale = Vector3.one;

            _imageImage = CreateChild(canvasGo.transform, "Image").AddComponent<RawImage>();
            _imageImage.uvRect = new Rect(0.005f, 0.005f, 0.99f, 0.99f);
            _imageFitter = _imageImage.gameObject.AddComponent<AspectRatioFitter>();
            _imageFitter.aspectMode = AspectRatioFitter.AspectMode.EnvelopeParent;
            _imageFitter.aspectRatio = 16f / 9f;
            Stretch(_imageImage.rectTransform);
            _imageImage.transform.localScale = new Vector3(1.02f, 1.02f, 1f);
            _imageImage.gameObject.SetActive(false);

            _captionText = CreateChild(canvasGo.transform, "Caption").AddComponent<Text>();
            _captionText.alignment = TextAnchor.LowerCenter;
            _captionText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            _captionText.fontSize = 22;
            _captionText.fontStyle = FontStyle.Bold;
            _captionText.color = Color.white;
            var captionRect = _captionText.rectTransform;
            captionRect.anchorMin = new Vector2(0.1f, 0f);
            captionRect.anchorMax = new Vector2(0.9f, 0.18f);
            captionRect.offsetMin = Vector2.zero;
            captionRect.offsetMax = Vector2.zero;

            _hintText = CreateChild(canvasGo.transform, "Hint").AddComponent<Text>();
            _hintText.alignment = TextAnchor.LowerCenter;
            _hintText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            _hintText.fontSize = 14;
            _hintText.color = new Color(1f, 1f, 1f, 0.6f);
            _hintText.text = Texts.Cutscene.HINT;
            var hintRect = _hintText.rectTransform;
            hintRect.anchorMin = new Vector2(0f, 0f);
            hintRect.anchorMax = new Vector2(1f, 0f);
            hintRect.pivot = new Vector2(0.5f, 0f);
            hintRect.anchoredPosition = new Vector2(0f, 12f);
            hintRect.sizeDelta = new Vector2(0f, 30f);
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

        private void BuildVideoPlayer()
        {
            _audioSource = GetComponent<AudioSource>();
            if (_audioSource == null) _audioSource = gameObject.AddComponent<AudioSource>();
            _audioSource.playOnAwake = false;
            _audioSource.spatialBlend = 0f;

            _videoPlayer = GetComponent<VideoPlayer>();
            if (_videoPlayer == null) _videoPlayer = gameObject.AddComponent<VideoPlayer>();
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

        private void OnVideoError(VideoPlayer source, string message)
        {
            Debug.LogError($"{nameof(CutsceneManager)} VideoPlayer error: {message}");
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

        private void EnsureRenderTexture(int w, int h)
        {
            if (_videoTarget != null && _videoTarget.width == w && _videoTarget.height == h) return;
            if (_videoTarget != null) _videoTarget.Release();
            _videoTarget = new RenderTexture(w, h, 0);
            _videoTarget.Create();
        }

        private void Start()
        {
            if (_configuration == null) return;
            if (_configuration.Enabled == false) return;
            if (_playOnStart == false) return;
            if (_configuration.Steps == null || _configuration.Steps.Length == 0) return;

            BeginCutscene();
        }

        private void BeginCutscene()
        {
            _isPlaying = true;
            IsCutsceneActive = true;
            _currentStep = -1;
            _canvas.gameObject.SetActive(true);
            EnterStep(0);
            StartCoroutine(FadeOutBlackAfterDelay());
        }

        private System.Collections.IEnumerator FadeOutBlackAfterDelay()
        {
            yield return new WaitForSecondsRealtime(1f);
            if (ScreenFader.Instance != null) ScreenFader.Instance.FadeFromBlack(0.6f);
        }

        private void EnterStep(int index)
        {
            if (_videoPlayer.isPlaying == true) _videoPlayer.Stop();

            _currentStep = index;
            _stepStartedTime = Time.unscaledTime;

            if (index < 0 || index >= _configuration.Steps.Length) return;

            var step = _configuration.Steps[index];

            _captionText.text = string.IsNullOrEmpty(step._caption) == true ? string.Empty : step._caption;

            if (step._video != null)
            {
                _videoImage.gameObject.SetActive(true);
                _imageImage.gameObject.SetActive(false);
                _videoPlayer.clip = step._video;
                _videoPlayer.time = 0;
                _videoFitter.aspectRatio = (float)step._video.width / Mathf.Max(1, (int)step._video.height);
                _videoStarted = false;
                _videoPlayer.Play();
            }
            else
            {
                _videoImage.gameObject.SetActive(false);
                if (step._image != null)
                {
                    _imageImage.gameObject.SetActive(true);
                    _imageImage.texture = step._image.texture;
                    _imageFitter.aspectRatio = (float)step._image.texture.width / Mathf.Max(1, step._image.texture.height);
                }
                else
                {
                    _imageImage.gameObject.SetActive(false);
                }
            }
        }

        private void EndCutscene()
        {
            if (_videoPlayer.isPlaying == true) _videoPlayer.Stop();
            _videoPlayer.clip = null;
            if (_audioSource != null) _audioSource.Stop();
            _canvas.gameObject.SetActive(false);
            _isPlaying = false;
            IsCutsceneActive = false;
            Time.timeScale = 1f;

            if (string.IsNullOrEmpty(_nextSceneOnEnd) == false)
            {
                SceneManager.LoadScene(_nextSceneOnEnd);
            }
        }

        private void Update()
        {
            if (_isPlaying == false) return;

            if (Input.GetKeyDown(KeyCode.Escape) == true)
            {
                EndCutscene();
                return;
            }

            var step = _configuration.Steps[_currentStep];
            var elapsed = Time.unscaledTime - _stepStartedTime;
            var advance = false;

            if (step._video != null)
            {
                if (_videoPlayer.isPlaying == true) _videoStarted = true;
                if (_videoStarted == false && _videoPlayer.isPlaying == false && elapsed < 2f)
                {
                    _videoPlayer.time = 0;
                    _videoPlayer.Play();
                }
                if (_videoStarted == true && _videoPlayer.isPlaying == false && elapsed > 0.5f)
                    advance = true;
            }
            else if (step._duration > 0f && elapsed >= step._duration)
            {
                advance = true;
            }

            if (Input.GetKeyDown(KeyCode.Space) == true) advance = true;
            if (Input.GetKeyDown(KeyCode.Return) == true) advance = true;
            if (Input.GetMouseButtonDown(0) == true) advance = true;

            if (advance == true)
            {
                var next = _currentStep + 1;
                if (next >= _configuration.Steps.Length) EndCutscene();
                else EnterStep(next);
            }
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
