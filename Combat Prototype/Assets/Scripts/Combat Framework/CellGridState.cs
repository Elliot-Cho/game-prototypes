using UnityEngine;
using System.Linq;

public abstract class CellGridState
{
    protected CellGrid _cellGrid;
    
    protected CellGridState(CellGrid cellGrid)
    {
        _cellGrid = cellGrid;
    }

    public virtual void OnEntityClicked(Entity entity)
    { }
    // All virtual functions can be overwritten in different CellGrid States
    public virtual void OnUnitClicked(Unit unit)
    { } // overwritten in CellGridStateUnitSelected.cs

    public virtual void OnObsClicked(Obstacle obs)
    { } // overwritten in CellGridStateUnitSelected.cs

    public virtual void OnCellDeselected(Cell cell)
    {
        cell.UnMark();
    }
    public virtual void OnCellSelected(Cell cell)
    {
        cell.Mark(new Color(0.8f, 0.8f, 0.8f, 0.5f));
    }
    public virtual void OnCellClicked(Cell cell)
    { }

    public virtual void OnStateEnter()
    {
        if (_cellGrid.Units.Select(u => u.PlayerNumber).Distinct().ToList().Count == 1)
        {
            _cellGrid.CellGridState = new CellGridStateGameOver(_cellGrid);
        }
    }
    public virtual void OnStateExit()
    {
    }
}