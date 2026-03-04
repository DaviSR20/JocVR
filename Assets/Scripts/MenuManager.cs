using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public void CargarJuego1()
    {
        SceneManager.LoadScene("Juego1");
    }

    public void CargarJuego2()
    {
        SceneManager.LoadScene("Juego2");
    }
}   