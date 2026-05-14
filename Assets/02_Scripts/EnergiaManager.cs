using UnityEngine;
using TMPro;

public class EnergiaManager : MonoBehaviour
{
    public static EnergiaManager Instance;

    [Header("Economía")]
    public int energiaTotal = 0;
    public TextMeshProUGUI textoUI;

    private void Awake()
    {
        // Esto es importante para que otros scripts puedan encontrarlo siempre
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Método para sumar energía (plantas) o restar (tienda)
    public void AñadirEnergia(int cantidad)
    {
        energiaTotal += cantidad;

        // Evitamos que la energía sea menor a 0 por si acaso
        if (energiaTotal < 0) energiaTotal = 0;

        ActualizarTexto();
    }

    public void ActualizarTexto()
    {
        if (textoUI != null)
            textoUI.text = energiaTotal.ToString();
    }
}