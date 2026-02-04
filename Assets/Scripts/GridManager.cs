using UnityEngine;

public class GridManagerWithBorders : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject tilePrefab;        // Prefab del tile interior
    public GameObject borderPrefab;      // Prefab de los bordes laterales
    public GameObject cornerPrefab;      // Prefab de las esquinas

    [Header("Tamaño y rotación")]
    public float tileSize = 2f;          // Tamaño que ocupa cada tile
    public Vector3 tileRotation = new Vector3(86.803f, -5.987f, -4.2340f);
    public Vector3 borderRotation = new Vector3(-89.98f, 0f, 0f);
    public Vector3 cornerRotation = new Vector3(-89.98f, 0f, 0f);

    /// <summary>
    /// Genera un grid cuadrado con bordes y esquinas centrado en el origin
    /// </summary>
    public void GenerateGrid(int size)
    {
        if (tilePrefab == null)
        {
            Debug.LogError("No se ha asignado tilePrefab");
            return;
        }

        // Limpiar grid anterior
        foreach (Transform child in transform)
            Destroy(child.gameObject);

        float offset = (size - 1) * tileSize / 2f;
        Debug.Log($"Generando grid {size}x{size}, tileSize: {tileSize}, offset: {offset}");

        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                Vector3 position = new Vector3(
                    x * tileSize - offset,
                    0f,
                    y * tileSize - offset
                );

                GameObject prefabToUse = tilePrefab; // default

                // Detectar esquinas
                bool esEsquina = (x == 0 && y == 0) || (x == 0 && y == size - 1) ||
                                 (x == size - 1 && y == 0) || (x == size - 1 && y == size - 1);

                // Detectar bordes (no esquinas)
                bool esBorde = !esEsquina && (x == 0 || y == 0 || x == size - 1 || y == size - 1);

                if (esEsquina && cornerPrefab != null)
                    prefabToUse = cornerPrefab;
                else if (esBorde && borderPrefab != null)
                    prefabToUse = borderPrefab;

                // Instanciar el prefab adecuado
                GameObject tileGO = Instantiate(
                    prefabToUse,
                    position,
                    Quaternion.Euler(prefabToUse == tilePrefab ? tileRotation :
                                     prefabToUse == borderPrefab ? borderRotation :
                                     cornerRotation),
                    transform
                );

                // Asignar ID si existe componente Tile
                Tile tile = tileGO.GetComponent<Tile>();
                if (tile != null)
                    tile.id = new TokenID(x + 1, y + 1);

                tileGO.name = $"Tile {x + 1},{y + 1} ({prefabToUse.name})";
                Debug.Log($"Instanciado {prefabToUse.name} en {position}");
            }
        }
    }
}
