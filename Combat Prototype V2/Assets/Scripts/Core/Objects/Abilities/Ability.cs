using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;

namespace Wayfinder {
  [System.Serializable]
  public class Ability {
    public string name;
    public string description;

    public Sprite icon;

    public GenericDictionary<string, string> abilityValues;

    public float baseActionPointCost = 1f;

    public AppliesTo appliesTo;

    public enum DangerType { harmful, neutral, helpful }
    public DangerType dangerType;

    public enum TargetType { point, self, burst, cone, beam }
    public TargetType targetType;

    public Unit user;

    public Dictionary<Cell, Cell> abilityRangeCells = new Dictionary<Cell, Cell>();

    // Temporary workaround until xml importing is implemented
    public List<string> abilityEffects;
    // public List<IAbilityEffect> abilityEffects;

    public void Activate() {
      if (this.baseActionPointCost > this.user.actionPoints) return;

      GameManager.Instance.state = new UnitAbilityActivatedState(this);
    }

    public void GetAbilityRange() {
      var frontier = new HeapPriorityQueue<Cell>();
      var cameFrom = new Dictionary<Cell, Cell>();
      var currentCost = new Dictionary<Cell, float>();

      float abilityRange = this.abilityValues.GetConvertedValue("range", 1f);

      foreach (Cell originCell in this.user.occupiedCells) {
        frontier.Enqueue(originCell, 0);
        currentCost[originCell] = 0;
      }

      while (frontier.Count > 0) {
        Cell currentCell = frontier.Dequeue();

        foreach (Cell nextCell in currentCell.neighbourCells) {
          float newPathCost = currentCost[currentCell] + currentCell.DistanceToCell(nextCell);

          if (newPathCost > abilityRange) continue;

          if (this.user.occupiedCells.Contains(nextCell)) continue;

          if (!currentCost.Keys.Contains(nextCell) || newPathCost < currentCost[nextCell]) {
            currentCost[nextCell] = newPathCost;
            frontier.Enqueue(nextCell, newPathCost);
            cameFrom[nextCell] = currentCell;
          }
        }
      }

      this.abilityRangeCells = cameFrom;
    }

    public List<Cell> GetTargetCells(Cell mouseOverCell) {
      switch (this.targetType) {
        case TargetType.point:
          return GetPointTargetCells(mouseOverCell);
        case TargetType.burst:
          return GetBurstTargetCells(mouseOverCell);
        case TargetType.cone:
          return GetConeTargetCells(mouseOverCell);
        case TargetType.beam:
          return GetBeamTargetCells(mouseOverCell);
        case TargetType.self:
        default:
          return new List<Cell>();
      }
    }

    public List<Cell> GetPointTargetCells(Cell mouseOverCell = null) {
      if (!this.abilityRangeCells.Keys.ToList().Contains(mouseOverCell)) return new List<Cell>();

      List<Cell> targetCells = new List<Cell>();
      targetCells.Add(mouseOverCell);

      float areaRange = this.abilityValues.GetConvertedValue("areaRange", 0f);

      if (areaRange <= 0) {
        return targetCells.Intersect(this.abilityRangeCells.Keys.ToList()).ToList();
      }

      var frontier = new HeapPriorityQueue<Cell>();
      var currentCost = new Dictionary<Cell, float>();

      frontier.Enqueue(mouseOverCell, 0);
      currentCost[mouseOverCell] = 0;

      while (frontier.Count > 0) {
        Cell currentCell = frontier.Dequeue();

        foreach (Cell nextCell in currentCell.neighbourCells) {
          float newRangeCost = currentCost[currentCell] + currentCell.DistanceToCell(nextCell);

          if (newRangeCost > areaRange) continue;

          if (!currentCost.Keys.Contains(nextCell) || newRangeCost < currentCost[nextCell]) {
            currentCost[nextCell] = newRangeCost;
            frontier.Enqueue(nextCell, newRangeCost);
            targetCells.Add(nextCell);
          }
        }
      }

      return targetCells;
    }

    public List<Cell> GetBurstTargetCells(Cell mouseOverCell) {
      List<Cell> burstCells = this.abilityRangeCells.Keys.ToList();
      if (burstCells.Contains(mouseOverCell)) return burstCells;

      return new List<Cell>();
    }

    public Vector2 GetLimitedRangeCellPosition(Vector2 userPosition, Vector2 direction) {
      // Raycasts should only consider cells on cell layer
      int layerID = 8;
      int layerMask = 1 << layerID;

      RaycastHit2D[] raycastCells = Physics2D.RaycastAll(
        userPosition,
        direction,
        Mathf.Infinity,
        layerMask
      );

      Vector3 targetPosition = new Vector3();
      float furthestDistance = 0f;
      foreach (RaycastHit2D raycastHit in raycastCells) {
        Cell raycastCell = raycastHit.transform.gameObject.GetComponent<Cell>();

        if (!this.abilityRangeCells.ContainsKey(raycastCell)) continue;

        if (raycastHit.distance > furthestDistance) {
          targetPosition = raycastCell.transform.position;
          furthestDistance = raycastHit.distance;
        }
      }

      return targetPosition;
    }

