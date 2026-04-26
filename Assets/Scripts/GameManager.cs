using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace MioritzaGame
{
    public class GameManager : MonoBehaviour
    {
        [Header("Buttons")]
        [SerializeField] private Button playBtn;
        [SerializeField] private Button creditsBtn;
        [SerializeField] private Button settingsBtn;
        [SerializeField] private Button exitBtn;

        [Header("Canvases (Assign in Inspector)")]
        [SerializeField] private GameObject mainCanvas;
        [SerializeField] private GameObject creditsCanvas;
        [SerializeField] private GameObject settingsCanvas;

        [Header("Credits Scrolling")]
        [SerializeField] private RectTransform creditsTextTransform;
        [SerializeField] private float scrollSpeed = 50f;

        [Header("Settings - Audio")]
        [SerializeField] private Slider musicSlider;
        [SerializeField] private Slider volumeSlider;

        private Vector2 initialCreditsPosition;
        private bool isScrollingCredits = false;

        void Start()
        {
            // Save the starting position of the credits text
            if (creditsTextTransform != null)
            {
                initialCreditsPosition = creditsTextTransform.anchoredPosition;
            }

            // Assign button click events
            if (playBtn != null) playBtn.onClick.AddListener(OnPlayPressed);
            if (creditsBtn != null) creditsBtn.onClick.AddListener(OnCreditsPressed);
            if (settingsBtn != null) settingsBtn.onClick.AddListener(OnSettingsPressed);
            if (exitBtn != null) exitBtn.onClick.AddListener(OnExitPressed);

            // Load and set volume settings
            if (musicSlider != null)
            {
                // Default value is 1 (max volume) if not found in PlayerPrefs
                musicSlider.value = MioritzaGame.Audio.AudioSettings.GetMusicVolume();
                musicSlider.onValueChanged.AddListener((v) => MioritzaGame.Audio.AudioSettings.SetMusicVolume(v));
            }

            if (volumeSlider != null)
            {
                // Default value is 1 (max volume) if not found in PlayerPrefs
                volumeSlider.value = MioritzaGame.Audio.AudioSettings.GetMasterVolume();
                volumeSlider.onValueChanged.AddListener((v) => MioritzaGame.Audio.AudioSettings.SetMasterVolume(v));
            }

            // Make sure only the main canvas is active at start
            ShowCanvas(mainCanvas);
        }

        private void OnPlayPressed()
        {
            // You can add scene loading here, e.g., SceneManager.LoadScene(1);
            Debug.Log("Play Button Pressed");
            SceneManager.LoadScene("Level_01");
        }

        private void OnCreditsPressed()
        {
            ShowCanvas(creditsCanvas);

            // Reset position and start scrolling
            if (creditsTextTransform != null)
            {
                creditsTextTransform.anchoredPosition = initialCreditsPosition;
                isScrollingCredits = true;
            }
        }

        private void OnSettingsPressed()
        {
            ShowCanvas(settingsCanvas);
        }

        private void OnExitPressed()
        {
            Debug.Log("Exit Button Pressed");
            Application.Quit();
        }

        void Update()
        {
            if (isScrollingCredits && creditsTextTransform != null)
            {
                // Moves the text up on the Y axis consistently regardless of frame rate
                creditsTextTransform.anchoredPosition += Vector2.up * scrollSpeed * Time.deltaTime;
            }
        }

        /// <summary>
        /// Hides all assigned canvases except the one you want to show
        /// </summary>
        private void ShowCanvas(GameObject canvasToShow)
        {
            if (mainCanvas != null) mainCanvas.SetActive(canvasToShow == mainCanvas);
            if (creditsCanvas != null) creditsCanvas.SetActive(canvasToShow == creditsCanvas);
            if (settingsCanvas != null) settingsCanvas.SetActive(canvasToShow == settingsCanvas);

            // Stop scrolling if we leave the credits canvas
            if (canvasToShow != creditsCanvas)
            {
                isScrollingCredits = false;
            }
        }

        /// <summary>
        /// Useful if your "Back" buttons on other canvases need an OnClick method
        /// </summary>
        public void ShowMainCanvas()
        {
            ShowCanvas(mainCanvas);
        }

        // --- Audio Settings Methods --- //

        // These methods are preserved for compatibility but now delegate to AudioSettings events
        public void SetMusicVolume(float volume)
        {
            MioritzaGame.Audio.AudioSettings.SetMusicVolume(volume);
        }

        public void SetMasterVolume(float volume)
        {
            MioritzaGame.Audio.AudioSettings.SetMasterVolume(volume);
        }
    }
}
