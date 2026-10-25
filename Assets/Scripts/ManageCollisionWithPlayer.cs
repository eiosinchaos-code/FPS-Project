using UnityEngine;

public class ManageCollisionWithPlayer : MonoBehaviour
{
    void Start()
    {

    }

    void Update()
    {

    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (transform.childCount > 0)
        {
            ManageWeapons weaponsScript = transform.GetChild(0).GetComponent<ManageWeapons>();
            if (weaponsScript != null)
            {
                weaponsScript.manageCollisions(hit);
            }
        }
    }
}