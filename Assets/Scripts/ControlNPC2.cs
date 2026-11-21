using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
public class ControlNPC2 : MonoBehaviour
{
    private Animator anim;
    private NavMeshAgent agent;
    private AnimatorStateInfo info;

    [Header("Patrol Settings")]
    public Transform[] waypoints;
    private int currentWPIndex = 0;
    private float patrolTimer = 0f;
    private int nbWPReached = 0;
    private Vector3 startingPosition;

    [Header("Sensory Settings")]
    public float hearingDistance = 3f;
    public float sightDistance = 20f;
    public float smellDistance = 2f;
    private string playerTag = "Player"; // Searching by tag now!

    void Start()
    {
        anim = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();

        startingPosition = transform.position;
        patrolTimer = 0f;
        nbWPReached = 0;
    }

    void Update()
    {
        info = anim.GetCurrentAnimatorStateInfo(0);

        // Run sensory evaluation every frame
        Listen();
        Look();
        Smell();

        // State Machine Handling
        if (info.IsName("idle"))
        {
            patrolTimer += Time.deltaTime;
            if (patrolTimer > 5f)
            {
                patrolTimer = 0f;
                anim.SetTrigger("startPatrol");
            }
        }
        else if (info.IsName("patrol"))
        {
            if (waypoints.Length > 0 && agent.enabled)
            {
                agent.SetDestination(waypoints[currentWPIndex].position);

                if (Vector3.Distance(transform.position, waypoints[currentWPIndex].position) < 1.0f)
                {
                    currentWPIndex = (currentWPIndex + 1) % waypoints.Length;
                    nbWPReached++;

                    if (nbWPReached >= 4)
                    {
                        nbWPReached = 0;
                        anim.SetTrigger("startToGoBackToStart");
                    }
                }
            }
        }
        else if (info.IsName("backToStart"))
        {
            if (agent.enabled)
            {
                agent.SetDestination(startingPosition);

                if (Vector3.Distance(transform.position, startingPosition) < 1.0f)
                {
                    anim.SetTrigger("hasReachedStart");
                }
            }
        }
        else if (info.IsName("followPlayer"))
        {
            GameObject player = GameObject.FindWithTag(playerTag);
            if (player != null && agent.enabled)
            {
                agent.SetDestination(player.transform.position);
            }
        }
    }

    void Listen()
    {
        GameObject player = GameObject.FindWithTag(playerTag);
        if (player != null)
        {
            float distance = Vector3.Distance(transform.position, player.transform.position);
            anim.SetBool("canHearPlayer", distance < hearingDistance);
        }
    }

    void Look()
    {
        Vector3 rayOrigin = transform.position + Vector3.up * 0.7f;
        Debug.DrawRay(rayOrigin, transform.forward * sightDistance, Color.red);

        if (Physics.Raycast(rayOrigin, transform.forward, out RaycastHit hit, sightDistance))
        {
            if (hit.collider.CompareTag(playerTag))
            {
                anim.SetBool("canSeePlayer", true);
                return;
            }
        }
        anim.SetBool("canSeePlayer", false);
    }

    void Smell()
    {
        GameObject[] allBCs = GameObject.FindGameObjectsWithTag("BC");
        bool detectedBC = false;

        foreach (GameObject bc in allBCs)
        {
            if (Vector3.Distance(transform.position, bc.transform.position) < smellDistance)
            {
                detectedBC = true;
                break;
            }
        }

        anim.SetBool("canSmellPlayer", detectedBC);
    }
}