using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealAbility : OnEntityAbility {

    // Use this for initialization
    public override void Initialize()
    {
        Name = "Heal";
        Desc = "Heal a unit.";

        Target = Targets.Entities;
        Type = Types.Healing;

        Affects.Enemies = false;
        Affects.Obstacles = false;
        Affects.Self = true;
        Affects.Allies = true;

        effectMult = 5f;
        BaseRange = 4f;
        ActionCost = 1f;
    }

    public override void Apply(Unit user, Entity target)
    {
        var unit = target as Unit;
        unit.TakeHealing(user, effectMult);
        //unit.HitPoints = Mathf.Max(unit.HitPoints + effectMult, unit.MaxHitPoints);
    }
}
