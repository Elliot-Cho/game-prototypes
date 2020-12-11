using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Wayfinder {
  public class UnitMovementActionGameState : BattleGameState {
    protected Unit currentUnit;
    private List<Vector2> currentPath;

    public UnitMovementActionGameState(Unit unit) : base(unit) {
      this.currentUnit = unit;
    }

    public override void OnStateEnter() {
      base.OnStateEnter();
      Debug.Log($"Unit {this.currentUnit.entityName} preparing to move...");

      CameraManager.Instance.FocusCameraOn(this.currentUnit.gameObject);

      if (this.reachableCells == null) {
        this.currentUnit.GetAvailableDestinations();
        this.reachableCells = this.currentUnit.CellsInMoveRange();
      }
      this.DisplayMovementOptions(this.reachableCells);
    }

    public override void Execute() {
      base.Execute();
      // Test skip to next unit functionality
      if (Input.GetKeyUp("n")) {
        GameManager.Instance.state = new UnitTurnEndGameState(this.currentUnit);
      }
    }

    public override void OnStateExit() {
      base.OnStateExit();
      Cell.UnmarkAllCells();
    }

    public override void OnCellMouseOver(object sender, EventArgs e) {
      Cell mouseOverCell = sender as Cell;

      if (this.reachableCells.Contains(mouseOverCell)) {
        this.currentPath = this.currentUnit.FindPathToCell(mouseOverCell);

        this.currentUnit.
          GridSizeCellsAtPoint(this.currentPath).
          Except(this.currentUnit.occupiedCells).ToList().
          ForEach(cell => cell.Mark("path"));
      } else {
        mouseOverCell.Mark("highlight");
      }
    }

    public override void OnCellMouseExit(object sender, EventArgs e) {
      Cell mouseOverCell = sender as Cell;

      mouseOverCell.UnMark();
      this.DisplayMovementOptions(this.reachableCells);
    }

    public override void OnCellClicked(object sender, EventArgs e) {
      Cell clickedCell = sender as Cell;

      if (!this.reachableCells.Contains(clickedCell)) return;

      GameManager.Instance.state = new UnitMovingGameState(this.currentUnit);
      this.currentUnit.MoveAlongPointPath(this.currentPath);
    }

    public override void SelectAbility(object sender, EventArgs e) {
      AbilityButton abilityButton = sender as AbilityButton;

      abilityButton.ability.Activate();
    }

    public override void OnEntityMouseOver(object sender, EventArgs e) { }

    public override void OnEntityMouseExit(object sender, EventArgs e) { }

    public override void OnEntityClicked(object sender, EventArgs e) {
      // Entity entity = sender as Entity;
      // if (!(entity is Unit)) return;
      // Unit unit = entity as Unit;
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

    public override void OnCancelKey() {
      GameManager.Instance.state = new UnitAwaitingInputGameState(currentUnit);
    }

    private void DisplayMovementOptions(List<Cell> cells) {
      cells.ForEach(cell => cell.Mark("reachable"));
    }
  }
}