using System;
using UnityEngine;
using System.Collections;

public class TileController : MonoBehaviour
{
    private Material previousMaterial;
    private TileState previousState;

    public enum TileState
    {
        Apagado,
        Azul,
        Rojo
    }

    [System.Serializable]
    public struct TokenID
    {
        public int x, y;
        public TokenID(int x, int y) { this.x = x; this.y = y; }
        public override string ToString() => $"({x},{y})";
    }

    [Header("ID del Tile")]
    public TokenID id;

    [Header("Renderer")]
    [SerializeField] private Renderer targetRenderer;

    private GameManager gameManager;
    private bool playerDentro = false;

    private Material currentMaterial;
    private TileState currentState = TileState.Apagado;

    public TileState CurrentState => currentState;

    // Guardamos el color original del tile
    private Material originalMaterial;
    private TileState originalState;
    
    private MaterialPropertyBlock propBlock;
    private Color baseColor;
    public void Initialize(TokenID newID)
    {
        id = newID;

        if (targetRenderer == null)
            targetRenderer = GetComponentInChildren<Renderer>();

        gameManager = GameManager.Instance;
        gameManager?.RegisterTile(this);

        // Guardar estado inicial
        originalMaterial = targetRenderer.material;
        originalState = currentState;
        propBlock = new MaterialPropertyBlock();

        if (targetRenderer != null)
        {
            baseColor = targetRenderer.material.color;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (playerDentro) return;

        playerDentro = true;
        ActivarTile();
    }
    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        playerDentro = false;
    }
    
    public void ActivarDesdeClick()
    {
        ActivarTile();
    }
    // ==========================
    // CAMBIO DE MATERIAL + ESTADO
    // ==========================
    public void SetVisualMaterial(Material mat)
    {
        var renderer = GetComponent<Renderer>();
        renderer.material = mat;
    }
    public void SetMaterial(Material newMaterial, TileState newState)
    {
        if (currentState == TileState.Rojo && newState != TileState.Rojo)
            return;

        currentMaterial = newMaterial;
        currentState = newState;
        UpdateRenderer();
    }
    public void ResetTile(Material apagadoMaterial)
    {
        ForceSetMaterial(apagadoMaterial, TileState.Apagado);
    }

    public void SaveCurrentState()
    {
        previousMaterial = currentMaterial;
        previousState = currentState;
    }

    public void RestorePreviousState()
    {
        if(previousMaterial != null)
        {
            currentMaterial = previousMaterial;
            currentState = previousState;
            UpdateRenderer();
        }
    }

    private void UpdateRenderer()
    {
        if (targetRenderer != null && currentMaterial != null)
            targetRenderer.material = currentMaterial;
    }

// Fuerza un nuevo material y lo guarda como actual
    public void ForceSetMaterial(Material newMaterial, TileState newState)
    {
        SaveCurrentState(); // Guardamos antes de cambiar
        currentMaterial = newMaterial;
        currentState = newState;
        UpdateRenderer();
    }
    private void ActivarTile()
    {
        switch (currentState)
        {
            case TileState.Apagado:
                break;

            case TileState.Azul:
                gameManager.AddPunto();
                StartCoroutine(ParpadeoYDesactivar());
                Debug.Log($"Tile {id} azul: +1 punto");
                break;

            case TileState.Rojo:
                gameManager.RestarVida();
                Debug.Log($"Tile {id} rojo: -1 vida");
                gameManager.PararYParpadearBarra();
                break;
        }
    }
    public void ApplyOverlayColor(Color color)
    {
        if (targetRenderer == null) return;

        targetRenderer.material.color = color;
    }
    public void RestoreBaseColor()
    {
        if (targetRenderer == null) return;

        if (currentMaterial != null)
            targetRenderer.material.color = currentMaterial.color;
    }
    public void ParpadearAlPisar(Material apagadoMat, float duracion = 0.6f)
    {
        StartCoroutine(ParpadeoRutina(apagadoMat, duracion));

    }

    private IEnumerator ParpadeoRutina(Material apagadoMat, float duracion)
    {
        if (targetRenderer == null || currentMaterial == null)
            yield break;

        float intervalo = 0.1f;
        float tiempo = 0f;

        Material materialOriginal = currentMaterial;

        while (tiempo < duracion)
        {
            // Apagado
            targetRenderer.material = apagadoMat;
            yield return new WaitForSeconds(intervalo);

            // Original
            targetRenderer.material = materialOriginal;
            yield return new WaitForSeconds(intervalo);

            tiempo += intervalo * 2;
        }

        // Asegurar que termina en su material real
        targetRenderer.material = materialOriginal;
    }
}