using UnityEngine;
using System.Collections.Generic;
using System;

namespace Wayfinder {
  public class UnitTurnStartGameState : BattleGameState {
    protected Unit currentUnit;

    public UnitTurnStartGameState(Unit unit) : base(unit) {
      this.currentUnit = unit;
    }

    public override void OnStateEnter() {
      Debug.Log($"Unit {this.currentUnit.entityName} selected with init {this.currentUnit.stats.initiative.value}");

      this.SetAbilityBarUI();

      GameManager.Instance.state = new UnitMovementActionGameState(currentUnit);
    }

    private void SetAbilityBarUI() {
      List<AbilityButton> abilityButtons = UIManager.Instance.battleUI.abilityButtons;
      int abilityBarSlots = 11;

      for (int i = 0; i < abilityBarSlots; i++) {
        if (i >= this.currentUnit.abilities.Count) {
          abilityButtons[i].Empty();
          continue;
        }

        abilityButtons[i].SetAbility(this.currentUnit.abilities[i]);
      }
    }
  }
}
