using UnityEngine;

public class GridManager : MonoBehaviour
{
    public GameObject tilePrefab;
    public float tileSize = 1f;

    public void GenerateGrid(int size)
    {
        // Limpiar grid anterior
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        float offset = (size - 1) * tileSize / 2f;

        for (int x = 1; x <= size; x++)
        {
            for (int y = 1; y <= size; y++)
            {
                Vector3 position = new Vector3(
                    (x - 1) * tileSize - offset,
                    0,
                    (y - 1) * tileSize - offset
                );

                GameObject tileGO = Instantiate(
                    tilePrefab,
                    position,
                    Quaternion.identity,
                    transform
                );

                // Asignar TokenID
                Tile tile =
                    tileGO.GetComponent<Tile>();

                tile.id = new TokenID(x, y);

                // Nombre en la jerarquía (muy útil)
                tileGO.name = $"Tile {tile.id}";
            }
        }
    }
}