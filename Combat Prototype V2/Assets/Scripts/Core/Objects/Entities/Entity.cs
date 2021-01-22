using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.EventSystems;

namespace Wayfinder {
  /// <summary>
  /// Base class for all non-cell entities that exist in the game, including items, objects, people, and the player.
  /// </summary>
  public abstract class Entity : MonoBehaviour {
    /// <summary>
    /// Keeps track of all entities in existence
    /// </summary>
    public static List<Entity> EntityList = new List<Entity>();

    public string id;

    public string entityName;

    /// <summary>
    /// List of cells that the entity currently occupies.
    /// </summary>
    [HideInInspector]
    public List<Cell> occupiedCells = new List<Cell>();

    public Vector2 gridSize;
    public float altitude;

    public CharacterStat maxHitPoints = new CharacterStat();

    public float hitPoints;

    // Properties of an entity
    public EntityProperties properties;

    /// <summary>
    /// Cell obstructions that this entity can ignore when moving
    /// </summary>
    public List<string> obstructionImmunities;

    public Faction faction;

    private EntityState _entityState;
    public EntityState state {
      get {
        return _entityState;
      }
      set {
        if (_entityState != null)
          _entityState.OnStateExit();
        _entityState = value;
        _entityState.OnStateEnter();
      }
    }

    /// <summary>
    /// EntityClicked event is invoked when user clicks the enitity. It requires a collider on the entity game object to work.
    /// </summary>
    public event EventHandler EntityClicked;
    public event EventHandler EntityHighlighted;
    public event EventHandler EntityDehighlighted;

    private SpriteRenderer sprite;
    protected readonly Dictionary<string, Color> HighlightColours = new Dictionary<string, Color> {
      ["harmful"] = new Color(1f, 0.5f, 0.5f, 1),
      ["helpful"] = new Color(0.5f, 1f, 0.5f, 1),
      ["neutral"] = new Color(1f, 1f, 0.5f, 1)
    };

    protected virtual void Awake() {
      // Place entity into EntityList on instantiation
      EntityList.Add(this);

      this.sprite = transform.GetComponent<SpriteRenderer>();

      // Set entity size based on its GridSize
      transform.localScale = new Vector3(gridSize.x, gridSize.y, 1);
    }

    // Use this for initialization
    protected virtual void Start () {
      // Set health to max health
      this.hitPoints = this.maxHitPoints.value;
      this.faction = this.GetComponentInParent<Faction>();
    }

    public void SetColor(Color color) {
      var _renderer = GetComponent<SpriteRenderer>();
      if (_renderer != null) {
        _renderer.color = color;
      }
    }

    protected virtual void OnDestroy() {
      // Free cells that the entity used to occupy
      foreach (Cell cell in occupiedCells) {
        cell.occupyingEntities.Remove(this);
      }

      // Remove entity from EntityList on destruction
      EntityList.Remove(this);
    }

    void OnTriggerEnter2D(Collider2D other) {
      if (this.state != null)
        this.state.OnTriggerEnter2D(other);
    }

    void OnTriggerExit2D(Collider2D other) {
      this.state.OnTriggerExit2D(other);
    }

    protected virtual void OnMouseEnter() {
      if (EventSystem.current.IsPointerOverGameObject())
        return;

      if (this.EntityHighlighted != null)
        this.EntityHighlighted.Invoke(this, new EventArgs());
    }

    protected virtual void OnMouseExit() {
      if (this.EntityDehighlighted!= null)
        this.EntityDehighlighted.Invoke(this, new EventArgs());
    }
    protected virtual void OnMouseDown() {
      if (EventSystem.current.IsPointerOverGameObject())
        return;

      if (this.EntityClicked!= null)
        this.EntityClicked.Invoke(this, new EventArgs());
    }

    /// <summary>
    /// Gets a list of gridSize.x * gridSize.y cells, in a given direction from the origin cell
    /// </summary>
    public virtual List<Cell> GridSizeCells(Cell targetCell) {
      List<Cell> gridSizeCells = new List<Cell>();

      gridSizeCells.Add(targetCell);

      // Add first row of cells from left to right starting from origin
      if (this.gridSize.x > 1) {
        for (int i = 1; i < this.gridSize.x; i++) {
          var adjacentCell = gridSizeCells.Last().AdjacentCell(new Vector2(1, 0));
          if (adjacentCell) {
            gridSizeCells.Add(adjacentCell);
          }
        }
      }
      // Add columns of cells from top to bottom for each cell in row
      if (this.gridSize.y > 1) {
        foreach (Cell cell in new List<Cell>(gridSizeCells)) {
          for (int i = 1; i < this.gridSize.y; i++) {
            if (i == 1) {
              var adjCell = cell.AdjacentCell(new Vector2(0, -1));
              if (adjCell)
                gridSizeCells.Add(adjCell);
            }
            else {
              var adjCell = gridSizeCells.Last().AdjacentCell(new Vector2(0, -1));
              if (adjCell)
                gridSizeCells.Add(adjCell);
            }
          }
        }
      }
      return gridSizeCells;
    }

