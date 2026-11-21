using UnityEngine;

public class BCCleanup : MonoBehaviour
{
    void Start()
    {
        Destroy(gameObject, 5f);
    }
}