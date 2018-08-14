using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;

public abstract class Obstacle : Entity {


    public event EventHandler<ObsAttackEventArgs> ObsAttacked;
    public event EventHandler<ObsAttackEventArgs> ObsDestroyed;

    public override void Initialize()
    {
        base.Initialize();
    }

    // Attacking unit calls TakeDamage method on obstacle
    public override void TakeDamage(Entity other, float damage)
    {
        Unit attacker = other as Unit;

        HitPoints -= Mathf.Clamp(damage - Defense, 1, damage);
                                                                
        if (ObsAttacked != null)
            ObsAttacked.Invoke(this, new ObsAttackEventArgs(attacker, this, damage));

        if (HitPoints <= 0)
        {
            if (ObsDestroyed != null)
                ObsDestroyed.Invoke(this, new ObsAttackEventArgs(attacker, this, damage));
            OnDestroyed();
        }
    }

    public override void MarkAsDestroyed() { }

    public override void MarkAsTargetable() { }
}
public class ObsAttackEventArgs : EventArgs
{
    public Unit Attacker;
    public Obstacle Obstacle;

    public float Damage;

    public ObsAttackEventArgs(Unit attacker, Obstacle obstacle, float damage)
    {
        Attacker = attacker;
        Obstacle = obstacle;

        Damage = damage;
    }
}
