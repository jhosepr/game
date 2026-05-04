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

    // Agregamos una referencia a la cámara
    private Transform mainCameraTransform;

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

    private Rigidbody rb;
    private Animator anim;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        anim = GetComponentInChildren<Animator>();

        // Obtenemos la cámara principal al inicio
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
    }

    void FixedUpdate()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveZ = Input.GetAxisRaw("Vertical");

        Vector3 inputDir = new Vector3(moveX, 0, moveZ).normalized;
        Vector3 moveDir = Vector3.zero;

        // LÓGICA DE MOVIMIENTO RELATIVO A LA CÁMARA
        if (inputDir.magnitude >= 0.1f && mainCameraTransform != null)
        {
            // Calculamos la dirección hacia donde apunta la cámara en el plano horizontal
            Vector3 camForward = mainCameraTransform.forward;
            Vector3 camRight = mainCameraTransform.right;

            camForward.y = 0; // Evitamos que el jugador quiera volar o enterrarse
            camRight.y = 0;

            camForward.Normalize();
            camRight.Normalize();

            // La dirección de movimiento final basada en los ejes de la cámara
            moveDir = (camForward * inputDir.z + camRight * inputDir.x).normalized;
        }

        bool moviendose = moveDir.magnitude >= 0.1f;
        if (anim != null)
        {
            anim.SetBool("isWalking", moviendose);
        }

        if (moviendose)
        {
            Vector3 nextPos = rb.position + moveDir * currentSpeed * Time.fixedDeltaTime;
            rb.MovePosition(nextPos);

            // Rotación suave hacia la dirección de movimiento
            Quaternion targetRotation = Quaternion.LookRotation(moveDir);
            rb.rotation = Quaternion.Slerp(rb.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
        }
        else
        {
            rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);
            rb.angularVelocity = Vector3.zero;
        }
    }

    // El resto de tus funciones (HandleStamina, ActualizarInterfaz, etc.) se mantienen igual
    void HandleStamina()
    {
        bool isMoving = Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0;
        bool isRunning = Input.GetKey(KeyCode.LeftShift) && isMoving && !isExhausted;

        // --- AGREGA ESTO AQUÍ ---
        if (anim != null)
        {
            anim.SetBool("isRunning", isRunning);
        }
        // ------------------------

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
        if (barraVidaRelleno != null)
            barraVidaRelleno.fillAmount = currentHealth / maxHealth;
        if (barraEstaminaRelleno != null)
            barraEstaminaRelleno.fillAmount = currentStamina / maxStamina;
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Morir();
        }
    }

    void Morir()
    {
        SceneManager.LoadScene("GameOver");
    }
}