using Unity.VisualScripting;
using UnityEngine;

public class ManageNPC : MonoBehaviour
{
    private int health;
    public GameObject smoke;
    public GameObject ammoPrefab; // Drag your ammo_gun prefab here in the Inspector

    void Start()
    {
        health = 100;
    }

    public void gotHit()
    {
        health -= 50;
    }

    public void DestroyTarget()
    {
        // 1. Spawn smoke effect if assigned
        if (smoke != null)
        {
            GameObject lastSmoke = Instantiate(smoke, transform.position, Quaternion.identity);
            Destroy(lastSmoke, 3f);
        }

        // 2. Spawn the ammo_gun where the target was destroyed
        if (ammoPrefab != null)
        {
            Instantiate(ammoPrefab, transform.position, Quaternion.identity);
        }

        // 3. Destroy the target object
        Destroy(gameObject);
    }

    void Update()
    {
        if (health <= 0)
        {
            DestroyTarget();
        }
    }
    public void gotHitByGrenade()
    {
        print("Hit by grenade");
        health = 0;
    } 
}