using UnityEngine;

public class ProduccionPlanta : MonoBehaviour
{
    [Header("Configuración")]
    public int puntosPorSegundo = 1;

    // ESTA ES LA CLAVE: El slot activará esto al sembrar
    public bool estaEnSlot = false;

    private float cronometro = 0f;

    void Update()
    {
        // Solo suma si está marcada como sembrada en un slot
        if (estaEnSlot)
        {
            cronometro += Time.deltaTime;

            if (cronometro >= 1f)
            {
                if (EnergiaManager.Instance != null)
                {
                    EnergiaManager.Instance.AñadirEnergia(puntosPorSegundo);
                }
                cronometro = 0f;
            }
        }
        else
        {
            cronometro = 0f;
        }
    }
}