using UnityEngine;

public class ConfiguradorEscenario : MonoBehaviour
{
    [Header("Referencias")]
    public GameObject escenarioPrefab;
    public Transform manoIzquierda; // Arrastra el LeftControllerAnchor
    public OVRPassthroughLayer passthroughLayer;

    private GameObject instanciaPreview;
    private bool previewActivo = false;

    [Header("Ajustes de Transform")]
    public float velocidadRotacion = 100f;
    public float velocidadEscala = 0.5f;

    public void TogglePreview()
    {
        if (!previewActivo)
        {
            instanciaPreview = Instantiate(escenarioPrefab, manoIzquierda.position, manoIzquierda.rotation);
            // Hacerlo hijo de la mano para que lo siga inicialmente o dejarlo libre
            previewActivo = true;
        }
    }

    void Update()
    {
        if (previewActivo && instanciaPreview != null)
        {
            // 1. Posición: Sigue a la mano izquierda
            instanciaPreview.transform.position = manoIzquierda.position;

            // 2. Rotación: Joystick Derecho Horizontal
            Vector2 joystickDerecho = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, OVRInput.Controller.RTouch);
            instanciaPreview.transform.Rotate(Vector3.up, -joystickDerecho.x * velocidadRotacion * Time.deltaTime);

            // 3. Escala: Joystick Derecho Vertical
            float factorEscala = joystickDerecho.y * velocidadEscala * Time.deltaTime;
            instanciaPreview.transform.localScale += new Vector3(factorEscala, factorEscala, factorEscala);
        }
    }

    public void TogglePassthrough()
    {
        if (passthroughLayer != null)
            passthroughLayer.enabled = !passthroughLayer.enabled;
    }

    public void ConfirmarAjustes()
    {
        // Guardar posición/escala en PlayerPrefs o una variable estática para la siguiente escena
        previewActivo = false;
        // Cambiar de escena...
    }
}