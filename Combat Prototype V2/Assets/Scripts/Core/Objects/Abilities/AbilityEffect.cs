using System.Collections.Generic;

namespace Wayfinder {
  public class AbilityEffect : IAbilityEffect {
    public Ability ability { get; set; }
    public List<Cell> targetCells { get; set; }

    public AbilityEffect(Ability ability, List<Cell> targetCells = null) {
      this.ability = ability;
      this.targetCells = targetCells;
    }

    public void ApplyEffect() { }
  }
}
