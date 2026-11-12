
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class GamePreferencesManager : MonoBehaviour
{
    public static bool isPaused = true;

    private bool isTransitioning = false;

    [Header("Pause Settings")]
    public TextMeshProUGUI pauseInstructionText;

    [Header("Max Ammo Settings")]
    public TMP_Dropdown maxAmmoDropdown;

    [Header("Reload Time Multipliers")]
    public TMP_Dropdown reloadTimeDropdown;
    public float currentReloadMultiplier = 0.5f;

    [Header("Scene Transition")]
    public TMP_Dropdown screenTranstion;

    void Start()
    {
        // Start each scene paused.
        SetPaused(true);

        // Register dropdown listeners.
        if (maxAmmoDropdown != null)
        {
            maxAmmoDropdown.onValueChanged.AddListener(
                OnMaxAmmoChanged
            );
        }

        if (reloadTimeDropdown != null)
        {
            reloadTimeDropdown.onValueChanged.AddListener(
                OnReloadTimeChanged
            );
        }

        if (screenTranstion != null)
        {
            screenTranstion.onValueChanged.AddListener(
                OnSceneSelected
            );

            SetCurrentSceneDropdown();
        }
        else
        {
            Debug.LogWarning(
                "GamePreferencesManager: Scene transition dropdown is not assigned."
            );
        }

        UpdateUI();

        Debug.Log(
            "GamePreferencesManager initialized. Game is paused."
        );
    }

    void OnDestroy()
    {
        // Remove dropdown listeners.
        if (maxAmmoDropdown != null)
        {
            maxAmmoDropdown.onValueChanged.RemoveListener(
                OnMaxAmmoChanged
            );
        }

        if (reloadTimeDropdown != null)
        {
            reloadTimeDropdown.onValueChanged.RemoveListener(
                OnReloadTimeChanged
            );
        }

        if (screenTranstion != null)
        {
            screenTranstion.onValueChanged.RemoveListener(
                OnSceneSelected
            );
        }
    }

    void Update()
    {
        // P or Escape toggles pause.
        if (Input.GetKeyDown(KeyCode.P) ||
            Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    // =========================================
    // PAUSE MANAGEMENT
    // =========================================

    public void TogglePause()
    {
        SetPaused(!isPaused);
    }

    public void SetPaused(bool pauseState)
    {
        isPaused = pauseState;

        Time.timeScale = isPaused ? 0f : 1f;

        if (isPaused)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        if (pauseInstructionText != null)
        {
            pauseInstructionText.text = isPaused
                ? "GAME PAUSED - Press P to Play"
                : "Press P to Pause";
        }

        Debug.Log(
            isPaused ? "Game Paused" : "Game Resumed"
        );
    }

    // =========================================
    // AMMUNITION SETTINGS
    // =========================================

    public void OnMaxAmmoChanged(int index)
    {
        int bulletCount = 2;

        if (index == 1)
            bulletCount = 5;
        else if (index == 2)
            bulletCount = 10;

        ManageWeapons2 weapons =
            FindAnyObjectByType<ManageWeapons2>();

        if (weapons != null)
        {
            weapons.UpdateWeaponStats(
                bulletCount,
                currentReloadMultiplier
            );
        }
    }

    // =========================================
    // RELOAD SETTINGS
    // =========================================

    public void OnReloadTimeChanged(int index)
    {
        if (index == 0)
            currentReloadMultiplier = 0.5f;
        else if (index == 1)
            currentReloadMultiplier = 1.0f;
        else if (index == 2)
            currentReloadMultiplier = 2.0f;

        if (maxAmmoDropdown != null)
        {
            OnMaxAmmoChanged(maxAmmoDropdown.value);
        }
    }

    // =========================================
    // SCENE DROPDOWN INITIALIZATION
    // =========================================

    private void SetCurrentSceneDropdown()
    {
        if (screenTranstion == null)
            return;

        string currentScene =
            SceneManager.GetActiveScene().name;

        Debug.Log(
            "Initializing dropdown for scene: " + currentScene
        );

        int sceneIndex = -1;

        // Find the current scene in the dropdown options.
        for (int i = 0;
             i < screenTranstion.options.Count;
             i++)
        {
            string option =
                screenTranstion.options[i].text.Trim();

            if (string.Equals(
                option,
                currentScene,
                System.StringComparison.OrdinalIgnoreCase))
            {
                sceneIndex = i;
                break;
            }
        }

        if (sceneIndex >= 0)
        {
            screenTranstion.SetValueWithoutNotify(sceneIndex);
            screenTranstion.RefreshShownValue();

            Debug.Log(
                "Dropdown set to index: " + sceneIndex
            );
        }
        else
        {
            Debug.LogWarning(
                "Could not find scene '" + currentScene +
                "' in the scene transition dropdown."
            );
        }
    }

    // =========================================
    // SCENE TRANSITIONS
    // =========================================


    public void OnSceneSelected(int index)
    {
        if (isTransitioning)
            return;

        Debug.Log("Scene dropdown selected index: " + index);

        string sceneToLoad = "";

        switch (index)
        {
            case 0:
                sceneToLoad = "gameLevel";
                break;

            case 1:
                sceneToLoad = "gameLevelAuto";
                break;

            case 2:
                sceneToLoad = "loseScene";
                break;

            case 3:
                sceneToLoad = "winScene";
                break;

            case 4:
                sceneToLoad = "offmesh";
                break;

            case 5:
                sceneToLoad = "scene paths";
                break;

            default:
                Debug.LogWarning("Invalid scene dropdown index: " + index);
                return;
        }

        // Don't reload the currently active scene.
        if (SceneManager.GetActiveScene().name == sceneToLoad)
        {
            Debug.Log("Scene is already active: " + sceneToLoad);
            return;
        }

        isTransitioning = true;

        StartCoroutine(LoadSceneRoutine(sceneToLoad));
    }

    private IEnumerator LoadSceneRoutine(string sceneToLoad)
    {
        Debug.Log("Preparing transition to: " + sceneToLoad);

        // Restore normal game state.
        Time.timeScale = 1f;
        isPaused = false;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Find the existing player.
        GameObject player = GameObject.Find("FPSController");

        // Fallback search in case the player has a different object name.
        if (player == null)
        {
            FirstPersonController fpsController =
                FindAnyObjectByType<FirstPersonController>();

            if (fpsController != null)
            {
                player = fpsController.gameObject;
            }
        }

        // Destroy the existing player on EVERY transition.
        if (player != null)
        {
            Debug.Log("Destroying FPSController before scene transition.");

            Destroy(player);
        }

        // Allow Unity to complete destruction before loading the next scene.
        yield return null;

        Debug.Log("Loading scene: " + sceneToLoad);

        SceneManager.LoadScene(sceneToLoad);
    }


    // =========================================
    // UI INITIALIZATION
    // =========================================

    private void UpdateUI()
    {
        if (maxAmmoDropdown != null)
        {
            OnMaxAmmoChanged(maxAmmoDropdown.value);
        }

        if (reloadTimeDropdown != null)
        {
            OnReloadTimeChanged(reloadTimeDropdown.value);
        }
    }

    // =========================================
    // QUIT GAME
    // =========================================

    public void QuitGame()
    {
        Debug.Log("Quitting game...");

        Application.Quit();
    }
}
