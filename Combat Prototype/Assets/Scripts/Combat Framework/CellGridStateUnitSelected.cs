using UnityEngine;
using System.Collections.Generic;
using System.Linq;

class CellGridStateUnitSelected : CellGridState
{
    private Unit _unit;
    private List<Cell> _pathsInRange;
    private Dictionary<Cell, Cell> _pathsInGridSizeRange;
    //private List<Unit> _unitsInRange;
    //private List<Obstacle> _obsInRange;
    private List<Ability> _abilityBar;

    private Cell _unitCell;

    public CellGridStateUnitSelected(CellGrid cellGrid, Unit unit) : base(cellGrid)
    {
        _unit = unit;
        _pathsInRange = new List<Cell>();
        //_unitsInRange = new List<Unit>();
        //_obsInRange = new List<Obstacle>();

        _pathsInGridSizeRange = new Dictionary<Cell, Cell>();
    }

    // Base cell interaction functions
    public override void OnCellDeselected(Cell cell)
    {
        base.OnCellDeselected(cell);

        foreach (var _cell in _pathsInRange)
        {
            foreach (var gridSizeCell in _unit.GetGridSizeCells(_cell))
            {
                if (gridSizeCell.MovementCost > 1)
                    gridSizeCell.Mark(new Color(1f, 0.6f, 0.1f, 0.3f));
                else
                    gridSizeCell.Mark(new Color(1, 0.92f, 0.16f, 0.3f));
            }
        }
    }

    // When cell is hovered over
    public override void OnCellSelected(Cell cell)
    {
        base.OnCellSelected(cell);

        if (!_unit.State.Equals(Unit.CurrentState.Normal)) return;

        var inGridSizeRange = false;

        // if cell is outside selected unit's range, do nothing
        if (!_pathsInGridSizeRange.ContainsKey(cell))
            return;
        else
            inGridSizeRange = true;

        // if within movement range, highlight the cell (and any gridsize cells)
        if (_pathsInRange.Contains(cell))
        {
            // Pathfind for green highlighted path
            var path = _unit.FindPath(cell);
            foreach (var _cell in path)
            {
                var gridSizeCells = _unit.GetGridSizeCells(_cell);

                foreach (var gridSizeCell in gridSizeCells)
                {
                    gridSizeCell.Mark(new Color(0, 1, 0, 0.5f));
                }
            }
        }
        // if outside movement range, but within movement range + gridsize cells, highlight the cell that the gridsize cell belongs to
        else if (inGridSizeRange)
        {
            var gridSizeParentCell = _pathsInGridSizeRange[cell];
            var path = _unit.FindPath(gridSizeParentCell);
            foreach (var _cell in path)
            {
                var gridSizeCells = _unit.GetGridSizeCells(_cell);

                foreach (var gridSizeCell in gridSizeCells)
                {
                    gridSizeCell.Mark(new Color(0, 1, 0, 0.5f));
                }
            }
        }
    }

    // When cell is clicked
    public override void OnCellClicked(Cell cell)
    {
        if (!_unit.State.Equals(Unit.CurrentState.Normal))
            return;
        if(!_pathsInGridSizeRange.ContainsKey(cell))
            return;

        if (_pathsInRange.Contains(cell))
        {
            if (_unit.IsCellTraversable(cell))
            {
                // Pathfind to clicked path and move there
                var path = _unit.FindPath(cell);
                _unit.Move(cell, path);
                _cellGrid.CellGridState = new CellGridStateUnitSelected(_cellGrid, _unit);
            }

        }
        else
        {
            var gridSizeParentCell = _pathsInGridSizeRange[cell];
            if (_unit.IsCellTraversable(gridSizeParentCell))
            {
                var path = _unit.FindPath(gridSizeParentCell);
                _unit.Move(gridSizeParentCell, path);
                _cellGrid.CellGridState = new CellGridStateUnitSelected(_cellGrid, _unit);
            }
        }
    }

    public override void OnEntityClicked(Entity entity)
    {
        if (entity is Obstacle)
            return;

        if (entity is Unit)
        {
            var unit = entity as Unit;
            if (unit.Equals(_unit) || !_unit.State.Equals(Unit.CurrentState.Normal))
                return;

            // Select unit if it's their turn
            if (_cellGrid.InitTracker[0].Equals(unit))
            {
                _cellGrid.CellGridState = new CellGridStateUnitSelected(_cellGrid, unit);
            }
        }
    }

    // When a unit is selected (it becomes their turn)
    public override void OnStateEnter()
    {
        base.OnStateEnter();

        _unit.OnUnitSelected();
        _unitCell = _unit.Cells[0];

        // Update unit's ValidConnections dictionary
        _unit.GetAvailableDestinations(_unitCell, _unit.MovementPoints);

        _pathsInRange = _unit.ValidConnections.Keys.ToList();
        _pathsInRange.Remove(_unitCell);
        var cellsNotInRange = _cellGrid.Cells.Except(_pathsInRange);

        foreach (var cell in cellsNotInRange)
        {
            cell.UnMark();
        }
        foreach (var cell in _pathsInRange)
        {
            foreach (var gridSizeCell in _unit.GetGridSizeCells(cell))
            {
                _pathsInGridSizeRange[gridSizeCell] = cell;
                if (gridSizeCell.MovementCost > 1)
                    gridSizeCell.Mark(new Color(1f, 0.6f, 0.1f, 0.3f));
                else
                    gridSizeCell.Mark(new Color(1, 0.92f, 0.16f, 0.3f));
            }
        }

        if (_unit.ActionPoints <= 0) return;

        if (_unit is Character)
        {
            var character = _unit as Character;
            _abilityBar = character.AbilityBar;
        }

        // If the unit can't move any further or attack anyone, mark them as finished
        if (_unitCell.Neighbours.FindAll(c => c.MovementCost <= _unit.MovementPoints).Count == 0 
            && _unit.ActionPoints <= 0)
            _unit.SetState(new UnitStateMarkedAsFinished(_unit));
    }
    public override void OnStateExit()
    {
        _unit.OnUnitDeselected();
        /*foreach (var unit in _unitsInRange)
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

