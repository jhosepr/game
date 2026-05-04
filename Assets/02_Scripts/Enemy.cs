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

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();

        GameObject target = GameObject.FindGameObjectWithTag("Player");
        if (target != null) player = target.transform;

        rb.freezeRotation = true;

        // [NUEVO] Inicializamos el cronómetro al máximo para que el primer golpe sea inmediato
        cronometroAtaque = tiempoEntreAtaques;
    }

    void FixedUpdate()
    {
        if (player == null) return;

        float distancia = Vector3.Distance(transform.position, player.position);

        // 1. Mirar al jugador
        Vector3 posJugador = new Vector3(player.position.x, transform.position.y, player.position.z);
        transform.LookAt(posJugador);

        // 2. IA de Movimiento y Ataque
        if (distancia > distanciaAtaque)
        {
            // Caminar
            Vector3 movimiento = transform.forward * velocidad * Time.fixedDeltaTime;
            rb.MovePosition(rb.position + movimiento);

            if (anim != null) anim.SetBool("isWalking", true);

            // [OPCIONAL] Si quieres que al alejarte se "resetee" el golpe, descomenta la línea de abajo
            // cronometroAtaque = tiempoEntreAtaques; 
        }
        else
        {
            // Detenerse y Atacar
            if (anim != null) anim.SetBool("isWalking", false);

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
            anim.ResetTrigger("attack");
            anim.SetTrigger("attack");
        }

        Player playerScript = player.GetComponent<Player>();
        if (playerScript != null) playerScript.TakeDamage(daño);
    }
}