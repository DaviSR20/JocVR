using UnityEngine;

public class EscenarioLoader : MonoBehaviour
{
    void Start()
    {
        // Si existen datos guardados, los aplicamos
        if (PlayerPrefs.HasKey("EscenarioPosX"))
        {
            float x = PlayerPrefs.GetFloat("EscenarioPosX");
            float y = PlayerPrefs.GetFloat("EscenarioPosY");
            float z = PlayerPrefs.GetFloat("EscenarioPosZ");
            transform.position = new Vector3(x, y, z);

            float rotY = PlayerPrefs.GetFloat("EscenarioRotY");
            transform.rotation = Quaternion.Euler(0, rotY, 0);

            float escala = PlayerPrefs.GetFloat("EscenarioEscala");
            transform.localScale = new Vector3(escala, escala, escala);
            
            Debug.Log("🏗️ Escenario posicionado según los ajustes del menú.");
        }
    }
}