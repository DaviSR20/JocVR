using UnityEngine;

public class VRPointer : MonoBehaviour
{
    private LineRenderer line;
    private MenuVR lastButton;

    [Header("Laser")]
    public float distanciaMax = 5f;
    public float lineWidth = 0.01f;

    [Header("Color")]
    public Color startColor = Color.red;
    public Color endColor = Color.red;

    void Awake()
    {
        CrearLaser();
    }

    void CrearLaser()
    {
        line = GetComponent<LineRenderer>();

        // Si no existe -> crearlo automáticamente
        if (line == null)
        {
            line = gameObject.AddComponent<LineRenderer>();
        }

        // Configuración básica
        line.positionCount = 2;

        line.startWidth = lineWidth;
        line.endWidth = lineWidth;

        line.material = new Material(
            Shader.Find("Unlit/Color"));

        line.startColor = startColor;
        line.endColor = endColor;

        line.useWorldSpace = true;

        line.shadowCastingMode =
            UnityEngine.Rendering.ShadowCastingMode.Off;

        line.receiveShadows = false;
    }

    void Update()
    {
        if (line == null)
            return;

        line.SetPosition(0, transform.position);

        RaycastHit hit;

        if (Physics.Raycast(
            transform.position,
            transform.forward,
            out hit,
            distanciaMax))
        {
            line.SetPosition(1, hit.point);

            MenuVR button =
                hit.collider.GetComponentInParent<MenuVR>();

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
                ClearHover();
            }
        }
        else
        {
            line.SetPosition(
                1,
                transform.position +
                transform.forward * distanciaMax);

            ClearHover();
        }
    }

    void ClearHover()
    {
        if (lastButton != null)
        {
            lastButton.OnHoverExit();
            lastButton = null;
        }
    }
}