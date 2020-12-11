using UnityEngine;
using System.Collections.Generic;

namespace Wayfinder {
  public class DamageEffectDecorator : AbilityEffectDecorator {
    public override Ability ability { get; set; }
    public override List<Cell> targetCells { get; set; }

    public DamageEffectDecorator(float effectMultiplier, IAbilityEffect abilityEffect) :
      base(effectMultiplier, abilityEffect) { }

    public override void ApplyEffect() {
      this.abilityEffect.ApplyEffect();

      Debug.Log($"Damage effect used by {ability.user.entityName}");
    }
  }
}
