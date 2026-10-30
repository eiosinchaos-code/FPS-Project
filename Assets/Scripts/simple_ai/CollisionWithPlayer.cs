using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CollisionWithPlayer : MonoBehaviour
{
    private int score = 0;
    private float runningTime = 0f;
    private bool isPaused = true;

    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI timeText;
    public TextMeshProUGUI messageText;
    public TextMeshProUGUI pauseText;
    public TextMeshProUGUI intensityValueText;
    public TextMeshProUGUI spawnTimeValueText;

    public Slider intensitySlider;
    public Slider spawnTimeSlider;
    public TMP_Dropdown targetScoreDropdown;

    public static CollisionWithPlayer Instance { get; private set; }

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        score = 0;
        runningTime = 0f;
        isPaused = true;
        Time.timeScale = 0f;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (messageText != null) messageText.text = "";
        if (pauseText != null) pauseText.text = "Paused. Press 'P' to Start/Resume.";

        if (targetScoreDropdown != null)
        {
            targetScoreDropdown.onValueChanged.AddListener(OnDropdownValueChanged);
        }

        UpdateScoreUI();
        UpdateTimeUI();
        UpdateSettingsUI();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P)) TogglePause();
        UpdateSettingsUI();

        if (!isPaused)
        {
            runningTime += Time.deltaTime;
            UpdateTimeUI();
        }
    }

    public void TogglePause()
    {
        isPaused = !isPaused;
        Time.timeScale = isPaused ? 0f : 1f;

        if (isPaused)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        if (pauseText != null)
        {
            pauseText.text = isPaused ? "Paused. Press 'P' to Start/Resume." : "";
        }
    }

    void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            int targetScore = GetTargetScore();
            scoreText.text = "Score: " + score + " / " + targetScore;
        }
    }

    void UpdateTimeUI()
    {
        if (timeText != null)
        {
            int minutes = Mathf.FloorToInt(runningTime / 60f);
            int seconds = Mathf.FloorToInt(runningTime % 60f);
            timeText.text = string.Format("Time: {0:00}:{1:00}", minutes, seconds);
        }
    }

    void UpdateSettingsUI()
    {
        if (intensitySlider != null && intensityValueText != null)
        {
            intensityValueText.text = "Intensity: " + Mathf.RoundToInt(GetProjectileIntensity());
        }
        if (spawnTimeSlider != null && spawnTimeValueText != null)
        {
            spawnTimeValueText.text = "Spawn Time: " + Mathf.RoundToInt(GetSpawnTime()) + "s";
        }
        UpdateScoreUI();
    }

    public int GetTargetScore()
    {
        if (targetScoreDropdown != null && targetScoreDropdown.options.Count > 0)
        {
            string selectedText = targetScoreDropdown.options[targetScoreDropdown.value].text;
            string numericOnly = System.Text.RegularExpressions.Regex.Replace(selectedText, "[^0-9]", "");
            if (int.TryParse(numericOnly, out int parsedScore)) return parsedScore;
        }
        return 4;
    }

    public float GetProjectileIntensity()
    {
        if (intensitySlider != null)
        {
            float val = intensitySlider.value;
            return (val <= 10f) ? Mathf.Round(val) * 100f : Mathf.Round(val / 100f) * 100f;
        }
        return 1000f;
    }

    public float GetSpawnTime()
    {
        return intensitySlider != null ? Mathf.Clamp(Mathf.Round(spawnTimeSlider.value), 1f, 10f) : 2f;
    }

    public void OnDropdownValueChanged(int index)
    {
        UpdateSettingsUI();
    }

    // Public getters for the EndZone script to use
    public int GetCurrentScore() { return score; }

    public bool HasReachedTargetScore() { return score >= GetTargetScore(); }

    void OnTriggerEnter(Collider other)
    {
        // Only handles collectible pickups now
        if (other.gameObject.CompareTag("pick_me"))
        {
            Destroy(other.gameObject);
            score++;
            UpdateScoreUI();
            print("Score: " + score);
        }
    }
}