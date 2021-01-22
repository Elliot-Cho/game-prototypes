using UnityEngine;
using System;
using System.Collections.Generic;

namespace Wayfinder {
  public class UnitAbilityActivatedState : BattleGameState {
    protected Ability currentAbility;
    protected Unit currentUnit;

    public UnitAbilityActivatedState(Ability ability) : base(ability) {
      this.currentAbility = ability;
      this.currentUnit = ability.user;
    }

    public override void OnStateEnter() {
      base.OnStateEnter();
      Debug.Log($"{this.currentUnit.entityName} activated ability {this.currentAbility.name}");

      this.currentAbility.GetAbilityRange();
      this.DisplayTargetOptions();
    }

    public override void OnStateExit() {
      base.OnStateExit();
      Cell.UnmarkAllCells();
      Entity.UnmarkAllEntities();
    }

    public override void OnCancelKey() {
      GameManager.Instance.state = new UnitAwaitingInputGameState(this.currentUnit);
    }

    public override void OnCellMouseOver(object sender, EventArgs e) {
      Cell mouseOverCell = sender as Cell;

      this.DisplayTargets(mouseOverCell);
    }

    public override void OnCellMouseExit(object sender, EventArgs e) {
      Cell mouseOverCell = sender as Cell;

      Cell.UnmarkAllCells();
      Entity.UnmarkAllEntities();

      this.DisplayTargetOptions();
    }

    public override void OnCellClicked(object sender, EventArgs e) {
      Cell clickedCell = sender as Cell;

      this.ActivateAbility(clickedCell);
    }

    public override void OnCameraEdgeScroll(object sender, EventArgs e) {
      CameraManager camera = sender as CameraManager;
      CameraEventArgs eventArgs = e as CameraEventArgs;

      camera.EdgeScroll(eventArgs.direction);
    }

    public override void OnCameraZoom(object sender, EventArgs e) {
      CameraManager camera = sender as CameraManager;
      CameraEventArgs eventArgs = e as CameraEventArgs;

      camera.ZoomScroll(eventArgs.zoom);
    }

    public override void SelectAbility(object sender, EventArgs e) {
      AbilityButton abilityButton = sender as AbilityButton;

      abilityButton.ability.Activate();
    }

    private void DisplayTargetOptions() {
      foreach (Cell targetCell in this.currentAbility.abilityRangeCells.Keys) {
        targetCell.Mark("harmful");
      }
    }

    private void DisplayTargets(Cell mouseOverCell) {
      foreach (Cell targetCell in this.currentAbility.GetTargetCells(mouseOverCell)) {
        targetCell.Mark("harmfulTarget");

        this.HighlightTargetEntities(targetCell);
      }
    }

    private void HighlightTargetEntities(Cell targetCell) {
      if (targetCell.occupyingEntities.Count == 0) return;

      string markColour = "";
      switch (this.currentAbility.dangerType) {
        case Ability.DangerType.harmful:
          markColour = "harmful";
          break;
        case Ability.DangerType.neutral:
          markColour = "neutral";
          break;
        case Ability.DangerType.helpful:
          markColour = "helpful";
          break;
        default:
          markColour = "neutral";
          break;
      }

      foreach (Entity targetEntity in targetCell.occupyingEntities) {
        if (this.currentAbility.CanTargetEntity(targetEntity)) {
          targetEntity.Mark(markColour);
        }
      }
    }

    private void ActivateAbility(Cell clickedCell) {
      if (this.currentAbility.baseActionPointCost > this.currentUnit.actionPoints) return;
      List<Cell> targetCells = this.currentAbility.GetTargetCells(clickedCell);
      if (targetCells.Count == 0) return;

      this.currentUnit.actionPoints -= this.currentAbility.baseActionPointCost;
      this.currentAbility.ApplyAbilityEffects(targetCells);

      if (this.currentUnit.HasActions()) {
        GameManager.Instance.state = new UnitAwaitingInputGameState(this.currentUnit);
      } else {
        GameManager.Instance.state = new UnitTurnEndGameState(this.currentUnit);
      }
    }
  }
}
