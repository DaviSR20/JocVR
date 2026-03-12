using UnityEngine;
using UnityEngine.SceneManagement;
public class MenuVR : MonoBehaviour
{
    public GameObject panelMenuPrincipal;
    public GameObject panelJuego1;
    public GameObject panelJuego2;

    public void AbrirJuego1()
    {
        panelMenuPrincipal.SetActive(false);
        panelJuego1.SetActive(true);
        panelJuego2.SetActive(false);
    }
    public void AbrirJuego2()
    {
        panelMenuPrincipal.SetActive(false);
        panelJuego1.SetActive(false);
        panelJuego2.SetActive(true);
    }

    public void VolverMenu()
    {
        panelJuego1.SetActive(false);
        panelMenuPrincipal.SetActive(true);
        panelJuego2.SetActive(false);
    }
    public void JugarJuego1()
    {
        SceneManager.LoadScene("Juego1");
    }
    public void JugarJuego2()
    {
        SceneManager.LoadScene("Juego2");
    }
}