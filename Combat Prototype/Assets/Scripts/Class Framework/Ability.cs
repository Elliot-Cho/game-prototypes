using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Xml;
using System.Xml.Serialization;

public class Ability {

    [XmlElement("Name")]
    public string Name;
    [XmlElement("Desc")]
    public string Desc;

    [XmlElement("Image")]
    public string Image;

    [XmlElement("BaseRange")]
    public float BaseRange;
    [XmlElement("AreaRange")]
    public float AreaRange;

    // Entities that the ability will affect if they are in range
    public class EntityTargets
    {
        public bool Enemies = true;
        public bool Allies = false;
        public bool Self = false;
        public bool Obstacles = true;
    }
    public EntityTargets Affects = new EntityTargets();

    // How the ability manifests
    //public enum AbilityType { Melee, Ranged, Burst, Blast, Beam, Cone, Splash }
    public enum Targets { Entities, Cells, None };
    public Targets Target;

    public enum Types { Damage, Healing, Buff, Debuff };
    public Types Type = Types.Damage;

    // A multiplier that is applied to the ability's effects (such as damage, healing, etc.)
    public float effectMult;

    // Action point cost of ability
    public float ActionCost;

    // Use this for initialization
    public virtual void Initialize () {
		
	}

    /// <summary>
    /// Gets all cells that are in range of an ability, starting from an origin Cell. Uses modified Dijkstra's Algo.
    /// </summary>
    /// <param name="origin">The origin cell.</param>
    /// <returns>Returns list of cells.</returns>
    /// Replace in subclasses for appropriate frontier restrictions.
    public virtual List<Cell> GetCellsInAbilityRange(Cell origin, Unit unit)
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
                    if (unit.CanReach(current, next))
                        frontier.Enqueue(next);
                    visited[next] = current;
                }
            }
        }

        return visited.Keys.ToList();
    }

    /// <summary>
    /// Get cells in area range of ability. Seperate from GetCellsInAbiliyRange to allow for different frontier restrictions between the two.
    /// </summary>
    /// <param name="origin">The origin cell.</param>
    /// <param name="unit">The unit using the ability.</param>
    /// <returns>Returns list of cells.</returns>
    public virtual List<Cell> GetCellsInAreaRange(Cell origin, Unit unit)
    {
        var frontier = new Queue<Cell>();
        var visited = new Dictionary<Cell, Cell>();
        var costSoFar = new Dictionary<Cell, float>();

        frontier.Enqueue(origin);
        visited[origin] = null;
        costSoFar[origin] = 0f;

        // Dijkstra's find all cells in range
        while (frontier.Count > 0)
        {
            var current = frontier.Dequeue();
            foreach (var next in current.Neighbours)
            {
                if (!CanAbilityReach(current, next))
                    continue;

                var newCost = costSoFar[current] + current.GetDistance(next);

                if (newCost > AreaRange)
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

    /// <summary>
    /// Checks if ability area of effect can propagate from origin cell to destination cell. Used for Djkstra's in GetCellsInAbilityRange.
    /// </summary>
    /// <param name="origin">The origin cell.</param>
    /// <param name="destination">The destination cell.</param>
    /// <returns>Returns bool.</returns>
    /// Replace in subclasses for appropriate area of effect type.
    public virtual bool CanAbilityReach(Cell origin, Cell destination)
    {
        if (!origin.CanReachAltitude(destination))
            return false;

        return true;
    }

    public virtual List<Entity> GetTargets(Unit user, Entity target, List<Entity> entitiesInRange)
    {
        var ret = new List<Entity>();
        ret.Add(target);

        return ret;
    }

    /// <summary>
    /// Apply this ability by the active user on a specified target.
    /// </summary>
    /// <param name="user">The user of this ability.</param>
    /// <param name="target">The target of the ability.</param>
    /// Override in subclasses for different ability effects.
    public virtual void Apply(Unit user, Entity target)
    {
        user.DealDamage(target, user.Damage * effectMult);
    }
}
