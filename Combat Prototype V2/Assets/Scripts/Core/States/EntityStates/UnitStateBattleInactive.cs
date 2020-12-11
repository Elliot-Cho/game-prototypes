using UnityEngine;
using System.Linq;

namespace Wayfinder {
  /// <summary>
  /// Unit state for when it is participating in the battle but not their turn
  /// </summary>
  public class UnitStateBattleInactive : EntityState
  {
    protected Unit _unit;

    public UnitStateBattleInactive(Unit unit) : base(unit) {
      _unit = unit;
    }

    public override void OnStateEnter() { }

    public override void Execute() { }

    public override void OnStateExit() { }

  }
}