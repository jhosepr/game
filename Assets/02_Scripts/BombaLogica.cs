using UnityEngine;

public class BombaLogica : MonoBehaviour
{
    [Header("Ajustes de Explosión")]
    public float tiempoParaExplotar = 3f; // El delay que querías
    public float radioExplosion = 5f;
    public int danioBomba = 50;

    [Header("Sonido")]
    public AudioClip sonidoExplosion; // Para que aparezca en el Inspector

    private bool activada = false;

    public void ActivarBomba()
    {
        if (activada) return;
        activada = true;

        // Empieza la cuenta atrás de 3 segundos
        Invoke("Explotar", tiempoParaExplotar);
        Debug.Log("Bomba activada, explotará en " + tiempoParaExplotar + " segundos");
    }

    void Explotar()
    {
        // 1. SONIDO: Multiplicamos la potencia por código
        if (sonidoExplosion != null)
        {
            // El truco: lo reproducimos varias veces a la vez para sumar las ondas
            // Cambia el "4" por el nivel de potencia que quieras
            for (int i = 0; i < 4; i++)
            {
                AudioSource.PlayClipAtPoint(sonidoExplosion, transform.position, 1f);
            }
        }

        // 2. DAÑO: Detectamos enemigos
        Collider[] objetosCercanos = Physics.OverlapSphere(transform.position, radioExplosion);

        foreach (Collider col in objetosCercanos)
        {
            Enemy enemigo = col.GetComponent<Enemy>();
            if (enemigo != null)
            {
                enemigo.TakeDamage(danioBomba);
            }
        }

        Debug.Log("¡BOOM! Bomba destruida");

        // 3. DESTRUCCIÓN
        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radioExplosion);
    }
}