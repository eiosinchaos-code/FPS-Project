using UnityEngine;

public class Grenade : MonoBehaviour
{
    public float grenadeTimer;
    public bool grenadeTimerStarted;
    public float grenadeTimerLimit;
    public bool grenadeExplode;
    public GameObject explosion;
    private float radius = 5.0f;
    private float power = 500.0f;
    private float timer;
    private float explosionTime;
    private bool hasExploded;

    void Start()
    {
        timer = 0.0f;
        explosionTime = 2.0f;
        hasExploded = false;
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= explosionTime)
        {
            if (hasExploded == false)
            {
                // Optional: Instantiate an explosion visual effect if assigned
                if (explosion != null)
                {
                    Instantiate(explosion, transform.position, Quaternion.identity);
                }

                Vector3 explosionPos = gameObject.transform.position;
                Collider[] colliders = Physics.OverlapSphere(explosionPos, radius);

                for (int i = 0; i < colliders.Length; i++)
                {
                    // Fixed tag check syntax (!= instead of ! =)
                    if (colliders[i].gameObject.GetComponent<Rigidbody>() != null && colliders[i].gameObject.tag != "Player")
                    {
                        GameObject objectTargeted = colliders[i].gameObject;

                        if (objectTargeted.tag == "target")
                        {
                            ManageNPC npc = objectTargeted.GetComponent<ManageNPC>();
                            if (npc != null)
                            {
                                // Ensure this method exists in your ManageNPC script
                                npc.gotHit(); // Or npc.gotHitByGrenade() if you implemented that specifically
                            }
                        }
                    }
                }
                GameObject.Instantiate(explosion, transform.position, Quaternion.identity);
                hasExploded = true;
                Destroy(gameObject);
            }
        }
    }
}