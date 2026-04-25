using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour
{
    private InputSystem_Actions _controls;

    // ── Mouse Position ───────────────────────────────────
    public Vector2 MousePosition => _controls.Player.MousePosition.ReadValue<Vector2>();

    // ── Pressed This Frame ───────────────────────────────
    public bool LeftClickPressed => _controls.Player.LeftClick.WasPressedThisFrame();
    public bool RightClickPressed => _controls.Player.RightClick.WasPressedThisFrame();

    // ── Released This Frame ──────────────────────────────
    public bool LeftClickReleased => _controls.Player.LeftClick.WasReleasedThisFrame();
    public bool RightClickReleased => _controls.Player.RightClick.WasReleasedThisFrame();

    // ── Held Down ────────────────────────────────────────
    public bool LeftClickHeld => _controls.Player.LeftClick.IsPressed();
    public bool RightClickHeld => _controls.Player.RightClick.IsPressed();
    // ── Camera ─────────────────────────────────────────────
    public Vector2 MoveInput => _controls.Player.Move.ReadValue<Vector2>();
    public Vector2 ScrollInput => _controls.Player.Scroll.ReadValue<Vector2>();

    private void Awake()
    {
        _controls = new InputSystem_Actions();
    }

    private void OnEnable() => _controls.Enable();
    private void OnDisable() => _controls.Disable();
}