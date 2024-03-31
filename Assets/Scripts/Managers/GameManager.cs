using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameManager : MonoBehaviour
{
    private int width = 8;
    private int height = 8;
    public float cellSizeX = 0.71f;
    public float cellSizeY = 0.81f;
    private Vector3 originPosition;
    public GameObject[] cellItemPrefabs;
    public GridBoard<GameItem> grid;
    private Dictionary<string, ItemFactory> itemFactories;
    public GameObject gridBackground;
    public List<CubeColor> cubeColors;
    public Dictionary<string, CubeColor> colorDictionary;
    private IEnumerator coCollapseGrid;
    private Level level;
    public bool IsTapEnabled = true;

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
        particleManager = ParticleManager.Instance;
        InitializeFactories();
        level = LevelManager.Instance.getCurrentLevel();
        width = level.grid_width;
        height = level.grid_height;
        EndGameManager.Instance.setMoveCount(level.move_count);
        InitializeGrid();
        FindBombableCubes();
    }
    
    void Update()
    {
        // && !IsAnimationContinue
        if (Input.GetMouseButtonDown(0) && IsTapEnabled)
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2Int gridPosition = grid.GetXY(mousePosition);
            TapObject(gridPosition.x, gridPosition.y);
        }
    }

    public void GameFinished()
    {
        IsTapEnabled = false;
    }
    
    void TapObject(int x, int y)
    {
        GameItem tappedGameItem = grid.GetValue(x, y);
        if (tappedGameItem == null)
            return;
        tappedGameItem.OnTap();
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
                        StartCoroutine(grid.GetValue(x, yempty).MoveToPosition(grid.GetWorldPositionCenter(x, yempty)));
                        yield return null;
                    }
                }
                break;
            }
        }

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
    
    void FindBombableCubes()
    {
        List<List<Cube>> allConnectedCubes = FindAllConnectedCubes();
        foreach (List<Cube> connectedCube in allConnectedCubes)
        {
            if (connectedCube.Count >= 5)
            {
                foreach (Cube cube in connectedCube)
                {
                    cube.ChangeState(1);
                }
            }
            else
            {
                foreach (Cube cube in connectedCube)
                {
                    cube.ChangeState(0);
                }
            }
        }
    }
    
    List<List<Cube>> FindAllConnectedCubes()
    {
        List<List<Cube>> connectedCubesLists = new List<List<Cube>>();
        bool[,] visited = new bool[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (grid.GetValue(x, y) == null)
                {
                    continue;
                }

                GameItem currentGameItem = grid.GetValue(x, y);

                if (!visited[x, y] && currentGameItem != null  && currentGameItem.GetItemType() == ItemType.Cube)
                {
                    List<Cube> connectedCubes = new List<Cube>();
                    Cube currentCube = (Cube)currentGameItem;
                    currentCube.FindConnectedCubes(currentCube.cubeType, ref visited, ref connectedCubes);
                    connectedCubesLists.Add(connectedCubes);
                }
            }
        }

        return connectedCubesLists;
    }
    
    private void MoveItemToPosition(GameItem gameItem, int x, int y)
    {
        grid.SetValue(gameItem.x, gameItem.y, null);
        grid.SetValue(x, y, gameItem);
        gameItem.setCoordinate(x, y);

        StartCoroutine(gameItem.MoveToPosition(grid.GetWorldPositionCenter(x, y)));
    }

    public void StartSafeCoroutine(IEnumerator coroutine)
    {
        StartCoroutine(coroutine);
    }
}
