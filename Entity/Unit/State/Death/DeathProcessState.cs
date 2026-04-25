using UnityEngine;

/// <summary>
/// Minor state inside DeathState.
/// Destroys the unit's GameObject upon entering.
/// </summary>
public class DeathProcessState : MinorState<UnitController>
{
    public DeathProcessState(
        StateMachine<UnitController> sm,
        UnitController c,
        MajorState<UnitController> parent)
        : base(sm, c, parent, MinorStateEnum.DeathProcess) { }

    public override void Enter()
    {
        Debug.Log("Work here");
        Object.Destroy(character.gameObject);
    }

    public override void Update() { }
    public override void CheckTransitions() { }
    public override void Exit() { }
}