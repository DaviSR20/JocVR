using UnityEngine;
using System.Collections.Generic;

public class GridManager : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject tilePrefab;
    public GameObject borderPrefab;
    public GameObject cornerPrefab;

    [Header("Tamaño y rotación")]
    public float tileSize = 2f;
    private Vector3 tileRotation = new Vector3(90f, 0f, 0f);

    [Header("Ajustes de bordes")]
    public float borderInset = 3.6f;

    [Header("Back Ground color")]
    public Material BgText;

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
                    // 🎨 Asignar color aleatorio
                    CubeMaterialController tileScript = tileGO.GetComponent<CubeMaterialController>();
                    if(tileScript != null)
                    {
                        int r = Random.Range(0, 3);
                        if (r == 0) tileScript.ChangeMaterial(TipusMaterial.MaterialA);
                        else if (r == 1) tileScript.ChangeMaterial(TipusMaterial.MaterialB);
                        else tileScript.ChangeMaterial(TipusMaterial.MaterialC);
                    }

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
                        position.y += 0.64f;
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
        CreateSideMessages(size);
    }
    
    private void CreateSideMessages(int size)
    {
        float halfGrid = (size * tileSize) / 2f;
        float textHeight = tileSize * 0.6f;   // altura del texto
        float offsetFromBorder = borderInset - 5f; // separación extra

        // ARRIBA
        Vector3 arribaPos = new Vector3(0, textHeight, halfGrid + offsetFromBorder);
        Quaternion arribaRot = Quaternion.Euler(60, 0, 0);
        CreateSideText(arribaPos, "ARRIBA", arribaRot);

        // ABAJO
        Vector3 abajoPos = new Vector3(0, textHeight, -halfGrid - offsetFromBorder);
        Quaternion abajoRot = Quaternion.Euler(60, 180, 0);
        CreateSideText(abajoPos, "ABAJO", abajoRot);

        // IZQUIERDA
        Vector3 izquierdaPos = new Vector3(-halfGrid - offsetFromBorder, textHeight, 0);
        Quaternion izquierdaRot = Quaternion.Euler(60, 270, 0);
        CreateSideText(izquierdaPos, "IZQUIERDA", izquierdaRot);

        // DERECHA
        Vector3 derechaPos = new Vector3(halfGrid + offsetFromBorder, textHeight,0);
        Quaternion derechaRot = Quaternion.Euler(60, 90, 0);
        CreateSideText(derechaPos, "DERECHA", derechaRot);
    }
    
    private void CreateSideText(Vector3 position, string text, Quaternion rotation)
    {
        GameObject textObj = new GameObject($"SideText_{text}");
        textObj.transform.SetParent(transform);
        textObj.transform.localPosition = position;
        textObj.transform.localRotation = rotation;

        // ===== TEXTO =====
        TMPro.TextMeshPro textMesh = textObj.AddComponent<TMPro.TextMeshPro>();
        textMesh.text = text;
        textMesh.fontSize = 3;
        textMesh.alignment = TMPro.TextAlignmentOptions.Center;
        textMesh.color = Color.white;
        textMesh.enableAutoSizing = false;
        textMesh.rectTransform.sizeDelta = new Vector2(2f, 1f);

        // ===== FONDO =====
        GameObject background = GameObject.CreatePrimitive(PrimitiveType.Quad);
        background.name = "Background";
        background.transform.SetParent(textObj.transform);

        background.transform.localPosition = new Vector3(0, 0, 0.01f);
        background.transform.localRotation = Quaternion.identity;
        background.transform.localScale = new Vector3(2.2f, 1.2f, 1f);

        if (BgText != null)
        {
            background.GetComponent<MeshRenderer>().material = BgText;
        }
        else
        {
            Debug.LogWarning("BgText no está asignado en el inspector.");
        }

        Destroy(background.GetComponent<Collider>());
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