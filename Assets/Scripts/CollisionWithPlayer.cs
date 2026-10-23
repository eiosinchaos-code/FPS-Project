using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CollisionWithPlayer : MonoBehaviour
{
    private int score = 0;
    private float runningTime = 0f;
    private bool isPaused = true;

    // UI References (TextMeshPro)
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI timeText;
    public TextMeshProUGUI messageText;
    public TextMeshProUGUI pauseText;

    // Live Settings Value Text References
    public TextMeshProUGUI intensityValueText;
    public TextMeshProUGUI spawnTimeValueText;

    // Sliders & Dropdown
    public Slider intensitySlider;
    public Slider spawnTimeSlider;
    public TMP_Dropdown targetScoreDropdown;

    // Static instance for easy access from other scripts
    public static CollisionWithPlayer Instance { get; private set; }

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        score = 0;
        runningTime = 0f;

        // Game starts in a paused state
        isPaused = true;
        Time.timeScale = 0f;

        if (messageText != null) messageText.text = "";
        if (pauseText != null) pauseText.text = "Paused. Press 'P' to Start/Resume.";

        UpdateScoreUI();
        UpdateTimeUI();
        UpdateSettingsUI();
    }

    void Update()
    {
        // Toggle Pause on 'P' key
        if (Input.GetKeyDown(KeyCode.P))
        {
            TogglePause();
        }

        // Update settings UI live while adjusting sliders (even when paused)
        UpdateSettingsUI();

        // Running time only increments when the game is unpaused
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
        // Live update for Intensity display
        if (intensitySlider != null && intensityValueText != null)
        {
            int intensityVal = Mathf.RoundToInt(GetProjectileIntensity());
            intensityValueText.text = "Intensity: " + intensityVal;
        }

        // Live update for Spawn Time display
        if (spawnTimeSlider != null && spawnTimeValueText != null)
        {
            int spawnVal = Mathf.RoundToInt(GetSpawnTime());
            spawnTimeValueText.text = "Spawn Time: " + spawnVal + "s";
        }

        UpdateScoreUI();
    }

    public int GetTargetScore()
    {
        if (targetScoreDropdown != null && targetScoreDropdown.options.Count > 0)
        {
            string selectedText = targetScoreDropdown.options[targetScoreDropdown.value].text;
            if (int.TryParse(selectedText, out int parsedScore))
            {
                return parsedScore;
            }
        }
        return 4; // Default fallback target score
    }

    // Scales 1-10 slider value to 100-1000 intensity range
    public float GetProjectileIntensity()
    {
        if (intensitySlider != null)
        {
            float val = intensitySlider.value;
            if (val <= 10f)
            {
                return Mathf.Round(val) * 100f;
            }
            return Mathf.Round(val / 100f) * 100f;
        }
        return 1000f;
    }

    // Clean 1 to 10 seconds range
    public float GetSpawnTime()
    {
        if (spawnTimeSlider != null)
        {
            return Mathf.Clamp(Mathf.Round(spawnTimeSlider.value), 1f, 10f);
        }
        return 2f;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.gameObject.CompareTag("pick_me"))
        {
            Destroy(collision.collider.gameObject);
            score++;
            UpdateScoreUI();
            print("Score: " + score);
        }

        if (collision.collider.gameObject.name == "end" && score >= GetTargetScore())
        {
            print("Congratulations!");
            if (messageText != null)
            {
                messageText.text = "Congratulations!";
            }
            Time.timeScale = 0f; // Pause game on completion
        }
    }
}