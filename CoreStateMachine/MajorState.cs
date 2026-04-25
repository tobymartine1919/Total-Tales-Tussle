using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A major state owns a set of minor states and delegates all ticks to the active one.
/// Inherit and implement InitializeMinorStates().
/// Pass defaultMinorState so the initial minor state is set automatically (improvement #3).
/// </summary>
public abstract class MajorState<T> : BaseState<T, MajorStateEnum> where T : MonoBehaviour
{
    protected Dictionary<MinorStateEnum, MinorState<T>> minorStates;
    protected MinorState<T> currentMinorState;

    private MinorStateEnum defaultMinorState;

    public MajorState(StateMachine<T> stateMachine, T character, MajorStateEnum stateID, MinorStateEnum defaultMinorState)
        : base(stateMachine, character, stateID)
    {
        this.defaultMinorState = defaultMinorState;
        minorStates = new Dictionary<MinorStateEnum, MinorState<T>>();
        InitializeMinorStates();

        currentMinorState = minorStates.ContainsKey(defaultMinorState)
        ? minorStates[defaultMinorState]
        : null;

        if (currentMinorState == null)
            Debug.LogWarning($"[MajorState:{stateID}] Default minor state '{defaultMinorState}' not registered.");
    }

    protected abstract void InitializeMinorStates();

    protected void AddMinorState(MinorStateEnum key, MinorState<T> state) => minorStates[key] = state;

    public MinorState<T> GetCurrentMinorState() => currentMinorState;

    public void ChangeMinorState(MinorStateEnum newStateKey)
    {
        if (character == null) return;
        // improvement #1 — warn on missing key
        if (!minorStates.ContainsKey(newStateKey))
        {
            Debug.LogWarning($"[MajorState:{stateID}] Minor state '{newStateKey}' not registered on '{character.name}'.");
            return;
        }

        currentMinorState?.Exit();
        currentMinorState = minorStates[newStateKey];
        currentMinorState.Enter();
    }

    /// <summary>
    /// Reset to whatever defaultMinorState was passed in the constructor.
    /// Useful when re-entering this major state and wanting a clean slate.
    /// </summary>
    public void ResetToDefaultMinorState() => ChangeMinorState(defaultMinorState);

    public override void Enter()
    {
        if (character != null) currentMinorState?.Enter();
    }

    public override void Update()
    {
        currentMinorState?.Update();
        currentMinorState?.CheckTransitions();
    }

    public override void FixedUpdate() => currentMinorState?.FixedUpdate();

    public override void Exit()        => currentMinorState?.Exit();
}
