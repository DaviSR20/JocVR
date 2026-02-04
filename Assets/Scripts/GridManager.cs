using UnityEngine;

public class GridManager : MonoBehaviour
{
    public GameObject tilePrefab;
    public float tileSize = 1f; // tamaño por defecto si no se puede calcular

    public void GenerateGrid(int size)
    {
        // Limpiar grid anterior
        foreach (Transform child in transform)
            Destroy(child.gameObject);

        // Intentar calcular el tamaño real del prefab
        float realTileSize = tileSize;

        // 1️⃣ Usar Renderer si existe
        Renderer r = tilePrefab.GetComponentInChildren<Renderer>();
        if (r != null)
        {
            realTileSize = Mathf.Max(r.bounds.size.x, r.bounds.size.z);
            Debug.Log($"Tile size calculado por Renderer: {realTileSize}");
        }
        else
        {
            // 2️⃣ Usar Collider si hay
            Collider c = tilePrefab.GetComponentInChildren<Collider>();
            if (c != null)
            {
                realTileSize = Mathf.Max(c.bounds.size.x, c.bounds.size.z);
                Debug.Log($"Tile size calculado por Collider: {realTileSize}");
            }
            else
            {
                // 3️⃣ Usar Scale del prefab
                realTileSize = Mathf.Max(
                    tilePrefab.transform.localScale.x,
                    tilePrefab.transform.localScale.z
                );
                Debug.Log($"Tile size calculado por Scale: {realTileSize}");
            }
        }

        float offset = (size - 1) * realTileSize / 2f;

        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                Vector3 position = new Vector3(
                    x * realTileSize - offset,
                    0f,
                    y * realTileSize - offset
                );

                GameObject tileGO = Instantiate(
                    tilePrefab,
                    position,
                    Quaternion.Euler(90f, 0f, 0f), // rotación para plano
                    transform
                );

                Tile tile = tileGO.GetComponent<Tile>();
                if (tile != null)
                    tile.id = new TokenID(x + 1, y + 1);

                tileGO.name = $"Tile {x + 1},{y + 1}";

                // Log de depuración
                Debug.Log($"Instanciando Tile {x+1},{y+1} en {position} con size {realTileSize}");
            }
        }
    }
}
