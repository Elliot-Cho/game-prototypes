using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;

public abstract class Unit : Entity {

    // UnitSelected event is invoked when user clicks on unit that belongs to him.
    public event EventHandler UnitSelected;
    public event EventHandler UnitDeselected;

    public event EventHandler<AttackEventArgs> UnitAttacked;
    public event EventHandler<AttackEventArgs> UnitDestroyed;
    public event EventHandler<MovementEventArgs> UnitMoved;

    // Camera focus events invoked when unit's turn starts or ends
    public event EventHandler CameraFocusOn;
    public event EventHandler CameraFocusOff;
    [HideInInspector]
    public bool cameraFocus;

    public UnitState UnitState { get; set; }
    public void SetState(UnitState state)
    {
        UnitState.MakeTransition(state);
    }

    // Base Unit Stats
    public float MaxHitPoints { get; private set; }
    public float MaxMovementPoints;   // Distance unit can travel
    public float MaxActionPoints;     // How many actions unit can perform

    public float MovementSpeed;     // Speed of movement animation
    public float Damage;              // How much damage unit deals
    public float AttackRange;         // Range at which unit can attack

    // Unit Attributes
    public int Dexterity;       // Affects init, move points, accuracy, evasion

    // Current Unit Stats
    public float MovementPoints;
    public float ActionPoints;

    // Unit Abilities
    [HideInInspector]
    public List<Ability> Abilities;

    // Indicates the player that the unit belongs to
    // Corresponds with PlayerNumber in Player script
    public int PlayerNumber;

    // Checks if unit is moving
    public enum CurrentState { Normal, Moving, Ability };
    public CurrentState State;

    // Pathfinder object for pathfinding
    //private static IPathfinding _pathfinder = new AStarPathfinding();

    // Dictionary of cell connetions within the unit's available destninations; calculated using GetAvailableDestinations function
    public Dictionary<Cell, Cell> ValidConnections = new Dictionary<Cell, Cell>();

    // Animator object for animations
    private Animator animator;
    private Vector2 prevMove;   // Keep track of previous moves for the animator

    // Initialization of unit after instantiation
    public override void Initialize()
    {
        UnitState = new UnitStateNormal(this);

        MaxHitPoints = HitPoints;
        MaxMovementPoints = MovementPoints;
        MaxActionPoints = ActionPoints;

        animator = GetComponent<Animator>();

        cameraFocus = false;

        base.Initialize();
    }

    // On mouse interaction with unit
    /*protected virtual void OnMouseDown()
    {
        if (UnitClicked != null)
            UnitClicked.Invoke(this, new EventArgs());
    }
    protected virtual void OnMouseEnter()
    {
        if (UnitHighlighted != null)
            UnitHighlighted.Invoke(this, new EventArgs());
    }
    protected virtual void OnMouseExit()
    {
        if (UnitDehighlighted != null)
            UnitDehighlighted.Invoke(this, new EventArgs());
    }*/

    // On unit turn start and end
    public virtual void OnTurnStart()
    {
        MovementPoints = MaxMovementPoints;
        ActionPoints = MaxActionPoints;

        // Focus camera on unit
        if (CameraFocusOn != null)
            CameraFocusOn.Invoke(this, new EventArgs());
    }
    public virtual void OnTurnEnd()
    {
        // Turn off camera focusing in CameraController
        if (CameraFocusOff != null)
            CameraFocusOff.Invoke(this, new EventArgs());

        SetState(new UnitStateNormal(this));
    }

    // On unit selection/deselction
    // Called from CellGridState for units
    public virtual void OnUnitSelected()
    {
        SetState(new UnitStateMarkedAsSelected(this));
        if (UnitSelected != null)
            UnitSelected.Invoke(this, new EventArgs());
    }
    public virtual void OnUnitDeselected()
    {
        SetState(new UnitStateMarkedAsFriendly(this));
        if (UnitDeselected != null)
            UnitDeselected.Invoke(this, new EventArgs());
    }

    // Method deals damage to entity given as parameter
    public virtual void DealDamage (Entity other, float damage)
    {
        if (!this.State.Equals(CurrentState.Normal))
            return;
        if (!other.Properties.IsAttackable)
            return;

        MarkAsAttacking(other);
        other.TakeDamage(this, damage);

        if (ActionPoints == 0 && MovementPoints < 1)
            SetState(new UnitStateMarkedAsFinished(this));
    }

