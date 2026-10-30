using UnityEngine;

public class Grenade2 : MonoBehaviour
{
    private float radius = 5.0f;
    public GameObject explosion;
    private bool hasExploded = false;

    private float spawnTimer = 0.0f;
    private float armingDelay = 0.1f; // Shortened slightly so it can detonate quickly on impact

    void Start()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.useGravity = true;
            rb.linearDamping = 0.05f;

            // Lowered and flattened arc trajectory (reduced upward force from 6.0f to 1.5f)
            Vector3 arcVelocity = (transform.forward * 8.0f) + (Vector3.up * 1.5f);
            rb.linearVelocity = arcVelocity;
        }
    }

    void Update()
    {
        if (spawnTimer < armingDelay)
        {
            spawnTimer += Time.deltaTime;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (spawnTimer < armingDelay) return;
        if (hasExploded) return;
        hasExploded = true;

        if (explosion != null)
        {
            Instantiate(explosion, transform.position, Quaternion.identity);
        }

        Collider[] colliders = Physics.OverlapSphere(transform.position, radius);
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].gameObject.CompareTag("target"))
            {
                ManageNPC2 npc2 = colliders[i].gameObject.GetComponent<ManageNPC2>();
                if (npc2 != null) npc2.gotHitByGrenade();
            }
        }

        Destroy(gameObject);
    }
}