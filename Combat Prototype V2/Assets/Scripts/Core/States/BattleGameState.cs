using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Wayfinder {
  public class BattleGameState : GameState {
    public static List<Unit> initiativeOrderedUnits;

    protected List<Cell> reachableCells = null;

    public BattleGameState(dynamic battleObject = null) : base(null) { }

    public override void OnStateEnter() {
      this.SubscribeToBattleUI();
    }

    public override void Execute() {
      this.GetInputs();
    }

    public override void OnStateExit() {
      this.UnsubscribeToBattleUI();
    }

    public override void OnCellMouseOver(object sender, EventArgs e) { }
    public override void OnCellMouseExit(object sender, EventArgs e) { }
    public override void OnCellClicked(object sender, EventArgs e) { }
    public override void OnEntityMouseOver(object sender, EventArgs e) { }
    public override void OnEntityMouseExit(object sender, EventArgs e) { }
    public override void OnEntityClicked(object sender, EventArgs e) { }

    public virtual void OnCancelKey() { }

    public virtual void SelectAbility(object sender, EventArgs e) { }

    public void GetInputs() {
      if (Input.GetButtonUp("Cancel")) this.OnCancelKey();
    }

    public void RotateInitiative() {
      Unit rotatingUnit = initiativeOrderedUnits.First();
      initiativeOrderedUnits.Add(rotatingUnit);
      initiativeOrderedUnits.RemoveAt(0);
    }

    public Unit GetCurrentUnit() {
      return initiativeOrderedUnits.First();
    }

    public Unit GetNextUnitInInitiative() {
      this.RotateInitiative();
      return this.GetCurrentUnit();
    }

    public void SubscribeToBattleUI() {
      BattleUI battleUI = UIManager.Instance.battleUI;

      foreach (AbilityButton abilityButton in battleUI.abilityButtons) {
        abilityButton.AbilityButtonClicked += this.SelectAbility;
      }
    }

    public void UnsubscribeToBattleUI() {
      BattleUI battleUI = UIManager.Instance.battleUI;

      foreach (AbilityButton abilityButton in battleUI.abilityButtons) {
        abilityButton.AbilityButtonClicked -= this.SelectAbility;
      }
    }
  }
}
