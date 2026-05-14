using UnityEngine;

public class SistemaDiaNoche : MonoBehaviour
{
    [Header("Configuración de Tiempo")]
    public float duracionDiaNocheSegundos = 60f;
    [Range(0, 1)] public float progresoTiempo = 0f;

    [Header("Referencias")]
    public RectTransform objetoGirable;
    public Light luzSolar;

    [Header("Ajustes de Iluminación")]
    public float intensidadDiaMax = 1.3f;
    public float intensidadNocheMin = 0.01f;
    public Color colorDia = new Color(1f, 0.95f, 0.8f);
    public Color colorNoche = new Color(0.05f, 0.05f, 0.2f);

    [Header("Duración del Punto Máximo")]
    [Range(1, 5)]
    public float persistenciaLuz = 2.5f;

    [Header("Ajustes de Audio")]
    public AudioSource musicaDia;
    public AudioSource musicaNoche;
    public float velocidadTransicion = 0.5f;
    [Range(0, 1)] public float volumenMaximoMusica = 0.3f; // <--- NUEVA VARIABLE

    void Update()
    {
        progresoTiempo += Time.deltaTime / duracionDiaNocheSegundos;
        if (progresoTiempo >= 1) progresoTiempo = 0;

        float anguloZ = progresoTiempo * -360f;
        objetoGirable.localRotation = Quaternion.Euler(0, 0, anguloZ);

        float anguloXSol = (progresoTiempo * 360f) + 90f;
        luzSolar.transform.localRotation = Quaternion.Euler(anguloXSol, -90f, 0);

        float rawCos = Mathf.Cos(progresoTiempo * Mathf.PI * 2);
        float cosSaturado = Mathf.Clamp(rawCos * persistenciaLuz, -1f, 1f);
        float factorLuz = (cosSaturado + 1f) / 2f;

        luzSolar.intensity = Mathf.Lerp(intensidadNocheMin, intensidadDiaMax, factorLuz);
        luzSolar.color = Color.Lerp(colorNoche, colorDia, factorLuz);
        RenderSettings.ambientLight = luzSolar.color * 0.4f;

        ControlarMusica();
    }

    void ControlarMusica()
    {
        bool esNoche = (progresoTiempo >= 0.25f && progresoTiempo <= 0.75f);

        if (esNoche)
        {
            // Sube hasta volumenMaximoMusica en lugar de 1f
            musicaNoche.volume = Mathf.MoveTowards(musicaNoche.volume, volumenMaximoMusica, velocidadTransicion * Time.deltaTime);
            musicaDia.volume = Mathf.MoveTowards(musicaDia.volume, 0f, velocidadTransicion * Time.deltaTime);

            if (!musicaNoche.isPlaying) musicaNoche.Play();
        }
        else
        {
            // Sube hasta volumenMaximoMusica en lugar de 1f
            musicaNoche.volume = Mathf.MoveTowards(musicaNoche.volume, 0f, velocidadTransicion * Time.deltaTime);
            musicaDia.volume = Mathf.MoveTowards(musicaDia.volume, volumenMaximoMusica, velocidadTransicion * Time.deltaTime);

            if (!musicaDia.isPlaying) musicaDia.Play();
        }
    }
}