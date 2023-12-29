using System.Collections.Generic;
using Pathing;
using UnityEngine;

public class Manager : MonoBehaviour
{
    //Visualize
    [SerializeField] private Color endPointCol;
    [SerializeField] private Color pathCol;
    [SerializeField] private Color wrongCol;
    [SerializeField] private LayerMask terrainMask;
    
    //Pathing
    private IList<IAStarNode> pathList;
    private Grid gridScript;
    private IAStarNode startNode;
    private IAStarNode endNode;
    private bool pathStarted;
    
    //Other
    private Camera cam;
    
    private void Start()
    {
        gridScript = Grid.instance;
        cam = Camera.main;
    }

    private void Update()
    {
       PlayerInput(); 
    }
    
    //----------------------------Player Input-----------------------------

    /// <summary>
    /// Checks for where the player clicked on the grid and assign a start and end point for the A*
    /// </summary>
    private void PlayerInput()
    {
        if (!Input.GetMouseButtonDown(0)) return;
        
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, terrainMask))
        {
            (int,int) terrainPosInGrid = gridScript.TerrainObj[hit.collider.gameObject];

            if (!pathStarted)
            {
                if(startNode != null || endNode != null) ResetVisuals();
                
                startNode = gridScript.TerrainPos[terrainPosInGrid];

                if (((Terrain)startNode).canWalk)
                {
                    (startNode as Terrain)?.SetColor(endPointCol);
                    pathStarted = true;
                }
                else
                {
                    (startNode as Terrain)?.SetColor(wrongCol);
                    pathStarted = false;
                }
                
            }
            else
            {
                //if previous was an un-walkable terrain reset its col
                if(endNode !=null && !((Terrain)endNode).canWalk) ResetEndNodeVisual();
                
                endNode = gridScript.TerrainPos[terrainPosInGrid];
                
                if (((Terrain)endNode).canWalk)
                {
                    pathStarted = false;
                    pathList = AStar.GetPath(startNode, endNode); 
                    VisualizePath(pathList);
                }
                else
                {
                    (endNode as Terrain)?.SetColor(wrongCol);
                    pathStarted = true;
                }
            }
        }
    }

    //----------------------------Visuals-----------------------------
    private void ResetVisuals()
    {
        (startNode as Terrain)?.SetColor(Color.white);
        
        foreach (var terrainNode in pathList)
        {
            (terrainNode as Terrain)?.SetColor(Color.white);
        }
    }

    private void ResetEndNodeVisual()
    {
        (endNode as Terrain)?.SetColor(Color.white);
    }

    private void VisualizePath(IList<IAStarNode> pathList)
    {
        foreach (var terrainNode in pathList)
        {
            if(((Terrain)terrainNode).canWalk)
                (terrainNode as Terrain)?.SetColor(pathCol);
            else //stop the path if it's un-walkable
            {
                (terrainNode as Terrain)?.SetColor(wrongCol);
                return;
            }
        }

        (startNode as Terrain)?.SetColor(endPointCol);
        (endNode as Terrain)?.SetColor(endPointCol);
    }
}
