using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class OnCellAbility : Ability {

    // Cells that will be targeted by ability. TargetCells[0] is origin cell.
    public List<Cell> TargetCells;

    // Use this for initialization
    public override void Initialize()
    {

    }
}
