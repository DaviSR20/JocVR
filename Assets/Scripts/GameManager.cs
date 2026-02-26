using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Grid")]
    public int gridSize = 6;
    public GridManager GridManagerWithBorders;

    [Header("Materiales")]
    public Material Apagat;
    public Material AzulObjetivo;
    public Material RojoBarra;

    [Header("Barra")]
    public float tiempoMovimientoBarra = 1f;
    private int direccionBarra = 1; // 1 = hacia abajo, -1 = hacia arriba
    private bool barraPausada = false;
    public Material GrisParpadeo;
    
    [Header("Vidas")]
    public int vidas = 3;
    public TextMeshPro textoVidas;

    private Dictionary<string, TileController> tiles = new Dictionary<string, TileController>();
    private List<TileController> blueTiles = new List<TileController>();
    private List<TileController> barraActual = new List<TileController>();

    private int filaActualBarra = 0;
    private bool rondaActiva = false;
    private int puntos = 0;

    void Awake()
    {
        Instance = this;
    }
    public void AddPunto()
    {
        puntos += 1;
        Debug.Log("Puntos actuales: " + puntos);
    }

    public void RestarPunto()
    {
        puntos -= 1;
        Debug.Log("Puntos actuales: " + puntos);
        vidas--;
        ActualizarTextoVidas();

        if (vidas <= 0)
        {
            Debug.Log("Game Over");
        }
    }

    void ActualizarTextoVidas()
    {
        if (textoVidas != null)
            textoVidas.text = "Vidas: " + vidas;
    }
    void Start()
    {
        StartCoroutine(StartGame());
        GridManagerWithBorders.ActualizarTextoVidas(vidas);
    }
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            DetectarClick();
        }
    }
    public void RestarVida()
    {
        vidas--;

        Debug.Log("Vidas restantes: " + vidas);

        GridManagerWithBorders.ActualizarTextoVidas(vidas);

        if (vidas <= 0)
        {
            Debug.Log("GAME OVER");
            rondaActiva = false;
        }
    }
    void DetectarClick()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            TileController tile = hit.collider.GetComponent<TileController>();

            if (tile != null)
            {
                tile.ActivarDesdeClick();
            }
        }
    }
    IEnumerator StartGame()
    {
        tiles.Clear();
        GridManagerWithBorders.GenerateGrid(gridSize);
        yield return null;

        InitializeTiles();
        StartNewRound();
    }

    void InitializeTiles()
    {
        tiles.Clear();

        foreach (var tile in FindObjectsByType<TileController>(FindObjectsSortMode.None))
        {
            tiles[tile.id.ToString()] = tile;
            tile.ForceSetMaterial(Apagat, TileController.TileState.Apagado);
        }
    }

    // ===============================
    // RONDA
    // ===============================
    void StartNewRound()
    {
        rondaActiva = true;

        ResetAllTiles();
        GenerateBlueTiles(5);

        filaActualBarra = 0;

        StartCoroutine(MoverBarra());
    }

    void GenerateBlueTiles(int cantidad)
    {
        blueTiles.Clear();

        var candidatos = tiles.Values
            .Where(t => !barraActual.Contains(t)) // 🚫 ignorar los que están en la barra
            .OrderBy(x => Random.value)
            .Take(cantidad);

        foreach (var tile in candidatos)
        {
            tile.ForceSetMaterial(AzulObjetivo, TileController.TileState.Azul);
            blueTiles.Add(tile);
        }
    }
    // ===============================
    // MOVIMIENTO DE BARRA
    // ===============================
    IEnumerator MoverBarra()
    {
        while (true)
        {
            if (!barraPausada)
            {
                PintarFila(filaActualBarra);

                filaActualBarra += direccionBarra;

                if (filaActualBarra >= gridSize - 1)
                {
                    filaActualBarra = gridSize - 1;
                    direccionBarra = -1;
                }
                else if (filaActualBarra <= 0)
                {
                    filaActualBarra = 0;
                    direccionBarra = 1;
                }
            }

            yield return new WaitForSeconds(tiempoMovimientoBarra);
        }
    }
    IEnumerator ParpadeoBarra()
    {
        barraPausada = true;

        float tiempoTotal = 2f;
        float intervalo = 0.2f;
        float contador = 0f;

        while (contador < tiempoTotal)
        {
            // Gris encima
            foreach (var tile in barraActual)
                tile.ApplyOverlayColor(GrisParpadeo.color);

            yield return new WaitForSeconds(intervalo);

            // Rojo encima
            foreach (var tile in barraActual)
                tile.ApplyOverlayColor(RojoBarra.color);

            yield return new WaitForSeconds(intervalo);

            contador += intervalo * 2;
        }

        // Restaurar material original
        foreach (var tile in barraActual)
            tile.RestoreBaseColor();

        barraPausada = false;
    }
    public void PararYParpadearBarra()
    {
        StartCoroutine(ParpadeoBarra());
    }

    void PintarFila(int fila)
    {
        // Restaurar fila anterior
        foreach (var tile in barraActual)
        {
            tile.RestorePreviousState();
        }

        barraActual.Clear();

        // Pintar nueva fila
        foreach (var tile in tiles.Values)
        {
            if (tile.id.y == fila)
            {
                // 🔥 Solo pintamos los que no sean azul
                tile.SaveCurrentState();
                tile.ForceSetMaterial(RojoBarra, TileController.TileState.Rojo);
                barraActual.Add(tile);
            }
        }
    }
    void ResetAllTiles()
    {
        foreach (var tile in tiles.Values)
        {
            // Fuerza a apagado todos los tiles
            tile.ForceSetMaterial(Apagat, TileController.TileState.Apagado);
        }

        // Limpiamos las listas
        blueTiles.Clear();
        barraActual.Clear();
        filaActualBarra = 0;
    }
    // ===============================
    // EVENTO TILE
    // ===============================
    public void TileReleased(TileController.TokenID id, TileController tile) { }

    IEnumerator RestartRound()
    {
        rondaActiva = false;

        yield return new WaitForSeconds(1f);

        StartNewRound();
    }
    public void RegisterTile(TileController tile)
    {
        string key = tile.id.ToString();

        if (!tiles.ContainsKey(key))
            tiles.Add(key, tile);
    }
    IEnumerator ResetAutomatico()
    {
        rondaActiva = false;

        // Restaurar posibles rojos activos
        foreach (var tile in barraActual)
            tile.RestorePreviousState();

        barraActual.Clear();

        yield return new WaitForSeconds(0.2f);

        ResetAllTiles();

        yield return new WaitForSeconds(0.5f);

        StartNewRound();
    }
    public void RemoveBlueTile(TileController tile)
    {
        if (blueTiles.Contains(tile))
            blueTiles.Remove(tile);

        if (blueTiles.Count <= 0)
        {
            Debug.Log("Ronda completada");

            GenerateBlueTiles(5); // genera nuevos azules
        }
    }
}