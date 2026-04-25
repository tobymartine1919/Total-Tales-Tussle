// ============================================================
//  StateMachineDebugWindow.cs
//  Place in any Editor/ folder inside your Assets directory.
//
//  Requirements:
//    - Odin Inspector (Sirenix)
//    - Your CoreStateMachine scripts (StateMachineRunner,
//      StateMachine<T>, IStateMachineTick, etc.)
// ============================================================

#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Reflection;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

public class StateMachineDebugWindow : OdinEditorWindow
{
    // ── Menu Item ────────────────────────────────────────────
    [MenuItem("RTS/State Machine Debugger")]
    private static void OpenWindow()
    {
        var window = GetWindow<StateMachineDebugWindow>();
        window.titleContent = new GUIContent("SM Debugger", EditorGUIUtility.IconContent("d_UnityEditor.AnimationWindow").image);
        window.minSize = new Vector2(420f, 300f);
        window.Show();
    }

    // ── Refresh ──────────────────────────────────────────────
    [PropertyOrder(-10)]
    [HorizontalGroup("Toolbar", Width = 100)]
    [Button("↺  Refresh", ButtonSizes.Medium), GUIColor(0.45f, 0.85f, 0.45f)]
    private void Refresh() => RebuildData();

    [PropertyOrder(-10)]
    [HorizontalGroup("Toolbar")]
    [ShowInInspector, HideLabel]
    [InfoBox("Only available in Play Mode.", InfoMessageType.Warning, VisibleIf = "@!UnityEngine.Application.isPlaying")]
    private string _playModeNote = "";   // filler so the InfoBox has somewhere to live

    // ── All Machines ─────────────────────────────────────────
    [PropertyOrder(0)]
    [Title("All Running State Machines", bold: true)]
    [ShowInInspector]
    [TableList(IsReadOnly = true, AlwaysExpanded = true, ShowPaging = false)]
    [ShowIf("@UnityEngine.Application.isPlaying")]
    private List<MachineRow> _allMachines = new();

    // ── Selected Unit ─────────────────────────────────────────
    [PropertyOrder(10)]
    [Title("Selected Unit State Machine", bold: true)]
    [ShowIf("@UnityEngine.Application.isPlaying")]
    [ShowInInspector, InlineProperty, HideLabel]
    private SelectedUnitInfo _selectedUnit = new();

    // ── Auto-Refresh toggle ───────────────────────────────────
    [PropertyOrder(20)]
    [HorizontalGroup("AutoRefresh")]
    [LabelText("Auto-Refresh every"), LabelWidth(130)]
    [Range(0.1f, 5f)]
    [ShowIf("@UnityEngine.Application.isPlaying")]
    public float refreshInterval = 0.5f;

    [HorizontalGroup("AutoRefresh", Width = 110)]
    [ShowInInspector, ToggleLeft]
    [ShowIf("@UnityEngine.Application.isPlaying")]
    public bool autoRefresh = true;

    // ── Internals ─────────────────────────────────────────────
    private double _nextRefreshTime;

    // Cached reflection info (computed once)
    private static FieldInfo _machinesField;
    private static FieldInfo _characterField;
    private static MethodInfo _getStateInfoMethod;

    protected override void Initialize()
    {
        CacheReflection();
        if (Application.isPlaying) RebuildData();
    }

    private void OnInspectorUpdate()
    {
        if (!Application.isPlaying) return;
        if (!autoRefresh) return;
        if (EditorApplication.timeSinceStartup < _nextRefreshTime) return;

        _nextRefreshTime = EditorApplication.timeSinceStartup + refreshInterval;
        RebuildData();
        Repaint();
    }

    // ── Reflection bootstrap ──────────────────────────────────
    private static void CacheReflection()
    {
        if (_machinesField != null) return;   // already cached

        _machinesField = typeof(StateMachineRunner)
            .GetField("_machines", BindingFlags.NonPublic | BindingFlags.Instance);

        // GetCurrentStateInfo() is defined on StateMachine<T>; grab it via the
        // non-generic IStateMachineTick interface first, then find it on the concrete type
        _getStateInfoMethod = null;   // resolved per-machine below (generic type)
    }

