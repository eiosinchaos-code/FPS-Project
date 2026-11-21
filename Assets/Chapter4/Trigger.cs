using UnityEngine;

public class Trigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider coll)
    {
        if (coll.gameObject.CompareTag("Player") || coll.gameObject.name == "FPSController")
        {
            GameObject npc = GameObject.Find("NPC3");
            if (npc != null)
            {
                npc.GetComponent<ControlNPC3>()?.PlayerEnteredAmbushArea();
            }
        }
    }
}