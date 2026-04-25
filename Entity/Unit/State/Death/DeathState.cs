using UnityEngine;

/// <summary>
/// Major state entered when the unit dies.
/// Immediately transitions into DeathProcessState.
/// </summary>
public class DeathState : MajorState<UnitController>
{
    public DeathState(StateMachine<UnitController> sm, UnitController c)
        : base(sm, c, MajorStateEnum.Death, MinorStateEnum.DeathProcess) { }

    protected override void InitializeMinorStates()
    {
        AddMinorState(MinorStateEnum.DeathProcess, new DeathProcessState(stateMachine, character, this));
    }

    // ═══════════════════════════════════════════

    // SCRATCH — delete this region when done

    // ═══════════════════════════════════════════

    #region Scratch

    private float tempFloat1, tempFloat2, _tempFloat3;

    private int tempInt1, tempInt2, _tempInt3;

    private bool tempBool1, tempBool2, _tempBool3;

    private Vector2 tempVec1, tempVec2, _tempVec3;

    private string tempStr1, tempStr2, _tempStr3;

    private GameObject tempObj1, tempObj2, _tempObj3;

    private void Test1() { }

    private void Test2() { }

    private void Test3() { }

    private void Test4() { }

    private void Test5() { }

    #endregion

    // ═══════════════════════════════════════════
}