using UnityEngine;

public class GridPreviewManager : MonoBehaviour
{
    [Header("Referencias de mandos")]
    public Transform leftController;
    public Transform rightController;

    [Header("Configuración del Escenario")]
    public GameObject tokenPrefab;
    public int columnasX = 4;
    public int filasZ = 4;
    public float separacion = 1.0f;

    [Header("Parámetros de edición")]
    public float scaleSpeed = 0.5f;
    public float rotationSpeed = 90f;

    [Header("Locomoción")]
    [Tooltip("Arrastra aquí el objeto que tiene el OVRPlayerController")]
    public MonoBehaviour playerController; 

    private GameObject previewRoot;
    private bool editMode = false;

    void Update()
    {
        HandleAButton();
        HandleExitEdit();

        if (!editMode || previewRoot == null)
            return;

        // Ahora ambos ejes del Joystick DERECHO controlan todo
        HandleEditActions();
    }

    void HandleAButton()
    {
        // Botón A (Mando Derecho) para Crear o Reposicionar donde esté el Mando Izquierdo
        if (OVRInput.GetDown(OVRInput.Button.One, OVRInput.Controller.RTouch))
        {
            if (previewRoot == null)
            {
                CreateGrid();
                SetEditMode(true); 
            }
            else if (editMode)
            {
                RepositionGrid();
            }
        }
    }

    void HandleExitEdit()
    {
        // Botón X (Mando Izquierdo) para activar/desactivar edición
        if (OVRInput.GetDown(OVRInput.Button.One, OVRInput.Controller.LTouch))
        {
            SetEditMode(!editMode);
        }
    }

    [Header("Locomoción")]
    [Tooltip("Arrastra aquí el objeto 'Locomotion' que está dentro del OVRInteractionRig")]
    public GameObject playerObject; 

    void SetEditMode(bool state)
    {
        editMode = state;
    
        if (playerObject != null)
        {
            // Al desactivar el objeto Locomotion, los joysticks dejan de mover al jugador
            // y pasan a controlar el escalado y rotación del escenario.
            playerObject.SetActive(!state); 
        }

        Debug.Log(state ? "🟢 MODO EDICIÓN: Movimiento bloqueado" : "🔴 MODO JUEGO: Movimiento activado");
    }

    void CreateGrid()
    {
        previewRoot = new GameObject("Escenario_Preview_Root");
        
        // Se instancia en la posición del mando IZQUIERDO como pediste
        previewRoot.transform.position = leftController.position;
        previewRoot.transform.localScale = Vector3.one * 0.1f;

        float offsetX = (columnasX - 1) * separacion / 2f;
        float offsetZ = (filasZ - 1) * separacion / 2f;

        for (int x = 0; x < columnasX; x++)
        {
            for (int z = 0; z < filasZ; z++)
            {
                Vector3 posicionLocal = new Vector3((x * separacion) - offsetX, 0, (z * separacion) - offsetZ);
                GameObject token = Instantiate(tokenPrefab, previewRoot.transform);
                token.transform.localPosition = posicionLocal;
                
                // Asignar el ID al token si el prefab tiene el script Tile_test
                Tile_test tScript = token.GetComponent<Tile_test>();
                if (tScript != null) tScript.id = new Tile_test.TokenID(x, z);
            }
        }
    }

    void RepositionGrid()
    {
        // El mando izquierdo manda la posición
        previewRoot.transform.position = leftController.position;
    }

    void HandleEditActions()
    {
        // Leemos el Joystick DERECHO únicamente
        Vector2 input = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, OVRInput.Controller.RTouch);

        // 1. Eje Vertical (Y) -> ESCALAR
        if (Mathf.Abs(input.y) > 0.1f)
        {
            float scaleAmount = input.y * scaleSpeed * Time.deltaTime;
            previewRoot.transform.localScale += Vector3.one * scaleAmount;
            
            // Límite de seguridad
            if (previewRoot.transform.localScale.x < 0.01f) 
                previewRoot.transform.localScale = Vector3.one * 0.01f;
        }

        // 2. Eje Horizontal (X) -> ROTAR (Eje Y)
        if (Mathf.Abs(input.x) > 0.1f)
        {
            previewRoot.transform.Rotate(Vector3.up, -input.x * rotationSpeed * Time.deltaTime, Space.World);
        }
    }
}