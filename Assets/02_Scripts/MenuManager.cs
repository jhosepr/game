using UnityEngine;
using UnityEngine.SceneManagement; // ¡No olvides esta línea!

public class MenuManager : MonoBehaviour
{
    // Carga el juego principal
    public void Jugar()
    {
        SceneManager.LoadScene("SampleScene");
    }

    // Vuelve al menú principal
    public void IrAlMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    // Por si quieres cerrar el juego
    public void Salir()
    {
        Application.Quit();
        Debug.Log("Saliendo del juego...");
    }
}