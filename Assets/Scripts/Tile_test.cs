using UnityEngine;
/*
public class Tile_test : MonoBehaviour
{
    [System.Serializable]
    public struct TokenID
    {
        public int x;
        public int y;

        public TokenID(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public override string ToString()
        {
            return $"({x}, {y})";
        }
    }

    [Header("Panel")]
    [SerializeField] private Renderer panelRenderer;

    [Header("Materiales")]
    [SerializeField] private Material Apagat; // Ej: Apagat
    [SerializeField] private Material Verd; // Ej: Verd
    [SerializeField] private Material Blau; // Ej: Blau
    [SerializeField] private Material Vermell; // Otro color adicional si quieres

    [Header("ID del Tile")]
    public TokenID id;

    private Material currentMaterial;
    private bool playerDentro = false;

    private GameManager gameManager;

    private void Start()
    {
        // Buscar autom�ticamente el hijo llamado "panel"
        if (panelRenderer == null)
            panelRenderer = transform.Find("panel")?.GetComponent<Renderer>();

        // Buscar GameManager
        gameManager = FindObjectOfType<GameManager>();
        if (gameManager == null)
            Debug.LogError("No se ha encontrado GameManager en la escena");

        // Preguntar al GameManager qu� material debe tener seg�n el ID
        if (gameManager != null)
        {
            currentMaterial = gameManager.GetMaterialForTile(id);
            if (panelRenderer != null && currentMaterial != null)
                panelRenderer.material = currentMaterial;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !playerDentro)
        {
            playerDentro = true;

            // Notificar al GameManager que este tile ha sido pulsado
            if (gameManager != null)
            {
                gameManager.TilePressed(id, this);
            }

            // Cambiar visual del tile pulsado seg�n GameManager
            if (panelRenderer != null && currentMaterial != null)
                panelRenderer.material = currentMaterial;

            Debug.Log($"Player ENTRA en Tile {id} con material {currentMaterial.name}");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && playerDentro)
        {
            playerDentro = false;

            // Notificar al GameManager que ha dejado de pulsar el tile
            if (gameManager != null)
                gameManager.TileReleased(id, this);

            // Mantener material seg�n currentMaterial asignado por GameManager
            if (panelRenderer != null && currentMaterial != null)
                panelRenderer.material = currentMaterial;

            Debug.Log($"Player SALE de Tile {id} con material {currentMaterial.name}");
        }
    }

    // Mtodo p�blico para que GameManager pueda cambiar el material
    public void SetMaterial(Material newMaterial)
    {
        currentMaterial = newMaterial;
        if (panelRenderer != null && currentMaterial != null)
            panelRenderer.material = currentMaterial;
    }

    // Mtodo para obtener los 4 materiales si se necesitan
    public Material GetApagat( ) => Apagat;
    public Material GetVerd() => Verd;
    public Material GetBlau() => Blau;
    public Material GetVermell() => Vermell;
}
*/