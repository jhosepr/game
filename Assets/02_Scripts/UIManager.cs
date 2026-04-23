using UnityEngine;
using TMPro; // Importante para TextMeshPro

public class UIManager : MonoBehaviour
{
    public TextMeshProUGUI textoRonda; // Arrastra aquí el objeto de la jerarquía

    // Método que llamaremos desde el Spawner
    public void ActualizarRondaUI(int numeroRonda)
    {
        textoRonda.text = "Ronda " + numeroRonda.ToString();
    }
}