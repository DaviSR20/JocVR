using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuVR : MonoBehaviour
{
    // --- PANELES ---
    public GameObject panelMenuPrincipal;
    public GameObject panelJuego1;
    public GameObject panelJuego2;

    // --- VISUAL (HOVER) ---
    private Renderer rend;
    public Color colorNormal = Color.white;
    public Color colorHover = Color.green;

    void Start()
    {
        rend = GetComponent<Renderer>();

        if (rend != null)
            rend.material.color = colorNormal;
    }

    public void OnHoverEnter()
    {
        if (rend != null)
            rend.material.color = colorHover;
    }

    public void OnHoverExit()
    {
        if (rend != null)
            rend.material.color = colorNormal;
    }

    // --- NAVEGACIÓN MENÚ ---
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

    // --- CARGAR ESCENAS ---
    public void JugarJuego1()
    {
        SceneManager.LoadScene("Juego1");
    }

    public void JugarJuego2()
    {
        SceneManager.LoadScene("Juego2");
    }
}