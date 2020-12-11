using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Wayfinder {
  public class OverworldGameState : GameState {
    public GameManager _manager;

    public OverworldGameState(GameManager manager) : base(manager) {
      _manager = manager;
    }

    Camera mainCamera = Camera.main;

    private float minCameraPositionX;
    private float maxCameraPositionX;
    private float minCameraPositionY;
    private float maxCameraPositionY;

    private Vector3 cameraOffset;

    private PlayerCharacter player = PlayerCharacter.Instance;

    public override void OnStateEnter() {
      this.cameraOffset = mainCamera.transform.position - player.transform.position;
    }

    public override void Execute() {
      mainCamera.transform.position = player.transform.position + this.cameraOffset;
    }

    public override void OnStateExit() { }

    // Cell stuff
    public override void OnCellMouseOver(object sender, EventArgs e) { }

    public override void OnCellMouseExit(object sender, EventArgs e) { }

    public override void OnCellClicked(object sender, EventArgs e) { }

    // Entity stuff
    public override void OnEntityMouseOver(object sender, EventArgs e) { }

    public override void OnEntityMouseExit(object sender, EventArgs e) { }

    public override void OnEntityClicked(object sender, EventArgs e) { }
  }
}