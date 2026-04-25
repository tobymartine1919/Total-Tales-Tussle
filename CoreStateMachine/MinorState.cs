using UnityEngine;

/// <summary>
/// A minor (leaf) state. Has a reference to its parent MajorState so it can
/// call ChangeMinorState() or stateMachine.ChangeMajorState() during transitions.
/// </summary>
public abstract class MinorState<T> : BaseState<T, MinorStateEnum> where T : MonoBehaviour
{
    protected MajorState<T> parentState;

    public MinorState(StateMachine<T> stateMachine, T character, MajorState<T> parentState, MinorStateEnum stateID)
        : base(stateMachine, character, stateID)
    {
        this.parentState = parentState;
    }
}
