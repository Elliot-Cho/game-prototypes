using UnityEngine;

class CellGridStateWaitingForInput : CellGridState
{
    public CellGridStateWaitingForInput(CellGrid cellGrid) : base(cellGrid)
    {
    }

    public override void OnEntityClicked(Entity entity)
    {
        if (entity is Obstacle)
            return;
        if (entity is Unit)
        {
            var unit = entity as Unit;
            // set state of clicked unit to selected, only if it's that unit's turn
            if (_cellGrid.InitTracker[0].Equals(unit))
                _cellGrid.CellGridState = new CellGridStateUnitSelected(_cellGrid, unit);
        }
    }
}
