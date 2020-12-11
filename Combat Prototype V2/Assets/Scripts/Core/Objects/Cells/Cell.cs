using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.EventSystems;

namespace Wayfinder {
  public abstract class Cell : MonoBehaviour, IGraphNode {
    public static readonly Vector2[] _directions = {
      new Vector2(1, 0), new Vector2(-1, 0), new Vector2(0, 1), new Vector2(0, -1),
      new Vector2(1, 1), new Vector2 (-1, 1), new Vector2(1,-1), new Vector2(-1, -1)
    };

    protected readonly Dictionary<string, Color> HighlightColours = new Dictionary<string, Color> {
      ["reachable"] = new Color(0.8f, 0.8f, 0.8f, 1),
      ["path"] = new Color(0.7f, 0.7f, 0.7f, 1),
      ["highlight"] = new Color(0.5f, 0.5f, 0.5f, 1),
      ["harmful"] = new Color(0.8f, 0.8f, 0.8f, 1),
      ["harmfulTarget"] = new Color(0.5f, 0.5f, 0.5f, 1)
    };

    /// <summary>
    /// Keeps track of all cells in existence
    /// </summary>
    public static List<Cell> CellList = new List<Cell>();

    public static float cellBoundsMinX = Mathf.Infinity;
    public static float cellBoundsMaxX = Mathf.NegativeInfinity;
    public static float cellBoundsMinY = Mathf.Infinity;
    public static float cellBoundsMaxY = Mathf.NegativeInfinity;

    /// <summary>
    /// A set of obstructions on this cell.
    /// </summary>
    //public Obstructions obstructions;
    public List<string> obstructions;

    /// <summary>
    /// Cost of moving through the cell.
    /// </summary>
    public float movementCost;

    public List<Cell> neighbourCells;
    public List<Entity> occupyingEntities;

    public float altitude;

    public CellProperties properties;

    /// <summary>
    /// CellClicked event is invoked when user clicks the cell. It requires a collider on the cell game object to work.
    /// </summary>
    public event EventHandler CellClicked;

    /// <summary>
    /// CellHighlighed event is invoked when user moves cursor over the cell.
    /// It requires a collider on the cell game object to work.
    /// </summary>
    public event EventHandler CellHighlighted;
    public event EventHandler CellDehighlighted;

    /// <summary>
    /// Keeps track of the current state of the cell.
    /// </summary>
    public CellState _state;
    public CellState state {
      get {
        return _state;
      }
      set {
        if (_state != null)
          _state.OnStateExit();
        _state = value;
        _state.OnStateEnter();
      }
    }

    protected virtual void Awake() {
      CellList.Add(this);
      this.setCellMapBounds();
    }

    protected virtual void Start() {
      this.state = new CellStateNormal(this);
    }

    protected virtual void Update() {
      // Empty implementation
    }

    protected virtual void OnMouseEnter() {
      if (EventSystem.current.IsPointerOverGameObject())
        return;

      if (this.CellHighlighted != null)
        this.CellHighlighted.Invoke(this, new EventArgs());
    }

    protected virtual void OnMouseExit() {
      if (this.CellDehighlighted != null)
        this.CellDehighlighted.Invoke(this, new EventArgs());
    }
    protected virtual void OnMouseDown() {
      if (EventSystem.current.IsPointerOverGameObject())
        return;

      if (this.CellClicked != null)
        this.CellClicked.Invoke(this, new EventArgs());
    }

    /// <summary>
    /// Get the cell at a given position
    /// </summary>
    /// <param name="position">The coordinates to look for the cell.</param>
    /// <returns>A cell at the given position.</returns>
    public static Cell CellAtPosition(Vector2 position) {
      // Detect only colliders on the cell layer
      int layerID = 8;
      int layerMask = 1 << layerID;

      Collider2D cellCollider = Physics2D.OverlapPoint(position, layerMask);

      if (!cellCollider) return null;

      return cellCollider.gameObject.GetComponent<Cell>();
    }

    public static void UnmarkAllCells() {
      CellList.ForEach(cell => cell.UnMark());
    }

    /// <summary>
    /// Return absolute distance between current cell and
    ///  another cell. Adjancent cells count as 1 distance, diagonal cells count as 1.5.
    /// </summary>
    /// <param name="cell">The other cell.</param>
    /// <returns>Distance to the other cell.</returns>
    public abstract float DistanceToCell(Cell cell);

    /// <summary>
    /// Get a cell adjacent to current cell in a given direction.
    /// </summary>
    /// <param name="direction">The direction to look in.</param>
    /// <returns>An adjacent cell.</returns>
    public abstract Cell AdjacentCell(Vector2 direction);

    /// <summary>
    /// Get the direction of a given cell relative to the current cell.
    /// </summary>
    /// <param name="cell">The other cell.</param>
    /// <returns>A Vector2 direction to the other cell.</returns>
    public abstract Vector2 DirectionToCell(Cell cell);

    /// <summary>
    /// Get a list of cells within a certain distance around the current cell.
    /// </summary>
    /// <param name="distance">The search distance.</param>
    /// <returns>A list of cells in distance.</returns>
    public abstract List<Cell> GetSurroundingCells(int distance);

    public abstract bool IsAdjacentTo(Cell otherCell);

    /// <summary>
    /// Method marks the cell using a specified colour.
    /// </summary>
    public abstract void Mark(String colour);

    /// <summary>
    /// Method returns the cell to its base appearance.
    /// </summary>
    public abstract void UnMark();

    public float GetDistance(IGraphNode other) {
      return this.DistanceToCell(other as Cell);
    }

    /// <summary>
    /// Get the boundaries of the current map of cells
    /// </summary>
    private void setCellMapBounds() {
      var cellBounds = this.GetComponent<Renderer>().bounds;

      if (cellBounds.min.x < cellBoundsMinX)
        cellBoundsMinX = cellBounds.min.x;
      if (cellBounds.max.x > cellBoundsMaxX)
        cellBoundsMaxX = cellBounds.max.x;
      if (cellBounds.min.y < cellBoundsMinY)
        cellBoundsMinY = cellBounds.min.y;
      if (cellBounds.max.y > cellBoundsMaxY)
        cellBoundsMaxY = cellBounds.max.y;
    }
  }
}
