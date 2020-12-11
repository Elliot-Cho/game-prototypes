using System.Collections.Generic;

namespace Wayfinder {
  public class EnterBattleGameState : BattleGameState {
    protected List<Unit> unitList;

    public EnterBattleGameState(List<Unit> unitList) : base(unitList) {
      this.unitList = unitList;
    }

    public override void OnStateEnter() {
      this.EnterUnitsInBattle();
      BattleGameState.initiativeOrderedUnits = this.InitiativeOrderedUnits();
      GameManager.Instance.state = new UnitTurnStartGameState(base.GetCurrentUnit());
    }

    private void EnterUnitsInBattle() {
      this.unitList.ForEach(unit => {
        unit.state = new UnitStateEnterBattle(unit);
      });
    }

    private List<Unit> InitiativeOrderedUnits() {
      List<Unit> units = new List<Unit>(Unit.UnitList);
      units.Sort((x, y) => SortUnitsByInitiative(x, y));

      return units;
    }

    private int SortUnitsByInitiative(Unit unit1, Unit unit2) {
      return unit2.stats.initiative.value.CompareTo(unit1.stats.initiative.value);
    }
  }
}