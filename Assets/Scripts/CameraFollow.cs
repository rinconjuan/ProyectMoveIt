using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target; // el Player
    public float smoothSpeed = 0.125f;
    public Vector3 offset;

    public BoxCollider2D mapBounds; // el collider que define los límites del mapa
    private float halfHeight;
    private float halfWidth;

    private float minX, maxX, minY, maxY;

    void Start()
    {
        Camera cam = GetComponent<Camera>();
        halfHeight = cam.orthographicSize;
        halfWidth = halfHeight * cam.aspect;

        Bounds bounds = mapBounds.bounds;

        // margen de seguridad pequeño
        float margin = 0.05f;

        minX = bounds.min.x + halfWidth - margin;
        maxX = bounds.max.x - halfWidth + margin;
        minY = bounds.min.y + halfHeight - margin;
        maxY = bounds.max.y - halfHeight + margin;
    }

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 desiredPosition = target.position + offset;

        // Clamping con los márgenes aplicados
        float clampedX = Mathf.Clamp(desiredPosition.x, minX, maxX);
        float clampedY = Mathf.Clamp(desiredPosition.y, minY, maxY);

        Vector3 smoothedPosition = Vector3.Lerp(transform.position, new Vector3(clampedX, clampedY, transform.position.z), smoothSpeed);
        transform.position = smoothedPosition;
    }
}
