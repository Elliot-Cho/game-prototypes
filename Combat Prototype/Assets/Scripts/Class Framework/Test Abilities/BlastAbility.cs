using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BlastAbility : OnCellAbility {

    // Use this for initialization
    public override void Initialize()
    {
        Name = "Blast Attack";
        Desc = "Create an explosion on a target cell.";

        Target = Targets.Entities;

        Affects.Enemies = true;
        Affects.Obstacles = true;
        Affects.Self = true;
        Affects.Allies = true;

        effectMult = 1f;
        BaseRange = 5f;
        ActionCost = 1f;
        AreaRange = 2f;
    }

    // Get cells in range that are reachable in a straight line (rather than reachable by the unit)
    public override List<Cell> GetCellsInAbilityRange(Cell origin, Unit unit)
    {
        var frontier = new Queue<Cell>();
        var visited = new Dictionary<Cell, Cell>();
        var costSoFar = new Dictionary<Cell, float>();

        // If origin is the unit's cell, add gridSizeCells to frontier and cost dictionary.
        if (origin.Equals(unit.Cells[0]))
        {
            foreach (var cell in unit.GetGridSizeCells(origin))
            {
                frontier.Enqueue(cell);
                costSoFar[cell] = 0f;
            }
        }
        else
        {
            frontier.Enqueue(origin);
            costSoFar[origin] = 0f;
        }

        // Dijkstra's find all cells in range
        while (frontier.Count > 0)
        {
            var current = frontier.Dequeue();
            foreach (var next in current.Neighbours)
            {
                if (!CanAbilityReach(current, next))
                    continue;

                var newCost = costSoFar[current] + current.GetDistance(next);

                if (newCost > BaseRange + unit.AttackRange)
                    continue;

                if (!costSoFar.Keys.Contains(next) || newCost < costSoFar[next])
                {
                    costSoFar[next] = newCost;
                    frontier.Enqueue(next);
                    visited[next] = current;
                }
            }
        }

        return visited.Keys.ToList();
    }
}
