using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CellGridStateAbilityActive : CellGridState
{
    private Unit _unit;
    private List<Cell> _cellsInRange;
    private List<Cell> _areaCells;
    private List<Entity> _entitiesInRange;
    private List<Obstacle> _obsInRange;

    private Ability _ability;

    private Cell _unitCell;

    public CellGridStateAbilityActive(CellGrid cellGrid, Unit unit, Ability ability) : base(cellGrid)
    {
        _unit = unit;
        _ability = ability;
        _cellsInRange = new List<Cell>();
        _entitiesInRange = new List<Entity>();
        _obsInRange = new List<Obstacle>();
        _areaCells = new List<Cell>();
    }

    // When an ability is selected during a player unit's turn
    public override void OnStateEnter()
    {
        base.OnStateEnter();

        _unitCell = _unit.Cells[0];

        // Calculate cells that are within ability range
        _cellsInRange = _ability.GetCellsInAbilityRange(_unitCell, _unit);
        foreach (var cell in _cellsInRange)
        {
            cell.UnMark();
            if (_ability.Type.Equals(Ability.Types.Damage))
                cell.Mark(new Color(1f, 0.1f, 0.1f, 0.2f));
            else if (_ability.Type.Equals(Ability.Types.Healing))
                cell.Mark(new Color(0.1f, 1f, 0.1f, 0.4f));
        }

        // Get list of targetable entities in range
        _entitiesInRange = GetEntitiesInRange(_cellsInRange);
        // If abiliy used used by clicking an entity in ability range, mark the entities in range as targetable
        if (_ability is OnEntityAbility)
        {
            foreach (var entity in _entitiesInRange)
            {
                if (_ability.Type.Equals(Ability.Types.Damage))
                    entity.MarkAsTargetable();
                else if (_ability.Type.Equals(Ability.Types.Healing))
                    entity.Mark(new Color(0.5f, 0.8f, 0.5f, 1f));
            }
        }
    }

    public List<Entity> GetEntitiesInRange (List<Cell> cells)
    {
        var ret = new List<Entity>();

        foreach (var unit in _cellGrid.Units)
        {
            if (!CanAbilityTargetEntity(unit))
                continue;

            foreach (var gridSizeCell in unit.GetGridSizeCells(unit.Cells[0]))
            {
                if (cells.Contains(gridSizeCell))
                {
                    ret.Add(unit);
                    break;
                }
            }
        }
        if (_ability.Affects.Obstacles)
        {
            foreach (var obs in _cellGrid.Obstacles)
            {
                foreach (var gridSizeCells in obs.GetGridSizeCells(obs.Cells[0]))
                {
                    if (cells.Contains(gridSizeCells))
                    {
                        ret.Add(obs);
                        break;
                    }
                }
            }
        }

        return ret;
    }

    // Use ability on entity that is clicked (if applicable)
    public override void OnEntityClicked(Entity entity)
    {
        if (!(_ability is OnEntityAbility))
            return;

        if (!_unit.State.Equals(Unit.CurrentState.Normal) ||
            _unit.ActionPoints > _ability.ActionCost)
            return;

        if (_entitiesInRange.Contains(entity) && CanAbilityTargetEntity(entity))
        {
            // Subtract action points
            if (_unit.ActionPoints < _ability.ActionCost)
                return;
            _unit.ActionPoints -= _ability.ActionCost;

            // Apply ability effect to all targets specified by the ability
            var targets = _ability.GetTargets(_unit, entity, _entitiesInRange);
            if (!(targets.Count() > 0))
                return;
            foreach (var target in targets)
            {
                _ability.Apply(_unit, target);
            }
            _cellGrid.CellGridState = new CellGridStateUnitSelected(_cellGrid, _unit);
        }
    }

    // Checks if ability can target a specified entity
    public bool CanAbilityTargetEntity(Entity entity)
    {
        if (entity is Unit)
        {
            var unit = entity as Unit;
            if (!_ability.Affects.Self && unit.Equals(_unit) ||
                !_ability.Affects.Allies && _unit.PlayerNumber.Equals(unit.PlayerNumber) ||
                !_ability.Affects.Enemies && !_unit.PlayerNumber.Equals(unit.PlayerNumber))
                return false;
        }
        if (entity is Obstacle && !_ability.Affects.Obstacles)
        {
            return false;
        }

        return true;
    }

    public override void OnCellSelected(Cell cell)
    {
        if (!_cellsInRange.Contains(cell))
        {
            _areaCells = new List<Cell>();
            return;
        }

        _areaCells = _ability.GetCellsInAreaRange(cell, _unit);

        foreach (var areaCell in _areaCells)
        {
            if (_ability.Type.Equals(Ability.Types.Damage))
                areaCell.Mark(new Color(1f, 0.1f, 0.1f, 0.4f));
            else if (_ability.Type.Equals(Ability.Types.Healing))
                areaCell.Mark(new Color(0f, 1f, 0.1f, 0.6f));
        }
    }

    public override void OnCellDeselected(Cell cell)
    {
        base.OnCellDeselected(cell);
        if (_areaCells.Count > 0)
        {
            foreach (var areaCell in _areaCells)
            {
                areaCell.UnMark();
                if (_cellsInRange.Contains(areaCell))
                {
                    if (_ability.Type.Equals(Ability.Types.Damage))
                        areaCell.Mark(new Color(1f, 0.1f, 0.1f, 0.2f));
                    else if (_ability.Type.Equals(Ability.Types.Healing))
                        areaCell.Mark(new Color(0.1f, 1f, 0.1f, 0.4f));
                }
                    
            }
        }
        else if (_cellsInRange.Contains(cell))
        {
            if (_ability.Type.Equals(Ability.Types.Damage))
                cell.Mark(new Color(1f, 0.1f, 0.1f, 0.2f));
            else if (_ability.Type.Equals(Ability.Types.Healing))
                cell.Mark(new Color(0.1f, 1f, 0.1f, 0.4f));
        }
    }

    public override void OnCellClicked(Cell cell)
    {
        if (!(_ability is OnCellAbility))
            return;

        if (!_unit.State.Equals(Unit.CurrentState.Normal) ||
            _unit.ActionPoints > _ability.ActionCost ||
            !_cellsInRange.Contains(cell))
            return;

        // Subtract action points
        if (_unit.ActionPoints < _ability.ActionCost)
            return;
        _unit.ActionPoints -= _ability.ActionCost;

        var entitiesInArea = GetEntitiesInRange(_areaCells);
        foreach (var entity in entitiesInArea)
        {
            if (CanAbilityTargetEntity(entity))
            {
                _ability.Apply(_unit, entity);
            }
        }

        _cellGrid.CellGridState = new CellGridStateUnitSelected(_cellGrid, _unit);
    }

    public override void OnStateExit()
    {
        _unit.OnUnitDeselected();
        foreach (var entity in _entitiesInRange)
        {
            if (entity is Unit)
            {
                var unit = entity as Unit;
                unit.SetState(new UnitStateNormal(unit));
            }
            if (entity is Obstacle)
            {
                entity.UnMark();
            }
        }
        /*foreach (var unit in _cellGrid.Units)
        {
            if (unit == null) continue;
            unit.SetState(new UnitStateNormal(unit));
        }*/
        foreach (var cell in _cellGrid.Cells)
        {
            cell.UnMark();
        }
    }
}
