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

public class GridBoard : MonoBehaviour
{
    [SerializeField] int width = 8;
    [SerializeField] int height = 8;
    [SerializeField] float cellSizeX = 0.71f;
    [SerializeField] float cellSizeY = 0.81f;
    Vector3 originPosition;
    [SerializeField] bool debug = true;
    public GameObject[] cellItemPrefabs;
    public string[] keys;
    [SerializeField] CellItemType[] cellItemTypes;
    public GameObject tntPrefab;
    public GridSystem2D<CellItem> grid;
    CubeFactory _cubeFactory;

    public bool IsAnimationContinue;
    //private Empty emptyCell;

    public TextAsset textJson;

    private ObjectPooler objectPooler;

    private GoalManager goalManager;

    public GameObject gridBackground;


    public List<CubeColor> cubeColors;
    public Dictionary<string, CubeColor> colorDictionary;



    public Level level = new Level();

    public static GridBoard Instance { get; private set; }

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
        goalManager = FindObjectOfType<GoalManager>();
        colorDictionary = new Dictionary<string, CubeColor>();
        foreach (CubeColor cubeColor in cubeColors)
        {
            colorDictionary.Add(cubeColor.colorCode, cubeColor);
        }

        objectPooler = ObjectPooler.Instance;
        level = LevelManager.Instance.getCurrentLevel();
        width = level.grid_width;
        height = level.grid_height;
        goalManager.setMoveCount(level.move_count);
        InitializeGrid();
        FindBombableCubes();
        IsAnimationContinue = false;
    }

    void InitializeGrid()
    {
        originPosition = GetOriginPosition();
        grid = GridSystem2D<CellItem>.VerticalGrid(width, height, cellSizeX, cellSizeY, originPosition, debug);
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
                        int rn = Random.Range(0, 4);
                        GameObject cellItem =
                            objectPooler.SpawnFromPool("cube",
                                grid.GetWorldPositionCenter(x, yempty) + new Vector3(0, 10, 0), Quaternion.identity);
                        grid.SetValue(x, yempty, cellItem.GetComponent<Cube>());
                        ((Cube)grid.GetValue(x, yempty)).ChangeColor(
                            colorDictionary[new string[] { "b", "g", "r", "y" }[rn]]);
                        grid.GetValue(x, yempty).setCoordinate(x, yempty);
                        StartCoroutine(grid.GetValue(x, yempty).MoveToPosition(grid.GetWorldPositionCenter(x, yempty)));
                        yield return null;
                    }
                }
            }
        }

        IsAnimationContinue = false;
        FindBombableCubes();
    }

    void CreateCellItem(int x, int y, string key)
    {
        //TODO
        if (key == "bo")
        {
            GameObject cellItem = Instantiate(cellItemPrefabs[Array.IndexOf(keys, key)],
                grid.GetWorldPositionCenter(x, y), Quaternion.identity,
                transform);
            grid.SetValue(x, y, cellItem.GetComponent<Box>());
            grid.GetValue(x, y).setCoordinate(x, y);
        }
        else if (key == "v")
        {
            GameObject cellItem = Instantiate(cellItemPrefabs[Array.IndexOf(keys, key)],
                grid.GetWorldPositionCenter(x, y), Quaternion.identity,
                transform);
            grid.SetValue(x, y, cellItem.GetComponent<Vase>());
            grid.GetValue(x, y).setCoordinate(x, y);
        }
        else if (key == "s")
        {
            GameObject cellItem = Instantiate(cellItemPrefabs[Array.IndexOf(keys, key)],
                grid.GetWorldPositionCenter(x, y), Quaternion.identity,
                transform);
            grid.SetValue(x, y, cellItem.GetComponent<Stone>());
            grid.GetValue(x, y).setCoordinate(x, y);
        }
        else if (key == "rand")
        {
            int rn = Random.Range(0, 4);
            GameObject cellItem =
                objectPooler.SpawnFromPool("cube", grid.GetWorldPositionCenter(x, y), Quaternion.identity);
            grid.SetValue(x, y, cellItem.GetComponent<Cube>());
            ((Cube)grid.GetValue(x, y)).ChangeColor(colorDictionary[new string[] { "b", "g", "r", "y" }[rn]]);
            grid.GetValue(x, y).setCoordinate(x, y);
        }
        else
        {
            GameObject cellItem =
                objectPooler.SpawnFromPool("cube", grid.GetWorldPositionCenter(x, y), Quaternion.identity);
            //Instantiate(cellItemPrefabs[rn], grid.GetWorldPositionCenter(x, y), Quaternion.identity,
            //transform);
            //cellItem.SetType(cellItemTypes[rn]);
            grid.SetValue(x, y, cellItem.GetComponent<Cube>());
            ((Cube)grid.GetValue(x, y)).ChangeColor(colorDictionary[key]);
            grid.GetValue(x, y).setCoordinate(x, y);
        }
    }

    Vector3 GetOriginPosition()
    {
        return new Vector3(transform.position.x - width * cellSizeX / 2.0f,
            transform.position.y - height * cellSizeY / 2.0f, 0);
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

    void TapObject(int x, int y)
    {
        CellItem tappedCellItem = grid.GetValue(x, y);
        if (tappedCellItem == null)
            return;

        if (tappedCellItem.GetItemType() == ItemType.Cube)
        {
            DestroyConnectedCubes(x, y);
            CollapseGrid();
        }
        else if (tappedCellItem.GetItemType() == ItemType.Bomb)
        {
            tappedCellItem.OnTap();
            goalManager.decreaseMoveCount(1);
        }
    }

    public void TriggerTnT(int posx, int posy, int size)
    {
        for (int x = posx - size; x <= posx + size; x++)
        {
            if (x >= 0 && x < grid.width)
            {
                for (int y = posy - size; y <= posy + size; y++)
                {
                    if (y >= 0 && y < grid.height)
                    {
                        if (grid.GetValue(x, y) != null)
                        {
                            //&& !grid.GetValue(x, y).IsBeingCleared
                            Debug.Log("bomb");
                            grid.GetValue(x, y).Clear();
                            //StartCoroutine(CubeParticleCoroutine(x,y));
                        }
                    }
                }
            }
        }

    }

    public void getAdjacentItems(int x, int y, ref HashSet<CellItem> adjacentItems)
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

    void ChangeSprite(int x, int y)
    {
        CellItem tappedCellItem = grid.GetValue(x, y);
        if (tappedCellItem == null)
            return;

        if (tappedCellItem.type == ItemType.Cube)
        {
            tappedCellItem.OnTap();
        }
    }

    void DestroyConnectedCubes(int x, int y)
    {
        CellItem tappedCellItem = grid.GetValue(x, y);
        if (tappedCellItem == null)
            return;

        if (tappedCellItem.GetItemType() != ItemType.Cube)
        {
            return;
        }

        Cube tappedCube = (Cube)tappedCellItem;

        CubeType targetType = tappedCube.cubeType;

        bool[,] visited = new bool[width, height];

        List<CellItem> connectedCubes = new List<CellItem>();

        FindConnectedCubes(x, y, targetType, ref visited, ref connectedCubes);

        if (connectedCubes.Count <= 1)
        {

            return;
        }

        HashSet<CellItem> adjacentItems = new HashSet<CellItem>();
        getAdjacentItems(tappedCellItem.x, tappedCellItem.y, ref adjacentItems);

        if (connectedCubes.Count >= 5)
        {
            if (connectedCubes.Contains(tappedCellItem))
            {
                connectedCubes.Remove(tappedCellItem);
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

                if (adjacentItems.Contains(tappedCellItem))
                {
                    adjacentItems.Remove(tappedCellItem);
                }

                GameObject cellItem = Instantiate(tntPrefab, grid.GetWorldPositionCenter(x, y), Quaternion.identity,
                    transform);
                //cellItem.SetType(cellItemTypes[rn]);
                ObjectPooler.Instance.ReturnObjectToPool("cube", tappedCellItem.gameObject);
                grid.SetValue(x, y, cellItem.GetComponent<TNT>());
                grid.GetValue(x, y).setCoordinate(x, y);
                foreach (var item in adjacentItems)
                {
                    Debug.Log(item);
                    item.ClearOnNear();
                }
                goalManager.decreaseMoveCount(1);

                return;
            }
            else
            {
                Debug.Log("onject not exist");
            }
        }

        foreach (CellItem item in connectedCubes)
        {
            getAdjacentItems(item.x, item.y, ref adjacentItems);
            item.Clear();
            Debug.Log("why not working");
        }

        foreach (CellItem item in connectedCubes)
        {
            if (adjacentItems.Contains(item))
            {
                adjacentItems.Remove(item);
            }
        }

        if (adjacentItems.Contains(tappedCellItem))
        {
            adjacentItems.Remove(tappedCellItem);
        }

        foreach (var item in adjacentItems)
        {
            Debug.Log(item);
            item.ClearOnNear();
        }
        goalManager.decreaseMoveCount(1);

        // Destroy connected cubes
        // Handle grid update (optional)
        // Implement score calculation (optional)
    }

    void FindBombableCubes()
    {
        List<List<CellItem>> allConnectedMatches = FindAllConnectedCubes();
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

    void FindConnectedCubes(int x, int y, CubeType targetType, ref bool[,] visited, ref List<CellItem> connected)
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

        CellItem cellItem = grid.GetValue(x, y);

        if (cellItem != null && cellItem.GetItemType() == ItemType.Cube)
        {
            Cube cube = (Cube)cellItem;
            if (cellItem != null && cube.cubeType == targetType)
            {
                connected.Add(cellItem);

                // Check adjacent cells recursively
                FindConnectedCubes(x + 1, y, targetType, ref visited, ref connected);
                FindConnectedCubes(x - 1, y, targetType, ref visited, ref connected);
                FindConnectedCubes(x, y + 1, targetType, ref visited, ref connected);
                FindConnectedCubes(x, y - 1, targetType, ref visited, ref connected);
            }
        }
    }


    // Method to find all connected cubes
    List<List<CellItem>> FindAllConnectedCubes()
    {
        List<List<CellItem>> connectedCubesLists = new List<List<CellItem>>();

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
                CellItem currentCellItem = grid.GetValue(x, y);

                // If the cell is not visited and contains a cube, start DFS from this cell
                if (!visited[x, y] && currentCellItem != null && currentCellItem != null &&
                    currentCellItem.GetItemType() == ItemType.Cube)
                {
                    List<CellItem> connectedCubes = new List<CellItem>();
                    // Perform DFS to find connected cubes
                    FindConnectedCubes(x, y, ((Cube)currentCellItem).cubeType, ref visited, ref connectedCubes);
                    // Add connected cubes to the list
                    connectedCubesLists.Add(connectedCubes);
                }
            }
        }

        return connectedCubesLists;
    }

    public void CollapseGrid()
    {
        for (int x = 0; x < width; x++)
        {
            for (int yEmpty = 0; yEmpty < height; yEmpty++)
            {
                if (grid.GetValue(x, yEmpty) == null || grid.GetValue(x, yEmpty) == default)
                {
                    //print(x + " " +yEmpty);
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

    private void MoveItemToPosition(CellItem cellItem, int x, int y)
    {
        // remove the Item from its original grid position
        grid.SetValue(cellItem.x, cellItem.y, null);
        // place it the item at its new position
        grid.SetValue(x, y, cellItem);
        cellItem.setCoordinate(x, y);
        // update the matchable's internal grid position

        // start animation to move it on screen
        //Debug.Log(grid.GetWorldPositionCenter(x, y));

        StartCoroutine(cellItem.MoveToPosition(grid.GetWorldPositionCenter(x, y)));
    }

    public void StartCubeParticle(int x, int y, Material mat)
    {
        StartCoroutine(CubeParticleCoroutine(x, y, mat));
    }

    public IEnumerator CubeParticleCoroutine(int x, int y, Material mat)
    {

        GameObject particles = ObjectPooler.Instance.SpawnFromPool("cube_particle", grid.GetWorldPositionCenter(x, y), Quaternion.identity);
        particles.GetComponent<ParticleSystemRenderer>().material = mat;
        yield return new WaitForSecondsRealtime(1f);
        Debug.Log("Particle deleted");

        ObjectPooler.Instance.ReturnObjectToPool("cube_particle", particles);

    }
    
    public void StartTntParticle(int x, int y, GameObject prefab)
    {
        StartCoroutine(TntParticleCoroutine(x, y, prefab));
    }

    public IEnumerator TntParticleCoroutine(int x, int y, GameObject prefab )
    {

        GameObject particles = Instantiate(prefab, grid.GetWorldPositionCenter(x, y), Quaternion.identity, transform);
        yield return new WaitForSecondsRealtime(1f);
        Debug.Log("Particle deleted");
        Destroy(particles);
    }
}
