using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(ManageNPC2))]
public class ControlNPC3 : MonoBehaviour
{
    private Animator anim;
    private NavMeshAgent agent;
    private AnimatorStateInfo info;
    private ManageNPC2 manager;

    [Header("Patrol & Ambush Settings")]
    public Transform[] waypoints;
    private int currentWPIndex = 0;
    private float patrolTimer = 0f;
    private int nbWPReached = 0;
    private Vector3 startingPosition;

    [Header("Combat & Senses")]
    public float hearingDistance = 3f;
    public float sightDistance = 20f;
    public float smellDistance = 2f;
    private float shootTimer = 0f;
    public GameObject gun;

    void Start()
    {
        anim = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        manager = GetComponent<ManageNPC2>();

        startingPosition = transform.position;
        patrolTimer = 0f;
        nbWPReached = 0;

        if (gun != null) gun.SetActive(false);
    }

    void Update()
    {
        info = anim.GetCurrentAnimatorStateInfo(0);

        Listen();
        Look();
        Smell();

        anim.SetFloat("health", manager.GetHealth());
        anim.SetFloat("ammos", manager.GetAmmos());

        if (info.IsName("idle"))
        {
            patrolTimer += Time.deltaTime;
            agent.isStopped = true;
            if (patrolTimer > 5f)
            {
                patrolTimer = 0f;
                anim.SetTrigger("startPatrol");
            }
        }
        else if (info.IsName("patrol"))
        {
            agent.isStopped = false;
            if (waypoints.Length > 0)
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
        else if (info.IsName("lookForHealth"))
        {
            agent.isStopped = false;
            GameObject[] healthPacks = GameObject.FindGameObjectsWithTag("healthPack");

            if (healthPacks.Length == 0)
            {
                anim.SetBool("healthPackAvailable", false);
            }
            else
            {
                anim.SetBool("healthPackAvailable", true);
                float distanceToClosestPack = 1000f;
                int rankOfClosestPack = 100;

                for (int i = 0; i < healthPacks.Length; i++)
                {
                    float dist = Vector3.Distance(transform.position, healthPacks[i].transform.position);
                    if (dist < distanceToClosestPack)
                    {
                        distanceToClosestPack = dist;
                        rankOfClosestPack = i;
                    }
                }

                GameObject targetPack = healthPacks[rankOfClosestPack];
                if (targetPack != null)
                {
                    agent.SetDestination(targetPack.transform.position);
                    if (Vector3.Distance(transform.position, targetPack.transform.position) < 2f)
                    {
                        manager.SetHealth(100f);
                        Destroy(targetPack);
                    }
                }
            }
        }
        else if (info.IsName("followPlayer"))
        {
            agent.isStopped = false;
            GameObject player = GameObject.Find("FPSController");
            if (player != null)
            {
                agent.SetDestination(player.transform.position);
                shootTimer += Time.deltaTime;
                if (shootTimer >= 3f)
                {
                    shootTimer = 0f;
                    if (anim.GetBool("canSeePlayer") && manager.GetAmmos() > 0)
                    {
                        anim.SetTrigger("startToShoot");
                        manager.DecreaseAmmo();
                    }
                }
            }
        }
        else if (info.IsName("gotoAmbushSpot"))
        {
            agent.isStopped = false;
            GameObject ambushStart = GameObject.Find("ambush_start");
            if (ambushStart != null)
            {
                agent.SetDestination(ambushStart.transform.position);
                agent.speed = 5.5f;
                if (Vector3.Distance(transform.position, ambushStart.transform.position) < 2.5f)
                {
                    anim.SetTrigger("reachedAmbush");
                }
            }
        }
        else if (info.IsName("throwGrenade"))
        {
            agent.isStopped = true;
        }
        else if (info.IsName("dies"))
        {
            agent.isStopped = true;
            Destroy(gameObject, 3f);
        }
    }

    public void PlayerEnteredAmbushArea()
    {
        anim.SetTrigger("gotoAmbush");
    }

    void Listen()
    {
        GameObject player = GameObject.Find("FPSController");
        if (player != null)
        {
            anim.SetBool("canHearPlayer", Vector3.Distance(transform.position, player.transform.position) < hearingDistance);
        }
    }

    void Look()
    {
        Vector3 rayOrigin = transform.position + Vector3.up * 0.7f;
        if (Physics.Raycast(rayOrigin, transform.forward, out RaycastHit hit, sightDistance))
        {
            if (hit.collider.name == "FPSController" || hit.collider.CompareTag("Player"))
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