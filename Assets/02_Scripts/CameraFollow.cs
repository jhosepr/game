using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public float smoothSpeed = 0.125f;
    public Vector3 offset;

    [Header("Configuración de Rotación")]
    public float sensitivity = 3.0f;

    [Header("Configuración de Zoom")]
    public float zoomSpeed = 5.0f;
    public float minZoom = 5.0f;  // Lo más cerca que puede estar
    public float maxZoom = 20.0f; // Lo más lejos que puede estar

    private float currentRotationY = 0f;
    private float currentZoomModifier = 1f; // Multiplicador de distancia
    private Vector3 initialOffset;

    void Start()
    {
        // 1. Guardamos los valores base
        initialOffset = offset;
        currentRotationY = 0f;
        currentZoomModifier = 1f;

        // 2. Calculamos la posición exacta de inicio (Igual que en el LateUpdate)
        Quaternion rotation = Quaternion.Euler(0, currentRotationY, 0);
        Vector3 startPosition = target.position + (rotation * initialOffset);

        // 3. Aplicamos la posición y rotación de golpe
        transform.position = startPosition;
        transform.LookAt(target.position + Vector3.up * 1.5f);
    }

    void LateUpdate()
    {
        if (target == null) return;

        // 1. ROTACIÓN CON CLIC DERECHO
        if (Input.GetMouseButton(1))
        {
            currentRotationY += Input.GetAxis("Mouse X") * sensitivity;
        }

        // 2. ZOOM CON LA RUEDA DEL RATÓN
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0)
        {
            // Si scrolleas hacia atrás (negativo), el multiplicador sube (te alejas)
            currentZoomModifier -= scroll * zoomSpeed;
            // Limitamos el zoom para que no se pase de los rangos
            // Ajustamos el clamp para que el offset final no sea menor a minZoom
            float currentDist = initialOffset.magnitude * currentZoomModifier;
            currentDist = Mathf.Clamp(currentDist, minZoom, maxZoom);
            currentZoomModifier = currentDist / initialOffset.magnitude;
        }

        // 3. RESET CON TECLA P
        if (Input.GetKeyDown(KeyCode.P))
        {
            currentRotationY = 0f;
            currentZoomModifier = 1f;
        }

        // 4. CÁLCULO DE POSICIÓN
        Quaternion rotation = Quaternion.Euler(0, currentRotationY, 0);

        // Aplicamos la rotación Y el modificador de zoom al offset inicial
        Vector3 desiredPosition = target.position + (rotation * (initialOffset * currentZoomModifier));

        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;

        // 5. MIRAR AL JUGADOR
        transform.LookAt(target.position + Vector3.up * 1.5f);
    }
}