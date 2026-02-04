using UnityEngine;

public class Tile : MonoBehaviour
{
    [Header("ID del token")]
    public TokenID id;

    [Header("ReferÃ¨ncia al Renderer")]
    public Renderer childRenderer;

    [Header("Materials possibles")]
    public Material materialA;
    public Material materialB;
    public Material materialC;

    // Estat del cub
    [HideInInspector] public bool estaSendoPulsat = false;
    [HideInInspector] public TipusMaterial colorActual;

    private void Start()
    {
        // Inicialitzar el color a materialA
        colorActual = TipusMaterial.MaterialA;
        childRenderer.material = materialA;
    }

    // Canviar el material i actualitzar estat
    public void ChangeMaterial(TipusMaterial tipus, bool pulsat = false)
    {
        colorActual = tipus;
        estaSendoPulsat = pulsat;

        switch (tipus)
        {
            case TipusMaterial.MaterialA:
                childRenderer.material = materialA;
                break;
            case TipusMaterial.MaterialB:
                childRenderer.material = materialB;
                break;
            case TipusMaterial.MaterialC:
                childRenderer.material = materialC;
                break;
        }
    }
}