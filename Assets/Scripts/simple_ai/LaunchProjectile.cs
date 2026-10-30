using UnityEngine;

public class LaunchProjectile : MonoBehaviour
{
    public GameObject projectile;

    [Header("Tracking Settings")]
    public float trackingRange = 20f; // Maximum distance to detect and track the player

    private float timer = 0f;
    private Transform playerTransform;

    void Start()
    {
        timer = 0f;
        FindPlayer();
    }

    void Update()
    {
        // Don't do anything while the game is paused
        if (Time.timeScale <= 0f) return;

        // Ensure we have a reference to the player
        if (playerTransform == null)
        {
            FindPlayer();
            return;
        }

        // Calculate distance between launcher and player
        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);

        // Check if the player is within the tracking range
        if (distanceToPlayer <= trackingRange)
        {
            // Aim towards player position dynamically
            transform.LookAt(playerTransform.position);

            float spawnInterval = 2f;
            float intensity = 1000f;

            // Fetch live settings from the UI controller safely
            if (CollisionWithPlayer.Instance != null)
            {
                spawnInterval = CollisionWithPlayer.Instance.GetSpawnTime();
                intensity = CollisionWithPlayer.Instance.GetProjectileIntensity();
            }

            spawnInterval = Mathf.Max(spawnInterval, 1f);
            timer += Time.deltaTime;

            if (timer >= spawnInterval)
            {
                timer = 0f; // Reset timer

                if (projectile != null)
                {
                    GameObject t = Instantiate(projectile, transform.position, transform.rotation);
                    Destroy(t, 5f); // Clean up after 5 seconds

                    Rigidbody rb = t.GetComponent<Rigidbody>();
                    if (rb != null)
                    {
                        rb.AddForce(transform.forward * intensity);
                    }
                }
            }
        }
        else
        {
            // Reset timer so it doesn't instantly fire the moment the player steps back into range
            timer = 0f;
        }
    }

    void FindPlayer()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            playerTransform = playerObj.transform;
        }
    }

    // Optional: Draw a wire sphere in the Scene view to visualize the tracking range
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, trackingRange);
    }
}