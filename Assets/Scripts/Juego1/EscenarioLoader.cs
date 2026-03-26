using UnityEngine;

public class EscenarioLoader : MonoBehaviour
{
    void Start()
    {
        // Comprobamos si hay datos guardados de una configuración previa
        if (PlayerPrefs.HasKey("EscenarioPosX"))
        {
            // 1. Aplicar Posición
            float x = PlayerPrefs.GetFloat("EscenarioPosX");
            float y = PlayerPrefs.GetFloat("EscenarioPosY");
            float z = PlayerPrefs.GetFloat("EscenarioPosZ");
            transform.position = new Vector3(x, y, z);

            // 2. Aplicar Rotación (Eje Y)
            float rotY = PlayerPrefs.GetFloat("EscenarioRotY");
            transform.rotation = Quaternion.Euler(0, rotY, 0);

            // 3. Aplicar Escala
            float escala = PlayerPrefs.GetFloat("EscenarioEscala");
            transform.localScale = new Vector3(escala, escala, escala);
            
            Debug.Log("✅ Escenario real posicionado según los ajustes del menú.");
        }
        else
        {
            Debug.LogWarning("⚠️ No se encontraron ajustes de escenario. Usando posición por defecto.");
        }
    }
}