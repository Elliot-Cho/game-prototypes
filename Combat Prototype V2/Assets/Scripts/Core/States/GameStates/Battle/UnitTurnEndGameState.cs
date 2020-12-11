using UnityEngine;

namespace Wayfinder {
  public class UnitTurnEndGameState : BattleGameState {
    protected Unit currentUnit;

    public UnitTurnEndGameState(Unit unit) : base(unit) {
      this.currentUnit = unit;
    }

    public override void OnStateEnter() {
      this.currentUnit.actionPoints = this.currentUnit.stats.maxActionPoints.value;
      Debug.Log($"Unit {this.currentUnit.entityName}'s turn ended");
      Debug.Log("--------------------------------------------------");

      Unit nextUnit = base.GetNextUnitInInitiative();
      if (nextUnit.faction.controllable) {
        GameManager.Instance.state = new UnitTurnStartGameState(nextUnit);
      } else {
        GameManager.Instance.state = new AIUnitTurnGameState(nextUnit);
      }
    }
  }
}