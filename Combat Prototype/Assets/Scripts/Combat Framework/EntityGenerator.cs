using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EntityGenerator : MonoBehaviour, IUnitGenerator
{
    public Transform UnitsParent;
    public Transform ObstaclesParent;
    public Transform CellsParent;

    /// <summary>
    /// Returns entities that are children of the UnitsParent and ObstaclesParent game objects.
    /// </summary>
    public List<Entity> SpawnEntities(List<Cell> cells)
    {
        List<Entity> ret = new List<Entity>();

        List<Entity> entityList = new List<Entity>();
        for (int i = 0; i < UnitsParent.childCount; i++)
        {
            entityList.Add(UnitsParent.GetChild(i).GetComponent<Entity>());
        }
        for (int i = 0; i < ObstaclesParent.childCount; i++)
        {
            entityList.Add(ObstaclesParent.GetChild(i).GetComponent<Entity>());
        }

        foreach (Entity entity in entityList)
        {
            if(entity !=null)
            {
                // Cells occupied by generated entity
                var occupyingCells = new List<Cell>();

                // Get first cell (the closest cell to entity's position)
                occupyingCells.Add(cells.OrderBy(h => Math.Abs((h.transform.position - entity.transform.position).magnitude)).First());

                if (!occupyingCells[0].HasObstacle)
                {
                    occupyingCells[0].HasObstacle = true;

                    // Get GridSize number of cells in a row expanding to the right
                    for (int j = 1; j < entity.GridSize.x; j++)
                    {
                        var cell = occupyingCells.Last().GetAdjCell(new Vector2(1, 0))[0];
                        if (cell != null && !cell.HasObstacle)
                        {
                            occupyingCells.Add(cell);
                            cell.HasObstacle = true;
                        }
                        else
                        {
                            Debug.Log("Couldn't find space for " + entity.name + "in x-axis.");
                            Destroy(entity.gameObject);
                            break;
                        }
                    }

                    // Now get GridSize number of cells downwards using the previously found ones
                    if (entity.GridSize.y > 1)
                    {
                        foreach (var cell in new List<Cell>(occupyingCells))
                        {
                            for (int j = 1; j < entity.GridSize.y; j++)
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
                                    Debug.Log("Couldn't find space for " + entity.name + "in y-axis.");
                                    Destroy(entity.gameObject);
                                    break;
                                }
                            }
                        }
                    }

                    // Add occupied cells to unit Cells list, and place unit in position based on GridSize
                    var pos = new Vector3();
                    foreach (var cell in occupyingCells)
                    {
                        entity.Cells.Add(cell);
                        pos += new Vector3(cell.transform.position.x, cell.transform.position.y, entity.transform.position.z);
                    }
                    entity.transform.position = pos / (entity.GridSize.x * entity.GridSize.y);

                    entity.Initialize();
                    ret.Add(entity);
                }
                else
                {
                    Debug.Log("Couldn't find initial space for" + entity.name);
                    Destroy(entity.gameObject);
                }
            }
            else
            {
                Debug.LogError("Invalid object in Entity Parent game objects");
            }
            
        }
        return ret;
    }



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
}

