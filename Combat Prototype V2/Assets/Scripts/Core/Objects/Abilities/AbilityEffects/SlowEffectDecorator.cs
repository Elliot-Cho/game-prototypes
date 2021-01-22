using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace Wayfinder {
  public class SlowEffectDecorator : AbilityEffectDecorator {
    public override Ability ability { get; set; }
    public override List<Cell> targetCells { get; set; }

    public SlowEffectDecorator(float effectMultiplier, IAbilityEffect abilityEffect) :
      base(effectMultiplier, abilityEffect) { }

    public override void ApplyEffect() {
      this.abilityEffect.ApplyEffect();

      Debug.Log($"Slow effect used by {ability.user.entityName}");
      if (base.GetAbilityEntityTargets().Count > 0) {
        Debug.Log($"Slow effect targets: {base.GetAbilityEntityTargets().Select(i => i.entityName).Aggregate((i, j) => i + ", " + j)}");
      }
    }
  }
}
