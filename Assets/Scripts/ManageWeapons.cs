using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ManageWeapons : MonoBehaviour
{
    Camera playersCamera;
    Ray rayFromPlayer;
    RaycastHit hit;
    public GameObject sparksAtImpact;

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
    private int[] baseMaxAmmos;
    private float[] reloadTime;
    private float[] baseReloadTimes;
    private string[] weaponName;

    public GameObject grenade;

    void Awake()
    {
        playersCamera = GetComponent<Camera>();
        ammos = new int[3];
        hasWeapon = new bool[3];
        maxAmmos = new int[3];
        baseMaxAmmos = new int[3];
        reloadTime = new float[3];
        baseReloadTimes = new float[3];
        weaponName = new string[3];

        hasWeapon[WEAPON_GUN] = true;
        hasWeapon[WEAPON_AUTO_GUN] = true;
        hasWeapon[WEAPON_GRENADE] = true;

        weaponName[WEAPON_GUN] = "GUN";
        weaponName[WEAPON_AUTO_GUN] = "AUTOMATIC GUN";
        weaponName[WEAPON_GRENADE] = "GRENADE";

        // Set lowest default starting ammo (2 rounds)
        ammos[WEAPON_GUN] = 2;
        ammos[WEAPON_AUTO_GUN] = 2;
        ammos[WEAPON_GRENADE] = 2;

        baseMaxAmmos[WEAPON_GUN] = 2;
        baseMaxAmmos[WEAPON_AUTO_GUN] = 2;
        baseMaxAmmos[WEAPON_GRENADE] = 2;

        maxAmmos[WEAPON_GUN] = baseMaxAmmos[WEAPON_GUN];
        maxAmmos[WEAPON_AUTO_GUN] = baseMaxAmmos[WEAPON_AUTO_GUN];
        maxAmmos[WEAPON_GRENADE] = baseMaxAmmos[WEAPON_GRENADE];

        currentWeapon = WEAPON_GUN;

        baseReloadTimes[WEAPON_GUN] = 2.0f;
        baseReloadTimes[WEAPON_AUTO_GUN] = 0.5f;
        baseReloadTimes[WEAPON_GRENADE] = 3.0f;

        reloadTime[WEAPON_GUN] = baseReloadTimes[WEAPON_GUN];
        reloadTime[WEAPON_AUTO_GUN] = baseReloadTimes[WEAPON_AUTO_GUN];
        reloadTime[WEAPON_GRENADE] = baseReloadTimes[WEAPON_GRENADE];

        UpdateUI();
    }

    void Start()
    {
    }

    void Update()
    {
        if (Time.timeScale == 0f) return;

        if (playersCamera == null) return;

        if (timerStarted)
        {
            timer += Time.deltaTime;
            if (timer >= reloadTime[currentWeapon])
            {
                timerStarted = false;
                canShoot = true;
            }
        }

        rayFromPlayer = playersCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));

        if (Input.GetKeyDown(KeyCode.F))
        {
            if (ammos[currentWeapon] >= 1 && canShoot)
            {
                if (GetComponent<AudioSource>() != null)
                {
                    GetComponent<AudioSource>().Play();
                }

                if (currentWeapon == WEAPON_GRENADE)
                {
                    ammos[currentWeapon]--;
                    GameObject launcher = GameObject.Find("launcher");
                    if (launcher != null && grenade != null)
                    {
                        GameObject grenadeF = Instantiate(grenade, launcher.transform.position, Quaternion.identity);
                        Rigidbody rb = grenadeF.GetComponent<Rigidbody>();
                        if (rb != null)
                        {
                            rb.useGravity = true;
                            rb.linearDamping = 0.1f;
                            rb.AddForce(launcher.transform.forward * 80 + Vector3.up * 40, ForceMode.Impulse);
                        }
                    }
                }
                else
                {
                    ammos[currentWeapon]--;

                    if (Physics.Raycast(rayFromPlayer, out hit, 100))
                    {
                        if (sparksAtImpact != null)
                        {
                            Instantiate(sparksAtImpact, hit.point, Quaternion.identity);
                        }

                        if (hit.collider.CompareTag("target"))
                        {
                            ManageNPC npc = hit.collider.GetComponent<ManageNPC>();
                            if (npc != null) npc.gotHit();
                        }
                    }
                }

                canShoot = false;
                timer = 0.0f;
                timerStarted = true;
                UpdateUI();
            }
        }

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            currentWeapon = (currentWeapon + 1) % 3;
            UpdateUI();
        }
    }

    public void SetExactMaxAmmo(int exactRounds)
    {
        for (int i = 0; i < 3; i++)
        {
            baseMaxAmmos[i] = exactRounds;
            maxAmmos[i] = baseMaxAmmos[i];
            if (ammos[i] > maxAmmos[i]) ammos[i] = maxAmmos[i];
        }
        UpdateUI();
    }

    public void SetReloadTimeModifier(float modifier)
    {
        for (int i = 0; i < 3; i++)
        {
            reloadTime[i] = baseReloadTimes[i] * modifier;
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

    void UpdateUI()
    {
        GameObject uiObject = GameObject.Find("userInfo");
        if (uiObject != null)
        {
            TMP_Text tmpText = uiObject.GetComponent<TMP_Text>();
            if (tmpText != null)
            {
                tmpText.text = weaponName[currentWeapon] + " Ammo: " + ammos[currentWeapon];
            }
            else
            {
                Text legacyText = uiObject.GetComponent<Text>();
                if (legacyText != null)
                {
                    legacyText.text = weaponName[currentWeapon] + " Ammo: " + ammos[currentWeapon];
                }
            }
        }

        GameObject maxAmmoObj = GameObject.Find("maxAmmoText");
        if (maxAmmoObj != null)
        {
            TMP_Text maxTmp = maxAmmoObj.GetComponent<TMP_Text>();
            if (maxTmp != null)
            {
                maxTmp.text = "Max Ammos: Gun(" + maxAmmos[WEAPON_GUN] + ") Auto(" + maxAmmos[WEAPON_AUTO_GUN] + ") Grenade(" + maxAmmos[WEAPON_GRENADE] + ")";
            }
            else
            {
                Text maxLegacy = maxAmmoObj.GetComponent<Text>();
                if (maxLegacy != null)
                {
                    maxLegacy.text = "Max Ammos: Gun(" + maxAmmos[WEAPON_GUN] + ") Auto(" + maxAmmos[WEAPON_AUTO_GUN] + ") Grenade(" + maxAmmos[WEAPON_GRENADE] + ")";
                }
            }
        }
    }
}