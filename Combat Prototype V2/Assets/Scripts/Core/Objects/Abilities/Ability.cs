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

    public GenericDictionary<string, float> abilityValues;

    public float baseActionPointCost = 1f;

    public AppliesTo appliesTo;

    public enum DangerType { harmful, neutral, helpful }
    public DangerType dangerType;

    public enum TargetType { point, self, burst }
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

      float abilityRange = 0;
      // Try to get range and assign it to abilityRange, otherwise do nothing
      this.abilityValues.TryGetValue("range", out abilityRange);

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

    public List<Cell> GetTargetCells(Cell originCell) {
      switch (this.targetType) {
        case TargetType.point:
          return GetPointTargetCells(originCell);
        case TargetType.burst:
          return GetBurstTargetCells(originCell);
        case TargetType.self:
        default:
          return new List<Cell>();
      }
    }

    public List<Cell> GetPointTargetCells(Cell originCell = null) {
      List<Cell> targetCells = new List<Cell>();
      targetCells.Add(originCell);

      float areaRange = 0;
      // Try to get areaRange and assign it to areaRange, otherwise do nothing
      this.abilityValues.TryGetValue("areaRange", out areaRange);

      if (areaRange <= 0) {
        return targetCells.Intersect(this.abilityRangeCells.Keys.ToList()).ToList();
      } else if (!this.abilityRangeCells.Keys.ToList().Contains(originCell)) {
        return new List<Cell>();
      }

      var frontier = new HeapPriorityQueue<Cell>();
      var currentCost = new Dictionary<Cell, float>();

      frontier.Enqueue(originCell, 0);
      currentCost[originCell] = 0;

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

    public List<Cell> GetBurstTargetCells(Cell originCell) {
      List<Cell> burstCells = this.abilityRangeCells.Keys.ToList();
      if (burstCells.Contains(originCell)) return burstCells;

      return new List<Cell>();
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
