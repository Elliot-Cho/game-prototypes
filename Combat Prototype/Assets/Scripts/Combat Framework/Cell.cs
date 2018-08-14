using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public abstract class Cell : MonoBehaviour, IGraphNode
{
    [HideInInspector]
    [SerializeField]
    private Vector2 _offsetCoord;
    public Vector2 OffsetCoord { get { return _offsetCoord; } set { _offsetCoord = value; } }

    // Indicates if there is a ground obstacle on this cell
    public bool HasObstacle;

    /// <summary>
    /// Booleans that determine directions the cell is blocked from
    /// Units will not be able to travel INTO this cell from a blocked direction (but can still travel out of it)
    /// </summary>
    [System.Serializable]
    public class EdgeBlock
    {
        public bool BlockLeft;
        public bool BlockUp;
        public bool BlockRight;
        public bool BlockDown;
    }
    public EdgeBlock EdgeBlockers;

    /// <summary>
    /// What space(s) the cell occupies. If the cell has an obstacle, units occupying the same space as the cell cannot enter it
    /// </summary>
    [System.Serializable]
    public class OccupyingSpace
    {
        public bool Ground;
        public bool Air;
        public bool Sky;
        public bool Sub;
    }
    public OccupyingSpace Occupies;

    /// <summary>
    /// Cost of moving through the cell.
    /// </summary>
    public float MovementCost;

    /// <summary>
    /// List of neighbouring cells
    /// </summary>
    public List<Cell> Neighbours;

    /// <summary>
    /// CellClicked event is invoked when user clicks the unit. It requires a collider on the cell game object to work.
    /// </summary>
    public event EventHandler CellClicked;
    /// <summary>
    /// CellHighlighed event is invoked when user moves cursor over the cell. It requires a collider on the cell game object to work.
    /// </summary>
    public event EventHandler CellHighlighted;
    public event EventHandler CellDehighlighted;

    protected virtual void OnMouseEnter()
    {
        if (EventSystem.current.IsPointerOverGameObject())
            return;

        if (CellHighlighted != null)
            CellHighlighted.Invoke(this, new EventArgs());
    }
    protected virtual void OnMouseExit()
    {
        if (CellDehighlighted != null)
            CellDehighlighted.Invoke(this, new EventArgs());
    }
    void OnMouseDown()
    {
        if (EventSystem.current.IsPointerOverGameObject())
            return;

        if (CellClicked != null)
            CellClicked.Invoke(this, new EventArgs());
    }

    /// <summary>
    /// Method returns distance to other cell, that is given as parameter. 
    /// </summary>
    public abstract float GetDistance(Cell other);

    /// <summary>
    /// Method returns cell adjacent to current cell in the direction given. Returns empty list if no cell exists.
    /// </summary>
    public abstract List<Cell> GetAdjCell(Vector2 direction);

    /// <summary>
    /// Method returns a Vector2 direction of given cell relative to the current cell.
    /// </summary>
    public abstract Vector2 GetDirection(Cell cell);

    /// <summary>
    /// Method returns list of cells in given area around current cell.
    /// </summary>
    public abstract List<Cell> GetSurroundingCells(int distance);

    public abstract Vector3 GetCellDimensions(); //Cell dimensions are necessary for grid generators.

    /// <summary>
    /// Method marks the cell using a specified colour.
    /// </summary>
    public abstract void Mark(Color color);
    /// <summary>
    /// Method returns the cell to its base appearance.
    /// </summary>
    public abstract void UnMark();

    public float GetDistance(IGraphNode other)
    {
        return GetDistance(other as Cell);
    }

    /// <summary>
    /// Returns true if OccupyingSpace of this cell is higher or equal to the OccupyingSpace of a given cell.
    /// </summary>
    /// <param name="cell">A cell.</param>
    /// <returns>Returns bool.</returns>
    public virtual bool CanReachAltitude(Cell cell)
    {
        if (this.Occupies.Sky)
            return true;
        if (cell.Occupies.Sky)
            return false;
        if (cell.Occupies.Air && !this.Occupies.Air)
            return false;
        if (cell.Occupies.Ground && !this.Occupies.Ground)
            return false;

        return true;
    }
}