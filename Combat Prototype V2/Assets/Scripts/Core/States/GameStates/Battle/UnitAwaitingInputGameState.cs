using System;
using UnityEngine;

namespace Wayfinder {
  public class UnitAwaitingInputGameState : BattleGameState {
    protected Unit currentUnit;

    public UnitAwaitingInputGameState(Unit unit) : base(unit) {
      this.currentUnit = unit;
    }

    public override void OnStateEnter() {
      base.OnStateEnter();
      Debug.Log($"Unit {this.currentUnit.entityName} awaiting input...");
    }

    public override void OnStateExit() {
      base.OnStateExit();
    }

    public override void SelectAbility(object sender, EventArgs e) {
      AbilityButton abilityButton = sender as AbilityButton;

      abilityButton.ability.Activate();
    }

    public override void OnEntityClicked(object sender, System.EventArgs e) {
      Unit clickedUnit = sender as Unit;

      // start movement
      if (clickedUnit == this.currentUnit) {
        GameManager.Instance.state = new UnitMovementActionGameState(currentUnit);
      }
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
  }
}
