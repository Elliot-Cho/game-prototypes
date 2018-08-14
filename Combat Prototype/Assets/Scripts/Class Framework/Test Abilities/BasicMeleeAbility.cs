using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicMeleeAbility : OnEntityAbility {

    // Use this for initialization
    public override void Initialize () {
        Name = "Basic Melee";
        Desc = "Attack your enemy in close quarters.";

        Target = Targets.Entities;

        Affects.Enemies = true;
        Affects.Obstacles = true;
        Affects.Self = false;
        Affects.Allies = false;

        effectMult = 1f;
        BaseRange = 0f;
        ActionCost = 1f;
    }

    public override List<Entity> GetEntitiesInRange (Unit attacker, List<Entity> entities)
    {
        var ret = new List<Entity>();

        foreach (var entity in entities)
        {
            if (entity.InAttackRange(attacker))
            {
                ret.Add(entity);
            }
        }
        return ret;
    }
}
