using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class OnEntityAbility : Ability {

    // Use this for initialization
    public override void Initialize()
    {
        
    }

    /// <summary>
    /// Returns a list of entities that are in range of this ability.
    /// </summary>
    /// <param name="entities">List of all Entities.</param>
    /// <returns>Returns list of Entities.</returns>
    public virtual List<Entity> GetEntitiesInRange(Unit attacker, List<Entity> entities)
    {
        return entities;
    }
}
