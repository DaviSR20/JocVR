using UnityEngine;
using System.Collections;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class TileController : XRBaseInteractable
{
    private Material previousMaterialPanel;
    private Material previousMaterialQuad;
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

    [Header("Renderers")]
    [SerializeField] private Renderer panelRenderer;
    [SerializeField] private Renderer quadRenderer;

    private GameManager gameManager;
    private bool playerDentro = false;

    private Material currentPanelMaterial;
    private Material currentQuadMaterial;
    private TileState currentState = TileState.Apagado;

    public TileState CurrentState => currentState;

    private bool estaParpadeando = false;
    public bool EstaParpadeando => estaParpadeando;

    public void Initialize(TokenID newID)
    {
        id = newID;

        if (panelRenderer == null)
        {
            Transform t = transform.Find("panel");
            if (t != null) panelRenderer = t.GetComponent<Renderer>();
            else Debug.LogError("No se encontró el hijo 'panel' en el prefab.");
        }

        if (quadRenderer == null)
        {
            Transform t = transform.Find("Quad");
            if (t != null) quadRenderer = t.GetComponent<Renderer>();
            else Debug.LogWarning("No se encontró el hijo 'Quad' en el prefab.");
        }

        gameManager = GameManager.Instance;
        gameManager?.RegisterTile(this);
    }

    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        base.OnSelectEntered(args);
        ActivarDesdeClick();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (playerDentro) return;
        playerDentro = true;
        ActivarTile();
    }

    private void OnTriggerExit(Collider other)
    {
        playerDentro = false;
    }

    public void ActivarDesdeClick() => ActivarTile();

    // ==========================
    // CAMBIO DE MATERIAL + ESTADO
    // ==========================

    public void SetMaterials(Material panelMat, Material quadMat, TileState newState)
    {
        if (currentState == TileState.Rojo && newState != TileState.Rojo)
            return;

        currentPanelMaterial = panelMat;
        currentQuadMaterial  = quadMat;
        currentState         = newState;
        UpdateRenderers();
    }

    public void SetMaterial(Material newMaterial, TileState newState)
    {
        SetMaterials(newMaterial, newMaterial, newState);
    }

    public void ResetTile(Material panelMat, Material quadMat)
    {
        ForceSetMaterials(panelMat, quadMat, TileState.Apagado);
    }

    public void ResetTile(Material apagadoMaterial)
    {
        ResetTile(apagadoMaterial, apagadoMaterial);
    }

    public void SaveCurrentState()
    {
        previousMaterialPanel = currentPanelMaterial;
        previousMaterialQuad  = currentQuadMaterial;
        previousState         = currentState;
    }

    public void RestorePreviousState()
    {
        if (previousMaterialPanel != null)
        {
            currentPanelMaterial = previousMaterialPanel;
            currentQuadMaterial  = previousMaterialQuad;
            currentState         = previousState;
            UpdateRenderers();
        }
    }

    public void ForceSetMaterials(Material panelMat, Material quadMat, TileState newState)
    {
        SaveCurrentState();
        currentPanelMaterial = panelMat;
        currentQuadMaterial  = quadMat;
        currentState         = newState;
        UpdateRenderers();
    }

    public void ForceSetMaterial(Material newMaterial, TileState newState)
    {
        ForceSetMaterials(newMaterial, newMaterial, newState);
    }

    private void UpdateRenderers()
    {
        if (panelRenderer != null && currentPanelMaterial != null)
            panelRenderer.material = new Material(currentPanelMaterial);

        if (quadRenderer != null && currentQuadMaterial != null)
            quadRenderer.material = new Material(currentQuadMaterial);
    }

    private void ActivarTile()
    {
        switch (currentState)
        {
            case TileState.Apagado:
                break;

            case TileState.Azul:
                StartCoroutine(ParpadeoYDesactivar());
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
        if (panelRenderer != null) panelRenderer.material.color = color;
        if (quadRenderer  != null) quadRenderer.material.color  = color;
    }

    public void RestoreBaseColor()
    {
        if (currentPanelMaterial != null && panelRenderer != null)
            panelRenderer.material.color = currentPanelMaterial.color;

        if (currentQuadMaterial != null && quadRenderer != null)
            quadRenderer.material.color = currentQuadMaterial.color;
    }

    public void ParpadearAlPisar(Material panelApagado, Material quadApagado, float duracion = 0.6f)
    {
        StartCoroutine(ParpadeoRutina(panelApagado, quadApagado, duracion));
    }

    public void ParpadearAlPisar(Material apagadoMat, float duracion = 0.6f)
    {
        ParpadearAlPisar(apagadoMat, apagadoMat, duracion);
    }

    private IEnumerator ParpadeoRutina(Material panelApagado, Material quadApagado, float duracion)
    {
        if (panelRenderer == null || currentPanelMaterial == null)
            yield break;

        float    intervalo     = 0.1f;
        float    tiempo        = 0f;
        Material panelOriginal = currentPanelMaterial;
        Material quadOriginal  = currentQuadMaterial;

        while (tiempo < duracion)
        {
            if (panelRenderer != null) panelRenderer.material = panelApagado;
            if (quadRenderer  != null) quadRenderer.material  = quadApagado;
            yield return new WaitForSeconds(intervalo);

            if (panelRenderer != null) panelRenderer.material = panelOriginal;
            if (quadRenderer  != null) quadRenderer.material  = quadOriginal;
            yield return new WaitForSeconds(intervalo);

            tiempo += intervalo * 2;
        }

        if (panelRenderer != null) panelRenderer.material = panelOriginal;
        if (quadRenderer  != null) quadRenderer.material  = quadOriginal;
    }

    private IEnumerator ParpadeoYDesactivar()
    {
        estaParpadeando = true;

        gameManager.AddPunto();
        Debug.Log($"Tile {id} azul: +1 punto");

        yield return StartCoroutine(ParpadeoRutina(
            gameManager.ApagadoPanelMat,
            gameManager.ApagadoQuadMat,
            0.6f
        ));

        ForceSetMaterials(gameManager.ApagadoPanelMat, gameManager.ApagadoQuadMat, TileState.Apagado);
        gameManager.RemoveBlueTile(this);

        estaParpadeando = false;
    }
}