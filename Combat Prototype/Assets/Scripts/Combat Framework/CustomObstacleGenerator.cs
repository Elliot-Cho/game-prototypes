using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class CustomObstacleGenerator : MonoBehaviour {

    public Transform ObsParent;
    public Transform CellsParent;

    /// <summary>
    /// Returns units that are already children of UnitsParent object.
    /// </summary>
    public List<Obstacle> SpawnObstacles(List<Cell> cells)
    {
        List<Obstacle> ret = new List<Obstacle>();
        for (int i = 0; i < ObsParent.childCount; i++)
        {
            var obs = ObsParent.GetChild(i).GetComponent<Obstacle>();
            if (obs != null)
            {
                var cell = cells.OrderBy(h => Math.Abs((h.transform.position - obs.transform.position).magnitude)).First();
                if (!cell.HasObstacle)
                {
                    cell.HasObstacle = true;
                    obs.Cells[0] = cell;
                    obs.transform.position = cell.transform.position;
                    obs.Initialize();
                    ret.Add(obs);
                }
                else
                {
                    Destroy(obs.gameObject);
                }//If the nearest cell is taken, the unit gets destroyed.
            }
            else
            {
                Debug.LogError("Invalid object in Obstacle Parent game object");
            }

        }
        return ret;
    }

    public void SnapToGrid()
    {
        List<Transform> cells = new List<Transform>();

        foreach (Transform cell in CellsParent)
        {
            cells.Add(cell);
        }

        foreach (Transform obs in ObsParent)
        {
            var closestCell = cells.OrderBy(h => Math.Abs((h.transform.position - obs.transform.position).magnitude)).First();
            if (!closestCell.GetComponent<Cell>().HasObstacle)
            {
                Vector3 offset = new Vector3(0, 0, closestCell.GetComponent<Cell>().GetCellDimensions().z);
                obs.position = closestCell.transform.position - offset;
            }// Obstacle gets snapped to the nearest cell
        }
    }
}
