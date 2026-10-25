using UnityEngine;
using TMPro;

public class ManageWeapons : MonoBehaviour
{
    Camera playersCamera;
    Ray rayFromPlayer;
    RaycastHit hit;
    public GameObject sparksAtImpact;

    // Multi-weapon variables
    private const int WEAPON_GUN = 0;
    private const int WEAPON_AUTO_GUN = 1;
    private const int WEAPON_GRENADE = 2;

    private float timer;
    private bool timerStarted;
    private bool canShoot = true;
    private int currentWeapon;
    private bool[] hasWeapon;
    private int[] ammos;
    private int[] maxAmmos;
    private float[] reloadTime;
    private string[] weaponName;

    public GameObject grenade; // Fixed spelling from 'greande' to 'grenade'

    // Use this for initialization
    void Start()
    {
        playersCamera = GetComponent<Camera>();
        ammos = new int[3];
        hasWeapon = new bool[3];
        maxAmmos = new int[3];
        reloadTime = new float[3];
        weaponName = new string[3];

        hasWeapon[WEAPON_GUN] = true;
        hasWeapon[WEAPON_AUTO_GUN] = true;
        hasWeapon[WEAPON_GRENADE] = true;

        weaponName[WEAPON_GUN] = "GUN";
        weaponName[WEAPON_AUTO_GUN] = "AUTOMATIC GUN";
        weaponName[WEAPON_GRENADE] = "GRENADE";

        ammos[WEAPON_GUN] = 10;
        ammos[WEAPON_AUTO_GUN] = 10;
        ammos[WEAPON_GRENADE] = 10;

        maxAmmos[WEAPON_GUN] = 20;
        maxAmmos[WEAPON_AUTO_GUN] = 20;
        maxAmmos[WEAPON_GRENADE] = 10;

        currentWeapon = WEAPON_GUN;

        reloadTime[WEAPON_GUN] = 2.0f;
        reloadTime[WEAPON_AUTO_GUN] = 0.5f;
        reloadTime[WEAPON_GRENADE] = 3.0f;

        UpdateUI();
    }

    // Update is called once per frame
    void Update()
    {
        if (playersCamera == null) return;

        // Handle reload / fire rate timer
        if (timerStarted)
        {
            timer += Time.deltaTime;
            if (timer >= reloadTime[currentWeapon])
            {
                timerStarted = false;
                canShoot = true;
            }
        }

        // Setup ray from center of screen
        rayFromPlayer = playersCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        Debug.DrawRay(rayFromPlayer.origin, rayFromPlayer.direction * 100, Color.red);

        // Shooting input (F key) - Changed to GetKeyDown so it doesn't fire continuously unless intended, 
        // but kept structure matching your preference.
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (ammos[currentWeapon] >= 1 && canShoot)
            {
                // Play gunshot/fire sound effect
                if (GetComponent<AudioSource>() != null)
                {
                    GetComponent<AudioSource>().Play();
                }

                // Check if current weapon is the GRENADE launcher
                if (currentWeapon == WEAPON_GRENADE)
                {
                    ammos[currentWeapon]--;
                    GameObject launcher = GameObject.Find("launcher");
                    if (launcher != null && grenade != null)
                    {
                        GameObject grenadeF = Instantiate(grenade, launcher.transform.position, Quaternion.identity);
                        if (grenadeF.GetComponent<Rigidbody>() != null)
                        {
                            grenadeF.GetComponent<Rigidbody>().AddForce(launcher.transform.forward * 200);
                        }
                    }
                    else
                    {
                        Debug.LogWarning("Launcher or Grenade prefab is missing!");
                    }
                }
                else // Standard Raycast Weapons (Gun / Auto Gun)
                {
                    ammos[currentWeapon]--;

                    if (Physics.Raycast(rayFromPlayer, out hit, 100))
                    {
                        print("YOU SHOT: " + hit.collider.gameObject.name + " WITH TAG: " + hit.collider.tag);

                        Vector3 positionOfImpact = hit.point;
                        if (sparksAtImpact != null)
                        {
                            Instantiate(sparksAtImpact, positionOfImpact, Quaternion.identity);
                        }

                        if (hit.collider.CompareTag("target"))
                        {
                            ManageNPC npc = hit.collider.GetComponent<ManageNPC>();
                            if (npc != null)
                            {
                                npc.gotHit();
                            }
                        }
                    }
                }

                canShoot = false;
                timer = 0.0f;
                timerStarted = true;
                print("You have " + ammos[currentWeapon] + " " + weaponName[currentWeapon] + " bullets left");

                UpdateUI();
            }
            else
            {
                print("Out of ammo or weapon is reloading!");
            }
        }

        // Weapon switching input (Tab key)
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (hasWeapon[WEAPON_GUN] && hasWeapon[WEAPON_AUTO_GUN] && hasWeapon[WEAPON_GRENADE])
            {
                currentWeapon++;
                if (currentWeapon > 2) currentWeapon = 0;
            }
            else if (hasWeapon[WEAPON_GUN] && hasWeapon[WEAPON_AUTO_GUN])
            {
                if (currentWeapon == WEAPON_GUN) currentWeapon = WEAPON_AUTO_GUN;
                else currentWeapon = WEAPON_GUN;
            }
            else if (hasWeapon[WEAPON_GUN] && hasWeapon[WEAPON_GRENADE])
            {
                if (currentWeapon == WEAPON_GUN) currentWeapon = WEAPON_GRENADE;
                else currentWeapon = WEAPON_GUN;
            }
            else if (hasWeapon[WEAPON_AUTO_GUN] && hasWeapon[WEAPON_GRENADE])
            {
                if (currentWeapon == WEAPON_AUTO_GUN) currentWeapon = WEAPON_GRENADE;
                else currentWeapon = WEAPON_AUTO_GUN;
            }

            print("Current Weapon: " + weaponName[currentWeapon] + " (" + ammos[currentWeapon] + ")");
            UpdateUI();
        }
    }

    public void manageCollisions(ControllerColliderHit hit)
    {
        string tagOfTheOtherObject = hit.collider.gameObject.tag;
        if (tagOfTheOtherObject == "ammo_gun" || tagOfTheOtherObject == "ammo_automatic_gun" || tagOfTheOtherObject == "ammo_grenade")
        {
            int indexOfAmmoBeingUpdated = 0;
            if (tagOfTheOtherObject == "ammo_gun") indexOfAmmoBeingUpdated = WEAPON_GUN;
            if (tagOfTheOtherObject == "ammo_automatic_gun") indexOfAmmoBeingUpdated = WEAPON_AUTO_GUN;
            if (tagOfTheOtherObject == "ammo_grenade") indexOfAmmoBeingUpdated = WEAPON_GRENADE;

            ammos[indexOfAmmoBeingUpdated] += 5;
            if (ammos[indexOfAmmoBeingUpdated] > maxAmmos[indexOfAmmoBeingUpdated])
            {
                ammos[indexOfAmmoBeingUpdated] = maxAmmos[indexOfAmmoBeingUpdated];
            }

            Destroy(hit.collider.gameObject);
            UpdateUI();
        }
    }

    // Helper method to safely update your UI text element
    void UpdateUI()
    {
        GameObject uiObject = GameObject.Find("userInfo");
        if (uiObject != null)
        {
            TMP_Text uiText = uiObject.GetComponent<TMP_Text>();
            if (uiText != null)
            {
                uiText.text = weaponName[currentWeapon] + "(" + ammos[currentWeapon] + ")";
            }
        }
    }
}