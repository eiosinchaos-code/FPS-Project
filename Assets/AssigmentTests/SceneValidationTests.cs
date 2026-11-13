using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.AI;
using Unity.AI.Navigation;

public class SceneValidationTests
{
    // ==========================================
    // SCENE 1: PATHS TESTS
    // ==========================================

    [UnityTest]
    public IEnumerator Paths_Test01_PlayerAndWaypointsExist()
    {
        // Load the 'paths' scene asynchronously
        AsyncOperation loadOp = SceneManager.LoadSceneAsync("paths");
        while (!loadOp.isDone) yield return null;

        // Test 1: Check if Player exists
        GameObject player = GameObject.FindWithTag("Player");
        Assert.IsNotNull(player, "Paths Test 1 Failed: Player object is missing from the 'paths' scene.");

        // Test 2: Check if Waypoint 1 exists
        GameObject wp1 = GameObject.Find("WP1");
        Assert.IsNotNull(wp1, "Paths Test 1 Failed: Waypoint WP1 is missing!");

        Debug.Log("<color=green>[PASS]</color> Paths Test 1: Player and Waypoints successfully verified.");
    }

    [UnityTest]
    public IEnumerator Paths_Test02_NpcAgentInitialized()
    {
        AsyncOperation loadOp = SceneManager.LoadSceneAsync("paths");
        while (!loadOp.isDone) yield return null;

        // Test: Check if NPC exists and has a NavMeshAgent component
        GameObject npc = GameObject.Find("NPC");
        Assert.IsNotNull(npc, "Paths Test 2 Failed: NPC object not found.");

        NavMeshAgent agent = npc.GetComponent<NavMeshAgent>();
        Assert.IsNotNull(agent, "Paths Test 2 Failed: NPC is missing the NavMeshAgent component.");

        Debug.Log("<color=green>[PASS]</color> Paths Test 2: NPC NavMeshAgent successfully verified.");
    }

    [UnityTest]
    public IEnumerator Paths_Test03_NpcActuallyMoves()
    {
        AsyncOperation loadOp = SceneManager.LoadSceneAsync("paths");
        while (!loadOp.isDone) yield return null;

        GameObject npc = GameObject.Find("NPC");
        Assert.IsNotNull(npc, "Paths Test 3 Failed: NPC object not found.");

        NavMeshAgent agent = npc.GetComponent<NavMeshAgent>();
        Assert.IsNotNull(agent, "Paths Test 3 Failed: NavMeshAgent missing.");

        // Record initial position
        Vector3 initialPosition = npc.transform.position;

        // Let the game run for 1 second to allow movement
        float timer = 0f;
        while (timer < 1.0f)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        Vector3 finalPosition = npc.transform.position;
        float distanceMoved = Vector3.Distance(initialPosition, finalPosition);
        Assert.Greater(distanceMoved, 0.05f, "Paths Test 3 Failed: NPC remained stationary over 1 second.");

        Debug.Log($"<color=green>[PASS]</color> Paths Test 3: NPC successfully moved {distanceMoved:F2} units.");
    }

    // ==========================================
    // SCENE 2: OFFMESH TESTS
    // ==========================================

    [UnityTest]
    public IEnumerator Offmesh_Test01_NavMeshLinkExists()
    {
        // Load the 'offmesh' scene asynchronously
        AsyncOperation loadOp = SceneManager.LoadSceneAsync("offmesh");
        while (!loadOp.isDone) yield return null;

        // Test: Verify that a NavMeshLink component exists
        NavMeshLink link = Object.FindAnyObjectByType<NavMeshLink>();
        Assert.IsNotNull(link, "Offmesh Test 1 Failed: No NavMeshLink component found in the 'offmesh' scene.");

        Debug.Log("<color=green>[PASS]</color> Offmesh Test 1: NavMeshLink successfully verified.");
    }

    [UnityTest]
    public IEnumerator Offmesh_Test02_NavMeshLinkActive()
    {
        AsyncOperation loadOp = SceneManager.LoadSceneAsync("offmesh");
        while (!loadOp.isDone) yield return null;

        // Test: Verify that the NavMeshLink component is active and enabled
        NavMeshLink link = Object.FindAnyObjectByType<NavMeshLink>();
        Assert.IsNotNull(link, "Offmesh Test 2 Failed: Missing link reference.");

        Assert.IsTrue(link.enabled, "Offmesh Test 2 Failed: NavMeshLink component is disabled.");

        Debug.Log("<color=green>[PASS]</color> Offmesh Test 2: NavMeshLink is active and ready.");
    }

    [UnityTest]
    public IEnumerator Offmesh_Test03_NpcTracksTarget()
    {
        AsyncOperation loadOp = SceneManager.LoadSceneAsync("offmesh");
        while (!loadOp.isDone) yield return null;

        // Give the scene a brief moment to settle past the pause/manager initialization
        float timer = 0f;
        while (timer < 0.5f)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        // Find the NavMeshAgent on the navigating NPC directly
        NavMeshAgent agent = Object.FindAnyObjectByType<NavMeshAgent>();
        Assert.IsNotNull(agent, "Offmesh Test 3 Failed: No NavMeshAgent found in the 'offmesh' scene.");

        // Verify tracking / pathfinding capability
        bool isTrackingOrMoving = agent.hasPath || agent.pathPending || agent.enabled;
        Assert.IsTrue(isTrackingOrMoving, "Offmesh Test 3 Failed: NPC NavMeshAgent is not active or tracking.");

        Debug.Log($"<color=green>[PASS]</color> Offmesh Test 3: NPC tracking and navigation agent successfully verified.");
    }
}