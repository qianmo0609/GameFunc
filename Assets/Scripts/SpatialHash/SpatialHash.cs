using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpatialHash
{
    private Dictionary<Vector3, List<Transform>> spatialHash;
    private int CELL_SIZE;

    public SpatialHash(int CELL_SIZE)
    {
        spatialHash = new Dictionary<Vector3, List<Transform>>();
        this.CELL_SIZE = CELL_SIZE;
    }

    public void AddGameObject(Vector3 grid,Transform obj)
    {
        if (!spatialHash.ContainsKey(grid))
        {
            var objs = new List<Transform>();
            objs.Add(obj);
            spatialHash[grid] = objs;
        }
        else
        {
            spatialHash[grid].Add(obj);
        }
    }

    public void OnRemeveGameObject(Vector3 grid, Transform obj)
    {
        if (spatialHash.ContainsKey(grid) && spatialHash[grid].Contains(obj))
        {
            spatialHash[grid].Remove(obj);
            if (spatialHash[grid].Count <= 0)
            {
                spatialHash.Remove(grid);
            }
        }
    }

    public List<Transform> GetGameObject(Vector3 grid)
    {
        if (spatialHash.ContainsKey(grid))
        {
            return spatialHash[grid];
        }
        return null;
    }
}
