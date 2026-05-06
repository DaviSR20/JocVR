using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PcGameManager : MonoBehaviour
{
    [Header("Materiales Panel")]
    public Material Apagat;
    public Material Verd;
    public Material Blau;
    public Material BlauBlink;
    public Material Vermell;
    public Material VermellBlink;
    public Material Rosa;
    public Material RosaBlink;

    [Header("Materiales Quad")]
    public Material QuadApagat;
    public Material QuadVerd;
    public Material QuadBlau;
    public Material QuadBlauBlink;
    public Material QuadVermell;
    public Material QuadVermellBlink;
    public Material QuadRosa;
    public Material QuadRosaBlink;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip sonidoCorrecto;
    public AudioClip sonidoIncorrecto;
    
    [Header("Configuración del juego")]
    public float tiempoLimite = 60f;
    public float velocidadJuego = 1f;

    [Header("UI")]
    public TextMeshProUGUI textoPuntuacion;
    public TextMeshProUGUI textoTiempo;

    [Header("Estado del juego")]
    public int puntuacion;
    public float tiempoRestante;

    private bool juegoActivo = false;
    private Dictionary<string, Tile_test> tiles = new Dictionary<string, Tile_test>();

    private void Start()
    {
        tiempoRestante = tiempoLimite;
    }

    public void IniciarJuego()
    {
        juegoActivo = true;

        StopAllCoroutines();
        tiles.Clear();
        puntuacion = 0;
        tiempoRestante = tiempoLimite;

        Tile_test[] todosLosTiles = FindObjectsOfType<Tile_test>();

        foreach (var tile in todosLosTiles)
        {
            string key = tile.id.ToString();
            if (!tiles.ContainsKey(key))
                tiles.Add(key, tile);

            if (EsCasillaCentral(tile.id))
            {
                tile.SetMaterial(Verd);
            }
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

        if (textoPuntuacion != null)
            textoPuntuacion.text = "Puntuación: " + puntuacion;

        if (textoTiempo != null)
            textoTiempo.text = "Tiempo: " + Mathf.Ceil(tiempoRestante);

        if (tiempoRestante <= 0f)
            FinDelJuego();
    }

    private void FinDelJuego()
    {
        juegoActivo = false;

        foreach (var tile in tiles.Values)
            if (tile != null) tile.SetMaterial(Apagat);

        StopAllCoroutines();
    }

    public void TilePressed(Tile_test.TokenID id, Tile_test tile)
    {
        if (!juegoActivo) return;
        if (EsCasillaCentral(id)) return;
        
        Material matActual = tile.GetMaterialActual();
        bool acierto = false;

        if (matActual == Blau)
        {
            puntuacion += 1;
            acierto = true;
        }
        else if (matActual == Rosa)
        {
            puntuacion += 2;
            acierto = true;
        }
        else if (matActual == Vermell) puntuacion -= 1;
        
        if (audioSource != null)
        {
            if (acierto && sonidoCorrecto != null)
                audioSource.PlayOneShot(sonidoCorrecto);
            else if (!acierto && sonidoIncorrecto != null)
                audioSource.PlayOneShot(sonidoIncorrecto);
        }

        tile.CancelTemporal();
        StartCoroutine(BlinkAndResume(tile, matActual));
    }

    public void TileReleased(Tile_test.TokenID id, Tile_test tile)
    {
        tile.SetGlow(1f);
    }

    private bool EsCasillaCentral(Tile_test.TokenID id)
    {
        return (id.x == 1 || id.x == 2) && (id.y == 1 || id.y == 2);
    }

    public Material GetMaterialForTile(Tile_test.TokenID id)
    {
        return EsCasillaCentral(id) ? Verd : Apagat;
    }

    private IEnumerator ComportamientoAleatorio(Tile_test tile)
    {
        while (juegoActivo)
        {
            yield return new WaitForSeconds(Random.Range(1f, 4f) / velocidadJuego);

            if (tile == null) yield break;

            float random = Random.value;

            if (random < 0.20f)
                yield return ActivarTemporal(tile, Blau);
            else if (random < 0.30f)
                yield return ActivarTemporal(tile, Vermell);
            else if (random < 0.33f)
                yield return ActivarTemporal(tile, Rosa);
        }
    }

    private IEnumerator ActivarTemporal(Tile_test tile, Material color)
    {
        tile.SetMaterial(color);

        Coroutine c = StartCoroutine(CambiarTemporal(tile));
        tile.SetTemporalCoroutine(c);

        yield return c;
    }

    private IEnumerator CambiarTemporal(Tile_test tile)
    {
        yield return new WaitForSeconds(3f / velocidadJuego);

        if (juegoActivo && tile != null)
            tile.SetMaterial(Apagat);
    }

    public Material GetQuadMaterial(Material panelMaterial)
    {
        if (panelMaterial == Apagat) return QuadApagat;
        if (panelMaterial == Verd) return QuadVerd;
        if (panelMaterial == Blau) return QuadBlau;
        if (panelMaterial == BlauBlink) return QuadBlauBlink;
        if (panelMaterial == Vermell) return QuadVermell;
        if (panelMaterial == VermellBlink) return QuadVermellBlink;
        if (panelMaterial == Rosa) return QuadRosa;
        if (panelMaterial == RosaBlink) return QuadRosaBlink;

        return QuadApagat;
    }
    
    private Material GetBlinkMaterial(Material normalMat)
    {
        if (normalMat == Blau) return BlauBlink;
        if (normalMat == Vermell) return VermellBlink;
        if (normalMat == Rosa) return RosaBlink;

        return normalMat;
    }
    
    private IEnumerator BlinkAndResume(Tile_test tile, Material originalMat)
    {
        if (tile == null) yield break;

        // Obtener material Blink correcto
        Material blinkMat = GetBlinkMaterial(originalMat);

        // Cambiar a material Blink
        tile.SetMaterial(blinkMat);

        // Esperar 1.5 segundos (puedes cambiarlo a 1f o 2f)
        yield return new WaitForSeconds(1.5f);

        if (!juegoActivo || tile == null) yield break;

        // Volver a apagado
        tile.SetMaterial(Apagat);

        // Reanudar comportamiento aleatorio
        StartCoroutine(ComportamientoAleatorio(tile));
    }
}