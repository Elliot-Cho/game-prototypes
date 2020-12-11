using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Wayfinder {
  public abstract class CellState {
    protected Cell _cell;

    protected CellState(Cell cell) {
      _cell = cell;
    }

    public virtual void OnStateEnter() { }

    public virtual void Execute() { }

    public virtual void OnStateExit() { }

    public virtual void OnMouseOver() { }

    public virtual void OnMouseExit() { }

    public virtual void OnMouseDown() { }
  }

  public class CellStateNormal : CellState {
    public CellStateNormal(Cell cell) : base(cell) { }
  }
}
