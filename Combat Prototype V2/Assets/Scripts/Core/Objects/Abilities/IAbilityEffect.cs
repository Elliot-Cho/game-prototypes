using System.Collections.Generic;

namespace Wayfinder {
  public interface IAbilityEffect {
    Ability ability { get; set; }
    List<Cell> targetCells { get; set; }

    void ApplyEffect();
  }
}
