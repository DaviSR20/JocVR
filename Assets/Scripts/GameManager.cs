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
    }


    void Start()
    {
        StartCoroutine(StartGame());
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

        foreach (var tile in FindObjectsOfType<TileController>())
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

        var randomTiles = tiles.Values
            .OrderBy(x => Random.value)
            .Take(cantidad);

        foreach (var tile in randomTiles)
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
        while (rondaActiva)
        {
            PintarFila(filaActualBarra);

            filaActualBarra++;

            if (filaActualBarra >= gridSize)
                filaActualBarra = 0;

            yield return new WaitForSeconds(tiempoMovimientoBarra);
        }
    }

    void PintarFila(int fila)
    {
        // Limpiar barra anterior
        foreach (var tile in barraActual)
        {
            tile.RestorePreviousState(); // Solo se restauran los que no son rojos
        }

        barraActual.Clear();

        // Pintar nueva fila
        foreach (var tile in tiles.Values)
        {
            if (tile.id.y == fila)
            {
                tile.ForceSetMaterial(RojoBarra, TileState.Rojo);
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
        puntos = 0; // opcional
    }
    // ===============================
    // EVENTO TILE
    // ===============================
    public void TilePressed(TileController.TokenID id, TileController tile)
    {
        if (!rondaActiva) return;

        switch (tile.CurrentState)
        {
            case TileController.TileState.Azul:
                blueTiles.Remove(tile);
                tile.ForceSetMaterial(Apagat, TileController.TileState.Apagado);
                AddPunto();
                Debug.Log($"Tile {id} azul: +1 punto");

                if (blueTiles.Count <= 0)
                {
                    Debug.Log("✅ Ronda completada");
                    StartCoroutine(RestartRound());
                }
                break;

            case TileController.TileState.Rojo:
                RestarPunto();
                Debug.Log($"Tile {id} rojo: -1 punto");
                StartCoroutine(RestartRound());
                break;

            case TileController.TileState.Apagado:
                // No hace nada
                break;
        }
    }
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
}