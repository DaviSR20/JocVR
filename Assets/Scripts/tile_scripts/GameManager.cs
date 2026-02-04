using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public TipusMaterial materialActual = TipusMaterial.MaterialA;

    private List<CubeMaterialController> totsElsTokens;

    void Awake()
    {
        CubeMaterialController[] tots = FindObjectsOfType<CubeMaterialController>();
        totsElsTokens = new List<CubeMaterialController>(tots);
        Debug.Log($"GameManager: S'han trobat {totsElsTokens.Count} cubs a la escena.");
    }

    // Player entra al cub
    public void PlayerHaEntrat(TokenID id, bool pulsat)
    {
        CubeMaterialController cub = totsElsTokens.Find(t => t.id.x == id.x && t.id.y == id.y);

        if (cub != null)
        {
            cub.ChangeMaterial(materialActual, pulsat);
        }
    }

    // Player deixa el cub
    public void PlayerHaSortit(TokenID id)
    {
        CubeMaterialController cub = totsElsTokens.Find(t => t.id.x == id.x && t.id.y == id.y);

        if (cub != null)
        {
            cub.estaSendoPulsat = false;
            // Opcional: tornar a un color base
            cub.ChangeMaterial(cub.colorActual, false);
        }
    }

    // Opcional: canviar el material global
    public void SeguentMaterial()
    {
        switch (materialActual)
        {
            case TipusMaterial.MaterialA:
                materialActual = TipusMaterial.MaterialB;
                break;
            case TipusMaterial.MaterialB:
                materialActual = TipusMaterial.MaterialC;
                break;
            case TipusMaterial.MaterialC:
                materialActual = TipusMaterial.MaterialA;
                break;
        }
    }
}
