using System.Collections.Generic;
using Pathing;
using UnityEngine;

public class Terrain : MonoBehaviour, IAStarNode
{
    //Properties
    [SerializeField]private MeshRenderer meshRn;

    //Grid related
    public bool canWalk { get; private set; }
    private Grid gridScript;
    private int gridX;
    private int gridY;
    private float travelCost;

    void Start()
    {
        gridScript = Grid.instance;
    }
    
    //------------------------AStarNode Functions--------------------

    public IEnumerable<IAStarNode> Neighbours
    {
        get
        {
            // Offset for odd rows
            int offset = gridY % 2 == 0 ? 0 : 1;

            // Coordinates of potential neighbors
            int[] neighborX = { -1, 1, 0, 0, -1, -1 };
            int[] neighborY = { 0, 0, 1, -1, -1, 1 };

            for (int i = 0; i < neighborX.Length; i++)
            {
                int x;
                if (i > 1)
                    x = gridX + neighborX[i] + offset;
                else
                    x = gridX + neighborX[i];
                int y = gridY + neighborY[i];

                // Check if the coordinates are valid
                if (Grid.IsWithinGridBounds(x, y))
                {
                    // Get the terrain at the specified coordinates
                    Terrain neighborTerrain = (Terrain)gridScript.TerrainPos[(x, y)];

                    if (neighborTerrain != null)
                    {
                        yield return neighborTerrain;
                    }
                }
            }
        }
    }
    public float CostTo(IAStarNode neighbour)
    {
        float hValue = 0;
        Vector3 neighbourPosition = ((MonoBehaviour)neighbour).transform.position;

        hValue = Vector3.Distance(transform.position, neighbourPosition);
        float fValue = travelCost + hValue;

        return fValue;
    }

    public float EstimatedCostTo(IAStarNode goal)
    {
        float hValue = 0;
        Vector3 neighbourPosition = ((MonoBehaviour)goal).transform.position;

        hValue = Vector3.Distance(transform.position, neighbourPosition);

        return hValue;
    }
    
    //------------------------Setting Properties--------------------

    public void SetGridCoordinates(int x, int y)
    {
        gridX = x;
        gridY = y;
    }

    public void SetTravelCost(float cost)
    {
        travelCost = cost;
        canWalk = !float.IsPositiveInfinity(travelCost);
    }
    public void SetMaterial(Material material)
    {
        meshRn.material = material;
    }
    
    public void SetColor(Color color)
    {
        meshRn.material.color = color;
    }
    
}

enum TerrainType
{
    Grass, Desert, Water, Mountain, Forest
}
