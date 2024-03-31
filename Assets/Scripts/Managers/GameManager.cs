using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

[System.Serializable]
public class CubeColor
{
    public string colorCode;
    public CubeType cubeType;
    public Sprite normalState;
    public Sprite bombState;
    public Material particleMaterial;
}

public class GameManager : MonoBehaviour
{
    private int width = 8;
    private int height = 8;
    public float cellSizeX = 0.71f;
    public float cellSizeY = 0.81f;
    Vector3 originPosition;
    public GameObject[] cellItemPrefabs;
    public string[] keys; 
    public GameObject tntPrefab;
    public GridBoard<GameItem> grid;
    CubesFactory _cubesFactory;
    //public bool canCollapse = true;
    
    private Dictionary<string, ItemFactory> itemFactories;
    
    public bool IsAnimationContinue;
    //private Empty emptyCell;

    public TextAsset textJson;

    private ObjectPooler objectPooler;

    //private GoalManager goalManager;

    public GameObject gridBackground;


    public List<CubeColor> cubeColors;
    public Dictionary<string, CubeColor> colorDictionary;

    private IEnumerator coCollapseGrid;

    public Level level = new Level();

    public static GameManager Instance { get; private set; }
    public ParticleManager particleManager;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
        colorDictionary = new Dictionary<string, CubeColor>();
        foreach (CubeColor cubeColor in cubeColors)
        {
            colorDictionary.Add(cubeColor.colorCode, cubeColor);
        }
        objectPooler = ObjectPooler.Instance;
        particleManager = ParticleManager.Instance;
        InitializeFactories();
        level = LevelManager.Instance.getCurrentLevel();
        width = level.grid_width;
        height = level.grid_height;
        GoalManager.Instance.setMoveCount(level.move_count);
        InitializeGrid();
        FindBombableCubes();
        IsAnimationContinue = false;
    }
    
    void Update()
    {
        // && !IsAnimationContinue
        if (Input.GetMouseButtonDown(0) )
        {
            IsAnimationContinue = true;
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2Int gridPosition = grid.GetXY(mousePosition);
            TapObject(gridPosition.x, gridPosition.y);
        }
    }

    
    public void clearGridCell(int x, int y)
    {
        grid.SetValue(x,y, null);
    }
    
    public GameItem getItemAtCell(int x, int y)
    {
        return grid.GetValue(x, y);
    }

    public (int,int) getGridSize()
    {
        return (grid.width,grid.height);
    }
    
    
    Vector3 GetOriginPosition()
    {
        return new Vector3(transform.position.x - width * cellSizeX / 2.0f,
            transform.position.y - height * cellSizeY / 2.0f, 0);
    }

    void InitializeFactories()
    {
        itemFactories = new Dictionary<string, ItemFactory>();
        
        itemFactories.Add("bo", new BoxFactory( cellItemPrefabs[4]));
        itemFactories.Add("s", new StoneFactory( cellItemPrefabs[6]));
        itemFactories.Add("v", new VaseFactory( cellItemPrefabs[5]));
        itemFactories.Add("t", new TntFactory(cellItemPrefabs[7]));
        itemFactories.Add("b", new CubeFactory( cellItemPrefabs[0], colorDictionary));
        itemFactories.Add("g", new CubeFactory( cellItemPrefabs[0], colorDictionary));
        itemFactories.Add("y", new CubeFactory( cellItemPrefabs[0], colorDictionary));
        itemFactories.Add("r", new CubeFactory( cellItemPrefabs[0], colorDictionary));
        itemFactories.Add("rand", new CubeFactory( cellItemPrefabs[0], colorDictionary));
    }
    
    void InitializeGrid()
    {
        originPosition = GetOriginPosition();
        grid = GridBoard<GameItem>.Grid(width, height, cellSizeX, cellSizeY, originPosition);
        int i = 0;
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                CreateCellItem(x, y, level.grid[i]);
                i++;
            }
        }
        GameObject background = Instantiate(gridBackground, transform.position + new Vector3(0,0,2), Quaternion.identity);
        background.GetComponent<SpriteRenderer>().size = grid.GetSizeOfGrid();
    }

    public IEnumerator PopulateGrid()
    {
        IsAnimationContinue = true;
        for (int x = 0; x < width; x++)
        for (int y = height - 1; y >= 0; y--)
        {

            if ((grid.GetValue(x, y) != null && grid.GetValue(x, y) != default) || y == 0)
            {
                
                int yempty = y + 1;
                if (y == 0)
                {
                    yempty = 0;
                }

                for (; yempty < height; yempty++)
                {
                    if (grid.GetValue(x, yempty) == null || grid.GetValue(x, yempty) == default)
                    {
                        CreateCellItem(x,yempty,"rand");
                        grid.GetValue(x, yempty).transform.position =
                            grid.GetWorldPositionCenter(x, yempty) + new Vector3(0, 10, 0);
                        /*
                        int rn = Random.Range(0, 4);
                        GameObject cellItem =
                            objectPooler.SpawnFromPool("cube",
                                grid.GetWorldPositionCenter(x, yempty) + new Vector3(0, 10, 0), Quaternion.identity);
                        grid.SetValue(x, yempty, cellItem.GetComponent<Cube>());
                        ((Cube)grid.GetValue(x, yempty)).ChangeColor(
                            colorDictionary[new string[] { "b", "g", "r", "y" }[rn]]);
                        grid.GetValue(x, yempty).setCoordinate(x, yempty);
                        */
                        
                        StartCoroutine(grid.GetValue(x, yempty).MoveToPosition(grid.GetWorldPositionCenter(x, yempty)));
                        yield return null;
                    }
                }
                break;
            }
        }

        IsAnimationContinue = false;
        FindBombableCubes();
    }
    
    public void CollapseGrid()
    {
        for (int x = 0; x < width; x++)
        {
            for (int yEmpty = 0; yEmpty < height; yEmpty++)
            {
                if (grid.GetValue(x, yEmpty) == null || grid.GetValue(x, yEmpty) == default )
                {
                    for (int yNotEmpty = yEmpty + 1; yNotEmpty < height; yNotEmpty++)
                    {
                        if (grid.GetValue(x, yNotEmpty) != null && grid.GetValue(x, yNotEmpty) != default)
                        {
                            if (!grid.GetValue(x, yNotEmpty).moveable)
                            {
                                break;
                            }
                            //Debug.Log("(" + x + "," + yNotEmpty + ") to ("  + x + "," + yEmpty + ")");
                            MoveItemToPosition(grid.GetValue(x, yNotEmpty), x, yEmpty);
                            break;
                        }
                    }
                }
            }
        }
        StartCoroutine(PopulateGrid());
    }

    IEnumerator CollapseAfterDelay()
    {
        yield return new WaitForSeconds(0.25f);
        CollapseGrid();
    }
    
    void CreateCellItem(int x, int y, string key)
    {
        if (itemFactories != null && itemFactories.ContainsKey(key))
        {
            Vector3 position = grid.GetWorldPositionCenter(x, y);
            GameItem gameItem = itemFactories[key].CreateItem(key, position);
            if (gameItem != null)
            {
                grid.SetValue(x, y, gameItem);
                gameItem.setCoordinate(x, y);
            }
            else
            {
                Debug.LogError("Failed to create item for key: " + key);
            }
        }
    }

    void TapObject(int x, int y)
    {
        GameItem tappedGameItem = grid.GetValue(x, y);
        if (tappedGameItem == null)
            return;
        
        Debug.Log(tappedGameItem);

        if (tappedGameItem.GetItemType() == ItemType.Cube)
        {
            tappedGameItem.OnTap();
            CollapseGrid();
        }
        else if (tappedGameItem.GetItemType() == ItemType.Bomb)
        {
            Debug.Log("IsTNT abc");

            tappedGameItem.OnTap();
            GoalManager.Instance.decreaseMoveCount(1);
        }
    }
    
    public void StartCollapseCo()
    {
        coCollapseGrid = CollapseAfterDelay();
        StartCoroutine(coCollapseGrid);
    }

    public void StopCollapseCo()
    {
        if (coCollapseGrid != null)
        {
            StopCoroutine(coCollapseGrid);
        }

    }

    public void getAdjacentItems(int x, int y, ref HashSet<GameItem> adjacentItems)
    {

        if (x >= 0 && y - 1 >= 0)
        {
            if (grid.GetValue(x, y - 1) != null)
            {
                adjacentItems.Add(grid.GetValue(x, y - 1));
            }
        }

        if (x >= 0 && y + 1 < height)
        {
            if (grid.GetValue(x, y + 1) != null)
            {
                adjacentItems.Add(grid.GetValue(x, y + 1));
            }
        }

        if (x - 1 >= 0 && y >= 0)
        {
            if (grid.GetValue(x - 1, y) != null)
            {
                adjacentItems.Add(grid.GetValue(x - 1, y));

            }
        }

        if (x + 1 < width && y < height)
        {
            if (grid.GetValue(x + 1, y) != null)
            {
                adjacentItems.Add(grid.GetValue(x + 1, y));
            }
        }
    }
    
    /*
    void DestroyConnectedCubes(int x, int y)
    {
        GameItem tappedGameItem = grid.GetValue(x, y);
        if (tappedGameItem == null)
            return;

        if (tappedGameItem.GetItemType() != ItemType.Cube)
        {
            return;
        }

        Cube tappedCube = (Cube)tappedGameItem;

        CubeType targetType = tappedCube.cubeType;

        bool[,] visited = new bool[width, height];

        List<GameItem> connectedCubes = new List<GameItem>();

        FindConnectedCubes(x, y, targetType, ref visited, ref connectedCubes);

        if (connectedCubes.Count <= 1)
        {

            return;
        }

        HashSet<GameItem> adjacentItems = new HashSet<GameItem>();
        getAdjacentItems(tappedGameItem.x, tappedGameItem.y, ref adjacentItems);

        if (connectedCubes.Count >= 5)
        {
            if (connectedCubes.Contains(tappedGameItem))
            {
                connectedCubes.Remove(tappedGameItem);
                foreach (var item in connectedCubes)
                {
                    getAdjacentItems(item.x, item.y, ref adjacentItems);
                    grid.SetValue(item.x, item.y, null);
                    StartCoroutine(item.MoveToPositionAndDestroy(grid.GetWorldPositionCenter(x, y), 3));
                }

                foreach (var item in connectedCubes)
                {
                    if (adjacentItems.Contains(item))
                    {
                        adjacentItems.Remove(item);
                    }
                }

                if (adjacentItems.Contains(tappedGameItem))
                {
                    adjacentItems.Remove(tappedGameItem);
                }
                
                ObjectPooler.Instance.ReturnObjectToPool("cube", tappedGameItem.gameObject);
                GameObject cellItem = Instantiate(tntPrefab, grid.GetWorldPositionCenter(x, y), Quaternion.identity,
                    transform);
                //cellItem.SetType(cellItemTypes[rn]);
                grid.SetValue(x, y, cellItem.GetComponent<TNT>());
                grid.GetValue(x, y).setCoordinate(x, y);
                foreach (var item in adjacentItems)
                {
                    //Debug.Log(item);
                    item.ClearOnNear();
                }
                GoalManager.Instance.decreaseMoveCount(1);

                return;
            }
            else
            {
                //Debug.Log("onject not exist");
            }
        }

        foreach (GameItem item in connectedCubes)
        {
            getAdjacentItems(item.x, item.y, ref adjacentItems);
            item.Clear();
            //Debug.Log("why not working");
        }
        
        foreach (var item in adjacentItems)
        {
            //Debug.Log(item);
            item.ClearOnNear();
        }
        GoalManager.Instance.decreaseMoveCount(1);

        // Destroy connected cubes
        // Handle grid update (optional)
        // Implement score calculation (optional)
    }
    */

    void FindBombableCubes()
    {
        List<List<GameItem>> allConnectedMatches = FindAllConnectedCubes();
        foreach (var connectedMatch in allConnectedMatches)
        {
            if (connectedMatch.Count >= 5)
            {
                foreach (var gridObject in connectedMatch)
                {
                    Cube cube = (Cube)gridObject;
                    cube.changeState(1);
                }
            }
            else
            {
                foreach (var gridObject in connectedMatch)
                {
                    ((Cube)gridObject).changeState(0);
                }
            }
        }
    }

    void FindConnectedCubes(int x, int y, CubeType targetType, ref bool[,] visited, ref List<GameItem> connected)
    {
        if (x < 0 || y < 0 || x >= width || y >= height)
        {
            return;
        }

        if (grid.GetValue(x, y) == null)
        {
            return;
        }

        if (connected.Contains(grid.GetValue(x, y)))
        {
            return;
        }

        if (visited[x, y])
        {
            return;
        }

        GameItem gameItem = grid.GetValue(x, y);

        if (gameItem != null && gameItem.GetItemType() == ItemType.Cube)
        {
            Cube cube = (Cube)gameItem;
            if (gameItem != null && cube.cubeType == targetType)
            {
                connected.Add(gameItem);
                visited[x, y] = true;

                // Check adjacent cells recursively
                FindConnectedCubes(x + 1, y, targetType, ref visited, ref connected);
                FindConnectedCubes(x - 1, y, targetType, ref visited, ref connected);
                FindConnectedCubes(x, y + 1, targetType, ref visited, ref connected);
                FindConnectedCubes(x, y - 1, targetType, ref visited, ref connected);
            }
        }
    }

    // Method to find all connected cubes
    List<List<GameItem>> FindAllConnectedCubes()
    {
        List<List<GameItem>> connectedCubesLists = new List<List<GameItem>>();

        // Create a 2D array to keep track of visited cells
        bool[,] visited = new bool[width, height];

        // Iterate through each cell in the grid
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (grid.GetValue(x, y) == null)
                {
                    continue;
                }

                // Get the grid object at the current position
                GameItem currentGameItem = grid.GetValue(x, y);

                // If the cell is not visited and contains a cube, start DFS from this cell
                if (!visited[x, y] && currentGameItem != null && currentGameItem != null &&
                    currentGameItem.GetItemType() == ItemType.Cube)
                {
                    List<GameItem> connectedCubes = new List<GameItem>();
                    // Perform DFS to find connected cubes
                    FindConnectedCubes(x, y, ((Cube)currentGameItem).cubeType, ref visited, ref connectedCubes);
                    // Add connected cubes to the list
                    connectedCubesLists.Add(connectedCubes);
                }
            }
        }

        return connectedCubesLists;
    }
    
    private void MoveItemToPosition(GameItem gameItem, int x, int y)
    {
        // remove the Item from its original grid position
        grid.SetValue(gameItem.x, gameItem.y, null);
        // place it the item at its new position
        grid.SetValue(x, y, gameItem);
        gameItem.setCoordinate(x, y);
        // update the matchable's internal grid position

        // start animation to move it on screen
        //Debug.Log(grid.GetWorldPositionCenter(x, y));

        StartCoroutine(gameItem.MoveToPosition(grid.GetWorldPositionCenter(x, y)));
    }

    public void StartSafeCoroutine(IEnumerator coroutine)
    {
        StartCoroutine(coroutine);
    }
}
