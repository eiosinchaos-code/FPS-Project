
using UnityEngine;

public class ManageCollisionWithPlayer2 : MonoBehaviour
{
    private ManageWeapons2 weaponsScript;

    void Awake()
    {
        weaponsScript = GetComponentInChildren<ManageWeapons2>(true);

        if (weaponsScript == null)
        {
            Debug.LogError(
                "ManageWeapons2 could not be found on the player hierarchy!"
            );
        }
        else
        {
            Debug.Log("ManageWeapons2 successfully found.");
        }
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.collider == null || weaponsScript == null)
            return;

        string objectTag = hit.collider.tag;

        // Only process recognized pickup objects.
        switch (objectTag)
        {
            case "pick_me":
            case "health_pack":
            case "ammo_gun":
            case "ammo_automatic_gun":
            case "ammo_grenade":

                Debug.Log(
                    "PICKUP COLLISION: " +
                    hit.collider.name +
                    " | TAG: " + objectTag
                );

                weaponsScript.manageCollisions(hit);
                break;
        }
    }
}
