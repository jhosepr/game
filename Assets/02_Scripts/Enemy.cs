using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Configuración")]
    public float velocidad = 3f;
    public float daño = 10f;
    public float distanciaAtaque = 1.8f;
    public float tiempoEntreAtaques = 1.2f;

    private Transform player;
    private Rigidbody rb;
    private Animator anim;
    private float cronometroAtaque;

    [Header("Vida")]
    public float vidaActual = 100f;
    public float vidaMaxima = 100f; // Para resetear al reciclar

    // Cacheamos el componente del jugador para no buscarlo mil veces
    private Player playerScript;

    void Awake() // Awake ocurre antes que Start y solo una vez
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();
        rb.freezeRotation = true;
    }

    void OnEnable() // Se ejecuta cada vez que el enemigo "nace" o se "activa"
    {
        vidaActual = vidaMaxima; // Resetear vida
        cronometroAtaque = tiempoEntreAtaques; // Primer golpe listo

        if (player == null)
        {
            GameObject target = GameObject.FindGameObjectWithTag("Player");
            if (target != null)
            {
                player = target.transform;
                playerScript = player.GetComponent<Player>(); // Cacheamos el script
            }
        }
    }

    void FixedUpdate()
    {
        if (player == null) return;

        // Usar sqrMagnitude es más rápido que Vector3.Distance
        float distanciaSqr = (transform.position - player.position).sqrMagnitude;
        float rangoAtaqueSqr = distanciaAtaque * distanciaAtaque;

        Vector3 posJugador = new Vector3(player.position.x, transform.position.y, player.position.z);
        transform.LookAt(posJugador);

        if (distanciaSqr > rangoAtaqueSqr)
        {
            Vector3 movimiento = transform.forward * velocidad * Time.fixedDeltaTime;
            rb.MovePosition(rb.position + movimiento);

            // Solo cambiamos el bool si es necesario (ahorra CPU)
            if (anim != null && !anim.GetBool("isWalking")) anim.SetBool("isWalking", true);
        }
        else
        {
            if (anim != null && anim.GetBool("isWalking")) anim.SetBool("isWalking", false);

            cronometroAtaque += Time.fixedDeltaTime;
            if (cronometroAtaque >= tiempoEntreAtaques)
            {
                AtacarRapido();
                cronometroAtaque = 0;
            }
        }
    }

    void AtacarRapido()
    {
        if (anim != null)
        {
            anim.SetTrigger("attack");
        }

        // Usamos la referencia cacheada (mucho más rápido)
        if (playerScript != null) playerScript.TakeDamage(daño);
    }

    public void TakeDamage(float cantidad)
    {
        vidaActual -= cantidad;
        if (vidaActual <= 0) Morir();
    }

    void Morir()
    {
        if (playerScript != null) playerScript.RegistrarBajaEnemigo();
        // En lugar de Destroy, lo "apagamos" para que el Pooler lo recicle
        gameObject.SetActive(false);
        Debug.Log("Enemigo reciclado");
    }
}