    public virtual List<Cell> GridSizeCells(List<Cell> cells) {
      List<Cell> gridSizeCells = new List<Cell>();

      cells.ToList().ForEach(cell => {
        this.GridSizeCells(cell).ForEach(gridSizeCell => {
          if (!gridSizeCells.Contains(gridSizeCell)) gridSizeCells.Add(gridSizeCell);
        });
      });

      return gridSizeCells;
    }

    public virtual Vector3 GridSizePosition(Cell cell) {
      return new Vector3(
        (cell.transform.position.x + (cell.transform.position.x + this.gridSize.x - 1)) / 2f,
        (cell.transform.position.y + (cell.transform.position.y - this.gridSize.y + 1)) / 2f,
        transform.position.z
      );
    }

    /// <summary>
    /// Get all cells that are currently overlapping with this entity's collider.
    /// </summary>
    /// <returns>A list of overlapping cells.</returns>
    public virtual List<Cell> GetOverlappingCells() {
      Collider2D[] overlapColliders = new Collider2D[Mathf.RoundToInt(this.gridSize.x + 1 * this.gridSize.y + 1) + 1];
      ContactFilter2D contactFilter = new ContactFilter2D();

      // Detect only colliders on the cell layer
      int layerID = 8;
      contactFilter.layerMask = 1 << layerID;

      // Places overlapping colliders in the overlapColliders array
      this.GetComponent<Collider2D>().OverlapCollider(contactFilter, overlapColliders);

      return overlapColliders.Where(collider => collider != null).Select(collider => collider.GetComponent<Cell>()).ToList();
    }

    public List<Cell> GridSizeCellsAtPoint(Vector2 position) {
      int layerID = 8;
      int layerMask = 1 << layerID;

      Vector2 topLeftCorner = new Vector2(
        (position.x - this.gridSize.x/2) + 0.05f,
        (position.y + this.gridSize.y/2) - 0.05f
      );
      Vector2 bottomRightCorner = new Vector2(
        (position.x + this.gridSize.x/2) - 0.05f,
        (position.y - this.gridSize.y/2) + 0.05f
      );

      ContactFilter2D contactFilter = new ContactFilter2D();
      contactFilter.layerMask = layerMask;

      Collider2D[] overlapResults = new Collider2D[(int)(this.gridSize.x * this.gridSize.y)];
      Physics2D.OverlapArea(topLeftCorner, bottomRightCorner, contactFilter, overlapResults);

      if (overlapResults.Count() == 0) return new List<Cell>();

      return overlapResults.Where(collider => collider != null).
             Select(collider => {
               return collider.GetComponent<Cell>();
             }).ToList();
    }

    public List<Cell> GridSizeCellsAtPoint(List<Vector2> positions) {
      List<Cell> gridSizeCells = new List<Cell>();

      foreach (Vector2 position in positions) {
        this.GridSizeCellsAtPoint(position).ForEach(gridSizeCell => {
          if (!gridSizeCells.Contains(gridSizeCell)) gridSizeCells.Add(gridSizeCell);
        });
      }

      return gridSizeCells;
    }

    public void ReloadOccupiedCells(Cell targetCell) {
      this.occupiedCells.ForEach(occupiedCell => occupiedCell.occupyingEntities.Remove(this));
      this.occupiedCells = this.GridSizeCells(targetCell);
      this.occupiedCells.ForEach(occupiedCell => occupiedCell.occupyingEntities.Add(this));
    }

    public void ReloadOccupiedCells(Vector2 targetPoint) {
      this.occupiedCells.ForEach(occupiedCell => occupiedCell.occupyingEntities.Remove(this));
      this.occupiedCells = this.GridSizeCellsAtPoint(targetPoint);
      this.occupiedCells.ForEach(occupiedCell => occupiedCell.occupyingEntities.Add(this));
    }

    public bool SharesAltitudeWith(Entity otherEntity) {
      return this.altitude == otherEntity.altitude;
    }

    // Mark the entity using a given color
    public void Mark(String colour) {
      this.sprite.color = this.HighlightColours[colour];
    }

    // Method returns the entity to its base appearance.
    public void UnMark() {
      this.sprite.color = new Color(1, 1, 1, 1);
    }

    public static void UnmarkAllEntities() {
      EntityList.ForEach(entity => entity.UnMark());
    }
  }
}
