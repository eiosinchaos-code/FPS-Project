
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class ManagePlayerHealth : MonoBehaviour
{
    private int health = 100;
    private int nbLives = 3;

    private float alpha = 0f;
    private bool screenFlashBool = false;

    private Image screenFlashImage;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void Start()
    {
        GameObject[] clones = GameObject.FindGameObjectsWithTag("Player");

        if (clones.Length > 1)
            Destroy(clones[1]);

        FindScreenFlash();
        ResetScreenFlash();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        FindScreenFlash();
        ResetScreenFlash();

        Debug.Log("Scene loaded. Screen flash reset.");
    }

    private void FindScreenFlash()
    {
        GameObject screenFlashObj = GameObject.Find("screenFlash");

        if (screenFlashObj != null)
        {
            screenFlashImage = screenFlashObj.GetComponent<Image>();
        }
        else
        {
            screenFlashImage = null;
            Debug.LogWarning("Could not find screenFlash in the new scene.");
        }
    }

    private void ResetScreenFlash()
    {
        alpha = 0f;
        screenFlashBool = false;

        if (screenFlashImage != null)
        {
            Color flashColor = screenFlashImage.color;
            flashColor.a = 0f;
            screenFlashImage.color = flashColor;
        }
    }

    void Update()
    {
        GameObject healthObj = GameObject.Find("healthInfo");

        if (healthObj != null)
        {
            TextMeshProUGUI healthText =
                healthObj.GetComponent<TextMeshProUGUI>();

            if (healthText != null)
                healthText.text = "Health: " + health;
        }

        GameObject livesObj = GameObject.Find("livesInfo");

        if (livesObj != null)
        {
            TextMeshProUGUI livesText =
                livesObj.GetComponent<TextMeshProUGUI>();

            if (livesText != null)
                livesText.text = "Lives: " + nbLives;
        }

        if (screenFlashBool)
        {
            alpha -= Time.deltaTime;

            if (screenFlashImage != null)
            {
                Color flashColor = screenFlashImage.color;
                flashColor.a = Mathf.Clamp01(alpha);
                screenFlashImage.color = flashColor;
            }

            if (alpha <= 0f)
            {
                screenFlashBool = false;
                alpha = 0f;
            }
        }
    }

    public void decreaseHealth(int healthIncrement)
    {
        Debug.Log("Decreasing health by " + healthIncrement);

        health -= healthIncrement;

        if (health <= 0)
        {
            RestartLevel();
        }

        ScreenFlash();
    }

    public void IncreaseHealth(int healthIncrement)
    {
        health += healthIncrement;

        if (health > 100)
            health = 100;

        Debug.Log("Health is: " + health);
    }

    public void RestartLevel()
    {
        nbLives--;
        health = 100;

        if (nbLives > 0)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        else
        {
            SceneManager.LoadScene("lostScene");
        }
    }

    private void ScreenFlash()
    {
        screenFlashBool = true;
        alpha = 1f;
    }
}
