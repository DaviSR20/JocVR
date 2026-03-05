using UnityEngine;

public class PassthroughScript : MonoBehaviour
{
    [Header("Referencia al Passthrough Layer")]
    public OVRPassthroughLayer passthroughLayer;

    private bool isPassthroughEnabled = true;

    void Start()
    {
        if (passthroughLayer == null)
        {
            passthroughLayer = FindObjectOfType<OVRPassthroughLayer>();
        }

        if (passthroughLayer != null)
        {
            isPassthroughEnabled = passthroughLayer.enabled;
        }
        else
        {
            Debug.LogError("No se encontró ningún OVRPassthroughLayer en la escena.");
        }
    }

    public void TogglePassthroughMode()
    {
        if (passthroughLayer == null) return;

        isPassthroughEnabled = !isPassthroughEnabled;
        passthroughLayer.enabled = isPassthroughEnabled;
    }
}