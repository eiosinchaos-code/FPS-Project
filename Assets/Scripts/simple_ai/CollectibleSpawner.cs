using UnityEngine;

public class CollectibleSpawner : MonoBehaviour
{
    public GameObject collectiblePrefab; // Drag your 'pick_me' prefab here
    public float spawnAreaWidth = 15f;   // How far left/right items can spawn
    public float spawnAreaDepth = 15f;   // How far forward/back items can spawn
    public float spawnHeight = 0.5f;     // Height offset from the ground

    private int lastTargetScore = -1;

    void Update()
    {
        CollisionWithPlayer player = CollisionWithPlayer.Instance;
        if (player == null) return;

        int targetScore = player.GetTargetScore();

        // If the player changed the target score dropdown, adjust the collectibles on the map
        if (targetScore != lastTargetScore)
        {
            lastTargetScore = targetScore;
            AdjustCollectiblesCount(targetScore);
        }
    }

    void AdjustCollectiblesCount(int requiredAmount)
    {
        if (collectiblePrefab == null)
        {
            Debug.LogWarning("Collectible Prefab is not assigned on the Spawner!");
            return;
        }

        // Find all currently existing collectibles with the "pick_me" tag
        GameObject[] existingCollectibles = GameObject.FindGameObjectsWithTag("pick_me");
        int currentCount = existingCollectibles.Length;

        if (currentCount < requiredAmount)
        {
            // Spawn the missing amount
            int amountToSpawn = requiredAmount - currentCount;
            for (int i = 0; i < amountToSpawn; i++)
            {
                Vector3 randomOffset = new Vector3(
                    Random.Range(-spawnAreaWidth, spawnAreaWidth),
                    spawnHeight,
                    Random.Range(-spawnAreaDepth, spawnAreaDepth)
                );

                Vector3 spawnPosition = transform.position + randomOffset;
                Instantiate(collectiblePrefab, spawnPosition, Quaternion.identity);
            }
            Debug.Log("Spawned " + amountToSpawn + " extra collectibles to match target score of " + requiredAmount);
        }
        else if (currentCount > requiredAmount)
        {
            // Optional: Clean up extra ones if the player drops the target score back down
            int amountToRemove = currentCount - requiredAmount;
            for (int i = 0; i < amountToRemove; i++)
            {
                Destroy(existingCollectibles[i]);
            }
            Debug.Log("Removed " + amountToRemove + " excess collectibles.");
        }
    }
}