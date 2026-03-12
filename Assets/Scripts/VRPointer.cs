using UnityEngine;

public class VRPointer : MonoBehaviour
{
    private LineRenderer line;
    public float distanciaMax = 5f;

    void Start() => line = GetComponent<LineRenderer>();

    void Update()
    {
        line.SetPosition(0, transform.position);
        
        // Lanzamos un rayo hacia adelante para ver si golpea la UI
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, distanciaMax))
        {
            line.SetPosition(1, hit.point);
        }
        else
        {
            line.SetPosition(1, transform.position + transform.forward * distanciaMax);
        }
    }
}