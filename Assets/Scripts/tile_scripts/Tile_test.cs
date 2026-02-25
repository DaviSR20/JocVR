using UnityEngine;

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
    [SerializeField] private Material Apagat;
    [SerializeField] private Material Verd;
    [SerializeField] private Material Blau;
    [SerializeField] private Material Vermell;
    [SerializeField] private Material Rosa;

    [Header("ID del Tile")]
    public TokenID id;

    private Material currentMaterial;
    private bool playerDentro = false;

    private GameManager gameManager;

    private void Start()
    {
        if (panelRenderer == null)
            panelRenderer = transform.Find("panel")?.GetComponent<Renderer>();

        gameManager = FindObjectOfType<GameManager>();
        if (gameManager == null)
            Debug.LogError("No se ha encontrado GameManager en la escena");

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

            if (gameManager != null)
                gameManager.TilePressed(id, this);

            Debug.Log($"Player ENTRA en Tile {id} con material {currentMaterial.name}");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && playerDentro)
        {
            playerDentro = false;

            if (gameManager != null)
                gameManager.TileReleased(id, this);

            Debug.Log($"Player SALE de Tile {id} con material {currentMaterial.name}");
        }
    }

    // Método para que GameManager cambie el material
    public void SetMaterial(Material newMaterial)
    {
        currentMaterial = newMaterial;
        if (panelRenderer != null && currentMaterial != null)
            panelRenderer.material = currentMaterial;
    }

    // Método para obtener el material actual
    public Material GetMaterialActual()
    {
        return currentMaterial;
    }

    // Métodos para acceder a los materiales
    public Material GetApagat() => Apagat;
    public Material GetVerd() => Verd;
    public Material GetBlau() => Blau;
    public Material GetVermell() => Vermell;
    public Material GetRosa() => Rosa;
}