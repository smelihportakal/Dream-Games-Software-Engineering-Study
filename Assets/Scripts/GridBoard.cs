using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;


public class GridBoard : MonoBehaviour 
{
    [SerializeField] int width = 8;
    [SerializeField] int height = 8;
    [SerializeField] float cellSizeX = 0.71f;
    [SerializeField] float cellSizeY = 0.81f;
    Vector3 originPosition;
    [SerializeField] bool debug = true;

    [SerializeField] GameObject[] cellItemPrefabs; 
    [SerializeField] CellItemType[] cellItemTypes;
    public GameObject tntPrefab;
    public GridSystem2D<CellItem> grid;
    CubeFactory _cubeFactory;

    public bool IsAnimationContinue;
    //private Empty emptyCell;

    public static GridBoard Instance {get; private set; }
    
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
        InitializeGrid();
        FindBombableCubes();
        IsAnimationContinue = false;
    }

    void InitializeGrid()
    {
        originPosition = GetOriginPosition();
        grid = GridSystem2D<CellItem>.VerticalGrid(width, height, cellSizeX, cellSizeY, originPosition, debug);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                CreateCellItem(x, y);
            }
        }

    }

    public IEnumerator PopulateGrid()
    {
        IsAnimationContinue = true;
        for (int y= 0; y <height; y++)
            for (int x = 0; x < width; x++)
            {
                if (grid.GetValue(x, y) == null || grid.GetValue(x, y) == default) {
                    int rn = Random.Range(0, cellItemPrefabs.Length);
                    GameObject cellObject = Instantiate(cellItemPrefabs[rn], grid.GetWorldPositionCenter(x, y) + new Vector3(0,10, 0 ), Quaternion.identity,
                        transform);
                    //cellItem.SetType(cellItemTypes[rn]);
                    grid.SetValue(x,y,cellObject.GetComponent<Cube>());
                    grid.GetValue(x,y).setCoordinate(x,y);
                    StartCoroutine(grid.GetValue(x,y).MoveToPosition(grid.GetWorldPositionCenter(x, y)));
                    yield return null;
                }
            }
        IsAnimationContinue = false;
        FindBombableCubes();
    }

    void CreateCellItem(int x, int y)
    {
        //TODO
        int rn = Random.Range(0, cellItemPrefabs.Length);
        GameObject cellItem = Instantiate(cellItemPrefabs[rn], grid.GetWorldPositionCenter(x, y), Quaternion.identity,
            transform);
        //cellItem.SetType(cellItemTypes[rn]);
        grid.SetValue(x,y,cellItem.GetComponent<Cube>());
        grid.GetValue(x,y).setCoordinate(x,y);
    }
    
    Vector3 GetOriginPosition()
    {
        return new Vector3(transform.position.x - width * cellSizeX / 2.0f,
            transform.position.y - height * cellSizeY / 2.0f, 0);
    }
    
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !IsAnimationContinue)
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

        if (tappedCellItem.GetItemType() == ItemType.Cube )
        {
            DestroyConnectedCubes(x,y);
            CollapseGrid();
        } else if (tappedCellItem.GetItemType() == ItemType.Bomb)
        {
            tappedCellItem.OnTap();
        }
    }
    
    public void TriggerTnT(int posx,int posy, int size)
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
                            Debug.Log("bomb");
                            grid.GetValue(x, y).Clear();
                        }
                    }
                }
            }
        }

    }

    public List<CellItem> getAdjacentItems(int x, int y)
    {
        List<CellItem> cellItems = new List<CellItem>();

        if (x>= 0 && y-1 >= 0)
        {
            cellItems.Add(grid.GetValue(x,y-1));
        }
        if (x >= 0 && y + 1 < height)
        {
            cellItems.Add(grid.GetValue(x,y+1));
        }
        if (x - 1 >= 0 && y >= 0)
        {
            cellItems.Add(grid.GetValue(x-1,y));
        } 
        if (x + 1 < width && y  < height)
        {
            cellItems.Add(grid.GetValue(x+1,y));
        }
        
        return cellItems;
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
        
        if (tappedCellItem.GetItemType() != ItemType.Cube )
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
        
        if (connectedCubes.Count >= 5)
        {
            if (connectedCubes.Contains(tappedCellItem))
            {
                connectedCubes.Remove(tappedCellItem);
                GameObject cellItem = Instantiate(tntPrefab, grid.GetWorldPositionCenter(x, y), Quaternion.identity,
                    transform);
                //cellItem.SetType(cellItemTypes[rn]);
                Destroy(tappedCellItem.gameObject); //TODO
                grid.SetValue(x,y, cellItem.GetComponent<TNT>());
                grid.GetValue(x,y).setCoordinate(x,y);
            }
            else
            {
                Debug.Log("onject not exist");
            }
            
        }
        
        // Destroy connected cubes
        foreach (var item in connectedCubes)
        {
            item.Clear();
        }
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

    void FindConnectedCubes(int x, int y,CubeType targetType, ref bool[,] visited, ref List<CellItem> connected)
    {
        if (x < 0 || y < 0 || x >= width || y >= height)
        {
            return;
        }

        if (grid.GetValue(x,y) == null)
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
                if (grid.GetValue(x,y) == null)
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
                        if (grid.GetValue(x, yNotEmpty) != null && grid.GetValue(x, yNotEmpty) != default ) 
                        {
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
        grid.SetValue(cellItem.x,cellItem.y,null);
        // place it the item at its new position
        grid.SetValue(x,y,cellItem);
        cellItem.setCoordinate(x,y);
        // update the matchable's internal grid position

        // start animation to move it on screen
        //Debug.Log(grid.GetWorldPositionCenter(x, y));

        StartCoroutine(cellItem.MoveToPosition(grid.GetWorldPositionCenter(x, y)));
    }
    
}
