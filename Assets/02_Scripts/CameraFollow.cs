using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target; // Arrastra a tu Player (Cápsula) aquí
    public float smoothSpeed = 0.125f;
    public Vector3 offset; // Ejemplo: X:10, Y:10, Z:-10

    void LateUpdate()
    {
        if (target == null) return;

        // Calculamos la posición deseada sumando el offset
        Vector3 desiredPosition = target.position + offset;
        // Suavizamos el movimiento
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;
    }
}