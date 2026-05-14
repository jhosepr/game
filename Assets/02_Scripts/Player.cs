using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    [Header("Progreso de Juego")]
    public int enemigosDerrotados = 0;
    [Header("Ajustes de Escudo")]
    public bool tieneEscudoActivo = false;
    [Header("Límites de Mejora")]
    public int mejorasEstaminaRealizadas = 0;
    public int limiteMejorasEstamina = 5;
    [Header("Límites de Velocidad")]
    public int mejorasVelocidadRealizadas = 0;
    public int limiteMejorasVelocidad = 5;
    [Header("Movimiento y Sprint")]
    public float walkSpeed = 6f;
    public float runSpeed = 10f;
    public float rotationSpeed = 15f;

    private float currentSpeed;
    private Transform mainCameraTransform;

    public HotbarManager hotbar;

    [Header("Reciclaje")]
    public GameObject iconoBasura; // Arrastra el objeto "IconoBasura" aquí

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
    public Transform holdPointCactus;

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

        // Mantenemos el cursor libre como pediste
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    void Update()
    {
        HandleStamina();
        ActualizarInterfaz();
        HandleInteraccion();
    }

    // FUNCIÓN DE RECICLAJE (5% DEL PRECIO ORIGINAL)
    public void ReciclarPlantaEnMano()
    {
        if (plantaEnMano == null) return;

        // Calculamos el 5% basado en tus precios configurados
        int pagoReciclaje = 5; // 5% de 20 (Pera)

        if (plantaEnMano.name.Contains("planta2") || plantaEnMano.name.Contains("Tier2"))
            pagoReciclaje = 24; // 5% de 80 (Cactus)
        else if (plantaEnMano.name.Contains("planta3") || plantaEnMano.name.Contains("Tier3"))
            pagoReciclaje = 54; // 5% de 200 (Flor)

        // 1. Sumar energía al sistema
        EnergiaManager.Instance.AñadirEnergia(pagoReciclaje);

        // 2. Destruir la planta
        Destroy(plantaEnMano);
        plantaEnMano = null;

        // 3. Ocultar el bote de basura
        if (iconoBasura != null) iconoBasura.SetActive(false);

        Debug.Log("Reciclado al 5%. Energía recibida: " + pagoReciclaje);
    }

    void HandleInteraccion()
    {
        // --- TECLA E: RECOGER O SEMBRAR ---
        if (Input.GetKeyDown(KeyCode.E))
        {
            // Caso A: Recoger algo del suelo
            if (plantaEnMano == null && plantaCercana != null)
            {
                RecogerPlanta(plantaCercana);
            }
            // Caso B: Sembrar en un slot
            else if (slotCercano != null && !slotCercano.EstaOcupado)
            {
                if (plantaEnMano != null)
                {
                    // Solo sembrar si lo que tengo en la mano es una planta
                    if (plantaEnMano.CompareTag("Planta") || plantaEnMano.CompareTag("Comprado"))
                    {
                        if (slotCercano.SembrarPlanta(plantaEnMano))
                        {
                            plantaEnMano = null;
                            if (iconoBasura != null) iconoBasura.SetActive(false);
                        }
                    }
                }
                else if (hotbar != null)
                {
                    GameObject prefabHotbar = hotbar.ObtenerPlantaSeleccionada();
                    // IMPORTANTE: Solo instanciar si es una planta (No Bomba)
                    if (prefabHotbar != null && !prefabHotbar.CompareTag("ItemLanzable"))
                    {
                        GameObject nuevaPlanta = Instantiate(prefabHotbar);
                        if (slotCercano.SembrarPlanta(nuevaPlanta)) hotbar.ConsumirPlantaActual();
                        else Destroy(nuevaPlanta);
                    }
                    else if (prefabHotbar != null && prefabHotbar.CompareTag("ItemLanzable"))
                    {
                        Debug.Log("No puedes sembrar esto, presiona G para lanzarlo.");
                    }
                }
            }
        }

        // --- TECLA G: LANZAR ---
        if (Input.GetKeyDown(KeyCode.G))
        {
            // Prioridad 1: Si ya tienes algo en la mano (como una planta recogida)
            if (plantaEnMano != null)
            {
                SoltarPlanta();
            }
            // Prioridad 2: Si tienes seleccionada la bomba en la hotbar
            else if (hotbar != null)
            {
                GameObject prefabParaLanzar = hotbar.ObtenerPlantaSeleccionada();
                if (prefabParaLanzar != null && prefabParaLanzar.CompareTag("ItemLanzable"))
                {
                    LanzarBombaDesdeHotbar(prefabParaLanzar);
                }
            }
        }
    }

    void RecogerPlanta(GameObject planta)
    {
        Transform objetoAAgarrar = planta.transform;
        if (planta.transform.parent != null && !planta.transform.parent.CompareTag("Slot"))
        {
            objetoAAgarrar = planta.transform.parent;
        }

        plantaEnMano = planta;
        ProduccionPlanta prod = planta.GetComponentInChildren<ProduccionPlanta>();
        if (prod != null) prod.estaEnSlot = false;

        bool esEspecial = planta.name.Contains("planta2") || planta.name.Contains("planta3") ||
                          objetoAAgarrar.name.Contains("Tier2") || objetoAAgarrar.name.Contains("Tier3");

        Transform puntoDestino = (esEspecial && holdPointCactus != null) ? holdPointCactus : holdPoint;

        Animator pAnim = planta.GetComponent<Animator>();
        if (pAnim != null) pAnim.enabled = false;

        Rigidbody pRb = objetoAAgarrar.GetComponent<Rigidbody>();
        if (pRb == null) pRb = planta.GetComponent<Rigidbody>();

        if (pRb != null)
        {
            pRb.isKinematic = true;
            pRb.useGravity = false;
        }

        SlotTierra slotDondeEstaba = objetoAAgarrar.GetComponentInParent<SlotTierra>();
        if (slotDondeEstaba != null) slotDondeEstaba.VaciarSlot();

        objetoAAgarrar.SetParent(puntoDestino, false);
        objetoAAgarrar.localPosition = Vector3.zero;
        objetoAAgarrar.localRotation = Quaternion.identity;
        objetoAAgarrar.localScale = Vector3.one;

        plantaCercana = null;

        // ACTIVAR ICONO DE BASURA AL RECOGER
        if (iconoBasura != null) iconoBasura.SetActive(true);
    }

    void SoltarPlanta()
    {
        if (plantaEnMano == null) return;
        ProduccionPlanta prod = plantaEnMano.GetComponentInChildren<ProduccionPlanta>();
        if (prod != null) prod.estaEnSlot = false;
        GameObject objetoASoltar = plantaEnMano;
        Transform target = objetoASoltar.transform;

        if (objetoASoltar.transform.parent != null &&
            objetoASoltar.transform.parent != holdPoint &&
            objetoASoltar.transform.parent != holdPointCactus)
        {
            target = objetoASoltar.transform.parent;
        }

        target.SetParent(null);
        Rigidbody pRb = target.GetComponent<Rigidbody>();
        if (pRb == null) pRb = objetoASoltar.GetComponent<Rigidbody>();

        if (pRb != null)
        {
            pRb.isKinematic = false;
            pRb.useGravity = true;
            pRb.AddForce(transform.forward * 3f, ForceMode.Impulse);
        }

        plantaEnMano = null;

        // DESACTIVAR ICONO AL SOLTAR
        if (iconoBasura != null) iconoBasura.SetActive(false);
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Slot")) slotCercano = other.GetComponent<SlotTierra>();
        if ((other.CompareTag("Planta") || other.CompareTag("Comprado")) && plantaEnMano == null)
            plantaCercana = other.gameObject;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Slot")) slotCercano = null;
        if (other.CompareTag("Planta") || other.CompareTag("Comprado")) plantaCercana = null;
    }

    void FixedUpdate()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveZ = Input.GetAxisRaw("Vertical");
        Vector3 inputDir = new Vector3(moveX, 0, moveZ).normalized;
        Vector3 moveDir = Vector3.zero;

        if (inputDir.magnitude >= 0.1f && mainCameraTransform != null)
        {
            Vector3 camForward = mainCameraTransform.forward;
            Vector3 camRight = mainCameraTransform.right;
            camForward.y = 0; camRight.y = 0;
            camForward.Normalize(); camRight.Normalize();
            moveDir = (camForward * inputDir.z + camRight * inputDir.x).normalized;
        }

        if (anim != null) anim.SetBool("isWalking", moveDir.magnitude >= 0.1f);

        if (moveDir.magnitude >= 0.1f)
        {
            rb.MovePosition(rb.position + moveDir * currentSpeed * Time.fixedDeltaTime);
            rb.rotation = Quaternion.Slerp(rb.rotation, Quaternion.LookRotation(moveDir), rotationSpeed * Time.fixedDeltaTime);
        }
        else rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);
    }

    void HandleStamina()
    {
        bool isMoving = (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0);
        bool isRunning = Input.GetKey(KeyCode.LeftShift) && isMoving && !isExhausted;
        if (anim != null) anim.SetBool("isRunning", isRunning);

        if (isRunning)
        {
            currentSpeed = runSpeed;
            currentStamina -= staminaDrain * Time.deltaTime;
            if (currentStamina <= 0) { currentStamina = 0; isExhausted = true; }
        }
        else
        {
            currentSpeed = walkSpeed;
            if (currentStamina < maxStamina) currentStamina += staminaRegen * Time.deltaTime;
            if (isExhausted && currentStamina >= 20f) isExhausted = false;
        }
    }

    void ActualizarInterfaz()
    {
        if (barraVidaRelleno != null) barraVidaRelleno.fillAmount = currentHealth / maxHealth;
        if (barraEstaminaRelleno != null) barraEstaminaRelleno.fillAmount = currentStamina / maxStamina;
    }

    public void TakeDamage(float amount)
    {
        // Si tiene el escudo, ignoramos el daño por completo
        if (tieneEscudoActivo) return;

        currentHealth -= amount;
        if (currentHealth <= 0) SceneManager.LoadScene("GameOver");
    }
    void LanzarBombaDesdeHotbar(GameObject prefab)
    {
        // Usamos el holdPointCactus que está bien centrado
        Vector3 posicionSalida = holdPointCactus.position;

        // 1. Instanciar la bomba
        GameObject bombaInstanciada = Instantiate(prefab, posicionSalida, transform.rotation);

        // 2. Ajustar física
        Rigidbody bRb = bombaInstanciada.GetComponent<Rigidbody>();
        if (bRb != null)
        {
            bRb.isKinematic = false;
            bRb.useGravity = true;

            // Fuerza reducida (4f) para que no salga disparada tan lejos
            // Agregamos un pequeño impulso hacia arriba (2f) para que haga arco
            Vector3 fuerzaTiro = transform.forward * 4f + Vector3.up * 2f;
            bRb.AddForce(fuerzaTiro, ForceMode.Impulse);
        }

        // 3. Activar explosión
        BombaLogica scriptBomba = bombaInstanciada.GetComponent<BombaLogica>();
        if (scriptBomba != null) scriptBomba.ActivarBomba();

        // 4. Consumir de la hotbar
        hotbar.ConsumirPlantaActual();
    }
    // --- MEJORAS INSTANTÁNEAS ---

    public void CurarJugador()
    {
        // Calculamos el 25% de la vida máxima
        float cantidadCuracion = maxHealth * 0.25f;
        currentHealth += cantidadCuracion;

        // Aseguramos que no sobrepase el máximo
        if (currentHealth > maxHealth) currentHealth = maxHealth;

        Debug.Log("Vida aumentada. Vida actual: " + currentHealth);
    }
    public void MejorarVelocidad()
    {
        if (mejorasVelocidadRealizadas < limiteMejorasVelocidad)
        {
            // Aumentamos un 10% la velocidad de caminar y correr
            walkSpeed += walkSpeed * 0.10f;
            runSpeed += runSpeed * 0.10f;

            mejorasVelocidadRealizadas++;
            Debug.Log("Velocidad mejorada " + mejorasVelocidadRealizadas + "/5. Nueva Vel Caminar: " + walkSpeed);
        }
        else
        {
            Debug.Log("Límite de velocidad alcanzado.");
        }
    }

    public void MejorarResistenciaEstamina()
    {
        // Solo aplicamos la mejora si no hemos alcanzado el límite
        if (mejorasEstaminaRealizadas < limiteMejorasEstamina)
        {
            // Reduce el gasto un 10% cada vez
            staminaDrain -= staminaDrain * 0.10f;
            mejorasEstaminaRealizadas++;

            Debug.Log("Mejora " + mejorasEstaminaRealizadas + "/5 aplicada. Nuevo gasto: " + staminaDrain);
        }
        else
        {
            Debug.Log("Ya alcanzaste el límite máximo de resistencia.");
        }
    }
    public void ActivarEscudo(float duracion)
    {
        tieneEscudoActivo = true;
        Debug.Log("Escudo activado por " + duracion + " segundos");
        // Llamamos a una función que lo apague después del tiempo
        Invoke("DesactivarEscudo", duracion);
    }

    void DesactivarEscudo()
    {
        tieneEscudoActivo = false;
        Debug.Log("Escudo desactivado");
    }
    public void RegistrarBajaEnemigo()
    {
        enemigosDerrotados++;
        Debug.Log("Enemigos derrotados: " + enemigosDerrotados);
    }
}