using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class SimpleAIFollow : MonoBehaviour
{
    [Tooltip("The target transform the NPC should follow (e.g., the Player).")]
    public Transform target;

    private NavMeshAgent agent;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        // If no target is assigned manually, look for an object tagged "Player"
        if (target == null)
        {
            GameObject playerObj = GameObject.FindWithTag("Player");
            if (playerObj != null)
            {
                target = playerObj.transform;
            }
        }
    }

    void Update()
    {
        if (target != null)
        {
            // Tell the NavMesh agent to pathfind toward the target's position
            agent.SetDestination(target.position);
        }
    }
}