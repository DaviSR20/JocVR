using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement; // ¡Librería necesaria para cambiar de escenas!

public class PruebaBotonVR : MonoBehaviour
{
    [Header("UI Opcional")]
    public TextMeshProUGUI textoDelBoton; // Opcional, por si quieres que el texto cambie antes de cargar

    public void HacerClick()
    {
        // 1. (Opcional) Damos feedback visual de que el click funcionó
        if (textoDelBoton != null)
        {
            textoDelBoton.text = "Cargando...";
        }
        
        Debug.Log("Cargando la escena Countdown...");

        // 2. Cargamos la escena por su nombre exacto
        SceneManager.LoadScene("Countdown");
    }
}