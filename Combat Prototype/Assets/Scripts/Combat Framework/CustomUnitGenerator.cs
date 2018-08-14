using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CustomUnitGenerator : MonoBehaviour, IUnitGenerator
{
    public Transform UnitsParent;
    public Transform CellsParent;

    /// <summary>
    /// Returns units that are already children of UnitsParent object.
    /// </summary>
    public List<Unit> SpawnUnits(List<Cell> cells)
    {
        List<Unit> ret = new List<Unit>();
        for (int i = 0; i < UnitsParent.childCount; i++)
        {
            var unit = UnitsParent.GetChild(i).GetComponent<Unit>();
            if(unit !=null)
            {
                var occupyingCells = new List<Cell>();
                //var closestCells = cells.OrderBy(h => Math.Abs((h.transform.position - unit.transform.position).magnitude));

                // Get first cell (the closest cell to unit position)
                occupyingCells.Add(cells.OrderBy(h => Math.Abs((h.transform.position - unit.transform.position).magnitude)).First());

                if (!occupyingCells[0].HasObstacle)
                {
                    occupyingCells[0].HasObstacle = true;

                    // Get GridSize number of cells in a row expanding to the right
                    for (int j = 1; j < unit.GridSize.x; j++)
                    {
                        var cell = occupyingCells.Last().GetAdjCell(new Vector2(1, 0))[0];
                        if (cell != null && !cell.HasObstacle)
                        {
                            occupyingCells.Add(cell);
                            cell.HasObstacle = true;
                        }
                        else
                        {
                            Debug.Log("Couldn't find space for " + unit.name + "in x-axis.");
                            Destroy(unit.gameObject);
                            break;
                        }
                    }

                    // Now get GridSize number of cells downwards using the previously found ones
                    if (unit.GridSize.y > 1)
                    {
                        foreach (var cell in new List<Cell>(occupyingCells))
                        {
                            for (int j = 1; j < unit.GridSize.y; j++)
                            {
                                Cell ycell;
                                if (j == 1)
                                    ycell = cell.GetAdjCell(new Vector2(0, -1))[0];
                                else
                                    ycell = occupyingCells.Last().GetAdjCell(new Vector2(0, -1))[0];

                                if (ycell != null && !ycell.HasObstacle)
                                {
                                    occupyingCells.Add(ycell);
                                    ycell.HasObstacle = true;
                                }
                                else
                                {
                                    Debug.Log("Couldn't find space for " + unit.name + "in y-axis.");
                                    Destroy(unit.gameObject);
                                    break;
                                }
                            }
                        }
                    }

                    unit.Cells[0] = occupyingCells[0]; // REMOVE LATER
                    //unit.transform.position = occupyingCells[0].transform.position;
                    var pos = new Vector3();
                    foreach (var cell in occupyingCells)
                    {
                        unit.Cells.Add(cell);
                        pos += cell.transform.position;
                    }
                    unit.transform.position = pos / (unit.GridSize * unit.GridSize);
                    unit.Initialize();
                    ret.Add(unit);
                }
                else
                {
                    Debug.Log("Couldn't find initial space for" + unit.name);
                    Destroy(unit.gameObject);
                }
                /*var cell = cells.OrderBy(h => Math.Abs((h.transform.position - unit.transform.position).magnitude)).First();
                if (!cell.HasObstacle)
                {
                    cell.HasObstacle = true;
                    unit.Cell = cell;
                    unit.transform.position = cell.transform.position;
                    unit.Initialize();
                    ret.Add(unit);
                }//Unit gets snapped to the nearest cell
                else
                {
                    Destroy(unit.gameObject);
                }//If the nearest cell is taken, the unit gets destroyed.*/
            }
            else
            {
                Debug.LogError("Invalid object in Units Parent game object");
            }
            
        }
        return ret;
    }

    /*public List<Cell> GetOccupied(List<Cell> cells)
    {
        // Detect only colliders on the cell layer
        int layerID = 8;
        int layerMask = 1 << layerID;

        var ret = new List<Cell>();
        var cell = Physics2D.OverlapBoxAll(new Vector2()//OverlapPoint(new Vector2(transform.position.x, transform.position.y) + direction, layerMask);

        if (cell != null)
            ret.Add(cell.gameObject.GetComponent<Cell>());

        return ret;
    }*/

    public void SnapToGrid()
    {
        List<Transform> cells = new List<Transform>();

        foreach(Transform cell in CellsParent)
        {
            cells.Add(cell);
        }

        foreach(Transform unit in UnitsParent)
        {
            var closestCell = cells.OrderBy(h => Math.Abs((h.transform.position - unit.transform.position).magnitude)).First();
            if (!closestCell.GetComponent<Cell>().HasObstacle)
            {
                Vector3 offset = new Vector3(0,0, closestCell.GetComponent<Cell>().GetCellDimensions().z);
                unit.position = closestCell.transform.position - offset;
            }//Unit gets snapped to the nearest cell
        }
    }

    public List<Entity> SpawnEntities(List<Cell> cells)
    {
        throw new NotImplementedException();
    }
}

