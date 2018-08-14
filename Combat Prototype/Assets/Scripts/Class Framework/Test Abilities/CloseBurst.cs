using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloseBurst : OnEntityAbility {

    // Use this for initialization
    public override void Initialize()
    {
        Name = "Close Burst";
        Desc = "Attack all enemies in range.";

        Target = Targets.Entities;

        Affects.Enemies = true;
        Affects.Obstacles = true;
        Affects.Self = false;
        Affects.Allies = false;

        effectMult = 1f;
        BaseRange = 1f;
        ActionCost = 1f;
    }

    public override List<Entity> GetTargets(Unit user, Entity clicked, List<Entity> entitiesInRange)
    {
        return entitiesInRange;
    }
}
