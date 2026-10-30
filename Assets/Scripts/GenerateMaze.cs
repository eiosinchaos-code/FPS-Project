
using UnityEngine;

public class GenerateMaze : MonoBehaviour
{
    public GameObject wall, player, npc_guard;

    // Pickup & Scoring Prefabs
    public GameObject mcg1;        // 4 = collectible
    public GameObject ammoAuto;    // 5 = automatic ammo
    public GameObject ammoGrenade; // 6 = grenade ammo
    public GameObject ammoGun;     // 7 = gun ammo
    public GameObject healthPack;  // 8 = health pack

    // 0 = floor, 1 = wall, 2 = player, 3 = guard
    // 4 = collectible, 5 = auto ammo, 6 = grenade ammo
    // 7 = gun ammo, 8 = health pack

    private int[,] worldMap = new int[,]
    {
        {1,1,1,1,1,1,1,1,1,1},
        {1,2,0,4,0,0,0,3,0,1},
        {1,0,0,0,0,0,0,0,0,1},
        {1,0,0,0,0,5,0,0,0,1},
        {1,0,0,1,1,1,1,1,0,1},
        {1,3,0,0,0,0,0,0,6,1},
        {1,0,0,0,0,0,0,0,0,1},
        {1,0,0,7,0,0,0,0,0,1},
        {1,0,0,0,0,3,0,8,0,1},
        {1,1,1,1,1,1,1,1,1,1}
    };

    private Vector3 playerSpawnPosition;
    private bool hasPlayerSpawn = false;

    void Start()
    {
        int spacing = 5;

        // Generate maze objects and locate player spawn.
        for (int i = 0; i < worldMap.GetLength(0); i++)
        {
            for (int j = 0; j < worldMap.GetLength(1); j++)
            {
                float posX = 50 - (i * spacing);
                float posZ = 50 - (j * spacing);

                int cellValue = worldMap[i, j];

                Vector3 position = new Vector3(
                    posX,
                    1.5f,
                    posZ
                );

                if (cellValue == 1)
                {
                    Instantiate(wall, position, Quaternion.identity);
                }
                else if (cellValue == 2)
                {
                    // Record spawn position.
                    // Player will be moved/spawned AFTER maze generation.
                    playerSpawnPosition = position;
                    hasPlayerSpawn = true;
                }
                else if (cellValue == 3)
                {
                    Instantiate(
                        npc_guard,
                        new Vector3(posX, 0.5f, posZ),
                        Quaternion.identity
                    );
                }
                else if (cellValue == 4 && mcg1 != null)
                {
                    Instantiate(mcg1, position, Quaternion.identity);
                }
                else if (cellValue == 5 && ammoAuto != null)
                {
                    Instantiate(ammoAuto, position, Quaternion.identity);
                }
                else if (cellValue == 6 && ammoGrenade != null)
                {
                    Instantiate(ammoGrenade, position, Quaternion.identity);
                }
                else if (cellValue == 7 && ammoGun != null)
                {
                    Instantiate(ammoGun, position, Quaternion.identity);
                }
                else if (cellValue == 8 && healthPack != null)
                {
                    Instantiate(healthPack, position, Quaternion.identity);
                }
            }
        }

        // Move or create the player after maze generation.
        SpawnOrMovePlayer();

        // Clean up unwanted audio listeners.
        CleanupAudioListeners();
    }

    void SpawnOrMovePlayer()
    {
        if (!hasPlayerSpawn)
        {
            Debug.LogError("GenerateMaze: No player spawn point found!");
            return;
        }

        // Look for an existing persistent player.
        GameObject existingPlayer = GameObject.Find("FPSController");

        if (existingPlayer != null)
        {
            Debug.Log("Existing FPSController found. Resetting position.");

            CharacterController controller =
                existingPlayer.GetComponent<CharacterController>();

            // Temporarily disable CharacterController.
            if (controller != null)
            {
                controller.enabled = false;
            }

            // Reset player position and rotation.
            existingPlayer.transform.SetPositionAndRotation(
                playerSpawnPosition,
                Quaternion.identity
            );

            // Re-enable CharacterController.
            if (controller != null)
            {
                controller.enabled = true;
            }

            Debug.Log(
                "Existing player moved to maze spawn: " +
                playerSpawnPosition
            );
        }
        else
        {
            // No persistent player exists. Create a new one.
            if (player == null)
            {
                Debug.LogError(
                    "GenerateMaze: Player prefab is not assigned!"
                );
                return;
            }

            GameObject newPlayer = Instantiate(
                player,
                playerSpawnPosition,
                Quaternion.identity
            );

            newPlayer.name = "FPSController";

            Debug.Log(
                "New player spawned at: " +
                playerSpawnPosition
            );
        }

        // Update physics after repositioning.
        Physics.SyncTransforms();
    }

    void CleanupAudioListeners()
    {
        AudioListener[] listeners =
            Object.FindObjectsByType<AudioListener>(
                FindObjectsInactive.Exclude
            );

        GameObject playerObject = GameObject.Find("FPSController");

        foreach (AudioListener listener in listeners)
        {
            if (listener.gameObject.name != "FPSController" &&
                (playerObject == null ||
                 !listener.transform.IsChildOf(
                     playerObject.transform
                 )))
            {
                listener.enabled = false;
            }
        }
    }

    void Update()
    {
    }
}
