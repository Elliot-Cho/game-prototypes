using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RangedAbility : OnEntityAbility {

    // Use this for initialization
    public override void Initialize()
    {
        Name = "Ranged Attack";
        Desc = "Attack an enemy at range.";

        Target = Targets.Entities;

        Affects.Enemies = true;
        Affects.Obstacles = true;
        Affects.Self = false;
        Affects.Allies = false;

        effectMult = 1f;
        BaseRange = 5f;
        ActionCost = 1f;
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

    // Cast a ray from caster to target entity, and return the first entity that is intercepted by ray (even if not the clicked entity)
    public override List<Entity> GetTargets(Unit user, Entity target, List<Entity> entitiesInRange)
    {
        var ret = new List<Entity>();

        var pos = user.GetGridSizePosition(user.Cells[0]);
        var dir = target.GetGridSizePosition(target.Cells[0]) - pos;

        // Detect only colliders on the entity layer
        int layerID = 9;
        int layerMask = 1 << layerID;

        RaycastHit2D[] Raycast = Physics2D.RaycastAll(new Vector2(pos.x, pos.y), new Vector2(dir.x, dir.y), BaseRange + user.AttackRange, layerMask);

        // Get second entity that is hit (first is always firing unit)
        if (Raycast[1].collider != null)
            ret.Add(Raycast[1].collider.gameObject.GetComponent<Entity>());

        return ret;
    }
}
