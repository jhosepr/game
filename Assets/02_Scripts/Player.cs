using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement; // <-- IMPORTANTE: Para cambiar de escena

public class Player : MonoBehaviour
{
    [Header("Movimiento y Sprint")]
    public float walkSpeed = 6f;
    public float runSpeed = 10f;
    public float rotationSpeed = 15f;
    private float currentSpeed;

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

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

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
        float moveX = 0;
        float moveZ = 0;

        if (Input.GetKey(KeyCode.W)) moveZ = 1;
        if (Input.GetKey(KeyCode.S)) moveZ = -1;
        if (Input.GetKey(KeyCode.A)) moveX = -1;
        if (Input.GetKey(KeyCode.D)) moveX = 1;

        Vector3 moveDir = new Vector3(moveX, 0, moveZ).normalized;
        Vector3 nextPos;

        if (moveDir.magnitude >= 0.1f)
        {
            nextPos = rb.position + moveDir * currentSpeed * Time.fixedDeltaTime;
            Quaternion targetRotation = Quaternion.LookRotation(moveDir);
            rb.rotation = Quaternion.Slerp(rb.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
        }
        else
        {
            nextPos = rb.position;
            rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);
            rb.angularVelocity = Vector3.zero;
        }

        nextPos.y = 1.0f; // Tu ancla de seguridad para Unity 6
        rb.MovePosition(nextPos);
    }

    void HandleStamina()
    {
        bool isMoving = Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D);
        bool isRunning = Input.GetKey(KeyCode.LeftShift) && isMoving && !isExhausted;

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

    // --- ESTA ES LA FUNCIÓN QUE CAMBIA TODO ---
    public void TakeDamage(float amount)
    {
        currentHealth -= amount;

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Morir(); // Llamamos a la función de muerte
        }
    }

    void Morir()
    {
        Debug.Log("Cargando escena de Game Over...");
        // Asegúrate de que el nombre entre comillas sea EXACTO al de tu escena en la carpeta 01_Scenes
        SceneManager.LoadScene("GameOver");
    }
}