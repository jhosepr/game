using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    [Header("Movimiento y Sprint")]
    public float walkSpeed = 6f;
    public float runSpeed = 10f;
    public float rotationSpeed = 15f;

    private float currentSpeed;
    private Transform mainCameraTransform;

    public HotbarManager hotbar;

    [Header("Sistema de Estamina")]
    public float maxStamina = 100f;
    public float currentStamina;
    public float staminaDrain = 20f;
    public float staminaRegen = 15f;

    private bool isExhausted = false;

    [Header("Sistema de Vida")]
    public float maxHealth = 100f;
    public float currentHealth;

    [Header("Referencias de Interfaz")]
    public Image barraVidaRelleno;
    public Image barraEstaminaRelleno;

    [Header("Interacción con Plantas")]
    public Transform holdPoint;

    private GameObject plantaEnMano = null;
    private SlotTierra slotCercano = null;
    private GameObject plantaCercana = null;

    private Rigidbody rb;
    private Animator anim;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        anim = GetComponentInChildren<Animator>();

        if (Camera.main != null)
            mainCameraTransform = Camera.main.transform;

        currentSpeed = walkSpeed;
        currentStamina = maxStamina;
        currentHealth = maxHealth;
    }

    void Update()
    {
        HandleStamina();
        ActualizarInterfaz();
        HandleInteraccion();
    }

    void HandleInteraccion()
    {
        // RECOGER
        if (Input.GetKeyDown(KeyCode.E) &&
            plantaEnMano == null &&
            plantaCercana != null)
        {
            RecogerPlanta(plantaCercana);
        }

        // SEMBRAR
        else if (Input.GetKeyDown(KeyCode.E) &&
                 slotCercano != null &&
                 !slotCercano.EstaOcupado)
        {
            // CASO A: tienes una planta física
            if (plantaEnMano != null)
            {
                if (slotCercano.SembrarPlanta(plantaEnMano))
                {
                    plantaEnMano = null;
                }
            }

            // CASO B: desde inventario/hotbar
            else if (hotbar != null)
            {
                GameObject prefabParaSembrar =
                    hotbar.ObtenerPlantaSeleccionada();

                if (prefabParaSembrar != null)
                {
                    GameObject nuevaPlanta =
                        Instantiate(prefabParaSembrar);

                    if (slotCercano.SembrarPlanta(nuevaPlanta))
                    {
                        hotbar.ConsumirPlantaActual();
                    }
                    else
                    {
                        Destroy(nuevaPlanta);
                    }
                }
            }
        }

        // SOLTAR
        if (Input.GetKeyDown(KeyCode.G) &&
            plantaEnMano != null)
        {
            SoltarPlanta();
        }
    }

    void RecogerPlanta(GameObject planta)
    {
        // OBJETO REAL A AGARRAR
        Transform objetoAAgarrar = planta.transform;

        // Si tiene contenedor y NO es slot
        if (planta.transform.parent != null &&
            !planta.transform.parent.CompareTag("Slot"))
        {
            objetoAAgarrar = planta.transform.parent;
        }

        plantaEnMano = planta;

        // DESACTIVAR ANIMADOR
        Animator pAnim = planta.GetComponent<Animator>();

        if (pAnim != null)
            pAnim.enabled = false;

        // RIGIDBODY
        Rigidbody pRb = objetoAAgarrar.GetComponent<Rigidbody>();

        if (pRb == null)
            pRb = planta.GetComponent<Rigidbody>();

        if (pRb != null)
        {
            pRb.isKinematic = false;

            pRb.linearVelocity = Vector3.zero;
            pRb.angularVelocity = Vector3.zero;

            pRb.isKinematic = true;
            pRb.useGravity = false;
        }

        // VACIAR SLOT
        SlotTierra slotDondeEstaba =
            objetoAAgarrar.GetComponentInParent<SlotTierra>();

        if (slotDondeEstaba != null)
            slotDondeEstaba.VaciarSlot();

        // HACER HIJO DEL HOLDPOINT
        objetoAAgarrar.SetParent(holdPoint, false);

        // POSICIÓN EN LA MANO
        objetoAAgarrar.localPosition =
            new Vector3(0f, -0.2f, 0.3f);

        objetoAAgarrar.localRotation =
            Quaternion.identity;

        objetoAAgarrar.localScale =
            Vector3.one;

        plantaCercana = null;

        Debug.Log("Planta recogida correctamente");
    }

    void SoltarPlanta()
    {
        if (plantaEnMano == null)
            return;

        GameObject objetoASoltar = plantaEnMano;

        Transform target = objetoASoltar.transform;

        // Si tiene contenedor
        if (objetoASoltar.transform.parent != null &&
            objetoASoltar.transform.parent != holdPoint)
        {
            target = objetoASoltar.transform.parent;
        }

        target.SetParent(null);

        Rigidbody pRb = target.GetComponent<Rigidbody>();

        if (pRb == null)
            pRb = objetoASoltar.GetComponent<Rigidbody>();

        if (pRb != null)
        {
            pRb.isKinematic = false;
            pRb.useGravity = true;

            pRb.AddForce(
                transform.forward * 3f,
                ForceMode.Impulse
            );
        }

        plantaEnMano = null;
    }

    // DETECCIÓN
    private void OnTriggerStay(Collider other)
    {
        // SLOT
        if (other.CompareTag("Slot"))
        {
            slotCercano =
                other.GetComponent<SlotTierra>();
        }

        // PLANTA O COMPRADO
        if ((other.CompareTag("Planta") ||
             other.CompareTag("Comprado")) &&
            plantaEnMano == null)
        {
            plantaCercana = other.gameObject;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // SLOT
        if (other.CompareTag("Slot"))
        {
            slotCercano = null;
        }

        // PLANTA O COMPRADO
        if (other.CompareTag("Planta") ||
            other.CompareTag("Comprado"))
        {
            plantaCercana = null;
        }
    }

    // MOVIMIENTO
    void FixedUpdate()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveZ = Input.GetAxisRaw("Vertical");

        Vector3 inputDir =
            new Vector3(moveX, 0, moveZ).normalized;

        Vector3 moveDir = Vector3.zero;

        if (inputDir.magnitude >= 0.1f &&
            mainCameraTransform != null)
        {
            Vector3 camForward =
                mainCameraTransform.forward;

            Vector3 camRight =
                mainCameraTransform.right;

            camForward.y = 0;
            camRight.y = 0;

            camForward.Normalize();
            camRight.Normalize();

            moveDir =
                (camForward * inputDir.z +
                 camRight * inputDir.x).normalized;
        }

        bool moviendose =
            moveDir.magnitude >= 0.1f;

        if (anim != null)
            anim.SetBool("isWalking", moviendose);

        if (moviendose)
        {
            Vector3 nextPos =
                rb.position +
                moveDir *
                currentSpeed *
                Time.fixedDeltaTime;

            rb.MovePosition(nextPos);

            Quaternion targetRotation =
                Quaternion.LookRotation(moveDir);

            rb.rotation =
                Quaternion.Slerp(
                    rb.rotation,
                    targetRotation,
                    rotationSpeed * Time.fixedDeltaTime
                );
        }
        else
        {
            rb.linearVelocity =
                new Vector3(
                    0,
                    rb.linearVelocity.y,
                    0
                );
        }
    }

    void HandleStamina()
    {
        bool isMoving =
            Input.GetAxisRaw("Horizontal") != 0 ||
            Input.GetAxisRaw("Vertical") != 0;

        bool isRunning =
            Input.GetKey(KeyCode.LeftShift) &&
            isMoving &&
            !isExhausted;

        if (anim != null)
            anim.SetBool("isRunning", isRunning);

        if (isRunning)
        {
            currentSpeed = runSpeed;

            currentStamina -=
                staminaDrain * Time.deltaTime;

            if (currentStamina <= 0)
            {
                currentStamina = 0;
                isExhausted = true;
            }
        }
        else
        {
            currentSpeed = walkSpeed;

            if (currentStamina < maxStamina)
            {
                currentStamina +=
                    staminaRegen * Time.deltaTime;
            }

            if (isExhausted &&
                currentStamina >= 20f)
            {
                isExhausted = false;
            }
        }
    }

    void ActualizarInterfaz()
    {
        if (barraVidaRelleno != null)
        {
            barraVidaRelleno.fillAmount =
                currentHealth / maxHealth;
        }

        if (barraEstaminaRelleno != null)
        {
            barraEstaminaRelleno.fillAmount =
                currentStamina / maxStamina;
        }
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;

        if (currentHealth <= 0)
        {
            SceneManager.LoadScene("GameOver");
        }
    }
}