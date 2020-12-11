using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Wayfinder {
  public class GameState {
    protected GameState(dynamic genericStateVariable = null) { }

    public virtual void OnStateEnter() { }
    public virtual void Execute() { }
    public virtual void OnStateExit() { }

    // Cell stuff
    public virtual void OnCellMouseOver(object sender, EventArgs e) { }
    public virtual void OnCellMouseExit(object sender, EventArgs e) { }
    public virtual void OnCellClicked(object sender, EventArgs e) { }

    // Entity stuff
    public virtual void OnEntityMouseOver(object sender, EventArgs e) { }
    public virtual void OnEntityMouseExit(object sender, EventArgs e) { }
    public virtual void OnEntityClicked(object sender, EventArgs e) { }

    // Camera stuff
    public virtual void OnCameraEdgeScroll(object sender, EventArgs e) { }
    public virtual void OnCameraZoom(object sender, EventArgs e) { }
  }
}
