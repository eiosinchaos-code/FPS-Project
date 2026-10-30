using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class ManageNPC2 : MonoBehaviour
{
    private int health;
    public GameObject smoke;
    public GameObject ammoPrefab; // Drag your ammo_gun prefab here in the Inspector
    private bool isDead = false;

    void Start()
    {
        health = 100;
    }

    public void gotHit()
    {
        if (isDead) return;

        print("Got hit by bullet");
        GetComponent<ControlNPCFSM>().SetGotHitParameter();
        health -= 50;
    }

    public void DestroyTarget()
    {
        if (isDead) return;
        isDead = true;

        // Trigger the low health / falling animation state on the FSM script
        GetComponent<ControlNPCFSM>().SetLowHealthParameter();

        // Start a delayed routine to handle smoke, ammo drop, and cleanup cleanly
        StartCoroutine(DeathRoutine());
    }

    private IEnumerator DeathRoutine()
    {
        // Wait 0.5 seconds to let the death transition start cleanly
        yield return new WaitForSeconds(0.5f);

        // Spawn smoke effect (dissipates in 1.5 seconds)
        if (smoke != null)
        {
            GameObject lastSmoke = Instantiate(smoke, transform.position, Quaternion.identity);
            Destroy(lastSmoke, 1.5f);
        }

        // Spawn the ammo box slightly above the guard
        if (ammoPrefab != null)
        {
            Vector3 spawnPos = transform.position + Vector3.up * 0.5f;
            GameObject spawnedAmmo = Instantiate(ammoPrefab, spawnPos, Quaternion.identity);

            // Ignore collisions between the falling guard corpse and the new ammo box
            Collider guardCollider = GetComponent<Collider>();
            Collider ammoCollider = spawnedAmmo.GetComponent<Collider>();
            if (guardCollider != null && ammoCollider != null)
            {
                Physics.IgnoreCollision(guardCollider, ammoCollider);
            }
        }

        // Keep the guard body around for the rest of the 5-second animation, then delete
        yield return new WaitForSeconds(4.5f);
        Destroy(gameObject);
    }

    void Update()
    {
        if (health <= 0 && !isDead)
        {
            DestroyTarget();
        }
    }

    public void gotHitByGrenade()
    {
        if (isDead) return;

        print("Hit by grenade");
        GetComponent<ControlNPCFSM>().SetGotHitParameter();
        health = 0;
    }
}