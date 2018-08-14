using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GUIController : MonoBehaviour
{
    public CellGrid CellGrid;

    public Canvas Canvas;

    // Current turn unit
    public Unit CurrentUnit;

    void Start()
    {
        CurrentUnit = CellGrid.InitTracker[0];
    }

    /// <summary>
    /// Activates select ability in ability bar. Assigned to button in Unit Button object.
    /// </summary>
    /// <param name="num">Button number (starts from 0).</param>
    public void AbilityButton (int num)
    {
        if (!(CurrentUnit is Character) ||
            !(CurrentUnit.State.Equals(Unit.CurrentState.Normal)) ||
            !(CellGrid.CellGridState is CellGridStateUnitSelected))
            return;

        var CurrentChar = CurrentUnit as Character;

        var AbilityBar = CurrentChar.AbilityBar;
        if (AbilityBar == null || num >= AbilityBar.Count)
            return;

        AbilityBar[num].Initialize();

        if (CurrentUnit.ActionPoints < AbilityBar[num].ActionCost)
            return;

        CellGrid.CellGridState = new CellGridStateAbilityActive(CellGrid, CurrentUnit, AbilityBar[num]);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.N))
        {
            CellGrid.EndTurn();//User ends his turn by pressing "n" on keyboard.
            CurrentUnit = CellGrid.InitTracker[0];
        }

        if (Input.GetKeyDown(KeyCode.Escape) && CurrentUnit.State.Equals(Unit.CurrentState.Normal))
        {
            CellGrid.CellGridState = new CellGridStateWaitingForInput(CellGrid);
        }
    }
}

