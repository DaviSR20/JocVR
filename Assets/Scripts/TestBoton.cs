using UnityEngine;
using TMPro;

public class TestBoton : MonoBehaviour
{
    public TextMeshProUGUI textoPrueba;

    public void AlPulsarBoton()
    {
        textoPrueba.text = "¡BOTÓN FUNCIONA!";
        Debug.Log("Interacción VR detectada con éxito");
    }
}