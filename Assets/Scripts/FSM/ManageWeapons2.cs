
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class ManageWeapons2 : MonoBehaviour
{
    public static ManageWeapons2 Instance;

    [Header("Weapon Setup (Size should be 3)")]
    public string[] weaponName = new string[3]
    {
        "Gun", "AutoGun", "GrenadeLauncher"
    };

    // Independent current ammo for each weapon
    public int[] ammos = new int[3] { 2, 2, 2 };

    // Independent maximum capacity for each weapon
    public int[] maxAmmos = new int[3] { 2, 2, 2 };

    public int currentWeapon = 0;

    // Default dropdown capacity
    public int globalMaxAmmo = 2;

    [Header("Grenade Settings")]
    public GameObject grenadePrefab;
    public Transform throwPoint;

    [Header("Reload")]
    public float currentReloadTimeMultiplier = 0.5f;

    private int score;

    private const int WEAPON_COUNT = 3;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        if (Instance != this)
            return;

        score = 0;

        // Ensure arrays have the correct size
        if (weaponName == null ||
            weaponName.Length != WEAPON_COUNT)
        {
            weaponName = new string[WEAPON_COUNT]
            {
                "Gun", "AutoGun", "GrenadeLauncher"
            };
        }

        if (ammos == null ||
            ammos.Length != WEAPON_COUNT)
        {
            ammos = new int[WEAPON_COUNT] { 2, 2, 2 };
        }

        if (maxAmmos == null ||
            maxAmmos.Length != WEAPON_COUNT)
        {
            maxAmmos = new int[WEAPON_COUNT] { 2, 2, 2 };
        }

        // Default maximum capacity
        globalMaxAmmo = 2;

        // Apply default capacity without refilling ammo
        for (int i = 0; i < WEAPON_COUNT; i++)
        {
            maxAmmos[i] = globalMaxAmmo;

            ammos[i] = Mathf.Clamp(
                ammos[i], 0, maxAmmos[i]
            );
        }

        currentWeapon = Mathf.Clamp(
            currentWeapon, 0, WEAPON_COUNT - 1
        );

        UpdateHUDText();
    }

    void Update()
    {
        if (Instance != this)
            return;

        // Weapon switching
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            CycleWeapon();
        }

        // Fire weapon
        if (Input.GetKeyDown(KeyCode.F))
        {
            FireWeapon();
        }
    }

    void UpdateHUDText()
    {
        if (weaponName == null ||
            ammos == null ||
            maxAmmos == null)
            return;

        if (currentWeapon < 0 ||
            currentWeapon >= weaponName.Length ||
            currentWeapon >= ammos.Length ||
            currentWeapon >= maxAmmos.Length)
            return;

        GameObject userInfoObj =
            GameObject.Find("userInfo");

        if (userInfoObj == null)
            return;

        TextMeshProUGUI userText =
            userInfoObj.GetComponent<TextMeshProUGUI>();

        if (userText != null)
        {
            userText.text =
                weaponName[currentWeapon] +
                "\nAmmo: " +
                ammos[currentWeapon] +
                "/" +
                maxAmmos[currentWeapon];
        }
    }

    void CycleWeapon()
    {
        if (weaponName == null ||
            weaponName.Length == 0)
            return;

        currentWeapon =
            (currentWeapon + 1) % weaponName.Length;

        UpdateHUDText();

        Debug.Log(
            "Switched weapon: " +
            weaponName[currentWeapon]
        );
    }

    // Called by GamePreferencesManager
    // or the ammo dropdown callback
    public void UpdateWeaponStats(
        int newGlobalMax,
        float reloadMultiplier)
    {
        // Only allow supported ammo capacities
        if (newGlobalMax != 2 &&
            newGlobalMax != 5 &&
            newGlobalMax != 10)
        {
            Debug.LogWarning(
                "Invalid ammo capacity: " + newGlobalMax
            );
            return;
        }

        // Update reload setting
        currentReloadTimeMultiplier =
            reloadMultiplier;

        // Only change ammo capacities if needed
        if (globalMaxAmmo != newGlobalMax)
        {
            globalMaxAmmo = newGlobalMax;

            for (int i = 0; i < WEAPON_COUNT; i++)
            {
                maxAmmos[i] = globalMaxAmmo;

                // Reduce ammo only if it exceeds capacity
                // Never refill ammo on dropdown changes
                ammos[i] = Mathf.Clamp(
                    ammos[i], 0, maxAmmos[i]
                );
            }
        }

        UpdateHUDText();

        Debug.Log(
            "Max Ammo: " + globalMaxAmmo +
            " | Reload Multiplier: " +
            currentReloadTimeMultiplier
        );
    }

    public void SetMaxAmmo(int newMax)
    {
        UpdateWeaponStats(
            newMax,
            currentReloadTimeMultiplier
        );
    }

    void FireWeapon()
    {
        if (ammos == null ||
            currentWeapon < 0 ||
            currentWeapon >= ammos.Length)
            return;

        // Check ammo first
        if (ammos[currentWeapon] <= 0)
        {
            Debug.Log("Out of ammo!");
            return;
        }

        bool isGrenade =
            weaponName != null &&
            currentWeapon < weaponName.Length &&
            weaponName[currentWeapon].Contains("Grenade");

        // Grenade validation before consuming ammo
        if (isGrenade)
        {
            if (grenadePrefab == null)
            {
                Debug.LogWarning(
                    "Grenade Prefab is not assigned!"
                );
                return;
            }

            ThrowGrenade();
            ammos[currentWeapon]--;
            UpdateHUDText();
            return;
        }

        // Find player camera
        Camera playerCamera =
            GetComponentInChildren<Camera>();

        if (playerCamera == null)
        {
            playerCamera = Camera.main;
        }

        if (playerCamera == null)
        {
            Debug.LogWarning("Player camera not found!");
            return;
        }

        // Consume ammo only after validation
        ammos[currentWeapon]--;
        UpdateHUDText();

        // Raycast shooting
        RaycastHit hit;

        if (Physics.Raycast(
            playerCamera.transform.position,
            playerCamera.transform.forward,
            out hit,
            100f))
        {
            if (hit.collider.CompareTag("target"))
            {
                ManageNPC2 enemy =
                    hit.collider.GetComponent<ManageNPC2>();

                if (enemy != null)
                {
                    enemy.gotHit();
                }
            }
        }
    }

    void ThrowGrenade()
    {
        Transform spawnTransform =
            throwPoint != null ? throwPoint : transform;

        GameObject grenade = Instantiate(
            grenadePrefab,
            spawnTransform.position,
            spawnTransform.rotation
        );

        Rigidbody rb =
            grenade.GetComponent<Rigidbody>();

        if (rb != null)
        {
            Camera playerCamera =
                GetComponentInChildren<Camera>();

            Vector3 throwDirection =
                playerCamera != null
                ? playerCamera.transform.forward
                : transform.forward;

            // Original throw force
            rb.AddForce(
                throwDirection * 8.0f +
                Vector3.up * 1.5f,
                ForceMode.VelocityChange
            );
        }
    }

    public void manageCollisions(
        ControllerColliderHit hit)
    {
        if (hit == null || hit.collider == null)
            return;

        string objectTag = hit.collider.tag;

        HandlePickup(
            hit.collider.gameObject,
            objectTag
        );
    }

    private void HandlePickup(
        GameObject hitObject,
        string objectTag)
    {
        if (hitObject == null)
            return;

        if (objectTag == "pick_me")
        {
            Destroy(hitObject);
            score++;

            if (score >= 9)
            {
                SceneManager.LoadScene("winScene");
            }
        }
        else if (objectTag == "health_pack")
        {
            ManagePlayerHealth playerHealth =
                FindAnyObjectByType<ManagePlayerHealth>();

            if (playerHealth != null)
            {
                playerHealth.IncreaseHealth(25);
            }

            Destroy(hitObject);
        }
        else if (objectTag == "ammo_gun")
        {
            AddAmmoToWeaponSlot(0, 5);
            Destroy(hitObject);
        }
        else if (objectTag == "ammo_automatic_gun")
        {
            AddAmmoToWeaponSlot(1, 5);
            Destroy(hitObject);
        }
        else if (objectTag == "ammo_grenade")
        {
            AddAmmoToWeaponSlot(2, 5);
            Destroy(hitObject);
        }
    }

    void AddAmmoToWeaponSlot(
        int weaponIndex,
        int amount)
    {
        if (ammos == null || maxAmmos == null)
            return;

        if (weaponIndex < 0 ||
            weaponIndex >= ammos.Length ||
            weaponIndex >= maxAmmos.Length)
            return;

        // Add ammo only to the specified weapon
        ammos[weaponIndex] += amount;

        // Respect that weapon's maximum capacity
        ammos[weaponIndex] = Mathf.Clamp(
            ammos[weaponIndex],
            0,
            maxAmmos[weaponIndex]
        );

        UpdateHUDText();

        Debug.Log(
            "Added ammo to " +
            weaponName[weaponIndex] +
            ". New total: " +
            ammos[weaponIndex] +
            "/" +
            maxAmmos[weaponIndex]
        );
    }
}