    public List<Cell> GetConeTargetCells(Cell mouseOverCell = null) {
      bool canLimitRange = this.abilityValues.GetConvertedValue("canLimitRange", false);

      Vector3 userPosition = this.user.transform.position;
      Vector3 targetPosition = new Vector3();

      if (canLimitRange) {
        if (!this.abilityRangeCells.ContainsKey(mouseOverCell)) return new List<Cell>();
        targetPosition = mouseOverCell.transform.position;
      } else {
        targetPosition = this.GetLimitedRangeCellPosition(
          userPosition,
          mouseOverCell.transform.position - userPosition
        );
      }

      List<Cell> targetCells = new List<Cell>();

      float buffer = this.abilityValues.GetConvertedValue("buffer", 0f);
      float spread = this.abilityValues.GetConvertedValue("spread", 15f);

      // Prevent cone from being to thin or too wide
      spread = Mathf.Clamp(spread, 10f, 89f);

      Vector3 originPosition = userPosition + ((targetPosition - userPosition).normalized * buffer);

      Vector2 rayDirection = targetPosition - originPosition;
      float rayDistance = Vector2.Distance(originPosition, targetPosition);
      float angledRayDistance = rayDistance / Mathf.Cos((90f-spread)*(2*Mathf.PI/360)*-1+Mathf.PI/2);

      Vector3 positiveAngledDirection = Quaternion.AngleAxis(spread, Vector3.forward) * rayDirection;
      Vector3 negativeAngledDirection = Quaternion.AngleAxis(-spread, Vector3.forward) * rayDirection;

      Ray2D positiveAngledRay = new Ray2D(originPosition, positiveAngledDirection);
      Ray2D negativeAngledRay = new Ray2D(originPosition, negativeAngledDirection);

      foreach(Cell abilityRangeCell in this.abilityRangeCells.Keys.ToList()) {
        if (AbilityCalculations.VectorInsideTriangle(
          abilityRangeCell.transform.position,
          originPosition,
          positiveAngledRay.GetPoint(angledRayDistance),
          negativeAngledRay.GetPoint(angledRayDistance))
        ) {
          targetCells.Add(abilityRangeCell);
        }
      }

      return targetCells.Intersect(this.abilityRangeCells.Keys.ToList()).ToList();
    }

    public List<Cell> GetBeamTargetCells(Cell mouseOverCell = null) {
      List<Cell> targetCells = new List<Cell>();

      // Raycasts should only consider cells on cell layer
      int layerID = 8;
      int layerMask = 1 << layerID;

      float buffer = this.abilityValues.GetConvertedValue("buffer", 0f);
      float width = this.abilityValues.GetConvertedValue("width", 0f);
      bool canLimitRange = this.abilityValues.GetConvertedValue("canLimitRange", false);

      Vector3 userPosition = this.user.transform.position;
      Vector3 targetPosition = new Vector3();

      if (canLimitRange) {
        if (!this.abilityRangeCells.ContainsKey(mouseOverCell)) return new List<Cell>();
        targetPosition = mouseOverCell.transform.position;
      } else {
        targetPosition = this.GetLimitedRangeCellPosition(
          userPosition,
          mouseOverCell.transform.position - userPosition
        );
      }

      // Add width to starting position of circle cast so large casts don't get cells behind user
      Vector3 originPosition = userPosition + ((targetPosition - userPosition).normalized * width);

      RaycastHit2D[] raycastCells = Physics2D.CircleCastAll(
        originPosition,
        Mathf.Max(width, 0.01f),
        targetPosition - originPosition,
        Vector2.Distance(originPosition, targetPosition),
        layerMask
      );

      if (raycastCells.Length > 0) {
        foreach (RaycastHit2D raycastHit in raycastCells) {
          Cell raycastCell = raycastHit.transform.gameObject.GetComponent<Cell>();

          if (Vector2.Distance(raycastCell.transform.position, this.user.transform.position) < (buffer + 1) + 0.5f) continue;

          // Don't include raycastCells that are too far from the ray (cells that were hit in the corner)
          Vector2 direction = (originPosition - targetPosition).normalized;
          Vector2 height = raycastCell.transform.position - targetPosition;
          // This calculates the distance between the hit cell and the raycast
          if (Vector3.Cross(direction, height).magnitude > width + 0.5f) continue;

          targetCells.Add(raycastCell);
        }
      }

      return targetCells.Intersect(this.abilityRangeCells.Keys.ToList()).ToList();
    }

    public bool CanTargetEntity(Entity targetEntity) {
      if (this.user.faction == null) return false;

      if (this.appliesTo.friendlies && this.user.faction.IsAlly(targetEntity) ||
          this.appliesTo.enemies && this.user.faction.IsEnemy(targetEntity) ||
          this.appliesTo.neutrals && this.user.faction.IsNeutral(targetEntity) ||
          this.appliesTo.self && this.user == targetEntity) {
        return true;
      }

      return false;
    }

    public void ApplyAbilityEffects(Cell targetCell) {
      if (this.abilityEffects.Count == 0) return;

      List<Cell> targetCells = this.GetTargetCells(targetCell);

      this.ApplyAbilityEffects(targetCells);
    }

    public void ApplyAbilityEffects(List<Cell> targetCells) {
      if (this.abilityEffects.Count == 0) return;

      // Workaround using strings for testing until xml importing is implemented
      IAbilityEffect effectsToApply = new AbilityEffect(this, targetCells);
      foreach (string abilityEffect in this.abilityEffects) {
        var effectName = $"{abilityEffect}EffectDecorator";
        var type = Type.GetType($"Wayfinder.{effectName}");

        // All effects will use effectMultiplier of 1 until importing is implemented
        effectsToApply = (IAbilityEffect)Activator.CreateInstance(type, 1.0f, effectsToApply);
      }

      effectsToApply.ApplyEffect();
    }
  }
}
