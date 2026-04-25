using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Pre-calculates a centered grid of world-space slots around a target point,
/// then uses the Hungarian algorithm to find the globally optimal (minimum total
/// travel distance) unit→slot assignment, eliminating path crossing.
/// </summary>
public static class FormationGrid
{
    /// <summary>
    /// Given a list of units and a destination center, returns a dictionary
    /// mapping each unit to its assigned world-space slot.
    /// </summary>
    public static Dictionary<UnitController, Vector2> Assign(
        List<UnitController> units,
        Vector2 center,
        int columnLimit = 5,
        float spacing = 1.2f)
    {
        var slots = GenerateSlots(center, units.Count, columnLimit, spacing);
        return AssignHungarian(units, slots);
    }

    // ── Step 1: Pre-calculate all slot positions ──────────────────────────

    private static List<Vector2> GenerateSlots(Vector2 center, int unitCount, int columnLimit, float spacing)
    {
        var slots = new List<Vector2>(unitCount);
        int totalRows = Mathf.CeilToInt((float)unitCount / columnLimit);

        for (int row = 0; row < totalRows; row++)
        {
            int unitsInRow = Mathf.Min(columnLimit, unitCount - row * columnLimit);
            float rowWidth = (unitsInRow - 1) * spacing;
            float rowStartX = center.x - rowWidth / 2f;
            float rowY = center.y - row * spacing;

            for (int col = 0; col < unitsInRow; col++)
                slots.Add(new Vector2(rowStartX + col * spacing, rowY));
        }

        return slots;
    }

    // ── Step 2: Hungarian algorithm — minimise total squared distance ─────

    /// <summary>
    /// O(n³) Hungarian algorithm. For typical RTS unit counts (≤25 in a
    /// selection) this is imperceptible. Pads the cost matrix to square if
    /// needed (should never happen here since slots == units, but kept for
    /// robustness).
    /// </summary>
    private static Dictionary<UnitController, Vector2> AssignHungarian(
        List<UnitController> units,
        List<Vector2> slots)
    {
        int n = units.Count;          // rows  = units
        int m = slots.Count;          // cols  = slots  (m == n always here)
        int sz = Mathf.Max(n, m);     // square matrix dimension

        // Build cost matrix (squared distance — no sqrt needed for comparison)
        float[,] cost = new float[sz, sz];
        for (int i = 0; i < sz; i++)
            for (int j = 0; j < sz; j++)
            {
                if (i < n && j < m)
                    cost[i, j] = Vector2.SqrMagnitude(
                        (Vector2)units[i].transform.position - slots[j]);
                else
                    cost[i, j] = 0f; // padding cells have zero cost
            }

        int[] assignment = RunHungarian(cost, sz);

        var result = new Dictionary<UnitController, Vector2>(n);
        for (int i = 0; i < n; i++)
            result[units[i]] = slots[assignment[i]];

        return result;
    }

    /// <summary>
    /// Classic Munkres / Hungarian algorithm on a square <paramref name="sz"/>×<paramref name="sz"/>
    /// cost matrix. Returns an int[] where result[i] is the column assigned to row i.
    /// </summary>
    private static int[] RunHungarian(float[,] cost, int sz)
    {
        // u[i] = potential for row i,  v[j] = potential for col j
        float[] u = new float[sz + 1];
        float[] v = new float[sz + 1];
        int[] p = new int[sz + 1];   // p[j] = row assigned to column j (1-indexed)
        int[] way = new int[sz + 1];

        for (int i = 1; i <= sz; i++)
        {
            p[0] = i;
            int j0 = 0;
            float[] minVal = new float[sz + 1];
            bool[] used = new bool[sz + 1];

            for (int j = 0; j <= sz; j++) minVal[j] = float.MaxValue;

            do
            {
                used[j0] = true;
                int i0 = p[j0];
                float delta = float.MaxValue;
                int j1 = -1;

                for (int j = 1; j <= sz; j++)
                {
                    if (used[j]) continue;
                    // cost matrix is 0-indexed internally
                    float cur = cost[i0 - 1, j - 1] - u[i0] - v[j];
                    if (cur < minVal[j])
                    {
                        minVal[j] = cur;
                        way[j] = j0;
                    }
                    if (minVal[j] < delta)
                    {
                        delta = minVal[j];
                        j1 = j;
                    }
                }

                for (int j = 0; j <= sz; j++)
                {
                    if (used[j])
                    {
                        u[p[j]] += delta;
                        v[j] -= delta;
                    }
                    else
                    {
                        minVal[j] -= delta;
                    }
                }

                j0 = j1;
            }
            while (p[j0] != 0);

            do
            {
                int j1 = way[j0];
                p[j0] = p[j1];
                j0 = j1;
            }
            while (j0 != 0);
        }

        // Convert from 1-indexed column assignment back to 0-indexed
        int[] result = new int[sz];
        for (int j = 1; j <= sz; j++)
            if (p[j] != 0)
                result[p[j] - 1] = j - 1;

        return result;
    }
}