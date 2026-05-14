using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [Header("Ventanas Informativas")]
    public GameObject panelAyuda; // Arrastra tu PanelAyuda aquí en el Inspector

    // --- NUEVAS FUNCIONES PARA EL PANEL ---

    public void AbrirAyuda()
    {
        if (panelAyuda != null)
            panelAyuda.SetActive(true);
    }

    public void CerrarAyuda()
    {
        if (panelAyuda != null)
            panelAyuda.SetActive(false);
    }

    // --- TUS FUNCIONES ORIGINALES ---

    public void Jugar()
    {
        SceneManager.LoadScene("SampleScene");
    }

    public void IrAlMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void Salir()
    {
        Application.Quit();
        Debug.Log("Saliendo del juego...");
    }
}