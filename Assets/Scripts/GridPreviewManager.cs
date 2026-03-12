using UnityEngine;
using UnityEngine.SceneManagement;

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

    [Header("Locomoción (OVRInteractionRig)")]
    [Tooltip("Arrastra aquí el objeto 'Locomotion' que está dentro de tu OVRInteractionRig")]
    public GameObject playerObject; 

    [Header("Passthrough (Building Block)")]
    public OVRPassthroughLayer passthroughLayer;

    private GameObject previewRoot;
    private bool editMode = false;

    void Update()
    {
        HandleInputBase();

        if (editMode && previewRoot != null)
        {
            HandleEditActions();
        }
    }

    // =====================================================
    // ENTRADA DE CONTROLES
    // =====================================================
    void HandleInputBase()
    {
        // Botón A (Derecho): Crear o Reposicionar
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

        // Botón X (Izquierdo): Alternar modo edición
        if (OVRInput.GetDown(OVRInput.Button.One, OVRInput.Controller.LTouch))
        {
            SetEditMode(!editMode);
        }
    }

    void HandleEditActions()
    {
        Vector2 input = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, OVRInput.Controller.RTouch);

        // Escalar (Vertical)
        if (Mathf.Abs(input.y) > 0.1f)
        {
            float scaleAmount = input.y * scaleSpeed * Time.deltaTime;
            previewRoot.transform.localScale += Vector3.one * scaleAmount;
            if (previewRoot.transform.localScale.x < 0.01f) previewRoot.transform.localScale = Vector3.one * 0.01f;
        }

        // Rotar (Horizontal)
        if (Mathf.Abs(input.x) > 0.1f)
        {
            previewRoot.transform.Rotate(Vector3.up, -input.x * rotationSpeed * Time.deltaTime, Space.World);
        }
    }

    // =====================================================
    // GESTIÓN DEL ESCENARIO
    // =====================================================
    void CreateGrid()
    {
        previewRoot = new GameObject("Escenario_Preview_Root");
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
                
                Tile_test tileScript = token.GetComponent<Tile_test>();
                if (tileScript != null) tileScript.id = new Tile_test.TokenID(x, z);
            }
        }
    }

    void RepositionGrid() => previewRoot.transform.position = leftController.position;

    void SetEditMode(bool state)
    {
        editMode = state;
        if (playerObject != null) playerObject.SetActive(!state);
    }

    // =====================================================
    // FUNCIONES UI
    // =====================================================
    public void ConfirmarAjustes()
    {
        if (previewRoot != null)
        {
            PlayerPrefs.SetFloat("EscenarioPosX", previewRoot.transform.position.x);
            PlayerPrefs.SetFloat("EscenarioPosY", previewRoot.transform.position.y);
            PlayerPrefs.SetFloat("EscenarioPosZ", previewRoot.transform.position.z);
            PlayerPrefs.SetFloat("EscenarioRotY", previewRoot.transform.eulerAngles.y);
            PlayerPrefs.SetFloat("EscenarioEscala", previewRoot.transform.localScale.x);
            PlayerPrefs.Save();
            SceneManager.LoadScene("Countdown");
        }
    }

    public void ReiniciarAjustes()
    {
        if (previewRoot != null) { Destroy(previewRoot); previewRoot = null; SetEditMode(false); }
    }

    public void AlternarPassthrough()
    {
        if (passthroughLayer != null)
        {
            // 1. Alternamos el componente
            passthroughLayer.enabled = !passthroughLayer.enabled;
            
            // 2. Ajustamos la cámara para que el fondo sea transparente
            Camera cam = Camera.main;
            if (cam != null)
            {
                if (passthroughLayer.enabled)
                {
                    cam.clearFlags = CameraClearFlags.SolidColor;
                    cam.backgroundColor = new Color(0, 0, 0, 0); // Transparente
                }
                else
                {
                    cam.clearFlags = CameraClearFlags.Skybox;
                }
            }
        }
    }
}