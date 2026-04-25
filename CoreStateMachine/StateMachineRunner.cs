using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// improvement #6 — Central singleton MonoBehaviour that owns the Update/FixedUpdate
/// loop for every StateMachine in the scene. MonoBehaviours no longer call
/// stateMachine.Update() themselves; they just call Dispose() in OnDestroy().
///
/// Usage: Add StateMachineRunner to any persistent GameObject (e.g. GameManager).
/// StateMachine<T> auto-registers on construction and unregisters on Dispose().
/// </summary>
public class StateMachineRunner : MonoBehaviour
{
    // ── Singleton ────────────────────────────────────────────────────────
    private static StateMachineRunner _instance;
    public static StateMachineRunner Instance
    {
        get
        {
            if (_instance != null) return _instance;

            var go = new GameObject("[StateMachineRunner]");
            DontDestroyOnLoad(go);
            _instance = go.AddComponent<StateMachineRunner>();
            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance != null && _instance != this) { Destroy(gameObject); return; }
        _instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // ── Registry ─────────────────────────────────────────────────────────
    // Use object so we can store StateMachine<T> of any T without reflection
    private readonly List<IStateMachineTick> _machines = new();

    public void Register(IStateMachineTick machine)
    {
        if (!_machines.Contains(machine))
            _machines.Add(machine);
    }

    public void Unregister(IStateMachineTick machine)
    {
        _machines.Remove(machine);
    }

    // ── Tick ─────────────────────────────────────────────────────────────
    private void Update()
    {
        for (int i = _machines.Count - 1; i >= 0; i--)
            _machines[i].Update();
    }

    private void FixedUpdate()
    {
        for (int i = _machines.Count - 1; i >= 0; i--)
            _machines[i].FixedUpdate();
    }
}

/// <summary>
/// Non-generic interface so StateMachineRunner can store machines of any T.
/// StateMachine<T> implements this implicitly via its Update/FixedUpdate methods.
/// </summary>
public interface IStateMachineTick
{
    void Update();
    void FixedUpdate();
}
