using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float velocidad = 3f;
    public float daño = 10f;
    private Transform player;

    void Start()
    {
        // Buscamos al jugador por su Tag. 
        // ¡RECUERDA que tu Player en la jerarquía DEBE tener el Tag "Player"!
        GameObject target = GameObject.FindGameObjectWithTag("Player");
        if (target != null) player = target.transform;
    }

    void Update()
    {
        if (player != null)
        {
            // 1. Mirar al jugador (solo gira en el eje Y para no inclinarse)
            Vector3 posJugador = new Vector3(player.position.x, transform.position.y, player.position.z);
            transform.LookAt(posJugador);

            // 2. Movimiento constante hacia adelante
            transform.Translate(Vector3.forward * velocidad * Time.deltaTime);

            // 3. ANCLA DE ALTURA: Esto sustituye al "Freeze Position Y" que te daba crash.
            // Forzamos a que el enemigo siempre esté a ras de suelo (ajusta el 1f si queda muy arriba)
            Vector3 tempPos = transform.position;
            tempPos.y = 1f;
            transform.position = tempPos;
        }
    }

    // Al chocar con el jugador, le quitamos vida
    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Player playerScript = collision.gameObject.GetComponent<Player>();
            if (playerScript != null)
            {
                playerScript.TakeDamage(daño * Time.deltaTime);
            }
        }
    }
}