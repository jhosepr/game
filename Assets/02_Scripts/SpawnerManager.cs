using UnityEngine;
using System.Collections.Generic;

public class SpawnerManager : MonoBehaviour
{
    [Header("Referencias")]
    public GameObject enemigoPrefab;
    public Transform[] puntosSpawn;
    public SistemaDiaNoche sistemaTiempo;

    [Header("Interfaz de Usuario")]
    public UIManager miUIManager;

    [Header("Configuración de Rondas")]
    [Tooltip("Cantidad de enemigos por cada ronda (Elemento 0 = Ronda 1)")]
    public int[] cantidadPorRonda = { 4, 8, 12, 16, 20 };

    public int rondaActual = 1;
    private bool yaSpawneado = false;

    void Start()
    {
        // Inicializar UI al comenzar
        if (miUIManager != null)
        {
            miUIManager.ActualizarRondaUI(rondaActual);
        }
    }

    void Update()
    {
        // Si es de noche (progreso >= 0.5) y no hemos spawneado en esta noche
        if (sistemaTiempo.progresoTiempo >= 0.5f && !yaSpawneado)
        {
            SpawnOleada();
            yaSpawneado = true;
        }

        // Al amanecer (progreso < 0.1), permitimos spawnear para la siguiente noche
        if (sistemaTiempo.progresoTiempo < 0.1f)
        {
            yaSpawneado = false;
        }
    }

    void SpawnOleada()
    {
        // 1. Actualizar la interfaz con la ronda que va a comenzar
        if (miUIManager != null)
        {
            miUIManager.ActualizarRondaUI(rondaActual);
        }

        // 2. Determinar cuántos enemigos spawnear
        int indiceRonda = rondaActual - 1;
        int cantidadASpawnear;

        if (indiceRonda < cantidadPorRonda.Length)
        {
            cantidadASpawnear = cantidadPorRonda[indiceRonda];
        }
        else
        {
            // Dificultad infinita: toma el último valor y suma 5 por cada ronda extra
            cantidadASpawnear = cantidadPorRonda[cantidadPorRonda.Length - 1] + 5;
            Debug.LogWarning("Ronda no configurada, aumentando dificultad.");
        }

        // 3. Lógica de Reciclaje (Object Pooling)
        for (int i = 0; i < cantidadASpawnear; i++)
        {
            // Elegir punto de spawn aleatorio
            int indexAleatorio = Random.Range(0, puntosSpawn.Length);
            Transform puntoElegido = puntosSpawn[indexAleatorio];

            // Margen aleatorio para que no aparezcan todos en el mismo pixel
            float margenX = Random.Range(-1.5f, 1.5f);
            float margenZ = Random.Range(-1.5f, 1.5f);

            // Posición final (Y = 1.1f para estabilidad en Unity 6)
            Vector3 spawnPos = new Vector3(puntoElegido.position.x + margenX, 1.1f, puntoElegido.position.z + margenZ);

            // --- CAMBIO CLAVE: Pedimos un enemigo al Pooler en lugar de Instanciar uno nuevo ---
            GameObject enemigo = EnemyPooler.Instance.ObtenerEnemigo();

            if (enemigo != null)
            {
                enemigo.transform.position = spawnPos;
                enemigo.transform.rotation = Quaternion.identity;
                enemigo.SetActive(true); // Esto activa el OnEnable del enemigo y resetea su vida
            }
        }

        Debug.Log("Iniciando Ronda: " + rondaActual + " con " + cantidadASpawnear + " enemigos.");

        // 4. Incrementar contador de ronda
        rondaActual++;
    }
}