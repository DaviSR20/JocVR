using System.Collections;
using UnityEngine;

public class Tile_test : MonoBehaviour
{
    [System.Serializable]
    public struct TokenID
    {
        public int x;
        public int y;

        public TokenID(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
    }

    [Header("Renderers")]
    [SerializeField] private Renderer panelRenderer;
    [SerializeField] private Renderer quadRenderer;

    [Header("ID del Tile")]
    public TokenID id;

    private PcGameManager gameManager;
    private Material currentMaterial;

    private Coroutine blinkCoroutine;
    private Coroutine temporalCoroutine;

    private MaterialPropertyBlock panelBlock;
    private MaterialPropertyBlock quadBlock;

    private static readonly int GlowStrengthID = Shader.PropertyToID("Glow Strength");

    private bool playerDentro = false;

    private void Awake()
    {
        panelBlock = new MaterialPropertyBlock();
        quadBlock = new MaterialPropertyBlock();
    }

    private void Start()
    {
        if (panelRenderer == null)
            panelRenderer = transform.Find("panel")?.GetComponent<Renderer>();

        if (quadRenderer == null)
            quadRenderer = transform.Find("Quad")?.GetComponent<Renderer>();

        gameManager = FindObjectOfType<PcGameManager>();

        if (gameManager != null)
        {
            currentMaterial = gameManager.GetMaterialForTile(id);
            SetMaterial(currentMaterial);
        }

        SetGlow(1f);
    }

    // ---------------- MATERIAL ----------------

    public void SetMaterial(Material newMaterial)
    {
        currentMaterial = newMaterial;

        if (panelRenderer != null)
            panelRenderer.sharedMaterial = currentMaterial;

        if (quadRenderer != null && gameManager != null)
            quadRenderer.sharedMaterial = gameManager.GetQuadMaterial(newMaterial);
    }

    public Material GetMaterialActual()
    {
        return currentMaterial;
    }

    // ---------------- GLOW ----------------

    public void SetGlow(float value)
    {
        if (panelRenderer != null)
        {
            panelRenderer.GetPropertyBlock(panelBlock);
            panelBlock.SetFloat(GlowStrengthID, value);
            panelRenderer.SetPropertyBlock(panelBlock);
        }

        if (quadRenderer != null)
        {
            quadRenderer.GetPropertyBlock(quadBlock);
            quadBlock.SetFloat(GlowStrengthID, value);
            quadRenderer.SetPropertyBlock(quadBlock);
        }
    }

    // ---------------- TRIGGERS VR ----------------

    private void OnTriggerEnter(Collider other)
    {
        if (!playerDentro)
        {
            playerDentro = true;

            if (gameManager != null)
                gameManager.TilePressed(id, this);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (playerDentro)
        {
            playerDentro = false;

            if (gameManager != null)
                gameManager.TileReleased(id, this);
        }
    }

    // ---------------- BLINK ----------------

    public void StartBlink(float duration, float intensity)
    {
        if (blinkCoroutine != null)
            StopCoroutine(blinkCoroutine);

        blinkCoroutine = StartCoroutine(BlinkRoutine(duration, intensity));
    }

    private IEnumerator BlinkRoutine(float duration, float intensity)
    {
        SetGlow(intensity);

        yield return new WaitForSeconds(duration);

        SetGlow(1f);

        if (gameManager != null)
            SetMaterial(gameManager.Apagat);
    }

    // ---------------- TEMPORAL ----------------

    public void SetTemporalCoroutine(Coroutine c)
    {
        temporalCoroutine = c;
    }

    public void CancelTemporal()
    {
        if (temporalCoroutine != null)
        {
            StopCoroutine(temporalCoroutine);
            temporalCoroutine = null;
        }
    }
}