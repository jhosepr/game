using UnityEngine;

public class SlotTierra : MonoBehaviour
{
    public bool EstaOcupado = false;

    [Header("Referencias de Posición")]
    public Transform puntoEscenario;
    public Transform puntoCompra;   // Pera (Tier 1)
    public Transform puntoCactus;   // Cactus (Tier 2) y Flor (Tier 3)

    public bool SembrarPlanta(GameObject objetoEnMano)
    {
        if (EstaOcupado) return false;

        if (puntoEscenario == null) puntoEscenario = this.transform;
        if (puntoCactus == null) puntoCactus = puntoEscenario;

        Transform objetoAMover = objetoEnMano.transform;

        if (objetoEnMano.transform.parent != null && !objetoEnMano.transform.parent.CompareTag("Slot"))
        {
            objetoAMover = objetoEnMano.transform.parent;
        }

        // DETECCIÓN DE TIPO (Prioridad Especial)
        bool esEspecial = objetoEnMano.name.Contains("planta2") || objetoEnMano.name.Contains("planta3") ||
                          objetoAMover.name.Contains("Tier2") || objetoAMover.name.Contains("Tier3");

        bool esComprado = objetoEnMano.CompareTag("Comprado") || objetoAMover.CompareTag("Comprado");

        // EMPARENTAMIENTO (Cactus y Flor van al puntoCactus)
        if (esEspecial)
        {
            Debug.Log("<color=orange>Sembrando Especial (Cactus/Flor) en PUNTO CACTUS</color>");
            objetoAMover.SetParent(puntoCactus);
        }
        else if (esComprado)
        {
            Debug.Log("<color=green>Sembrando Pera en PUNTO COMPRA</color>");
            objetoAMover.SetParent(puntoCompra);
        }
        else
        {
            Debug.Log("<color=cyan>Sembrando en PUNTO ESCENARIO</color>");
            objetoAMover.SetParent(puntoEscenario);
        }

        // POSICIONAMIENTO
        objetoAMover.localPosition = Vector3.zero;
        objetoAMover.localRotation = Quaternion.identity;
        objetoEnMano.transform.localPosition = Vector3.zero;
        objetoEnMano.transform.localRotation = Quaternion.identity;

        // FÍSICAS (Sin warnings)
        Rigidbody pRb = objetoAMover.GetComponent<Rigidbody>();
        if (pRb == null) pRb = objetoEnMano.GetComponent<Rigidbody>();

        if (pRb != null)
        {
            pRb.isKinematic = false;
            pRb.linearVelocity = Vector3.zero;
            pRb.angularVelocity = Vector3.zero;
            pRb.isKinematic = true;
            pRb.useGravity = false;
        }

        // ANIMACIÓN
        Animator anim = objetoEnMano.GetComponent<Animator>();
        if (anim == null) anim = objetoAMover.GetComponent<Animator>();

        if (anim != null)
        {
            anim.enabled = true;
            anim.SetBool("estaSembrada", true);
        }
        ProduccionPlanta prod = objetoEnMano.GetComponentInChildren<ProduccionPlanta>();
        if (prod != null)
        {
            prod.estaEnSlot = true;
        }

        EstaOcupado = true;
        return true;
    }

    public void VaciarSlot()
    {
        EstaOcupado = false;
    }
}