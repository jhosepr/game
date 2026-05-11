using UnityEngine;
using UnityEngine.UI;

public class HotbarManager : MonoBehaviour
{
    [Header("Referencias de UI")]
    public RectTransform[] slots;
    public Image[] iconosUI;

    [Header("Sprites de Plantas")]
    public Sprite spritePera;
    public Sprite spriteCactus;
    public Sprite spriteFlor;

    [Header("Inventario")]
    public GameObject[] plantasEnSlots = new GameObject[6];

    [Header("Configuración del Efecto")]
    public float escalaSeleccionado = 1.25f;
    public float velocidadCambio = 12f;

    private int currentSlotIndex = -1;
    private Vector3 escalaOriginal;

    void Start()
    {
        if (slots.Length > 0 && slots[0] != null)
        {
            escalaOriginal = slots[0].localScale;
        }

        // Al inicio todos los iconos son transparentes
        foreach (Image img in iconosUI)
        {
            if (img != null) img.color = new Color(1, 1, 1, 0);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) IntentarCambiarODeseleccionar(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) IntentarCambiarODeseleccionar(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) IntentarCambiarODeseleccionar(2);
        if (Input.GetKeyDown(KeyCode.Alpha4)) IntentarCambiarODeseleccionar(3);
        if (Input.GetKeyDown(KeyCode.Alpha5)) IntentarCambiarODeseleccionar(4);
        if (Input.GetKeyDown(KeyCode.Alpha6)) IntentarCambiarODeseleccionar(5);

        SuavizarEscalas();
    }

    public void RecibirPlantaComprada(GameObject prefabPlanta)
    {
        for (int i = 0; i < plantasEnSlots.Length; i++)
        {
            if (plantasEnSlots[i] == null)
            {
                plantasEnSlots[i] = prefabPlanta;
                if (iconosUI[i] != null)
                {
                    iconosUI[i].sprite = ObtenerSpritePorNombre(prefabPlanta.name);
                    iconosUI[i].color = new Color(1, 1, 1, 1);
                }
                return;
            }
        }
    }

    Sprite ObtenerSpritePorNombre(string nombre)
    {
        if (nombre.Contains("Tier2")) return spriteCactus;
        if (nombre.Contains("Tier3")) return spriteFlor;
        return spritePera;
    }

    // Devuelve la planta del slot seleccionado
    public GameObject ObtenerPlantaSeleccionada()
    {
        if (currentSlotIndex >= 0 && currentSlotIndex < plantasEnSlots.Length)
            return plantasEnSlots[currentSlotIndex];
        return null;
    }

    // Quita la planta del inventario tras sembrar
    public void ConsumirPlantaActual()
    {
        if (currentSlotIndex >= 0)
        {
            plantasEnSlots[currentSlotIndex] = null;
            if (iconosUI[currentSlotIndex] != null)
                iconosUI[currentSlotIndex].color = new Color(1, 1, 1, 0);
        }
    }

    void IntentarCambiarODeseleccionar(int index)
    {
        currentSlotIndex = (currentSlotIndex == index) ? -1 : index;
    }

    void SuavizarEscalas()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i] == null) continue;
            Vector3 targetScale = (i == currentSlotIndex) ? escalaOriginal * escalaSeleccionado : escalaOriginal;
            slots[i].localScale = Vector3.Lerp(slots[i].localScale, targetScale, Time.deltaTime * velocidadCambio);
        }
    }
    public bool EstaLleno()
    {
        foreach (GameObject item in plantasEnSlots)
        {
            if (item == null) return false;
        }
        return true;
    }
}