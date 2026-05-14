using UnityEngine;
using TMPro; // Necesario para manejar TextMeshPro

public class UIManager : MonoBehaviour
{
    public TextMeshProUGUI textoRonda;

    [Header("Avisos de Tienda")]
    public TextMeshProUGUI textoAviso; // El objeto que estaba en "None"

    void Start()
    {
        // Al iniciar, ocultamos el mensaje de aviso para que no se vea el "hola"
        if (textoAviso != null)
            textoAviso.gameObject.SetActive(false);
    }

    // Actualiza el número de ronda en la parte inferior
    public void ActualizarRondaUI(int numeroRonda)
    {
        textoRonda.text = "Ronda " + numeroRonda.ToString();
    }

    // --- FUNCIÓN PARA LOS MENSAJES DE BLOQUEO ---
    public void MostrarMensajeTienda(string mensaje)
    {
        if (textoAviso == null)
        {
            Debug.LogError("¡ERROR! No has arrastrado el objeto TextoAvisoTienda al script en el Canvas.");
            return;
        }

        Debug.Log("UIManager cambiando texto a: " + mensaje);

        StopAllCoroutines(); // Detiene avisos anteriores para que no se encimen
        textoAviso.text = mensaje; // Cambia el texto (ej: "Derrota a 5 enemigos")
        textoAviso.gameObject.SetActive(true); // Lo hace visible

        StartCoroutine(OcultarAviso());
    }

    System.Collections.IEnumerator OcultarAviso()
    {
        // Usamos Realtime porque la tienda pausa el juego (Time.timeScale = 0)
        yield return new WaitForSecondsRealtime(2f);
        textoAviso.gameObject.SetActive(false);
    }
}