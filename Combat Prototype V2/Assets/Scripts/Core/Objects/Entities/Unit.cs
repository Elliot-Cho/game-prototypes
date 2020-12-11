using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Wayfinder {
  /// <summary>
  /// Units are entities that can move and take actions independently in the overworld.
  /// </summary>
  public class Unit : Entity {
    /// <summary>
    /// List of all instantiated units
    /// </summary>
    public static List<Unit> UnitList = new List<Unit>();

    // Unit Base Overworld Stats
    public float moveSpeed;

    // Unit Level
    public int level;

    // Unit Base Combat Stats
    public CombatStats stats = new CombatStats();

    // Current Unit Stats
    public float actionPoints;

    // Shortest paths to all grid size points in move range along with their costs
    public Dictionary<Vector2, (Vector2 previousPoint, float totalCost)> availableDestinations =
             new Dictionary<Vector2, (Vector2 previousPoint, float totalCost)>();

    public List<Ability> abilities = new List<Ability>();

    protected override void Awake() {
      base.Awake();

      UnitList.Add(this);
    }

    // Initialization of unit after instantiation
    protected override void Start() {
      base.Start();

      // Set actions to max
      actionPoints = stats.maxActionPoints.value;

      // Assign abilities to this unit
      this.abilities.ForEach(ability => ability.user = this);
    }

    public bool CanAccessCellAltitude(List<Cell> gridSizeCurrentCells, Cell targetCell) {
      float currentCellsMaxAltitude = gridSizeCurrentCells.Max(cell => cell.altitude);
      float currentCellsMinAltitude = gridSizeCurrentCells.Min(cell => cell.altitude);

      if (this.altitude > 0 &&
          this.altitude + currentCellsMaxAltitude >= targetCell.altitude) {
        return true;
      }

      if (Mathf.Abs(currentCellsMaxAltitude - targetCell.altitude) <= 0.5f ||
          Mathf.Abs(currentCellsMinAltitude - targetCell.altitude) <= 0.5f) {
        return true;
      }

      return false;
    }

    public bool CellObstructed(Cell targetCell) {
      foreach (Entity occupyingEntity in targetCell.occupyingEntities) {
        if (base.SharesAltitudeWith(occupyingEntity) &&
            occupyingEntity.properties.obstruction) {
          return true;
        }
      }

      if (targetCell.properties.fullObstruction) return true;
      if (targetCell.properties.altitudeObstruction && this.altitude == 0) return true;

      return false;
    }

    public bool DiagonalMovementBlocked(List<Cell> gridSizeCurrentCells, Vector2 originPoint, Vector2 nextPoint) {
      Vector2 originOrthogonalPoint = new Vector2(originPoint.x, nextPoint.y);
      Vector2 nextOrthogonalPoint = new Vector2(nextPoint.x, originPoint.y);

      List<Cell> orthogonalCells = base.GridSizeCellsAtPoint(originOrthogonalPoint);
      orthogonalCells.AddRange(base.GridSizeCellsAtPoint(nextOrthogonalPoint));

      foreach (Cell orthogonalCell in orthogonalCells) {
        if (this.occupiedCells.Contains(orthogonalCell)) continue;
        if (this.CellObstructed(orthogonalCell)) return true;
        if (!this.CanAccessCellAltitude(gridSizeCurrentCells, orthogonalCell)) return true;
      }

      return false;
    }

    public bool CanMoveOntoCell(Vector2 currentPoint, Vector2 nextPoint) {
      List<Cell> gridSizeTargetCells = base.GridSizeCellsAtPoint(nextPoint);
      List<Cell> gridSizeCurrentCells = base.GridSizeCellsAtPoint(currentPoint);

      foreach (Cell targetCell in gridSizeTargetCells) {
        if (this.occupiedCells.Contains(targetCell)) continue;
        if (this.CellObstructed(targetCell)) return false;
        if (!this.CanAccessCellAltitude(gridSizeCurrentCells, targetCell)) return false;
      }

      // Don't allow diagonal movement through corner obstructions
      Vector2 directionToNextPoint = (nextPoint - currentPoint).normalized;
      if (directionToNextPoint.x != 0 && directionToNextPoint.y != 0) {
        return !this.DiagonalMovementBlocked(gridSizeCurrentCells, currentPoint, nextPoint);
      }

      return true;
    }

    public void GetAvailableDestinations() {
      var frontier = new HeapPriorityQueue<Vector2>();
      var cameFrom = new Dictionary<Vector2, (Vector2 previousPoint, float totalCost)>{};
      var currentCost = new Dictionary<Vector2, float>{};

      Vector2 originPoint = this.transform.position;
      frontier.Enqueue(originPoint, 0);
      cameFrom[originPoint] = default;
      currentCost[originPoint] = 0;

      while (frontier.Count > 0) {
        Vector2 currentPoint = frontier.Dequeue();

        foreach (Vector2 direction in Cell._directions) {
          Vector2 nextPoint = currentPoint + direction;

          List<Cell> nextGridSizeCells = base.GridSizeCellsAtPoint(nextPoint);

          if (nextGridSizeCells.Count < this.gridSize.x * this.gridSize.y) continue; // if point goes off the map
          if (!this.CanMoveOntoCell(currentPoint, nextPoint)) continue;

          float distance = Vector2.Distance(currentPoint, nextPoint) > 1 ? 1.5f : 1f;
          float newPathCost =
            currentCost[currentPoint] +
            nextGridSizeCells.Max(cell => cell.movementCost) * distance;

          if (newPathCost > this.actionPoints) continue;

          if (!currentCost.Keys.Contains(nextPoint) || newPathCost < currentCost[nextPoint]) {
            currentCost[nextPoint] = newPathCost;
            frontier.Enqueue(nextPoint, newPathCost);
            cameFrom[nextPoint] = (currentPoint, newPathCost);
          }
        }
      }

      this.availableDestinations = cameFrom;
    }

    public List<Cell> CellsInMoveRange() {
      List<Cell> destinationCells = new List<Cell>();

      foreach(Vector2 point in this.availableDestinations.Keys) {
        foreach(Cell cell in base.GridSizeCellsAtPoint(point)) {
          destinationCells.Add(cell);
        }
      }

      return destinationCells.Except(this.occupiedCells).Distinct().ToList();
    }

    public List<Vector2> FindPathToCell(Cell targetCell) {
      Vector2 topLeftCorner = new Vector2(
        (targetCell.transform.position.x - this.gridSize.x/2) + 0.05f,
        (targetCell.transform.position.y + this.gridSize.y/2) - 0.05f
      );
      Vector2 bottomRightCorner = new Vector2(
        (targetCell.transform.position.x + this.gridSize.x/2) - 0.05f,
        (targetCell.transform.position.y - this.gridSize.y/2) + 0.05f
      );

      Vector2 destinationPoint = default;

      foreach (Vector2 point in this.availableDestinations.Keys) {
        if (point.x < topLeftCorner.x ||
            point.x > bottomRightCorner.x ||
            point.y < bottomRightCorner.y ||
            point.y > topLeftCorner.y) {
              continue;
            }

        if (destinationPoint == null) {
          destinationPoint = point;
          continue;
        }

        // Get point that has lowest path cost
        if (destinationPoint == default || this.availableDestinations[point].totalCost <
            this.availableDestinations[destinationPoint].totalCost) {
          destinationPoint = point;
        }

        // Get point in move range that is closest to mouse
        // Possible alternate pathing method, has it's own issues
        // Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        // if (destinationPoint == default || Vector2.Distance(point, mousePosition) <
        //     Vector2.Distance(destinationPoint, mousePosition)) {
        //   destinationPoint = point;
        // }
      }

      Vector2 currentPoint = destinationPoint;
      List<Vector2> path = new List<Vector2>();
      while(this.availableDestinations[currentPoint] != default) {
        path.Add(currentPoint);
        currentPoint = this.availableDestinations[currentPoint].previousPoint;
      }

      path.Reverse();
      return path;
    }

    public void MoveAlongPointPath(List<Vector2> pathPoints) {
      this.actionPoints -= this.availableDestinations[pathPoints.Last()].totalCost;
      base.ReloadOccupiedCells(pathPoints.Last());

      if (this.moveSpeed > 0) {
        StartCoroutine(this.MoveOnPathCoroutine(pathPoints));
      } else {
        Debug.Log($"Unit {this.entityName} tried to move but has 0 moveSpeed.");
        this.transform.position = pathPoints.Last();

        UnitMovingGameState state = GameManager.Instance.state as UnitMovingGameState;
        state.FinishMovement();
      }
    }

    public IEnumerator MoveOnPathCoroutine(List<Vector2> pathPoints) {
      foreach (Vector2 pathPoint in pathPoints) {
        Vector2 currentPosition = new Vector2(this.transform.position.x, this.transform.position.y);
        // Movement loop
        while (currentPosition != pathPoint) {
          this.transform.position = Vector2.MoveTowards(
            currentPosition,
            pathPoint,
            Time.deltaTime * this.moveSpeed
          );

          currentPosition = new Vector2(this.transform.position.x, this.transform.position.y);

          // Continue loop in next update frame
          yield return 0;
        }
      }

      UnitMovingGameState state = GameManager.Instance.state as UnitMovingGameState;
      state.FinishMovement();
    }

    public bool CanMoveWithActionPoints() {
      if (this.actionPoints <= 0) return false;

      Vector2 currentPoint = this.transform.position;

      return Cell._directions.Any(direction =>
        this.CanMoveOntoCell(currentPoint, currentPoint + direction)
      );
    }

    public bool CanUseAbilitiesWithActionPoints() {
      if (this.abilities.Count == 0) return false;

      return this.abilities.Any(ability => ability.baseActionPointCost <= this.actionPoints);
    }

    public bool HasActions() {
      return this.CanMoveWithActionPoints() || this.CanUseAbilitiesWithActionPoints();
    }
  }
}
