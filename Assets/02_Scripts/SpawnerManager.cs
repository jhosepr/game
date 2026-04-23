using UnityEngine;
using System.Collections.Generic;

public class SpawnerManager : MonoBehaviour
{
    public GameObject enemigoPrefab;
    public Transform[] puntosSpawn;
    public SistemaDiaNoche sistemaTiempo;

    // NUEVA REFERENCIA: Para conectar con el Canvas
    [Header("Interfaz de Usuario")]
    public UIManager miUIManager;

    [Header("Configuración de Rondas")]
    [Tooltip("Escribe aquí cuántos enemigos quieres por cada ronda (Elemento 0 = Ronda 1)")]
    public int[] cantidadPorRonda = { 4, 8, 12, 16, 20 };

    public int rondaActual = 1;
    private bool yaSpawneado = false;

    void Start()
    {
        // Al empezar el juego, nos aseguramos de que la UI muestre la Ronda 1
        if (miUIManager != null)
        {
            miUIManager.ActualizarRondaUI(rondaActual);
        }
    }

    void Update()
    {
        // Si es de noche (0.5) y no hemos spawneado aún
        if (sistemaTiempo.progresoTiempo >= 0.5f && !yaSpawneado)
        {
            SpawnOleada();
            yaSpawneado = true;
        }

        // Si amanece (menos de 0.1), reseteamos el permiso para la siguiente noche
        if (sistemaTiempo.progresoTiempo < 0.1f)
        {
            yaSpawneado = false;
        }
    }

    void SpawnOleada()
    {
        // 1. Actualizar la UI antes de subir el número o spawnear
        if (miUIManager != null)
        {
            miUIManager.ActualizarRondaUI(rondaActual);
        }

        // 2. Lógica de cantidad de enemigos
        int indiceRonda = rondaActual - 1;
        int cantidadAMostrar;

        if (indiceRonda < cantidadPorRonda.Length)
        {
            cantidadAMostrar = cantidadPorRonda[indiceRonda];
        }
        else
        {
            // Si llegas a una ronda mayor a las configuradas, añade 5 más a la última registrada
            cantidadAMostrar = cantidadPorRonda[cantidadPorRonda.Length - 1] + 5;
            Debug.LogWarning("Ronda no configurada, usando dificultad extra.");
        }

        // 3. Spawn de enemigos
        for (int i = 0; i < cantidadAMostrar; i++)
        {
            int indexAleatorio = Random.Range(0, puntosSpawn.Length);
            Transform puntoElegido = puntosSpawn[indexAleatorio];

            float margenX = Random.Range(-1.5f, 1.5f);
            float margenZ = Random.Range(-1.5f, 1.5f);

            // Mantenemos la altura de 1.1f para evitar los crashes de Unity 6
            Vector3 spawnPos = new Vector3(puntoElegido.position.x + margenX, 1.1f, puntoElegido.position.z + margenZ);

            Instantiate(enemigoPrefab, spawnPos, Quaternion.identity);
        }

        Debug.Log("Iniciando Ronda: " + rondaActual + " con " + cantidadAMostrar + " enemigos.");

        // 4. Incrementamos la ronda para la siguiente noche
        rondaActual++;
    }
}