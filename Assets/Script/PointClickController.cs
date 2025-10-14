using UnityEngine;

public class PointClickController : MonoBehaviour
{
    public LayerMask groundMask; // Layer tanah/area klik
    public Vector3 targetPosition;
    public bool hasTarget = false;

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Klik kiri mouse
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 100f, groundMask))
            {
                targetPosition = hit.point;
                hasTarget = true;
            }
        }
    }
}