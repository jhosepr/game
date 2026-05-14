using UnityEngine;

public class TiendaManager : MonoBehaviour
{
    public GameObject panelTienda;
    public GameObject[] prefabsPlantas;
    public HotbarManager hotbar;
    public Player playerScript; // <--- ARRASTRA AL PLAYER AQUÍ EN EL INSPECTOR
    public UIManager uiManager; // <--- ARRASTRA EL CANVAS/UIManager AQUÍ

    [Header("Configuración de Precios")]
    public int[] preciosPlantas = { 0, 50, 150 };

    private bool tiendaAbierta = false;

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
        Debug.Log("Botón presionado. Índice: " + indice + " | Enemigos: " + playerScript.enemigosDerrotados);

        // PLANTA 2 (Cactus)
        if (indice == 1 && playerScript.enemigosDerrotados < 5)
        {
            int faltan = 5 - playerScript.enemigosDerrotados;
            Debug.Log("Bloqueo detectado: Faltan " + faltan);
            uiManager.MostrarMensajeTienda(" Derrota a " + faltan + " enemigos más.");
            return;
        }

        // PLANTA 3 (Flor)
        if (indice == 2 && playerScript.enemigosDerrotados < 20)
        {
            int faltan = 20 - playerScript.enemigosDerrotados;
            uiManager.MostrarMensajeTienda("¡Necesitas " + faltan + " bajas más!");
            return;
        }

        // --- LÓGICA DE COMPRA NORMAL (Tu código anterior) ---
        if (hotbar != null && hotbar.EstaLleno())
        {
            uiManager.MostrarMensajeTienda("¡Inventario lleno!");
            return;
        }

        int costo = preciosPlantas[indice];
        if (EnergiaManager.Instance.energiaTotal >= costo)
        {
            EnergiaManager.Instance.AñadirEnergia(-costo);
            hotbar.RecibirPlantaComprada(prefabsPlantas[indice]);
            Debug.Log("Compraste: " + prefabsPlantas[indice].name);
        }
        else
        {
            uiManager.MostrarMensajeTienda("Energía insuficiente. Faltan: " + (costo - EnergiaManager.Instance.energiaTotal));
        }
    }
}