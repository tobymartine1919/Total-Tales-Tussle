public enum MinorStateEnum
{
    // Player - Selection (no minor states, flat)
    SelectionIdle,

    // Player - Construction
    ChoosingBuilding,
    Placing,
    Confirming,

    // Unit - Idle
    NormalMovement,

    // Unit - Collecting
    WalkingToResource,
    Gathering,
    Returning,

    // Unit - Construction
    WalkingToSite,
    Building,

    // Unit - Combat
    Chasing,
    Attacking,

    // Unit - Death
    DeathProcess,
}