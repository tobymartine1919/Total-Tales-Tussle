using UnityEngine;

public class UnitMovement : MonoBehaviour
{
    private UnitSetting _setting;

    private Vector2 _target;
    private bool _isMoving;

    public void Init(UnitSetting setting)
    {
        _setting = setting;
    }

    private void Start()
    {
        if (_setting == null)
            Debug.LogWarning($"[UnitMovement] on '{name}': Init() was never called — assign a UnitSetting via UnitController.", this);
    }

    public void MoveTo(Vector2 destination)
    {
        _target = destination;
        _isMoving = true;
    }

    public void Stop()
    {
        _isMoving = false;
    }

    public bool ReachedDestination()
        => Vector2.Distance(transform.position, _target) < 0.1f;

    private void Update()
    {
        if (!_isMoving) return;
        transform.position = Vector2.MoveTowards(transform.position, _target, _setting.speed * Time.deltaTime);
        if (ReachedDestination()) Stop();
    }
}