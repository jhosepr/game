using UnityEngine;

public class ControladorGranero : MonoBehaviour
{
    [Header("Asigna las puertas de esta zona específica")]
    public Transform puertaIzquierda;
    public Transform puertaDerecha;

    [Header("Configuración")]
    public float anguloApertura = 90f;
    public float velocidad = 2f;

    private bool jugadorCerca = false;
    private Quaternion rotOriginalIzquierda;
    private Quaternion rotOriginalDerecha;

    void Start()
    {
        // Guardamos la rotación inicial SOLO si la puerta está asignada
        if (puertaIzquierda != null)
            rotOriginalIzquierda = puertaIzquierda.localRotation;

        if (puertaDerecha != null)
            rotOriginalDerecha = puertaDerecha.localRotation;
    }

    void Update()
    {
        // Si el jugador está cerca, el objetivo es el ángulo de apertura; si no, la rotación original
        float anguloActual = jugadorCerca ? anguloApertura : 0f;

        // Movimiento Puerta Izquierda
        if (puertaIzquierda != null)
        {
            Quaternion objetivoIz = rotOriginalIzquierda * Quaternion.Euler(0, -anguloActual, 0);
            puertaIzquierda.localRotation = Quaternion.Slerp(puertaIzquierda.localRotation, objetivoIz, Time.deltaTime * velocidad);
        }

        // Movimiento Puerta Derecha
        if (puertaDerecha != null)
        {
            Quaternion objetivoDer = rotOriginalDerecha * Quaternion.Euler(0, anguloActual, 0);
            puertaDerecha.localRotation = Quaternion.Slerp(puertaDerecha.localRotation, objetivoDer, Time.deltaTime * velocidad);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Solo reacciona si el objeto tiene el Tag "Player"
        if (other.CompareTag("Player"))
        {
            jugadorCerca = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            jugadorCerca = false;
        }
    }
}