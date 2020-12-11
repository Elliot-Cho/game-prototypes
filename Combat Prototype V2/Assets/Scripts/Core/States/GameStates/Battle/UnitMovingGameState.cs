using UnityEngine;

namespace Wayfinder {
  public class UnitMovingGameState : BattleGameState {
    protected Unit currentUnit;

    public UnitMovingGameState(Unit unit) : base(unit) {
      this.currentUnit = unit;
    }

    public override void OnStateEnter() {
      Debug.Log("unit moving");
    }

    public void FinishMovement() {
      this.currentUnit.GetAvailableDestinations();
      this.reachableCells = this.currentUnit.CellsInMoveRange();

      if (this.UnitCanTakeActions()) {
        GameManager.Instance.state = new UnitMovementActionGameState(this.currentUnit);
      } else {
        GameManager.Instance.state = new UnitTurnEndGameState(this.currentUnit);
      }
    }

    private bool UnitCanTakeActions() {
      if (this.currentUnit.CellsInMoveRange().Count > 0) return true;

      return false;
    }
  }
}
