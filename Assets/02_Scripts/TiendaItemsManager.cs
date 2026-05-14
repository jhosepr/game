using UnityEngine;

public class TiendaItemsManager : MonoBehaviour
{
    [Header("Referencias")]
    public GameObject panelItems;
    public HotbarManager hotbar;
    public Player playerScript; // <--- ARRASTRA AL PLAYER AQUÍ EN EL INSPECTOR

    [Header("Prefabs de Ítems")]
    public GameObject prefabBomba;

    [Header("Precios")]
    public int precioBomba = 50;
    public int precioVida = 30;     // Precio del corazón
    public int precioEstamina = 40; // Precio del rayo
    public int precioVelocidad = 60; // Las botas son 
    [Header("Ajustes de Escudos")]
    public int precioEscudo5s = 40;
    public int precioEscudo10s = 70;

    private bool cooldownEscudo5Activo = false;
    private bool cooldownEscudo10Activo = false;

    public void AlternarPanelItems()
    {
        bool estadoActual = panelItems.activeSelf;
        panelItems.SetActive(!estadoActual);
        Time.timeScale = !estadoActual ? 0f : 1f;
    }

    // --- FUNCIÓN PARA COMPRAR VIDA ---
    public void ComprarVida()
    {
        if (EnergiaManager.Instance.energiaTotal >= precioVida)
        {
            EnergiaManager.Instance.AñadirEnergia(-precioVida);
            playerScript.CurarJugador(); // Llama a la función que pusimos en el Player
            Debug.Log("Vida comprada!");
        }
        else
        {
            Debug.LogWarning("Energía insuficiente para vida");
        }
    }

    // --- FUNCIÓN PARA COMPRAR ESTAMINA ---
    public void ComprarEstamina()
    {
        // Primero revisamos si el jugador aún puede mejorar
        if (playerScript.mejorasEstaminaRealizadas < playerScript.limiteMejorasEstamina)
        {
            if (EnergiaManager.Instance.energiaTotal >= precioEstamina)
            {
                EnergiaManager.Instance.AñadirEnergia(-precioEstamina);
                playerScript.MejorarResistenciaEstamina();
            }
            else
            {
                Debug.LogWarning("Energía insuficiente");
            }
        }
        else
        {
            Debug.Log("Límite de estamina alcanzado. ¡Ya eres muy resistente!");
            // Opcional: podrías cambiar el texto del botón a "MAX"
        }
    }

    public void ComprarBomba()
    {
        if (EnergiaManager.Instance.energiaTotal >= precioBomba)
        {
            if (hotbar != null && !hotbar.EstaLleno())
            {
                EnergiaManager.Instance.AñadirEnergia(-precioBomba);
                hotbar.RecibirPlantaComprada(prefabBomba);
                Debug.Log("Bomba comprada!");
            }
            else
            {
                Debug.LogWarning("Inventario lleno para la bomba");
            }
        }
        else
        {
            Debug.LogWarning("No hay energía suficiente");
        }
    }
    public void ComprarVelocidad()
    {
        if (playerScript.mejorasVelocidadRealizadas < playerScript.limiteMejorasVelocidad)
        {
            if (EnergiaManager.Instance.energiaTotal >= precioVelocidad)
            {
                EnergiaManager.Instance.AñadirEnergia(-precioVelocidad);
                playerScript.MejorarVelocidad();
                Debug.Log("¡Botas compradas!");
            }
            else
            {
                Debug.LogWarning("Energía insuficiente para velocidad");
            }
        }
        else
        {
            Debug.Log("Ya corres lo más rápido posible.");
        }
    }
    public void ComprarEscudo5s()
    {
        if (!cooldownEscudo5Activo && !playerScript.tieneEscudoActivo)
        {
            if (EnergiaManager.Instance.energiaTotal >= precioEscudo5s)
            {
                EnergiaManager.Instance.AñadirEnergia(-precioEscudo5s);
                playerScript.ActivarEscudo(5f);
                StartCoroutine(CooldownEscudo5());
            }
        }
    }

    public void ComprarEscudo10s()
    {
        if (!cooldownEscudo10Activo && !playerScript.tieneEscudoActivo)
        {
            if (EnergiaManager.Instance.energiaTotal >= precioEscudo10s)
            {
                EnergiaManager.Instance.AñadirEnergia(-precioEscudo10s);
                playerScript.ActivarEscudo(10f);
                StartCoroutine(CooldownEscudo10());
            }
        }
    }

    // Rutinas para los tiempos de espera
    System.Collections.IEnumerator CooldownEscudo5()
    {
        cooldownEscudo5Activo = true;
        yield return new WaitForSecondsRealtime(30f); // Usamos Realtime por si el juego se pausa
        cooldownEscudo5Activo = false;
    }

    System.Collections.IEnumerator CooldownEscudo10()
    {
        cooldownEscudo10Activo = true;
        yield return new WaitForSecondsRealtime(60f);
        cooldownEscudo10Activo = false;
    }
}