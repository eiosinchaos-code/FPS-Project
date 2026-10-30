
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GamePreferencesManager : MonoBehaviour
{
    public static bool isPaused = true;

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
        SetPaused(true);

        if (maxAmmoDropdown != null)
            maxAmmoDropdown.onValueChanged.AddListener(OnMaxAmmoChanged);

        if (reloadTimeDropdown != null)
            reloadTimeDropdown.onValueChanged.AddListener(OnReloadTimeChanged);

        if (screenTranstion != null)
        {
            screenTranstion.onValueChanged.AddListener(OnSceneSelected);
            SetCurrentSceneDropdown();
        }
        else
        {
            Debug.LogWarning(
                "GamePreferencesManager: screenTranstion dropdown is not assigned."
            );
        }

        UpdateUI();
    }

    void OnDestroy()
    {
        if (maxAmmoDropdown != null)
            maxAmmoDropdown.onValueChanged.RemoveListener(OnMaxAmmoChanged);

        if (reloadTimeDropdown != null)
            reloadTimeDropdown.onValueChanged.RemoveListener(OnReloadTimeChanged);

        if (screenTranstion != null)
            screenTranstion.onValueChanged.RemoveListener(OnSceneSelected);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P) ||
            Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

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
    }

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

    public void OnReloadTimeChanged(int index)
    {
        if (index == 0)
            currentReloadMultiplier = 0.5f;
        else if (index == 1)
            currentReloadMultiplier = 1.0f;
        else if (index == 2)
            currentReloadMultiplier = 2.0f;

        if (maxAmmoDropdown != null)
            OnMaxAmmoChanged(maxAmmoDropdown.value);
    }

    private void SetCurrentSceneDropdown()
    {
        string currentScene =
            SceneManager.GetActiveScene().name;

        int sceneIndex = -1;

        switch (currentScene)
        {
            case "gameLevel":
                sceneIndex = 0;
                break;

            case "gameLevelAuto":
                sceneIndex = 1;
                break;

            case "loseScene":
                sceneIndex = 2;
                break;

            case "winScene":
                sceneIndex = 3;
                break;
        }

        if (sceneIndex >= 0 &&
            sceneIndex < screenTranstion.options.Count)
        {
            screenTranstion.SetValueWithoutNotify(sceneIndex);
            screenTranstion.RefreshShownValue();
        }
        else
        {
            Debug.LogWarning(
                "Current scene '" + currentScene +
                "' is not mapped to a valid dropdown option."
            );
        }
    }

    public void OnSceneSelected(int index)
    {
        Debug.Log("Scene dropdown selected index: " + index);

        string sceneToLoad;

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

            default:
                Debug.LogWarning(
                    "Invalid scene dropdown index: " + index
                );
                return;
        }

        // Don't reload the currently active scene.
        if (SceneManager.GetActiveScene().name == sceneToLoad)
        {
            Debug.Log("Scene is already active: " + sceneToLoad);
            return;
        }

        Debug.Log("Loading scene: " + sceneToLoad);

        // Restore normal time before changing scenes.
        Time.timeScale = 1f;
        isPaused = false;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Remove the persistent gameplay player when entering
        // the Win or Lose screens.
        if (sceneToLoad == "loseScene" ||
            sceneToLoad == "winScene")
        {
            GameObject player =
                GameObject.Find("FPSController");

            if (player != null)
            {
                Debug.Log("Destroying persistent FPSController.");
                Destroy(player);
            }
        }

        SceneManager.LoadScene(sceneToLoad);
    }

    private void UpdateUI()
    {
        if (maxAmmoDropdown != null)
            OnMaxAmmoChanged(maxAmmoDropdown.value);

        if (reloadTimeDropdown != null)
            OnReloadTimeChanged(reloadTimeDropdown.value);
    }
}
