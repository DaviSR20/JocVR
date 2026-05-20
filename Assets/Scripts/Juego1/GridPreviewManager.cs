using System.Collections; // Necesario para las corrutinas de vibración
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

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
    
    [FormerlySerializedAs("pcGameManager")] [Header("Referencias de Escena")]
    public PcGameManager PcGameManager; // Arrastra aquí el objeto con PcGameManager
    public GameObject canvasMenu;
    public GameObject Laser;

    [Header("Locomoción (OVRInteractionRig)")]
    [Tooltip("Arrastra aquí el objeto 'Locomotion' que está dentro de tu OVRInteractionRig")]
    public GameObject playerObject; 

    [Header("Passthrough (Building Block)")]
    public OVRPassthroughLayer passthroughLayer;

    private GameObject previewRoot;
    private bool editMode = false;
    
    // Nueva variable para recordar si el gatillo estaba pulsado en el frame anterior
    private bool wasTriggerHeld = false; 

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

        // Botón B (Derecho): Alternar modo edición
        if (OVRInput.GetDown(OVRInput.Button.Two, OVRInput.Controller.RTouch))
        {
            SetEditMode(!editMode);
        }
    }

    // =====================================================
    // LÓGICA DE EDICIÓN CON GATILLO MODIFICADOR Y VIBRACIÓN
    // =====================================================
    void HandleEditActions()
    {
        Vector2 input = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, OVRInput.Controller.RTouch);
        float triggerValue = OVRInput.Get(OVRInput.RawAxis1D.RIndexTrigger);
        bool isTriggerHeld = triggerValue > 0.1f;

        // --- LÓGICA DE HAPTIC FEEDBACK ---
        // Si acaba de pulsar el gatillo (Entra en Modo Escala)
        if (isTriggerHeld && !wasTriggerHeld)
        {
            StartCoroutine(VibrateController(0.1f, 1f, 0.8f, OVRInput.Controller.RTouch));
        }
        // Si acaba de soltar el gatillo (Vuelve a Modo Rotación)
        else if (!isTriggerHeld && wasTriggerHeld)
        {
            StartCoroutine(VibrateController(0.1f, 0.5f, 0.3f, OVRInput.Controller.RTouch));
        }
        
        // Guardamos el estado del gatillo para el próximo frame
        wasTriggerHeld = isTriggerHeld;
        // ----------------------------------

        if (Mathf.Abs(input.x) > 0.1f)
        {
            if (isTriggerHeld)
            {
                // MODO ESCALAR
                float scaleAmount = input.x * scaleSpeed * Time.deltaTime;
                previewRoot.transform.localScale += Vector3.one * scaleAmount;
                
                if (previewRoot.transform.localScale.x < 0.01f) 
                {
                    previewRoot.transform.localScale = Vector3.one * 0.01f;
                }
            }
            else
            {
                // MODO ROTAR
                previewRoot.transform.Rotate(Vector3.up, -input.x * rotationSpeed * Time.deltaTime, Space.World);
            }
        }
    }

    // =====================================================
    // CORRUTINA PARA CONTROLAR LA VIBRACIÓN
    // =====================================================
    private IEnumerator VibrateController(float duration, float frequency, float amplitude, OVRInput.Controller controller)
    {
        // Encendemos el motor de vibración
        OVRInput.SetControllerVibration(frequency, amplitude, controller);
        
        // Esperamos los segundos indicados
        yield return new WaitForSeconds(duration);
        
        // Apagamos el motor de vibración
        OVRInput.SetControllerVibration(0, 0, controller);
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
            if (PcGameManager != null) PcGameManager.IniciarJuego();
            SetEditMode(false);
            if (canvasMenu != null) canvasMenu.SetActive(false);
            if (Laser != null) Laser.SetActive(false);
            
            this.enabled = false;
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
            passthroughLayer.enabled = !passthroughLayer.enabled;
            Camera cam = Camera.main;
            if (cam != null)
            {
                if (passthroughLayer.enabled)
                {
                    cam.clearFlags = CameraClearFlags.SolidColor;
                    cam.backgroundColor = new Color(0, 0, 0, 0); 
                }
                else
                {
                    cam.clearFlags = CameraClearFlags.Skybox;
                }
            }
        }
    }
}