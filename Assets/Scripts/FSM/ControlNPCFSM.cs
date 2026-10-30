
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(NavMeshAgent))]
public class ControlNPCFSM : MonoBehaviour
{
    private Animator animator;
    private NavMeshAgent agent;
    private Transform playerTarget;
    private bool isAgentReady = false;

    [Header("Weapon Settings")]
    public GameObject gun;

    [Header("Detection & Attack Settings")]
    public float sightDistance = 10f;
    public float attackDistance = 2f;
    public LayerMask obstacleMask;

    [Header("Damage Settings")]
    public int attackDamage = 5;

    private int lastCloseAttackLoop = -1;
    private int lastShootLoop = -1;

    private static readonly int CanSeePlayerHash =
        Animator.StringToHash("canSeePlayer");

    private static readonly int WithinArmsReachHash =
        Animator.StringToHash("withinArmsReach");

    private static readonly int GotHitHash =
        Animator.StringToHash("gotHit");

    private static readonly int LowHealthHash =
        Animator.StringToHash("lowHealth");

    void Start()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();

        gun = GameObject.Find("hand_gun");

        GameObject playerObj = GameObject.Find("playerMiddle");

        if (playerObj != null)
        {
            playerTarget = playerObj.transform;
        }
        else
        {
            Debug.LogError(
                "Could not find an object named 'playerMiddle' in the scene!"
            );
        }

        NavMeshHit hit;

        if (NavMesh.SamplePosition(
            transform.position,
            out hit,
            5.0f,
            NavMesh.AllAreas))
        {
            agent.Warp(hit.position);
            isAgentReady = true;
        }
        else
        {
            Debug.LogWarning(
                "NavMeshAgent could not find a valid NavMesh nearby!"
            );
        }
    }

    void Update()
    {
        if (animator == null ||
            agent == null ||
            !isAgentReady ||
            playerTarget == null)
            return;

        if (!agent.isOnNavMesh)
            return;

        float distanceToPlayer =
            Vector3.Distance(transform.position, playerTarget.position);

        bool canSee = false;

        if (distanceToPlayer <= sightDistance)
        {
            Vector3 directionToPlayer =
                (playerTarget.position - transform.position).normalized;

            if (!Physics.Raycast(
                transform.position + Vector3.up,
                directionToPlayer,
                distanceToPlayer,
                obstacleMask))
            {
                canSee = true;
            }
        }

        animator.SetBool(CanSeePlayerHash, canSee);

        animator.SetBool(
            WithinArmsReachHash,
            distanceToPlayer <= attackDistance
        );

        AnimatorStateInfo info =
            animator.GetCurrentAnimatorStateInfo(0);

        if (info.IsName("IDLE"))
        {
            lastCloseAttackLoop = -1;
            lastShootLoop = -1;

            if (!agent.isStopped)
                agent.isStopped = true;
        }
        else if (info.IsName("FOLLOW_PLAYER"))
        {
            lastCloseAttackLoop = -1;
            lastShootLoop = -1;

            if (agent.isStopped)
                agent.isStopped = false;

            agent.destination = playerTarget.position;
        }
        else if (info.IsName("ATTACK_CLOSE_RANGE"))
        {
            lastShootLoop = -1;

            if (!agent.isStopped)
                agent.isStopped = true;

            ApplyAttackDamageOnce(
                info,
                ref lastCloseAttackLoop
            );

            Vector3 targetDir =
                playerTarget.position - transform.position;

            targetDir.y = 0;

            if (targetDir != Vector3.zero)
            {
                transform.rotation = Quaternion.Slerp(
                    transform.rotation,
                    Quaternion.LookRotation(targetDir),
                    Time.deltaTime * 5f
                );
            }
        }
        else if (info.IsName("HIT"))
        {
            lastCloseAttackLoop = -1;
            lastShootLoop = -1;

            if (!agent.isStopped)
                agent.isStopped = true;
        }
        else if (info.IsName("SHOOT"))
        {
            lastCloseAttackLoop = -1;

            if (!agent.isStopped)
                agent.isStopped = true;

            if (gun != null)
            {
                if (animator.IsInTransition(0) &&
                    animator.GetNextAnimatorStateInfo(0).IsName("FOLLOW_PLAYER"))
                {
                    gun.SetActive(false);
                }
                else
                {
                    gun.SetActive(true);
                }
            }

            if (playerTarget != null)
                transform.LookAt(playerTarget);

            ApplyAttackDamageOnce(
                info,
                ref lastShootLoop
            );
        }
        else
        {
            lastCloseAttackLoop = -1;
            lastShootLoop = -1;
        }
    }

    private void ApplyAttackDamageOnce(
        AnimatorStateInfo info,
        ref int lastProcessedLoop)
    {
        int currentLoop =
            Mathf.FloorToInt(info.normalizedTime);

        float animationProgress =
            info.normalizedTime - currentLoop;

        // Damage only once, at the end of each animation cycle.
        if (animationProgress >= 0.98f &&
            currentLoop != lastProcessedLoop)
        {
            lastProcessedLoop = currentLoop;

            GameObject fpsController =
                GameObject.Find("FPSController");

            if (fpsController != null)
            {
                ManagePlayerHealth playerHealth =
                    fpsController.GetComponent<ManagePlayerHealth>();

                if (playerHealth != null)
                {
                    playerHealth.decreaseHealth(attackDamage);

                    Debug.Log(
                        name + " damaged player for " +
                        attackDamage + " damage."
                    );
                }
                else
                {
                    Debug.LogError(
                        "ManagePlayerHealth not found on FPSController!"
                    );
                }
            }
        }
    }

    public void SetGotHitParameter()
    {
        animator.SetTrigger(GotHitHash);
    }

    public void SetLowHealthParameter()
    {
        animator.SetBool(LowHealthHash, true);
    }
}
