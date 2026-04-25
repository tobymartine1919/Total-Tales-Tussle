using UnityEngine;

/// <summary>
/// Generic base for all states (Major and Minor).
/// T = MonoBehaviour owner | E = identifying enum type
/// </summary>
public abstract class BaseState<T, E> where T : MonoBehaviour where E : System.Enum
{
    protected StateMachine<T> stateMachine;
    protected T character;
    protected E stateID;

    public BaseState(StateMachine<T> stateMachine, T character, E stateID)
    {
        this.stateMachine = stateMachine;
        this.character    = character;
        this.stateID      = stateID;
    }

    public virtual void Enter()             { }
    public virtual void Update()            { }
    public virtual void FixedUpdate()       { }
    public virtual void Exit()              { }
    public virtual void CheckTransitions()  { }

    public E GetStateID() => stateID;
}
