using UnityEngine;
using System.Linq;

namespace Wayfinder {
  public class UnitStateEnterBattle : EntityState
  {
    protected Unit _unit;

    public UnitStateEnterBattle(Unit unit) : base(unit) {
      _unit = unit;
    }

    public override void OnStateEnter() {
      this.SanpEntityToGrid();
      _unit.state = new UnitStateBattleInactive(_unit);
    }

    public override void Execute() { }

    public override void OnStateExit() { }

    public override void OnTriggerEnter2D(Collider2D other) { }

    public override void OnTriggerExit2D(Collider2D other) { }

    /// <summary>
    /// Snap the entity to the cellgrid
    /// </summary>
    private void SanpEntityToGrid() {
      var cellList = _unit.occupiedCells.Any() ?
        _unit.occupiedCells :
        _unit.GetOverlappingCells();

      if (!cellList.Any())
        return;

      var closestPoint = _unit.GridSizePosition(cellList[0]);
      Cell snapCell = cellList[0];

      foreach (Cell cell in cellList) {
        var gridSizePosition = _unit.GridSizePosition(cell);
        if (Vector3.Distance(_unit.transform.position, gridSizePosition) <
          Vector3.Distance(_unit.transform.position, closestPoint)) {
            closestPoint = gridSizePosition;
            snapCell = cell;
          }
      }

      _unit.transform.position = new Vector3(
        closestPoint.x,
        closestPoint.y,
        Mathf.Round(_unit.transform.position.z)
      );
      _unit.ReloadOccupiedCells(snapCell);
    }
  }
}