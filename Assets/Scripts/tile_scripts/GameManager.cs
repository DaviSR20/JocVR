using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    [Header("Materiales")]
    public Material Apagat;
    public Material Verd;
    public Material Blau;
    public Material Vermell;
    public Material Rosa;

    [Header("Configuración del juego")]
    public float tiempoLimite = 60f;     // Editable en Inspector
    public float velocidadJuego = 1f;    // Editable en Inspector

    [Header("UI")]
    public TextMeshProUGUI textoPuntuacion; // Arrastrar texto puntuación
    public TextMeshProUGUI textoTiempo;     // Arrastrar texto tiempo
    public Camera cameraDisplay2;


    [Header("Estado del juego")]
    public int puntuacion = 0;
    public float tiempoRestante;

    private bool juegoActivo = true;

    private Dictionary<string, Tile_test> tiles =
        new Dictionary<string, Tile_test>();


    private void Start()
    {
        tiempoRestante = tiempoLimite;

        Tile_test[] todosLosTiles = FindObjectsOfType<Tile_test>();

        foreach (var tile in todosLosTiles)
        {
            string key = tile.id.ToString();

            if (!tiles.ContainsKey(key))
                tiles.Add(key, tile);

            if (EsCasillaCentral(tile.id))
                tile.SetMaterial(Verd);
            else
            {
                tile.SetMaterial(Apagat);
                StartCoroutine(ComportamientoAleatorio(tile));
            }
        }
    }


    private void Update()
    {
        if (!juegoActivo) return;

        tiempoRestante -= Time.deltaTime;

        // Actualizar UI
        if (textoPuntuacion != null)
            textoPuntuacion.text =
                "Puntuación: " + puntuacion;

        if (textoTiempo != null)
            textoTiempo.text =
                "Tiempo: " + Mathf.Ceil(tiempoRestante);


        if (tiempoRestante <= 0f)
        {
            juegoActivo = false;

            Debug.Log("Tiempo terminado");

            foreach (var tile in tiles.Values)
                tile.SetMaterial(Apagat);

            StopAllCoroutines();
            return;
        }


        // Click ratón
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray =
                cameraDisplay2.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                Tile_test tile =
                    hit.collider.GetComponent<Tile_test>();

                if (tile != null)
                    TilePressed(tile.id, tile);
            }
        }


        if (Input.GetMouseButtonUp(0))
        {
            Ray ray =
                cameraDisplay2.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                Tile_test tile =
                    hit.collider.GetComponent<Tile_test>();

                if (tile != null)
                    TileReleased(tile.id, tile);
            }
        }
    }

    public Material GetMaterialForTile(Tile_test.TokenID id)
    {
        if (EsCasillaCentral(id))
            return Verd;

        return Apagat;
    }


    public void TilePressed(Tile_test.TokenID id, Tile_test tile)
    {
        if (!juegoActivo) return;

        if (!EsCasillaCentral(id))
        {
            Material matActual =
                tile.GetMaterialActual();

            if (matActual == Blau)
                puntuacion += 1;

            else if (matActual == Rosa)
                puntuacion += 2;

            else if (matActual == Vermell)
                puntuacion -= 1;

            tile.SetMaterial(Apagat);
        }
    }


    public void TileReleased(Tile_test.TokenID id, Tile_test tile)
    {
        if (EsCasillaCentral(id))
            tile.SetMaterial(Verd);

        else
            tile.SetMaterial(Apagat);
    }


    private bool EsCasillaCentral(Tile_test.TokenID id)
    {
        return (id.x == 1 || id.x == 2) &&
               (id.y == 1 || id.y == 2);
    }


    private IEnumerator ComportamientoAleatorio(Tile_test tile)
    {
        while (juegoActivo)
        {
            yield return new WaitForSeconds(
                Random.Range(1f, 4f) / velocidadJuego);

            if (tile == null)
                yield break;

            float random = Random.value;


            if (random < 0.20f)
                yield return StartCoroutine(
                    CambiarTemporal(tile, Blau));

            else if (random < 0.30f)
                yield return StartCoroutine(
                    CambiarTemporal(tile, Vermell));

            else if (random < 0.33f)
                yield return StartCoroutine(
                    CambiarTemporal(tile, Rosa));
        }
    }


    private IEnumerator CambiarTemporal(
        Tile_test tile,
        Material color)
    {
        tile.SetMaterial(color);

        yield return new WaitForSeconds(
            3f / velocidadJuego);

        if (juegoActivo)
            tile.SetMaterial(Apagat);
    }
}