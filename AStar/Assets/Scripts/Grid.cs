using System;
using System.Collections.Generic;
using System.Linq;
using Pathing;
using UnityEngine;
using Random = UnityEngine.Random;

public class Grid : MonoBehaviour
{
    //Grid
    public static int Height = 8;
    public static int Width = 8;
    private float startX;
    private float startY;
    private float hexRadius = 0.55f;
    
    //Terrain
    [SerializeField]private Terrain terrainPrefab;
    [SerializeField]private TerrainType[] terrainTypes;
    [SerializeField]private Material[] terrainMaterials;
    [SerializeField]private float[] travelCosts;
    
    //Dictionaries
    private Dictionary<Material, float> terrainInfo = new Dictionary<Material, float>(); //get the travel cost based on the material of the terrain
    private Dictionary<(int,int), IAStarNode> terrainPos = new Dictionary<(int, int), IAStarNode>(); //get the node based on the pos in the grid
    private Dictionary<GameObject, (int,int)> terrainObj = new Dictionary<GameObject, (int, int) >(); //get the pos in the grid based on the game object
    public IReadOnlyDictionary<(int, int), IAStarNode> TerrainPos => terrainPos;
    public IReadOnlyDictionary<GameObject, (int, int)> TerrainObj => terrainObj;

    //Object Pooling
    private Queue<Terrain> pooledCells = new Queue<Terrain>();
    private Queue<Terrain> activeTerrains = new Queue<Terrain>();
    private int amountToPool = 20;

    public static Grid instance;

    private void Awake()
    {
        if (instance == null) instance = this;
        PoolObjects();
    }

    private void Start()
    {
        travelCosts[^1] = Mathf.Infinity;//water is infinite
        for (int i = 0; i < terrainMaterials.Length; i++)
            terrainInfo.Add(terrainMaterials[i], travelCosts[i]);
    }

    //----------------------Grid--------------------------

    public void CreateNewGrid()
    {
        if (Width == 0) Width = 8;
        if (Height == 0) Height = 8;
        
        DeactivateAllCells();
        
        startX = -(float)Width/2f + hexRadius*2;
        startY = -(float)Height/2f + hexRadius*2;
        
        for (int row = 0; row < Height; row++)
        {
            for (int col = 0; col < Width; col++)
            {
                float xPos = col * Mathf.Sqrt(3) * hexRadius;

                // Offset every other column
                if (row % 2 == 1)
                {
                    xPos += 0.9f * hexRadius;
                }

                xPos += startX;
                float yPos = startY + row * 1.3f * hexRadius;

                Vector3 terrainPosition = new Vector3(xPos,0f, yPos);
                Terrain newTerrain = CreateAndSetUpTerrain(terrainPosition, col, row);
                terrainPos.Add((col,row), newTerrain);
                terrainObj.Add(newTerrain.gameObject,(col,row));
            }
        }
        
        UIManager.instance.SetUpCamera(Width, Height);
    }

    private Terrain CreateAndSetUpTerrain(Vector3 pos, int gridX, int gridY)
    {
        Terrain terrain = ReturnPooledObject();
        terrain.transform.position = pos;
        int order = Array.IndexOf(terrainTypes, GetRandomTerrainType());
        terrain.SetGridCoordinates(gridX,gridY);
        terrain.SetMaterial(terrainMaterials[order]);
        terrain.SetColor(Color.white);
        terrain.SetTravelCost(terrainInfo[terrainMaterials[order]]);
        
        return terrain;
    }
    
    private TerrainType GetRandomTerrainType()
    {
        return terrainTypes[Random.Range(0, terrainTypes.Length)];
    }
    
    public static bool IsWithinGridBounds(int x, int y)
    {
        return x >= 0 && x < Width && y >= 0 && y < Height;
    }


    //----------------------Object Pooling--------------------------

    private void PoolObjects()
    {
        for (int i = 0; i < amountToPool; i++)
        {
            Terrain terrainClone = Instantiate(terrainPrefab, Vector3.zero, Quaternion.identity, transform);
            terrainClone.gameObject.SetActive(false);
            pooledCells.Enqueue(terrainClone);
        }
    }
    private Terrain ReturnPooledObject()
    {
        if (pooledCells.Count == 0)
        {
            PoolObjects();
        }
        Terrain newTerrain = pooledCells.Dequeue();
        activeTerrains.Enqueue(newTerrain);
        newTerrain.gameObject.SetActive(true);
        return newTerrain;
    }
    
    public void DeactivateAllCells()
    {
        if(activeTerrains.Count == 0) return;
        List<Terrain> activatedTerrains = activeTerrains.ToList();
        foreach (var terrain in activatedTerrains)
        {
            terrain.gameObject.SetActive(false);
            pooledCells.Enqueue(terrain);
            activeTerrains.Dequeue();
        }

        terrainPos.Clear();
        terrainObj.Clear();
    }
}
