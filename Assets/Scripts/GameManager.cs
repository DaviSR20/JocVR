using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.XR;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Grid")]
    public int gridSize = 6;
    public GridManager GridManagerWithBorders;

    [Header("Materiales — Panel")]
    public Material ApagadoPanelMat;
    public Material AzulPanelMat;
    public Material RojoPanelMat;

    [Header("Materiales — Quad (Glow)")]
    public Material ApagadoQuadMat;
    public Material AzulQuadMat;
    public Material RojoQuadMat;

    public Material Apagat => ApagadoPanelMat;

    [Header("Barra")]
    public float tiempoMovimientoBarra = 1f;
    private int direccionBarra = 1;
    private bool barraPausada = false;
    public Material GrisParpadeo;

    [Header("Botón de inicio")]
    public GameObject botonInicioCanvas;

    [Header("Vidas")]
    public int vidas = 3;
    public TextMeshPro textoVidas;

    [Header("VR")]
    public Transform vrController;
    public float vrRayDistance = 10f;

    [Header("Canvases")]
    public GameObject canvasGameOver;
    public GameObject canvasSeleccionGrid;

    public int FilaActualBarra => filaActualBarra;

    private Dictionary<string, TileController> tiles = new Dictionary<string, TileController>();
    private List<TileController> blueTiles = new List<TileController>();
    private List<TileController> barraActual = new List<TileController>();

    private int filaActualBarra = 0;
    private bool rondaActiva = false;
    private int puntos = 0;
    private Coroutine barraCoroutine;

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
            Debug.Log("Game Over");
    }

    void ActualizarTextoVidas()
    {
        if (textoVidas != null)
            textoVidas.text = "Vidas: " + vidas;
    }

    void Start()
    {
        GridManagerWithBorders.ActualizarTextoVidas(vidas);

        if (botonInicioCanvas != null)
            botonInicioCanvas.SetActive(true);
    }

    void Update()
    {
        DetectarInputVR();

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Space -> Start Game");
            IniciarJuegoDesdeBoton();
        }
    }

    public void RestarVida()
    {
        vidas--;
        Debug.Log("Vidas restantes: " + vidas);
        GridManagerWithBorders.ActualizarTextoVidas(vidas);

        if (vidas <= 0)
            StopGame();
    }

    void DetectarClick()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            TileController tile = hit.collider.GetComponent<TileController>();
            if (tile != null)
                tile.ActivarDesdeClick();
        }
    }

    IEnumerator StartGame()
    {
        Debug.Log("GRID SIZE = " + gridSize);

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
            tile.ForceSetMaterials(ApagadoPanelMat, ApagadoQuadMat, TileController.TileState.Apagado);
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

        if (barraCoroutine != null)
            StopCoroutine(barraCoroutine);

        barraCoroutine = StartCoroutine(MoverBarra());
    }

    void GenerateBlueTiles(int cantidad)
    {
        blueTiles.Clear();

        var candidatos = tiles.Values
            .Where(t => !barraActual.Contains(t))
            .OrderBy(x => Random.value)
            .Take(cantidad);

        foreach (var tile in candidatos)
        {
            tile.ForceSetMaterials(AzulPanelMat, AzulQuadMat, TileController.TileState.Azul);
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
        float intervalo   = 0.2f;
        float contador    = 0f;

        while (contador < tiempoTotal)
        {
            foreach (var tile in barraActual)
                tile.ApplyOverlayColor(GrisParpadeo.color);

            yield return new WaitForSeconds(intervalo);

            foreach (var tile in barraActual)
                tile.ApplyOverlayColor(RojoPanelMat.color);

            yield return new WaitForSeconds(intervalo);

            contador += intervalo * 2;
        }

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
        foreach (var tile in barraActual)
            tile.RestorePreviousState();

        barraActual.Clear();

        foreach (var tile in tiles.Values)
        {
            if (tile.id.y == fila)
            {
                if (tile.EstaParpadeando)
                    continue;

                tile.SaveCurrentState();
                tile.ForceSetMaterials(RojoPanelMat, RojoQuadMat, TileController.TileState.Rojo);
                barraActual.Add(tile);
            }
        }
    }

    void ResetAllTiles()
    {
        foreach (var tile in tiles.Values)
            tile.ForceSetMaterials(ApagadoPanelMat, ApagadoQuadMat, TileController.TileState.Apagado);

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
            GenerateBlueTiles(5);
        }
    }

    public bool BarraEstaEnFila(int filaTile)
    {
        int filaReal = filaActualBarra - direccionBarra;

        if (filaActualBarra == 0 || filaActualBarra == gridSize - 1)
            filaReal = filaActualBarra;

        return filaReal == filaTile;
    }

    public void AgregarTileABarra(TileController tile)
    {
        if (!barraActual.Contains(tile))
            barraActual.Add(tile);
    }

    public void IniciarJuegoDesdeBoton()
    {
        if (botonInicioCanvas != null)
            botonInicioCanvas.SetActive(false);

        StartCoroutine(EsperarYEmpezarJuego());
    }

    private IEnumerator EsperarYEmpezarJuego()
    {
        yield return new WaitForSeconds(5f);
        StartCoroutine(StartGame());
    }

    void DetectarInputVR()
    {
        InputDevice rightHand = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);
        bool triggerPressed;

        if (rightHand.TryGetFeatureValue(CommonUsages.triggerButton, out triggerPressed) && triggerPressed)
        {
            Ray ray = new Ray(vrController.position, vrController.forward);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, vrRayDistance))
            {
                TileController tile = hit.collider.GetComponent<TileController>();
                if (tile != null)
                    tile.ActivarDesdeClick();
            }
        }
    }

    public void VolverMenuGrid()
    {
        if (canvasGameOver != null)
            canvasGameOver.SetActive(false);

        if (canvasSeleccionGrid != null)
            canvasSeleccionGrid.SetActive(true);
    }

    public void ReiniciarJuego()
    {
        if (canvasGameOver != null)
            canvasGameOver.SetActive(false);

        vidas = 3;
        ActualizarTextoVidas();
        puntos = 0;
        ResetAllTiles();
        IniciarJuegoDesdeBoton();
    }

    public void StopGame()
    {
        rondaActiva = false;

        if (barraCoroutine != null)
        {
            StopCoroutine(barraCoroutine);
            barraCoroutine = null;
        }

        foreach (var tile in barraActual)
            tile.RestorePreviousState();

        barraActual.Clear();

        if (textoVidas != null)
            textoVidas.text = "GAME OVER";

        if (canvasGameOver != null)
            canvasGameOver.SetActive(true);

        Debug.Log("Juego detenido");
    }
}