using UnityEngine;
using System.Collections.Generic;

public class GridManagerWithBorders : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject tilePrefab;
    public GameObject borderPrefab;
    public GameObject cornerPrefab;

    [Header("Tamaño y rotación")]
    public float tileSize = 2f;
    private Vector3 tileRotation = new Vector3(86.803f, -5.987f, -4.2340f);

    [Header("Ajustes de bordes")]
    public float borderInset = 3.6f;

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

        float centerIndex = (size - 1) / 2f;
        List<GameObject> coreTiles = new List<GameObject>();

        // --- 1️⃣ Instanciamos solo tiles normales ---
        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                bool esBorde = (x == 0 || y == 0 || x == size - 1 || y == size - 1);
                bool esEsquina = 
                    (x == 0 && y == 0) ||
                    (x == 0 && y == size - 1) ||
                    (x == size - 1 && y == 0) ||
                    (x == size - 1 && y == size - 1);

                if (!esBorde && !esEsquina) // solo core
                {
                    Vector3 position = new Vector3(
                        (x - centerIndex) * tileSize,
                        0f,
                        (y - centerIndex) * tileSize
                    );

                    GameObject tileGO = Instantiate(tilePrefab, transform);
                    tileGO.transform.localPosition = position;
                    tileGO.transform.localRotation = Quaternion.Euler(tileRotation);
                    tileGO.transform.localScale = Vector3.one * 1.1f;
                    tileGO.name = $"Tile {x + 1},{y + 1} (Core)";

                    coreTiles.Add(tileGO);
                }
            }
        }

        // --- 2️⃣ Centramos el core en 0,0,0 ---
        CenterTilesCore(coreTiles);

        // --- 3️⃣ Instanciamos bordes y esquinas relativos al core ---
        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                bool esEsquina =
                    (x == 0 && y == 0) ||
                    (x == 0 && y == size - 1) ||
                    (x == size - 1 && y == 0) ||
                    (x == size - 1 && y == size - 1);

                bool esBorde =
                    !esEsquina &&
                    (x == 0 || y == 0 || x == size - 1 || y == size - 1);

                if (esBorde || esEsquina)
                {
                    Vector3 position = new Vector3(
                        (x - centerIndex) * tileSize,
                        0f,
                        (y - centerIndex) * tileSize
                    );

                    GameObject prefabToUse = esEsquina && cornerPrefab != null ? cornerPrefab : borderPrefab;
                    if(prefabToUse == null) continue;

                    Vector3 rotation = tileRotation;

                    // Ajustes de bordes
                    if (esBorde && borderPrefab != null)
                    {
                        if (x == 0) { position.x += borderInset; rotation = new Vector3(-90, 0, 0); }
                        else if (x == size - 1) { position.x -= borderInset; rotation = new Vector3(-90, 180, 0); }
                        else if (y == 0) { position.z += borderInset; rotation = new Vector3(-90, 270, 0); }
                        else if (y == size - 1) { position.z -= borderInset; rotation = new Vector3(-90, 90, 0); }
                    }

                    // Ajustes de esquinas
                    if (esEsquina && cornerPrefab != null)
                    {
                        position.y += 0.63f;
                        if (x == 0 && y == 0) rotation = new Vector3(-90, -90, 0);
                        else if (x == size - 1 && y == 0) rotation = new Vector3(-90, 180, 0);
                        else if (x == size - 1 && y == size - 1) rotation = new Vector3(-90, 90, 0);
                        else if (x == 0 && y == size - 1) rotation = new Vector3(-90, 0, 0);
                    }

                    GameObject tileGO = Instantiate(prefabToUse, transform);
                    tileGO.transform.localPosition = position;
                    tileGO.transform.localRotation = Quaternion.Euler(rotation);
                    tileGO.name = $"Tile {x + 1},{y + 1} ({prefabToUse.name})";
                }
            }
        }
    }

    private void CenterTilesCore(List<GameObject> coreTiles)
    {
        if (coreTiles.Count == 0) return;

        Vector3 totalCenter = Vector3.zero;
        int count = 0;

        foreach(GameObject go in coreTiles)
        {
            Renderer r = go.GetComponent<Renderer>();
            if (r == null) continue;
            totalCenter += r.bounds.center;
            count++;
        }

        if(count == 0) return;

        Vector3 averageCenter = totalCenter / count;
        Vector3 localOffset = transform.InverseTransformPoint(averageCenter);
        localOffset.y = 0; // solo XZ

        transform.localPosition -= localOffset;
        Debug.Log($"Grid centrada en el core: {transform.localPosition}");
    }
}