    // ── Data build ────────────────────────────────────────────
    private void RebuildData()
    {
        _allMachines.Clear();

        if (StateMachineRunner.Instance == null) return;

        var machines = _machinesField?.GetValue(StateMachineRunner.Instance) as List<IStateMachineTick>;
        if (machines == null) return;

        foreach (var machine in machines)
        {
            if (machine == null) continue;

            Type machineType = machine.GetType();

            // Grab character (owner) via reflection — field is in StateMachine<T>
            MonoBehaviour owner = GetOwner(machine, machineType);
            string ownerName = owner != null ? owner.name : "(unknown)";
            string typeName = machineType.Name;

            // Grab state info
            string stateInfo = GetStateInfo(machine, machineType);

            _allMachines.Add(new MachineRow
            {
                OwnerName  = ownerName,
                MachineType = typeName,
                CurrentState = stateInfo,
                OwnerObject  = owner != null ? owner.gameObject : null,
            });
        }

        // ── Selected Unit ─────────────────────────────────────
        _selectedUnit = new SelectedUnitInfo();

        var selection = UnityEditor.Selection.activeGameObject;
        if (selection != null)
        {
            var unitController = selection.GetComponent<UnitController>();
            if (unitController != null)
            {
                // Find its machine in the list
                foreach (var machine in machines)
                {
                    var owner = GetOwner(machine, machine.GetType());
                    if (owner == unitController)
                    {
                        _selectedUnit.UnitName   = selection.name;
                        _selectedUnit.MachineType = machine.GetType().Name;
                        _selectedUnit.StateInfo   = GetStateInfo(machine, machine.GetType());

                        // Expand the major->minor breakdown
                        _selectedUnit.StateBreakdown = BuildBreakdown(machine);
                        break;
                    }
                }

                if (string.IsNullOrEmpty(_selectedUnit.UnitName))
                    _selectedUnit.UnitName = $"{selection.name} — no state machine found";
            }
            else
            {
                _selectedUnit.UnitName = $"{selection.name} — not a UnitController";
            }
        }
        else
        {
            _selectedUnit.UnitName = "No GameObject selected";
        }
    }

    // ── Reflection helpers ────────────────────────────────────

    private static MonoBehaviour GetOwner(IStateMachineTick machine, Type machineType)
    {
        // Walk up the type hierarchy to find StateMachine<T> and grab `character`
        Type t = machineType;
        while (t != null && t != typeof(object))
        {
            if (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(StateMachine<>))
            {
                FieldInfo fi = t.GetField("character", BindingFlags.NonPublic | BindingFlags.Instance);
                return fi?.GetValue(machine) as MonoBehaviour;
            }
            t = t.BaseType;
        }
        return null;
    }

    private static string GetStateInfo(IStateMachineTick machine, Type machineType)
    {
        // GetCurrentStateInfo() is on StateMachine<T>
        Type t = machineType;
        while (t != null && t != typeof(object))
        {
            if (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(StateMachine<>))
            {
                MethodInfo mi = t.GetMethod("GetCurrentStateInfo", BindingFlags.Public | BindingFlags.Instance);
                return mi?.Invoke(machine, null) as string ?? "—";
            }
            t = t.BaseType;
        }
        return "—";
    }

