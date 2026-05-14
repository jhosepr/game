using UnityEngine;
using System.Collections.Generic;

public class EnemyPooler : MonoBehaviour
{
    public static EnemyPooler Instance;

    [Header("Configuración del Pool")]
    public GameObject enemigoPrefab;
    public int cantidadInicial = 20;
    public Transform contenedorEnemigos; // Arrastra un objeto vacío llamado "--ENEMIGOS--"

    private List<GameObject> piscinaDeEnemigos;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        piscinaDeEnemigos = new List<GameObject>();

        // Pre-creamos los enemigos apagados al inicio
        for (int i = 0; i < cantidadInicial; i++)
        {
            GameObject obj = Instantiate(enemigoPrefab);
            obj.transform.SetParent(contenedorEnemigos);
            obj.SetActive(false);
            piscinaDeEnemigos.Add(obj);
        }
    }

    public GameObject ObtenerEnemigo()
    {
        // Buscamos uno que esté "apagado" para reutilizarlo
        foreach (GameObject enemigo in piscinaDeEnemigos)
        {
            if (!enemigo.activeInHierarchy)
            {
                return enemigo;
            }
        }

        // Si no hay ninguno libre, creamos uno nuevo por si acaso
        GameObject nuevo = Instantiate(enemigoPrefab);
        nuevo.transform.SetParent(contenedorEnemigos);
        nuevo.SetActive(false);
        piscinaDeEnemigos.Add(nuevo);
        return nuevo;
    }
}