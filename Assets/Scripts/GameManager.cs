using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("UI Elements")]
    public TMP_Text pauseIndicatorText;
    public TMP_Text healthText;
    public TMP_Text maxAmmoText;

    [Header("Dropdown References")]
    public TMP_Dropdown maxAmmoDropdown;
    public TMP_Dropdown reloadDropdown;

    [Header("Player Control Reference")]
    public MonoBehaviour playerMovementScript;

    [Header("Player Stats")]
    public int playerHealth = 100;
    private bool isPaused = true;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        Time.timeScale = 0f;
        isPaused = true;
        ApplyPauseState();
        UpdatePauseUI();
        UpdateHealthUI();

        // Set default dropdown selections to the lowest option (index 0)
        if (maxAmmoDropdown != null)
        {
            maxAmmoDropdown.value = 0;
            maxAmmoDropdown.onValueChanged.AddListener(OnMaxAmmoDropdownChanged);
        }

        if (reloadDropdown != null)
        {
            reloadDropdown.value = 0;
            reloadDropdown.onValueChanged.AddListener(OnReloadTimeDropdownChanged);
        }

        // Initialize weapon stats to match the lowest defaults immediately
        ManageWeapons weapons = FindAnyObjectByType<ManageWeapons>();
        if (weapons != null)
        {
            weapons.SetExactMaxAmmo(2); // Lowest max ammo (2 rounds)
            weapons.SetReloadTimeModifier(0.5f); // Lowest reload time (Short)
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            isPaused = !isPaused;
            Time.timeScale = isPaused ? 0f : 1f;
            ApplyPauseState();
            UpdatePauseUI();
        }
    }

    void ApplyPauseState()
    {
        if (isPaused)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            if (playerMovementScript != null)
            {
                playerMovementScript.enabled = false;
            }
        }
        else
        {
            if (playerMovementScript != null)
            {
                playerMovementScript.enabled = true;
            }

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    void UpdatePauseUI()
    {
        if (pauseIndicatorText != null)
        {
            pauseIndicatorText.text = isPaused ? "Game Paused. Press [P] to Resume" : "Press [P] to Pause";
        }
    }

    public void UpdateHealthUI()
    {
        if (healthText != null)
        {
            healthText.text = "Health: " + playerHealth;
        }
    }

    public void OnMaxAmmoDropdownChanged(int index)
    {
        Debug.Log("Max Ammo Dropdown changed to index: " + index);

        int maxRounds = 2; // Index 0 = 2 rounds
        if (index == 1) maxRounds = 5;       // Index 1 = 5 rounds
        else if (index == 2) maxRounds = 10; // Index 2 = 10 rounds

        ManageWeapons weapons = FindAnyObjectByType<ManageWeapons>();
        if (weapons != null)
        {
            weapons.SetExactMaxAmmo(maxRounds);
        }
    }

    public void OnReloadTimeDropdownChanged(int index)
    {
        Debug.Log("Reload Time Dropdown changed to index: " + index);

        float modifier = 1.0f;
        if (index == 0) modifier = 0.5f;   // Short (Fast)
        else if (index == 1) modifier = 1.0f; // Medium (Normal)
        else if (index == 2) modifier = 2.0f; // Long (Slow)

        ManageWeapons weapons = FindAnyObjectByType<ManageWeapons>();
        if (weapons != null)
        {
            weapons.SetReloadTimeModifier(modifier);
        }
    }
}