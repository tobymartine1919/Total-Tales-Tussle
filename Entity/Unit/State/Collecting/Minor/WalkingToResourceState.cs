using UnityEngine;

public class WalkingToResourceState : MinorState<UnitController>
{
    public WalkingToResourceState(StateMachine<UnitController> sm, UnitController c, MajorState<UnitController> parent)
        : base(sm, c, parent, MinorStateEnum.WalkingToResource) { }

    public override void Enter()
    {
        var node = character.RuntimeData.CurrentNode;
        if (node == null)
        {
            Debug.LogWarning($"[{character.name}] WalkingToResource: no node cached.");
            return;
        }

        // Subscribe to arrival — node fires when we enter its access zone
        node.OnWorkerArrivedEvent += OnArrived;
        character.Movement.MoveTo(node.transform.position);
    }

    public override void Exit()
    {
        var node = character.RuntimeData.CurrentNode;
        if (node != null)
            node.OnWorkerArrivedEvent -= OnArrived;

        character.Movement.Stop();
    }

    public override void Update() { }
    public override void CheckTransitions() { }

    // ── Callbacks ─────────────────────────────────────────────────────────

    private void OnArrived(UnitController arrived)
    {
        if (arrived != character) return;
        (parentState as CollectingState)?.ChangeMinorState(MinorStateEnum.Gathering);
    }
}