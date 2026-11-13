using UnityEngine;
using UnityEngine.AI;

public class ControlNPC : MonoBehaviour
{
    public GameObject player;          // Drag your FPSController here
    private GameObject currentTarget;  // Active destination

    public GameObject WP1, WP2, WP3, WP4;
    public GameObject[] WayPoints;

    int WPCount;

    public bool isWandering = false;
    public bool isFollowingWayPoints = true;
    public float timer;
    public GameObject wanderingTarget;

    void Start()
    {
        // Setup waypoints array
        if (WP1 != null && WP2 != null && WP3 != null && WP4 != null)
        {
            WayPoints = new GameObject[] { WP1, WP2, WP3, WP4 };
            WPCount = 0;
        }

        // Hide waypoint visual markers at start
        if (WayPoints != null)
        {
            for (int i = 0; i < WayPoints.Length; i++)
            {
                if (WayPoints[i].GetComponent<Renderer>() != null)
                    WayPoints[i].GetComponent<Renderer>().enabled = false;
            }
        }

        // Initialize wandering target object if free wandering is used
        if (isWandering)
        {
            wanderingTarget = new GameObject("WanderingTarget");
            wanderingTarget.transform.position = transform.position;
            currentTarget = wanderingTarget;
        }
    }

    void Update()
    {
        // 1. Priority 1: Check if the player is close enough to chase (< 8 meters)
        if (player != null)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
            if (distanceToPlayer < 8.0f)
            {
                GetComponent<NavMeshAgent>().SetDestination(player.transform.position);
                return; // Stop other movement while chasing player
            }
        }

        // 2. Priority 2: Waypoint Navigation
        if (isFollowingWayPoints && WayPoints.Length > 0)
        {
            currentTarget = WayPoints[WPCount];

            if (Vector3.Distance(transform.position, currentTarget.transform.position) < 1.0f)
            {
                moveToRandomWP();
            }

            GetComponent<NavMeshAgent>().SetDestination(currentTarget.transform.position);
            return;
        }

        // 3. Priority 3: Free Wandering & Raycast Obstacle Avoidance (Phase 7)
        if (isWandering)
        {
            checkAhead(); // Continuously scan 2 meters ahead for walls[cite: 19]

            timer += Time.deltaTime; // Accumulate time[cite: 15, 16]
            if (timer > 4.0f)        // Every 4 seconds[cite: 16]
            {
                timer = 0f;          // Reset timer[cite: 16]
                wander();            // Calculate a new random wandering direction[cite: 16]
            }

            if (currentTarget != null)
            {
                GetComponent<NavMeshAgent>().SetDestination(currentTarget.transform.position);
            }
        }
    }

    // Random Waypoint Picker
    void moveToRandomWP()
    {
        int previous = WPCount;
        int randomNum = 0;
        do
        {
            randomNum = Random.Range(0, WayPoints.Length);
        } while (randomNum == previous);

        WPCount = randomNum;
    }

    // Phase 7: Free Wandering Logic & Raycasting
    void wander()
    {
        Ray ray = new Ray();
        RaycastHit hit;
        ray.origin = transform.position + Vector3.up * 0.7f; // Origin offset slightly upward[cite: 16]

        float randomDirectionX = Random.Range(-1.0f, 1.0f); // Random X between -1 and 1
        float randomDirectionZ = Random.Range(-1.0f, 1.0f); // Random Z between -1 and 1[cite: 17]
        ray.direction = (transform.forward * randomDirectionZ + transform.right * randomDirectionX).normalized;

        Debug.DrawRay(ray.origin, ray.direction * 20f, Color.red, 2f); // Visible debug ray[cite: 17]

        float castingDistance = 20f; // Cast up to 20 meters[cite: 16, 17]
        float distanceToObstacle = castingDistance;

        if (Physics.Raycast(ray.origin, ray.direction, out hit, castingDistance))
        {
            distanceToObstacle = hit.distance; // Save distance if an object is hit[cite: 17]
        }

        if (wanderingTarget == null)
        {
            wanderingTarget = new GameObject("WanderingTarget");
        }

        // Position the target 1 meter before the obstacle or max distance[cite: 17]
        wanderingTarget.transform.position = ray.origin + ray.direction * Mathf.Max(0, distanceToObstacle - 1f);
        currentTarget = wanderingTarget;
    }

    // Phase 7: Real-time Check Ahead to avoid getting stuck
    void checkAhead()
    {
        Ray ray = new Ray();
        RaycastHit hit;
        float castingDistance = 2f; // Check 2 meters ahead[cite: 19]
        ray.origin = transform.position + Vector3.up * 0.7f; //[cite: 16]
        ray.direction = transform.forward * castingDistance; //[cite: 19]

        Debug.DrawRay(ray.origin, ray.direction, Color.red); //[cite: 17, 19]

        if (Physics.Raycast(ray.origin, ray.direction, out hit, castingDistance))
        {
            wander(); // Instantly trigger wander direction recalculation if a wall is too close[cite: 16, 19]
        }
    }
}