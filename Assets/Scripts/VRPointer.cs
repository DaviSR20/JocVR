using UnityEngine;

public class VRPointer : MonoBehaviour
{
    private LineRenderer line;
    private MenuVR lastButton;
    public float distanciaMax = 5f;

    void Start()
    {
        line = GetComponent<LineRenderer>();
    }

    void Update()
    {
        line.SetPosition(0, transform.position);

        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, distanciaMax))
        {
            line.SetPosition(1, hit.point);

            MenuVR button = hit.collider.GetComponentInParent<MenuVR>();
            Debug.Log(hit.collider.name);
            if (button != null)
            {
                if (lastButton != button)
                {
                    if (lastButton != null)
                        lastButton.OnHoverExit();

                    button.OnHoverEnter();
                    lastButton = button;
                }
            }
            else
            {
                if (lastButton != null)
                {
                    lastButton.OnHoverExit();
                    lastButton = null;
                }
            }
        }
        else
        {
            line.SetPosition(1, transform.position + transform.forward * distanciaMax);

            if (lastButton != null)
            {
                lastButton.OnHoverExit();
                lastButton = null;
            }
        }
    }
}