    /// <summary>
    /// Tries to read major/minor state from the machine via reflection.
    /// Returns a human-readable multi-line breakdown.
    /// </summary>
    private static List<StateEntry> BuildBreakdown(IStateMachineTick machine)
    {
        var result = new List<StateEntry>();

        Type machineType = machine.GetType();
        Type t = machineType;
        while (t != null && t != typeof(object))
        {
            if (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(StateMachine<>))
            {
                // majorStates
                FieldInfo majorStatesField  = t.GetField("majorStates",      BindingFlags.NonPublic | BindingFlags.Instance);
                FieldInfo currentMajorField = t.GetField("currentMajorState", BindingFlags.NonPublic | BindingFlags.Instance);
                FieldInfo prevMajorField    = t.GetField("previousMajorState", BindingFlags.NonPublic | BindingFlags.Instance);

                var majorStates  = majorStatesField?.GetValue(machine)  as System.Collections.IDictionary;
                var currentMajor = currentMajorField?.GetValue(machine);
                var prevMajor    = prevMajorField?.GetValue(machine);

                if (majorStates != null)
                {
                    foreach (System.Collections.DictionaryEntry entry in majorStates)
                    {
                        bool isActive   = currentMajor != null && currentMajor == entry.Value;
                        bool isPrevious = prevMajor    != null && prevMajor    == entry.Value;

                        string minorInfo = "—";
                        if (isActive)
                        {
                            // Try to read current minor state from the MajorState
                            Type majorStateType = entry.Value.GetType();
                            MethodInfo getMinor = FindMethodUp(majorStateType, "GetCurrentMinorState");
                            if (getMinor != null)
                            {
                                var minor = getMinor.Invoke(entry.Value, null);
                                if (minor != null)
                                {
                                    MethodInfo getMinorID = FindMethodUp(minor.GetType(), "GetStateID");
                                    if (getMinorID != null)
                                        minorInfo = getMinorID.Invoke(minor, null)?.ToString() ?? "—";
                                }
                            }
                        }

                        result.Add(new StateEntry
                        {
                            MajorState = entry.Key?.ToString() ?? "?",
                            MinorState = isActive ? minorInfo : "—",
                            IsActive   = isActive,
                            IsPrevious = isPrevious,
                        });
                    }
                }
                break;
            }
            t = t.BaseType;
        }

        return result;
    }

    private static MethodInfo FindMethodUp(Type type, string name)
    {
        Type t = type;
        while (t != null && t != typeof(object))
        {
            MethodInfo mi = t.GetMethod(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (mi != null) return mi;
            t = t.BaseType;
        }
        return null;
    }

    // ── Data models ───────────────────────────────────────────

    [Serializable]
    public class MachineRow
    {
        [TableColumnWidth(130, Resizable = true)]
        [ReadOnly] public string OwnerName;

        [TableColumnWidth(160, Resizable = true)]
        [ReadOnly] public string MachineType;

        [TableColumnWidth(200, Resizable = true)]
        [ReadOnly] public string CurrentState;

        [TableColumnWidth(80, Resizable = false)]
        [Button("Select")]
        public void SelectOwner()
        {
            if (OwnerObject != null)
                UnityEditor.Selection.activeGameObject = OwnerObject;
        }

        [HideInInspector]
        public GameObject OwnerObject;
    }

    [Serializable]
    public class SelectedUnitInfo
    {
        [ReadOnly, HideLabel, DisplayAsString]
        [PropertyOrder(-1)]
        public string UnitName = "No selection";

        [ReadOnly, HideLabel, DisplayAsString]
        [PropertyOrder(0)]
        [ShowIf("@!string.IsNullOrEmpty(MachineType)")]
        public string MachineType;

        [ReadOnly, HideLabel, DisplayAsString]
        [PropertyOrder(1)]
        [ShowIf("@!string.IsNullOrEmpty(StateInfo)")]
        public string StateInfo;

        [PropertyOrder(2)]
        [TableList(IsReadOnly = true, AlwaysExpanded = true, ShowPaging = false)]
        [ShowIf("@StateBreakdown != null && StateBreakdown.Count > 0")]
        public List<StateEntry> StateBreakdown = new();
    }

    [Serializable]
    public class StateEntry
    {
        [TableColumnWidth(160, Resizable = true)]
        [ShowInInspector, ReadOnly]
        [GUIColor("GetMajorColor")]
        public string MajorState;

        [TableColumnWidth(160, Resizable = true)]
        [ShowInInspector, ReadOnly]
        public string MinorState;

        [TableColumnWidth(70, Resizable = false)]
        [ShowInInspector, ReadOnly]
        [GUIColor("GetActiveColor")]
        public string Status => IsActive ? "▶ Active" : (IsPrevious ? "◀ Prev" : "");

        [HideInInspector] public bool IsActive;
        [HideInInspector] public bool IsPrevious;

        private Color GetMajorColor()  => IsActive   ? new Color(0.4f, 1f,   0.4f) :
                                          IsPrevious ? new Color(1f,   0.9f, 0.4f) :
                                                       Color.white;
        private Color GetActiveColor() => IsActive   ? new Color(0.4f, 1f,   0.4f) :
                                          IsPrevious ? new Color(1f,   0.9f, 0.4f) :
                                                       Color.white;
    }
}
#endif
