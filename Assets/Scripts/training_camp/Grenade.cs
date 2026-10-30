using UnityEngine;

public class Grenade : MonoBehaviour
{
    private float timer;
    private float explosionTime = 5.0f; // Increased duration so it travels far slowly
    private bool hasExploded;
    private float radius = 5.0f;
    public GameObject explosion;

    void Start()
    {
        timer = 0.0f;
        hasExploded = false;
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= explosionTime && !hasExploded)
        {
            if (explosion != null)
            {
                Instantiate(explosion, transform.position, Quaternion.identity);
            }

            Collider[] colliders = Physics.OverlapSphere(transform.position, radius);
            foreach (var col in colliders)
            {
                if (col.CompareTag("target"))
                {
                    ManageNPC npc = col.GetComponent<ManageNPC>();
                    if (npc != null) npc.gotHitByGrenade();
                }
            }

            hasExploded = true;
            Destroy(gameObject);
        }
    }
}