using UnityEngine;

public class TiendaManager : MonoBehaviour
{
    public GameObject panelTienda;
    public GameObject[] prefabsPlantas;
    public HotbarManager hotbar;

    private bool tiendaAbierta = false;

    void Start()
    {
        panelTienda.SetActive(false);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void AlternarTienda()
    {
        tiendaAbierta = !tiendaAbierta;
        panelTienda.SetActive(tiendaAbierta);
        Time.timeScale = tiendaAbierta ? 0f : 1f;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void ComprarPlanta(int indice)
    {
        // VERIFICAR SI HAY ESPACIO
        if (hotbar != null && hotbar.EstaLleno())
        {
            Debug.LogWarning("¡Inventario lleno! No puedes comprar más.");
            return; // No hace nada si está lleno
        }

        if (indice < prefabsPlantas.Length)
        {
            hotbar.RecibirPlantaComprada(prefabsPlantas[indice]);
            Debug.Log("Compraste: " + prefabsPlantas[indice].name);
        }
    }
}
