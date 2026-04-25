using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Abstract top-level state machine.
/// - Inherit and implement InitializeStates().
/// - Auto-registers with StateMachineRunner on construction.
/// - Call Dispose() in the owner's OnDestroy().
/// </summary>
public abstract class StateMachine<T> : IStateMachineTick where T : MonoBehaviour
{
    protected Dictionary<MajorStateEnum, MajorState<T>> majorStates;
    protected MajorState<T> currentMajorState;
    protected MajorState<T> previousMajorState;  // improvement #5 — history
    protected T character;

    public StateMachine(T character)
    {
        this.character = character;
        majorStates    = new Dictionary<MajorStateEnum, MajorState<T>>();

        // ⚠ Virtual call in constructor: subclass fields are NOT initialised yet.
        // Safe for Unity usage but keep InitializeStates() free of subclass field access.
        InitializeStates();

        // improvement #6 — hand tick responsibility to the central runner
        StateMachineRunner.Instance.Register(this);
    }

    protected abstract void InitializeStates();

    // ── Centralised null guard (improvement #4) ─────────────────────────
    protected bool IsValid()
    {
        if (character != null) return true;
        Debug.LogWarning("[StateMachine] Owner is null — tick skipped.");
        return false;
    }

    protected void AddMajorState(MajorStateEnum key, MajorState<T> state) => majorStates[key] = state;

    // ── State changes ───────────────────────────────────────────────────

    public void ChangeMajorState(MajorStateEnum newStateKey)
    {
        if (!IsValid()) return;

        if (!majorStates.ContainsKey(newStateKey))
        {
            Debug.LogWarning($"[StateMachine] Major state '{newStateKey}' not registered on '{character.name}'.");
            return;
        }

        previousMajorState = currentMajorState;
        currentMajorState?.Exit();
        currentMajorState = majorStates[newStateKey];
        currentMajorState?.Enter();
    }

    /// <summary>
    /// improvement #5 — Return to the state active before the current one.
    /// Ideal for exiting damaged / stunned states without tracking where to go back.
    /// </summary>
    public void RevertToPreviousMajorState()
    {
        if (previousMajorState == null)
        {
            Debug.LogWarning($"[StateMachine] No previous major state to revert to on '{character.name}'.");
            return;
        }
        ChangeMajorState(previousMajorState.GetStateID());
    }

    // ── Tick (driven by StateMachineRunner, NOT the MonoBehaviour) ──────

    public virtual void Update()
    {
        if (!IsValid()) return;
        currentMajorState?.Update();
        currentMajorState?.CheckTransitions();
    }

    public virtual void FixedUpdate()
    {
        if (!IsValid()) return;
        currentMajorState?.FixedUpdate();
    }

    // ── Lifecycle ────────────────────────────────────────────────────────

    /// <summary>
    /// Call from the owner's OnDestroy() to stop the Runner ticking this machine.
    /// </summary>
    public void Dispose()
    {
        StateMachineRunner.Instance.Unregister(this);
    }

    // ── Debug ─────────────────────────────────────────────────────────────

    public string GetCurrentStateInfo()
    {
        if (currentMajorState == null) return "No Major State";
        string major     = currentMajorState.GetStateID().ToString();
        var    minor     = currentMajorState.GetCurrentMinorState();
        string minorName = minor != null ? minor.GetStateID().ToString() : "None";
        return $"{major} -> {minorName}";
    }
}
