using UnityEngine;

public class Collisionwithprojectile : MonoBehaviour
{
    public GameObject explosion;

    void Start()
    {

    }

    void Update()
    {

    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            GameObject startPoint = GameObject.Find("start");
            if (startPoint != null)
            {
                collision.gameObject.transform.position = startPoint.transform.position;
            }

            if (explosion != null)
            {
                Instantiate(explosion, transform.position, Quaternion.identity);
            }

            Destroy(gameObject);
        }
    }
}