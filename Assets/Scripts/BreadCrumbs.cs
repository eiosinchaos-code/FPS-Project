using UnityEngine;

public class BreadCrumbs : MonoBehaviour
{
    private Vector3 previousPosition;
    private float counter = 0;
    public GameObject BC; // Drag your BC prefab here in the Inspector

    void Start()
    {
        previousPosition = transform.position;
    }

    void Update()
    {
        Vector3 currentLocation = transform.position;
        float distance = Vector3.Distance(previousPosition, currentLocation);

        // Drop a breadcrumb marker every 1 meter of movement
        if (distance > 1.0f)
        {
            previousPosition = currentLocation;

            if (BC != null)
            {
                GameObject g = Instantiate(BC, currentLocation, Quaternion.identity);
                g.name = "BC_" + counter;
                counter++;
            }
        }
    }
}