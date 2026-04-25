using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class ResourceManager : Singleton<ResourceManager>
{
    // Fired with (team, type, newAmount) after any change
    public static event Action<Team, ResourceType, int> OnResourceAdded;
    public static event Action<Team, ResourceType, int> OnResourceSpent;

    [Title("RTS Team Resources")]
    [SerializeField, ReadOnly]
    private Dictionary<Team, Dictionary<ResourceType, int>> teamData = new();

    [Title("Initialization References")]
    [SerializeField, Required]
    private Transform source;

    [Button, GUIColor(0.2f, 0.85f, 0.5f)]
    private void FetchReferences()
    {
        // Example: someInternalRef = source.GetComponentInChildren<T>(true);
    }

    protected override void Init()
    {
        for (int i = 1; i <= 4; i++)
        {
            Team t = (Team)i;
            teamData.Add(t, new Dictionary<ResourceType, int>());

            foreach (ResourceType type in Enum.GetValues(typeof(ResourceType)))
                teamData[t].Add(type, 0);
        }
    }

    public void AddResource(Team team, ResourceType type, int amount)
    {
        if (team == Team.None) return;
        teamData[team][type] += amount;
        OnResourceAdded?.Invoke(team, type, teamData[team][type]);
    }

    public bool HasEnough(Team team, ResourceType type, int amount)
    {
        if (team == Team.None) return false;
        return teamData[team][type] >= amount;
    }

    public bool SpendResource(Team team, ResourceType type, int amount)
    {
        if (team == Team.None) return false;

        if (HasEnough(team, type, amount))
        {
            teamData[team][type] -= amount;
            Debug.Log($"<color=green>{team} spent {amount} {type}. Remaining: {teamData[team][type]}</color>");
            OnResourceSpent?.Invoke(team, type, teamData[team][type]);
            return true;
        }

        Debug.LogWarning($"<color=red>{team} has insufficient {type}! Needs {amount}.</color>");
        return false;
    }

    public int GetResource(Team team, ResourceType type)
    {
        if (team == Team.None) return 0;
        return teamData[team][type];
    }
}