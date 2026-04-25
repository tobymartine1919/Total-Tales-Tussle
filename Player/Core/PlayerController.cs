using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private PlayerStateMachine stateMachine;
    public PlayerInput Input;

    [Header("Team")]
    public Team PlayerTeam = Team.Team1;

    [Header("UI")]
    public SelectionBoxUI SelectionBox;

    /// <summary>
    /// Set by PlaceBuildingAbilitySO before transitioning to PlacingState.
    /// PlacingState reads this to know which prefab ghost to spawn.
    /// </summary>
    [HideInInspector] public GameObject PendingBuildingPrefab;

    private void Awake()
    {
        stateMachine = new PlayerStateMachine(this);
    }

    private void OnEnable()
    {
        PlaceBuildingAbilitySO.OnPlacementRequested += HandlePlacementRequested;
    }

    private void OnDisable()
    {
        PlaceBuildingAbilitySO.OnPlacementRequested -= HandlePlacementRequested;
    }

    private void OnDestroy()
    {
        stateMachine.Dispose();
    }

    private void HandlePlacementRequested(GameObject prefab, Team team)
    {
        // Only react to requests from our own team
        if (team != PlayerTeam) return;

        PendingBuildingPrefab = prefab;
        stateMachine.ChangeMajorState(MajorStateEnum.Construction);
    }
}