using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;

public class BaseCell : Cell {

    // Directionality of the cell
    protected static readonly Vector2[] _directions =
    {
        new Vector2(1, 0), new Vector2(-1, 0), new Vector2(0, 1), new Vector2(0, -1),
        new Vector2(1, 1), new Vector2 (-1, 1), new Vector2(1,-1), new Vector2(-1, -1)
    };

    // The cell sprite
    private SpriteRenderer sprite;

    private void Start()
    {
        sprite = transform.GetComponent<SpriteRenderer>();

        // get neighbours for this cell
        Neighbours = this.GetSurroundingCells(1);
    }

    // Returns Manhatten distance to other cell, no direct diagonal movement permitted
    public int GetManDistance(Cell other)
    {
        // Distance is given using Manhatten Norm
        return (int)(Mathf.Abs(OffsetCoord.x - other.OffsetCoord.x) + Mathf.Abs(OffsetCoord.y - other.OffsetCoord.y));
    }

    // Returns Pseudo-Euclidean distance to other cell: diagonals are 1.5 distance
    public override float GetDistance(Cell other)
    {
        var xDist = Mathf.Abs(OffsetCoord.x - other.OffsetCoord.x);
        var yDist = Mathf.Abs(OffsetCoord.y - other.OffsetCoord.y);

        // Distance is given using custom distance algorithm
        return (Mathf.Max(xDist, yDist) + (Mathf.Min(xDist, yDist) * 0.5f));
    }

    // Returns Chebyshev distance to other cell: adjacent and diagonals are 1 distance
    public int GetChebDistance(Cell other)
    {
        var xDist = Mathf.Abs(OffsetCoord.x - other.OffsetCoord.x);
        var yDist = Mathf.Abs(OffsetCoord.y - other.OffsetCoord.y);

        // Distance is given using Chebyshev distance
        return (int)(Mathf.Max(xDist, yDist));
    }

    // Return cells in given direction, returns empty list if no cell exists
    public override List<Cell> GetAdjCell(Vector2 direction)
    {
        // Detect only colliders on the cell layer
        int layerID = 8;
        int layerMask = 1 << layerID;

        var ret = new List<Cell>();
        var cell = Physics2D.OverlapPoint(new Vector2(transform.position.x, transform.position.y) + direction, layerMask);

        if (cell != null)
            ret.Add(cell.gameObject.GetComponent<Cell>());

        return ret;
    }

    // Returns direction of cell relative to current cell
    public override Vector2 GetDirection(Cell cell)
    {
        return new Vector2(cell.transform.position.x - transform.position.x, cell.transform.position.y - transform.position.y);
    }

    // Return list of cells in area of given value around the cell
    // NOTE: Diagonals are considered 1 distance
    public override List<Cell> GetSurroundingCells(int distance)
    {
        var ret = new List<Cell>();

        for (int i = 1; i <= distance; i++)
        {
            foreach (var direction in _directions)
            {
                var cells = GetAdjCell(direction * i);

                if (cells.Count > 0)
                    ret.Add(cells[0]);
            }
        }

        return ret;
    }

    // Cell dimensions are necessary for grid generators.
    public override Vector3 GetCellDimensions()
    {
        return GetComponent<SpriteRenderer>().bounds.size;
    }

    // Mark the cell using a given color
    public override void Mark(Color color)
    {
        sprite.color = color;
        sprite.sortingOrder = 2;
    }

    // Method returns the cell to its base appearance.
    public override void UnMark()
    {
        sprite.color = new Color(1, 1, 1, 0);
        sprite.sortingOrder = 0;
    }
}

