using UnityEngine;
using System;
using System.Collections.Generic;

namespace Wayfinder {
  public class BaseCell : Cell {

    // The cell sprite
    private SpriteRenderer sprite;

    protected override void Start() {
      base.Start();
      this.sprite = transform.GetComponent<SpriteRenderer>();

      // Get neighbours for this cell
      neighbourCells = this.GetSurroundingCells(1);
    }

    public override float DistanceToCell(Cell cell) {
      var xDistance = Mathf.Abs(this.transform.position.x - cell.transform.position.x);
      var yDistance = Mathf.Abs(this.transform.position.y - cell.transform.position.y);

      // Distance is given using custom distance algorithm
      return (Mathf.Max(xDistance, yDistance) + (Mathf.Min(xDistance, yDistance) * 0.5f));
    }

    // Returns Manhatten distance to other cell, no direct diagonal movement permitted
    public int ManhattenDistanceToCell(Cell cell) {
      var xDistance = Mathf.Abs(this.transform.position.x - cell.transform.position.x);
      var yDistance = Mathf.Abs(this.transform.position.y - cell.transform.position.y);

      // Distance is given using Manhatten Norm
      return (int)(Mathf.Abs(xDistance) + Mathf.Abs(yDistance));
    }

    // Returns Chebyshev distance to other cell: adjacent and diagonals are 1 distance
    public int ChebyshevDistanceToCell(Cell cell) {
      var xDistance = Mathf.Abs(this.transform.position.x - cell.transform.position.x);
      var yDistance = Mathf.Abs(this.transform.position.y - cell.transform.position.y);

      // Distance is given using Chebyshev distance
      return (int)(Mathf.Max(xDistance, yDistance));
    }

    public override Cell AdjacentCell(Vector2 direction) {
      return Cell.CellAtPosition(
        new Vector2(transform.position.x, transform.position.y) + direction
      );
    }

    public override bool IsAdjacentTo(Cell otherCell) {
      return Vector2.Distance(this.transform.position, otherCell.transform.position) < 1.9f;
    }

    // Returns direction of cell relative to current cell
    public override Vector2 DirectionToCell(Cell cell) {
      float xDirection;
      float yDirection;

      if (cell.transform.position.x - transform.position.x < 0) {
        xDirection = -1;
      } else if (cell.transform.position.x - transform.position.x > 0) {
        xDirection = 1;
      } else {
        xDirection = 0;
      }

      if (cell.transform.position.y - transform.position.y < 0) {
        yDirection = -1;
      } else if (cell.transform.position.y - transform.position.y > 0) {
        yDirection = 1;
      } else {
        yDirection = 0;
      }

      return new Vector2(xDirection, yDirection);
    }

    // NOTE: Diagonals are considered 1 distance for this method
    public override List<Cell> GetSurroundingCells(int distance) {
      var surroundingCells = new List<Cell>();

      for (int i = 1; i <= distance; i++) {
        foreach (var direction in _directions) {
          var cell = this.AdjacentCell(direction * i);

          if (cell)
            surroundingCells.Add(cell);
        }
      }

      return surroundingCells;
    }

    // Mark the cell using a given color
    public override void Mark(String colour) {
      this.sprite.color = base.HighlightColours[colour];
    }

    // Method returns the cell to its base appearance.
    public override void UnMark() {
      this.sprite.color = new Color(1, 1, 1, 1);
    }
  }
}
