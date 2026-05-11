using UnityEngine;

public class SlotTierra : MonoBehaviour
{
    public bool EstaOcupado = false;

    [Header("Referencias de Posición")]
    public Transform puntoEscenario;
    public Transform puntoCompra;

    public bool SembrarPlanta(GameObject objetoEnMano)
    {
        if (EstaOcupado) return false;

        // Seguridad: Si no hay puntos, usamos el propio Slot
        if (puntoEscenario == null) puntoEscenario = this.transform;
        if (puntoCompra == null) puntoCompra = puntoEscenario;

        Transform objetoAMover = objetoEnMano.transform;

        // 1. IDENTIFICAR EL CONTENEDOR (Padre)
        if (objetoEnMano.transform.parent != null && !objetoEnMano.transform.parent.CompareTag("Slot"))
        {
            objetoAMover = objetoEnMano.transform.parent;
        }

        // 2. DETECCIÓN DE TAG (Revisamos raíz e hijo)
        bool esComprado = objetoEnMano.CompareTag("Comprado") || objetoAMover.CompareTag("Comprado");

        // 3. EMPARENTAMIENTO DINÁMICO
        // Si es comprado, se vuelve hijo de 'puntoCompra'. Si no, de 'puntoEscenario'.
        if (esComprado)
        {
            Debug.Log("<color=green>Sembrando en PUNTO COMPRA</color>");
            objetoAMover.SetParent(puntoCompra);
        }
        else
        {
            Debug.Log("<color=cyan>Sembrando en PUNTO ESCENARIO</color>");
            objetoAMover.SetParent(puntoEscenario);
        }

        // 4. POSICIONAMIENTO ABSOLUTO (0,0,0 respecto a su nuevo padre)
        objetoAMover.localPosition = Vector3.zero;
        objetoAMover.localRotation = Quaternion.identity;

        // 5. RESET DE LA PERA INTERNA (Para que esté centrada en el contenedor)
        objetoEnMano.transform.localPosition = Vector3.zero;
        objetoEnMano.transform.localRotation = Quaternion.identity;

        // 6. FÍSICAS (Sin warnings)
        Rigidbody pRb = objetoAMover.GetComponent<Rigidbody>();
        if (pRb == null) pRb = objetoEnMano.GetComponent<Rigidbody>();

        if (pRb != null)
        {
            pRb.isKinematic = false; // Truco para Unity 6
            pRb.linearVelocity = Vector3.zero;
            pRb.angularVelocity = Vector3.zero;
            pRb.isKinematic = true;
            pRb.useGravity = false;
        }

        // 7. ANIMACIÓN
        Animator anim = objetoEnMano.GetComponent<Animator>();
        if (anim == null) anim = objetoAMover.GetComponent<Animator>();

        if (anim != null)
        {
            anim.enabled = true;
            anim.SetBool("estaSembrada", true);
        }

        EstaOcupado = true;
        return true;
    }

    public void VaciarSlot()
    {
        EstaOcupado = false;
    }
}