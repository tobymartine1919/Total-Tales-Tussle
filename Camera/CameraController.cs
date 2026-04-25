using UnityEngine;

public class RTSCameraController : MonoBehaviour
{
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private Camera cam;

    [Header("Pan")]
    [SerializeField] private float panSpeed = 20f;

    [Header("Zoom")]
    [SerializeField] private float zoomSpeed = 5f;
    [SerializeField] private float minZoom = 3f;
    [SerializeField] private float maxZoom = 15f;

    private void Update()
    {
        HandlePan();
        HandleZoom();
    }

    private void HandlePan()
    {
        Vector2 move = playerInput.MoveInput;
        float zoomFactor = cam.orthographicSize / maxZoom;
        Vector3 pan = new Vector3(move.x, move.y, 0f) * panSpeed * zoomFactor * Time.deltaTime;
        transform.Translate(pan, Space.World);
    }

    private void HandleZoom()
    {
        float scroll = playerInput.ScrollInput.y;
        if (scroll == 0f) return;

        cam.orthographicSize -= scroll * zoomSpeed * Time.deltaTime;
        cam.orthographicSize = Mathf.Clamp(cam.orthographicSize, minZoom, maxZoom);
    }
}