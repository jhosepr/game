using UnityEngine;
using UnityEngine.UI;

public class HotbarManager : MonoBehaviour
{
    [Header("Referencias de UI")]
    public RectTransform[] slots;

    [Header("Configuración del Efecto")]
    public float escalaSeleccionado = 1.25f;
    public float velocidadCambio = 12f;

    private int currentSlotIndex = -1; // Empezamos en -1 (nada seleccionado)
    private Vector3 escalaOriginal;

    void Start()
    {
        if (slots.Length > 0 && slots[0] != null)
        {
            escalaOriginal = slots[0].localScale;
        }
    }

    void Update()
    {
        // Detectar teclas del 1 al 6
        if (Input.GetKeyDown(KeyCode.Alpha1)) IntentarCambiarODeseleccionar(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) IntentarCambiarODeseleccionar(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) IntentarCambiarODeseleccionar(2);
        if (Input.GetKeyDown(KeyCode.Alpha4)) IntentarCambiarODeseleccionar(3);
        if (Input.GetKeyDown(KeyCode.Alpha5)) IntentarCambiarODeseleccionar(4);
        if (Input.GetKeyDown(KeyCode.Alpha6)) IntentarCambiarODeseleccionar(5);

        SuavizarEscalas();
    }

    void IntentarCambiarODeseleccionar(int index)
    {
        // Si presionas el mismo que ya está grande, deseleccionamos (-1)
        if (currentSlotIndex == index)
        {
            currentSlotIndex = -1;
            Debug.Log("Hotbar deseleccionada");
        }
        else
        {
            currentSlotIndex = index;
            Debug.Log("Slot seleccionado: " + (currentSlotIndex + 1));
        }
    }

    void SuavizarEscalas()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i] == null) continue;

            // Si i es igual al seleccionado, va a escala grande. Si no (o si es -1), va a escala original.
            Vector3 targetScale = (i == currentSlotIndex) ? escalaOriginal * escalaSeleccionado : escalaOriginal;

            slots[i].localScale = Vector3.Lerp(slots[i].localScale, targetScale, Time.deltaTime * velocidadCambio);
        }
    }
}