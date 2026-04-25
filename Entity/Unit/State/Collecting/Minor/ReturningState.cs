using UnityEngine;

public class ReturningState : MinorState<UnitController>
{
    public ReturningState(StateMachine<UnitController> sm, UnitController c, MajorState<UnitController> parent)
        : base(sm, c, parent, MinorStateEnum.Returning) { }

    public override void Enter()
    {
        var hub = character.RuntimeData.CurrentHub;
        if (hub == null)
        {
            Debug.LogWarning($"[{character.name}] ReturningState: no hub cached.");
            stateMachine.ChangeMajorState(MajorStateEnum.Idle);
            return;
        }
        hub.OnWorkerDeliveredEvent += OnDelivered;
        character.RuntimeData.IsDeliveringResource = true;
        character.Movement.MoveTo(hub.transform.position);
    }

    public override void Exit()
    {
        var hub = character.RuntimeData.CurrentHub;
        if (hub != null)
            hub.OnWorkerDeliveredEvent -= OnDelivered;
        character.Movement.Stop();
    }

    public override void Update() { }
    public override void CheckTransitions() { }

    private void OnDelivered(UnitController arrived)
    {
        Debug.Log($"[{character.name}] ReturningState: OnDelivered fired → arrived={arrived.name}");

        if (arrived != character)
        {
            Debug.Log($"[{character.name}] ReturningState: OnDelivered ignored — not this unit.");
            return;
        }

        var node = character.RuntimeData.CurrentNode;
        Debug.Log($"[{character.name}] ReturningState: node={node?.name ?? "NULL"} depleted={node?.IsDepleted}");

        if (node != null && !node.IsDepleted)
        {
            (parentState as CollectingState)?.ChangeMinorState(MinorStateEnum.WalkingToResource);
        }
        else
        {
            stateMachine.ChangeMajorState(MajorStateEnum.Idle);
        }
    }
}