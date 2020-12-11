using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Wayfinder {
  public class PlayerStateOverworld : EntityState
  {
    public PlayerStateOverworld(Entity entity) : base(entity) {
      _entity = entity;
    }

    private PlayerCharacter player = PlayerCharacter.Instance;

    public List<Cell> collidingCells = new List<Cell>();

    public override void OnStateEnter() {

    }

    public override void Execute() {
      this.MovePlayer();
    }

    public override void OnStateExit() { }

    public override void OnTriggerEnter2D(Collider2D other) {
      var cell = other.gameObject.GetComponent<Cell>();
      if (!CellObstructsPlayer(cell)) {
        player.occupiedCells.Add(cell);
      }
      this.collidingCells.Add(cell);
    }

    public override void OnTriggerExit2D(Collider2D other) {
      var cell = other.gameObject.GetComponent<Cell>();

      player.occupiedCells.Remove(cell);
      this.collidingCells.Remove(cell);
    }

    private void MovePlayer() {
      var xDirection = Input.GetAxisRaw("Horizontal");
      var yDirection = Input.GetAxisRaw("Vertical");

      var move = new Vector3();

      var obstructed = PlayerObstructedInDirection(new Vector2(xDirection, yDirection));

      move += new Vector3(
        obstructed[0] ? 0 : xDirection,
        obstructed[1] ? 0 : yDirection,
        0
      );

      player.transform.position += move * player.moveSpeed * Time.deltaTime;
    }

    // Quick and dirty implementation for collision - doesn't work properly at high move speeds
    private List<bool> PlayerObstructedInDirection(Vector2 direction) {
      var playerPosition = player.transform.position;
      var playerBounds = player.GetComponent<Collider2D>().bounds;
      var result = new List<bool>();

      result.Add(false);
      result.Add(false);

      foreach (Cell cell in this.collidingCells) {
        var cellPosition = cell.transform.position;
        var cellBounds = cell.GetComponent<Collider2D>().bounds;

        if (CellObstructsPlayer(cell)) {
          if ((direction.x > 0 && cellPosition.x >= playerPosition.x ||
            direction.x < 0 && cellPosition.x <= playerPosition.x) &&
            cellBounds.min.y + 0.15f < playerBounds.max.y &&
            cellBounds.max.y - 0.15f > playerBounds.min.y)
          {
            result[0] = true;
          }
          if ((direction.y > 0 && cellPosition.y >= playerPosition.y ||
            direction.y < 0 && cellPosition.y <= playerPosition.y) &&
            cellBounds.min.x + 0.15f < playerBounds.max.x &&
            cellBounds.max.x - 0.15f > playerBounds.min.x)
          {
            result[1] = true;
          }
        }
      }
      return result;
    }

    private bool CellObstructsPlayer(Cell cell) {
      return cell.obstructions.Except(player.obstructionImmunities).Count() > 0;
    }
  }
}
