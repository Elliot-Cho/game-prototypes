using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using UnityEngine.EventSystems;

public abstract class Entity : MonoBehaviour {

    // Cell that the entity is currently occupying.
    [HideInInspector]
    public List<Cell> Cells;
    // Grid size of the entity
    public Vector2 GridSize;
    // Space that the entity occupies. Entity can only move into cells that do not block the same spaces it occupies.
    [System.Serializable]
    public class OccupyingSpace
    {
        public bool Ground;
        public bool Air;
        public bool Sky;
        public bool Sub;
    }
    public OccupyingSpace Occupies;

    // Entity stats
    public float HitPoints;
    public float Defense;
    public float Weight;

    // Properties of an entity
    [System.Serializable]
    public class EntityProperties
    {
        public bool IsAttackable;
        public bool IsObstacle;
        public bool IsMovable;
    }
    public EntityProperties Properties;

    // Event invoked when user clicks entity
    public event EventHandler EntityClicked;
    // Events invoked when user moves cursor over the entity.
    public event EventHandler EntityHighlighted;
    public event EventHandler EntityDehighlighted;

    // Use this for initialization
    public virtual void Initialize ()
    {
        // Set entity size based on its GridSize
        transform.localScale = new Vector3(GridSize.x, GridSize.y, 1);
    }

    // Performed every frame (used for mouse actions, etc.)
    private void Update()
    {
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            // Mouse Events Raycast for Units
            RaycastHit2D[] onMouseEnter = Physics2D.RaycastAll(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            // OnMouseOver
            foreach (var hit in onMouseEnter)
            {
                if (hit.collider.gameObject.Equals(this.gameObject))
                {
                    // EntityHighlighted event happens (all functions subscribed to this event also fire)
                    if (EntityHighlighted != null)
                        EntityHighlighted.Invoke(this, new EventArgs());
                }
            }

            // OnMouseDown
            if (Input.GetMouseButtonDown(0))
            {
                foreach (var hit in onMouseEnter)
                {
                    if (hit.collider.gameObject.Equals(this.gameObject))
                    {
                        // EntityClicked event happens (all functions subscribed to this event also fire)
                        if (EntityClicked != null)
                            EntityClicked.Invoke(this, new EventArgs());
                    }
                }
            }
        }
    }

