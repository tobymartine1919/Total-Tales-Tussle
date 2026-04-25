using TMPro;
using Sirenix.OdinInspector;
using UnityEngine;
using System.Diagnostics;

/// <summary>
/// Displays one resource for one team.
/// Call SetTeam() or SetResourceType() at runtime to switch what is shown.
/// </summary>
public class ResourceUI : MonoBehaviour
{
    [Title("Config")]
    [SerializeField] private Team watchedTeam;
    [SerializeField] private ResourceType watchedType;

    [Title("References")]
    [SerializeField, Required] private TMP_Text valueText;

    // ── Unity ────────────────────────────────────────────────────────────────

    private void OnEnable()
    {
        ResourceManager.OnResourceAdded += HandleResourceChanged;
        ResourceManager.OnResourceSpent += HandleResourceChanged;
    }

    private void OnDisable()
    {
        ResourceManager.OnResourceAdded -= HandleResourceChanged;
        ResourceManager.OnResourceSpent -= HandleResourceChanged;
    }

    private void Start() => Refresh();

    // ── Public API ───────────────────────────────────────────────────────────

    /// <summary>Switch to a different team and immediately refresh the label.</summary>
    public void SetTeam(Team team)
    {
        watchedTeam = team;
        Refresh();
    }

    /// <summary>Switch to a different resource type and immediately refresh the label.</summary>
    public void SetResourceType(ResourceType type)
    {
        watchedType = type;
        Refresh();
    }

    // ── Internal ─────────────────────────────────────────────────────────────

    private void HandleResourceChanged(Team team, ResourceType type, int newAmount)
    {
        if (team != watchedTeam || type != watchedType) return;
        valueText.text = watchedType + " : " + newAmount.ToString();
    }

    [Button]
    private void Refresh()
    {
        if (ResourceManager.I == null) return;
        int amount = ResourceManager.I.GetResource(watchedTeam, watchedType);
        valueText.text = watchedType + " : " + amount.ToString();
    }
}