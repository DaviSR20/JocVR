using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public void CargarJuego1()
    {
        SceneManager.LoadScene("JocVR");
    }

    public void CargarJuego2()
    {
        SceneManager.LoadScene("Juego2");
    }
}   