    /// <summary>
    /// Check if this entity can be attacked by an attacking entity.
    /// </summary>
    /// <param name="attacker">The attacking unit.</param>
    /// <returns>Returns bool.</returns>
    public virtual bool InAttackRange (Unit attacker)
    {
        if (!Properties.IsAttackable)
            return false;

        if (attacker.Equals(this))
            return true;

        foreach (var sourceCell in Cells)
        {
            foreach (var cell in attacker.Cells)
            {
                if (sourceCell.GetDistance(cell) <= attacker.AttackRange)
                    return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Subtracts health from this entity when it receives an attack.
    /// </summary>
    /// <param name="other">The attacking entity.</param>
    /// <param name="damage">Amount of damage dealt.</param>
    public virtual void TakeDamage(Entity other, float damage)
    {
        if (!Properties.IsAttackable)
            return;

        HitPoints -= Mathf.Clamp(damage - Defense, 1, damage);

        if (HitPoints <= 0)
            OnDestroyed();
    }

    /// <summary>
    /// Gets a list of GridSize.x * GridSize.y cells, starting from the top-left as the origin cell.
    /// </summary>
    /// <param name="origin">The origin cell.</param>
    /// <returns>Returns List of Cells.</returns>
    public virtual List<Cell> GetGridSizeCells(Cell origin)
    {
        var newCells = new List<Cell>();

        // Add origin
        newCells.Add(origin);
        // Add first row of cells from left to right starting from origin
        if (GridSize.x > 1)
        {
            for (int i = 1; i < GridSize.x; i++)
            {
                var adjCell = newCells.Last().GetAdjCell(new Vector2(1, 0));
                if (adjCell.Count > 0)
                {
                    newCells.Add(adjCell[0]);
                }
            }
        }
        // Add columns of cells from top to bottom for each cell in row
        if (GridSize.y > 1)
        {
            foreach (Cell cell in new List<Cell>(newCells))
            {
                for (int i = 1; i < GridSize.y; i++)
                {
                    if (i == 1)
                    {
                        var adjCell = cell.GetAdjCell(new Vector2(0, -1));
                        if (adjCell.Count > 0)
                            newCells.Add(adjCell[0]);
                    }
                    else
                    {
                        var adjCell = newCells.Last().GetAdjCell(new Vector2(0, -1));
                        if (adjCell.Count > 0)
                            newCells.Add(adjCell[0]);
                    }
                }
            }
        }

        return newCells;
    }

    /// <summary>
    /// Gets the center position this entity should occupy with its GridSize
    /// </summary>
    /// <param name="cell">The origin cell.</param>
    /// <returns>Returns Vector3 center position.</returns>
    // Method to get the center position an entity should occupy with its GridSize
    public virtual Vector3 GetGridSizePosition(Cell cell)
    {
        return new Vector3((cell.transform.position.x + (cell.transform.position.x + GridSize.x - 1)) / 2f,
            (cell.transform.position.y + (cell.transform.position.y - GridSize.y + 1)) / 2f,
            transform.position.z);
    }

    /// <summary>
    /// Checks if this entity occupies the same OccupyingSpace as a given cell.
    /// </summary>
    /// <param name="cell">The cell to check against.</param>
    /// <returns>Returns bool.</returns>
    // Method to check if this entity occupies the same OccupyingSpace as a given cell
    public virtual bool SharesSpace(Cell cell)
    {
        return (this.Occupies.Ground && cell.Occupies.Ground ||
            this.Occupies.Air && cell.Occupies.Air ||
            this.Occupies.Sky && cell.Occupies.Sky ||
            this.Occupies.Sub && cell.Occupies.Sub);
    }

    /// <summary>
    /// Checks if this entity can move into given cell.
    /// </summary>
    /// <param name="cell">The cell to check.</param>
    /// <returns>Returns bool.</returns>
    public virtual bool IsCellTraversable(Cell cell)
    {
        var GridSizeCells = new List<Cell>(GetGridSizeCells(cell));

        // if GetGridSizeCells couldn't find all cells required
        if (GridSizeCells.Count < (GridSize.x * GridSize.y))
            return false;

        foreach (Cell _cell in GridSizeCells)
        {
            if (!Cells.Contains(_cell) && _cell.HasObstacle)    // if cell has an obstacle (that is not the current entity)
            {
                if (SharesSpace(_cell))
                    return false;
            }
        }

        return true;
    }

    /// <summary>
    /// Checks if this entity can move from one cell to another, adjacent cell. Used for Dijkstra's Algo.
    /// </summary>
    /// <param name="origin">The origin cell.</param>
    /// <param name="destination">The destination cell.</param>
    /// <returns>Returns bool.</returns>
    public virtual bool CanReach(Cell origin, Cell destination)
    {
        if (!Properties.IsMovable)
            return false;

        if (!IsCellTraversable(destination))
            return false;

        // If entity and cell occupy same space, check for edgeblockers and diagonals around obstacles
        if (SharesSpace(destination))
        {
            var direction = origin.GetDirection(destination);
            foreach (var cell in new List<Cell>(GetGridSizeCells(origin)))
            {
                var check = cell.GetAdjCell(direction)[0];
                // False if edge is blocked
                if (direction.x.Equals(-1) && check.EdgeBlockers.BlockRight || direction.x.Equals(1) && check.EdgeBlockers.BlockLeft
                || direction.y.Equals(-1) && check.EdgeBlockers.BlockUp || direction.y.Equals(1) && check.EdgeBlockers.BlockDown)
                    return false;

                // False on diagonal neighbour if it crosses an adjacent obstacle or corner (no diagonal movement on corners)
                if (Mathf.Abs(direction.x) == 1 && Math.Abs(direction.y) == 1)
                {
                    var xCell = cell.GetAdjCell(new Vector2(direction.x, 0));
                    var yCell = cell.GetAdjCell(new Vector2(0, direction.y));
                    if (xCell[0] != null)
                    {
                        if (xCell[0].HasObstacle)
                            return false;

                        if (direction.x.Equals(-1) && xCell[0].EdgeBlockers.BlockRight || direction.x.Equals(1) && xCell[0].EdgeBlockers.BlockLeft)
                            return false;
                    }
                    if (yCell[0] != null)
                    {
                        if (yCell[0].HasObstacle)
                            return false;

                        if (direction.y.Equals(-1) && yCell[0].EdgeBlockers.BlockUp || direction.y.Equals(1) && yCell[0].EdgeBlockers.BlockDown)
                            return false;
                    }
                }
            }
        }

        return true;
    }

    /// <summary>
    /// Called when the entity is destroyed.
    /// </summary>
    protected virtual void OnDestroyed()
    {
        foreach (Cell cell in Cells)
        {
            cell.HasObstacle = false;
        }

        MarkAsDestroyed();
        Destroy(gameObject);
    }

    // Gives visual indication that the entity is destroyed. It gets called right before the entity game object is
    // destroyed, so either instantiate some new object to indicate destruction or redesign Defend method. 
    public abstract void MarkAsDestroyed();

    public abstract void MarkAsTargetable();

    // Method marks the unit by changing its color or appearance
    public abstract void Mark(Color color);

    // Method returns the unit to its base appearance
    public abstract void UnMark();

    public void SetColor(Color color)
    {
        var _renderer = GetComponent<SpriteRenderer>();
        if (_renderer != null)
        {
            _renderer.color = color;
        }
    }
}
