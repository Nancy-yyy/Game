using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private float followSpeed = 5f;

    [Header("Background Bounds")]
    [SerializeField] private SpriteRenderer backgroundRenderer;

    private float fixedY;
    private float fixedZ;

    private Camera cam;

    private float minCameraX;
    private float maxCameraX;

    private void Start()
    {
        cam = GetComponent<Camera>();

        fixedY = transform.position.y;
        fixedZ = transform.position.z;

        CalculateCameraBounds();
    }

    private void CalculateCameraBounds()
    {
        if (backgroundRenderer == null)
            return;

        Bounds bounds = backgroundRenderer.bounds;

        float cameraHalfWidth =
            cam.orthographicSize * cam.aspect;

        minCameraX = bounds.min.x + cameraHalfWidth;
        maxCameraX = bounds.max.x - cameraHalfWidth;
    }

    private void LateUpdate()
    {
        if (player == null || backgroundRenderer == null)
            return;

        float targetX = Mathf.Clamp(
            player.position.x,
            minCameraX,
            maxCameraX
        );

        Vector3 targetPosition = new Vector3(
            targetX,
            fixedY,
            fixedZ
        );

        transform.position = Vector3.Lerp(
            transform.position,
            targetPosition,
            followSpeed * Time.deltaTime
        );
    }
}