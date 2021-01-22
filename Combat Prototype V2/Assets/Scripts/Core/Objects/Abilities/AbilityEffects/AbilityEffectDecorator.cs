using System.Collections.Generic;
using System.Linq;

namespace Wayfinder {
  public abstract class AbilityEffectDecorator : IAbilityEffect {
    protected IAbilityEffect abilityEffect;
    protected float effectMultiplier;

    public virtual Ability ability { get; set; }
    public virtual List<Cell> targetCells { get; set; }

    public AbilityEffectDecorator(float effectMultiplier, IAbilityEffect abilityEffect) {
      this.effectMultiplier = effectMultiplier;
      this.abilityEffect = abilityEffect;
      this.ability = abilityEffect.ability;
      this.targetCells = abilityEffect.targetCells;
    }

    public virtual void ApplyEffect() {
      this.abilityEffect.ApplyEffect();
    }

    public List<Entity> GetAbilityEntityTargets() {
      List<Entity> targetEntities = new List<Entity>();

      foreach (Cell cell in targetCells) {
        foreach (Entity occupyingEntity in cell.occupyingEntities) {
          if (ability.CanTargetEntity(occupyingEntity)) {
            targetEntities.Add(occupyingEntity);
          }
        }
      }

      return targetEntities.Distinct().ToList();
    }
  }
}