    // Attacking unit calls Defend method on defending unit. 
    public override void TakeDamage(Entity other, float damage)
    {
        Unit attacker = other as Unit;
        MarkAsDefending(this);
        HitPoints -= Mathf.Clamp(damage - Defense, 1, damage);  //Damage is calculated by subtracting attack factor of attacker and defence factor of defender. If result is below 1, it is set to 1.
                                                                      //This behaviour can be overridden in derived classes.
        if (UnitAttacked != null)
            UnitAttacked.Invoke(this, new AttackEventArgs(attacker, this, damage));

        if (HitPoints <= 0)
        {
            if (UnitDestroyed != null)
                UnitDestroyed.Invoke(this, new AttackEventArgs(attacker, this, damage));
            OnDestroyed();
        }
    }

    public virtual void TakeHealing(Unit other, float healing)
    {
        if (!other.State.Equals(CurrentState.Normal))
            return;
        if (!this.Properties.IsAttackable)
            return;

        MarkAsHealing(this);
        HitPoints = Mathf.Min(HitPoints + healing, MaxHitPoints);

        if (ActionPoints == 0 && MovementPoints < 1)
            SetState(new UnitStateMarkedAsFinished(this));
    }

    // Method that moves unit towards specified target with a coroutine
    public virtual void Move(Cell destinationCell, List<Cell> path)
    {
        if (!this.State.Equals(CurrentState.Normal))
            return;

        // Calculate cost of path
        var fullPath = new List<Cell>(path);
        var totalMovementCost = PathCost(fullPath);
        if (MovementPoints < totalMovementCost)
            return;

        MovementPoints -= totalMovementCost;

        foreach (var cell in Cells)
        {
            cell.HasObstacle = false;
        }
        // Set new cells; destinationCell is origin
        var newCells = new List<Cell>(GetGridSizeCells(destinationCell));
        foreach (var cell in newCells)
        {
            cell.HasObstacle = true;
        }
        Cells = newCells;

        if (MovementSpeed > 0)
            StartCoroutine(MovementAnimation(path));
        else
            transform.position = GetGridSizePosition(Cells[0]);

        if (UnitMoved != null)
            UnitMoved.Invoke(this, new MovementEventArgs(Cells[0], destinationCell, path));
    }
    protected virtual IEnumerator MovementAnimation(List<Cell> path)
    {
        this.State = CurrentState.Moving;
        animator.SetBool("Moving", this.State.Equals(CurrentState.Moving));

        int xMove = 0;
        int yMove = 0;

        path.Reverse();

        foreach (var cell in path)
        {
            // Change move variables for animator
            xMove = 0;
            yMove = 0;

            Vector3 GridSizePos = GetGridSizePosition(cell);

            // x-axis move animations
            if (transform.position.x > GridSizePos.x)
            {
                xMove = -1;
                prevMove = new Vector2(xMove, 0f);
            }
            else if (transform.position.x < GridSizePos.x)
            {
                xMove = 1;
                prevMove = new Vector2(xMove, 0f);
            }
            // y-axis move animations
            if (xMove == 0)
            {
                if (transform.position.y > GridSizePos.y)
                {
                    yMove = -1;
                    prevMove = new Vector2(0f, yMove);
                }
                else if (transform.position.y < GridSizePos.y)
                {
                    yMove = 1;
                    prevMove = new Vector2(0f, yMove);
                }
            }

            // Slow down if moving through rough terrain
            var finalSpeed = MovementSpeed;
            if (cell.MovementCost > 1)
            {
                finalSpeed = finalSpeed * 0.66f;
            }

            // Movement loop
            while (new Vector2(transform.position.x, transform.position.y) != new Vector2(GridSizePos.x, GridSizePos.y))
            {
                // Move sprite towards target position
                transform.position = Vector3.MoveTowards(transform.position, new Vector3(GridSizePos.x, GridSizePos.y, transform.position.z), Time.deltaTime * finalSpeed);

                // Change animator variables
                animator.SetFloat("MoveX", xMove);
                animator.SetFloat("MoveY", yMove);

                // Continue loop in next update frame
                yield return 0;
            }
        }

        this.State = CurrentState.Normal;
        // Stop movement animation & face character right direction
        animator.SetBool("Moving", this.State.Equals(CurrentState.Moving));
        animator.SetFloat("PrevMoveX", prevMove.x);
        animator.SetFloat("PrevMoveY", prevMove.y);
    }

