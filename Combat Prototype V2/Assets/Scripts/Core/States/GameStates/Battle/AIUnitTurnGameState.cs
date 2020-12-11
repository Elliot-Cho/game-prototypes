using UnityEngine;

namespace Wayfinder {
  public class AIUnitTurnGameState : BattleGameState {
    protected Unit currentUnit;

    public AIUnitTurnGameState(Unit unit) : base(unit) {
      this.currentUnit = unit;
    }

    public override void OnStateEnter() {
      // Implement AI later
      Debug.Log($"AI unit {this.currentUnit.entityName} turn (skipped)");
      GameManager.Instance.state = new UnitTurnEndGameState(this.currentUnit);
    }
  }
}