    // Method fills ValidMoves list with all cells that the unit is capable of moving to
    // Uses slightly modified Dijkstra's Algorithm
    public void GetAvailableDestinations(Cell origin, float movePoints)
    {
        ValidConnections.Clear(); // Empty ValidMoves and recalculate in the following

        var frontier = new HeapPriorityQueue<Cell>();
        frontier.Enqueue(origin, 0);

        var cameFrom = new Dictionary<Cell, Cell>();
        cameFrom[origin] = null;

        var costSoFar = new Dictionary<Cell, float>();
        costSoFar[origin] = 0;

        while (frontier.Count > 0)
        {
            Cell current = frontier.Dequeue();

            foreach (var next in current.Neighbours)
            {
                // Don't add if can't be reached
                if (!CanReach(current, next))
                    continue;

                // Don't add if cost exceeds movement points
                float newCost;
                float movementCost = 0f;
                // If the unit shares OccupyingSpace (ground, air, etc) with the cell
                if (SharesSpace(next))
                {
                    foreach (Cell cell in new List<Cell>(GetGridSizeCells(next)))
                    {
                        if (cell.MovementCost > movementCost)
                            movementCost = cell.MovementCost;
                    }
                    newCost = costSoFar[current] + (movementCost * current.GetDistance(next));
                }
                else // Otherwise the unit isn't effected by movement cost of the cell
                    newCost = costSoFar[current] + current.GetDistance(next);

                if (newCost > MovementPoints)
                    continue;

                if (!costSoFar.Keys.Contains(next) || newCost < costSoFar[next])
                {
                    costSoFar[next] = newCost;
                    frontier.Enqueue(next, newCost);
                    cameFrom[next] = current;
                }
            }
        }

        ValidConnections = cameFrom;
    }

    // Finds a shortest path using ValidMoves if it has been calculated.
    public List<Cell> FindPath(Cell goal)
    {
        if (ValidConnections.Keys.Count == 0)
            return new List<Cell>();

        var current = goal;
        var path = new List<Cell>();
        path.Add(goal);

        while (current != Cells[0]) {
            current = ValidConnections[current];
            path.Add(current);
        }

        return path;

        //return _pathfinder.FindPath(graph, Cells[0], destination);
    }

    // Calculate the movement cost of a given path
    public float PathCost(List<Cell> cells)
    {
        if (cells.Count == 0)
            return 0f;

        float total = 0f;

        float movementCost = 0f;

        int last = cells.Count - 1;

        // Add cost from current cell to first cell in provided path (if it's not already included)
        if (!cells[last].Equals(Cells[0]))
        {
            if (SharesSpace(cells[last]))
            {
                foreach (Cell cell in new List<Cell>(GetGridSizeCells(cells[last])))
                {
                    if (cell.MovementCost > movementCost)
                        movementCost = cell.MovementCost;
                }
                total += movementCost * Cells[0].GetDistance(cells[last]);
            }
            else
                total += Cells[0].GetDistance(cells[last]);

            movementCost = 0f;
        }

        // Add cost from cell to cell in path
        for (var i = last; i > 0; i--)
        {
            if (SharesSpace(cells[i - 1]))
            {
                foreach (Cell cell in new List<Cell>(GetGridSizeCells(cells[i - 1])))
                {
                    if (cell.MovementCost > movementCost)
                        movementCost = cell.MovementCost;
                }
                total += movementCost * cells[i].GetDistance(cells[i - 1]);
            }
            else
                total += cells[i].GetDistance(cells[i - 1]);

            movementCost = 0f;
        }

        return total;
    }

    // Gives visual indication that the unit is under attack.
    public abstract void MarkAsDefending(Unit other);
    // Gives visual indication that the unit is attacking.
    public abstract void MarkAsAttacking(MonoBehaviour other);
    // Gives visual indication that the unit is healing.
    public abstract void MarkAsHealing(Unit other);
    // Gives visual indication that the unit is destroyed. It gets called right before the unit game object is
    // destroyed, so either instantiate some new object to indicate destruction or redesign Defend method. 
    public override void MarkAsDestroyed() {}

    // Method marks unit as current players unit.
    public abstract void MarkAsFriendly();
    // Method mark units to indicate user that the unit is in range and can be attacked.
    public override void MarkAsTargetable() { }
    // Method marks unit as currently selected, to distinguish it from other units.
    public abstract void MarkAsSelected();
    // Method marks unit to indicate user that he can't do anything more with it this turn.
    public abstract void MarkAsFinished();
}

public class MovementEventArgs : EventArgs
{
    public Cell OriginCell;
    public Cell DestinationCell;
    public List<Cell> Path;

    public MovementEventArgs(Cell sourceCell, Cell destinationCell, List<Cell> path)
    {
        OriginCell = sourceCell;
        DestinationCell = destinationCell;
        Path = path;
    }
}
public class AttackEventArgs : EventArgs
{
    public Unit Attacker;
    public Unit Defender;

    public float Damage;

    public AttackEventArgs(Unit attacker, Unit defender, float damage)
    {
        Attacker = attacker;
        Defender = defender;

        Damage = damage;
    }
}
