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
    public GameObject emptyPrefab;
    GridSystem2D<GridObject<CellItem>> grid;
    CubeFactory _cubeFactory;
    //private Empty emptyCell;

    private void Start()
    {
        InitializeGrid();
        FindBombableCubes();
    }

    void InitializeGrid()
    {
        originPosition = GetOriginPosition();
        grid = GridSystem2D<GridObject<CellItem>>.VerticalGrid(width, height, cellSizeX, cellSizeY, originPosition, debug);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                CreateCellItem(x, y);
            }
        }

    }

    public Empty InitEmptyCell()
    {
        GameObject cellItem = Instantiate(emptyPrefab, new Vector3(-5,-5,-5), Quaternion.identity);
        Empty emptyCell = cellItem.GetComponent<Empty>();
        return emptyCell;
    }

    void CreateCellItem(int x, int y)
    {
        //TODO
        int rn = Random.Range(0, cellItemPrefabs.Length);
        GameObject cellItem = Instantiate(cellItemPrefabs[rn], grid.GetWorldPositionCenter(x, y), Quaternion.identity,
            transform);
        //cellItem.SetType(cellItemTypes[rn]);
        var gridObject = new GridObject<CellItem>(grid, x, y);
        gridObject.SetValue(cellItem.GetComponent<Cube>());
        grid.SetValue(x,y,gridObject);
    }
    
    Vector3 GetOriginPosition()
    {
        return new Vector3(transform.position.x - width * cellSizeX / 2.0f,
            transform.position.y - height * cellSizeY / 2.0f, 0);
    }
    
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2Int gridPosition = grid.GetXY(mousePosition);
            DestroyConnectedCubes(gridPosition.x, gridPosition.y);
            CollapseGrid();
            FindBombableCubes();
        }
    }

    void ChangeSprite(int x, int y)
    {
        var tappedGridObject = grid.GetValue(x, y);
        if (tappedGridObject == null)
            return;

        CellItem tappedCellItem = tappedGridObject.GetValue();
   
        if (tappedCellItem.type == ItemType.Cube)
        {
            tappedCellItem.OnTap();
        }
    }
    
    void DestroyConnectedCubes(int x, int y)
    {
        var tappedGridObject = grid.GetValue(x, y);
        if (tappedGridObject == null)
            return;

        CellItem tappedCellItem = tappedGridObject.GetValue();
        if (tappedCellItem == null)
            return;

        if (tappedCellItem.GetItemType() != ItemType.Cube )
        {
            return;
        }

        Cube tappedCube = (Cube)tappedCellItem;
        
        CubeType targetType = tappedCube.cubeType;
        
        bool[,] visited = new bool[width, height];
        
        List<GridObject<CellItem>> connectedCubes = new List<GridObject<CellItem>>();
        
        FindConnectedCubes(x, y, targetType, ref visited, ref connectedCubes);

        if (connectedCubes.Count <= 1)
        {
            return;
        }
        // Destroy connected cubes
        foreach (var gridObject in connectedCubes)
        {
            Destroy(gridObject.GetValue().gameObject);
            grid.SetValue(gridObject.x,gridObject.y,null); // Clear the grid object
        }
        // Handle grid update (optional)
        // Implement score calculation (optional)
    }

    void FindBombableCubes()
    {
        List<List<GridObject<CellItem>>> allConnectedMatches = FindAllConnectedCubes();
        foreach (var connectedMatch in allConnectedMatches)
        {
            if (connectedMatch.Count >= 5)
            {
                foreach (var gridObject in connectedMatch)
                {
                    Cube cube = (Cube)gridObject.GetValue();
                    cube.changeState(1);
                }
            }
            else
            {
                foreach (var gridObject in connectedMatch)
                {
                    ((Cube)gridObject.GetValue()).changeState(0);
                }
            }
        }
    }

    void FindConnectedCubes(int x, int y,CubeType targetType, ref bool[,] visited, ref List<GridObject<CellItem>> connected)
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

        GridObject<CellItem> gridObject = grid.GetValue(x, y);
        
        CellItem cellItem = gridObject.GetValue();

        if (cellItem != null && cellItem.GetItemType() == ItemType.Cube)
        {
            Cube cube = (Cube)cellItem;
            if (cellItem != null && cube.cubeType == targetType)
            {
                connected.Add(gridObject);
                
                // Check adjacent cells recursively
                FindConnectedCubes(x + 1, y, targetType, ref visited, ref connected);
                FindConnectedCubes(x - 1, y, targetType, ref visited, ref connected);
                FindConnectedCubes(x, y + 1, targetType, ref visited, ref connected);
                FindConnectedCubes(x, y - 1, targetType, ref visited, ref connected);
            }
        }

        return;
    }
    
    
    // Method to find all connected cubes
    List<List<GridObject<CellItem>>> FindAllConnectedCubes()
    {
        List<List<GridObject<CellItem>>> connectedCubesLists = new List<List<GridObject<CellItem>>>();

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
                GridObject<CellItem> currentGridObject = grid.GetValue(x, y);

                // If the cell is not visited and contains a cube, start DFS from this cell
                if (!visited[x, y] && currentGridObject != null && currentGridObject.GetValue() != null &&
                    currentGridObject.GetValue().GetItemType() == ItemType.Cube)
                {
                    List<GridObject<CellItem>> connectedCubes = new List<GridObject<CellItem>>();
                    // Perform DFS to find connected cubes
                    FindConnectedCubes(x, y, ((Cube)currentGridObject.GetValue()).cubeType, ref visited, ref connectedCubes);
                    // Add connected cubes to the list
                    connectedCubesLists.Add(connectedCubes);
                }
            }
        }

        return connectedCubesLists;
    }

    private void CollapseGrid()
    {
        for (int x = 0; x < width; x++)
        {
            for (int yEmpty = 0; yEmpty < height; yEmpty++)
            {
                if (grid.GetValue(x, yEmpty) == null || grid.GetValue(x, yEmpty) == default)
                {
                    print(x + " " +yEmpty);
                    for (int yNotEmpty = yEmpty + 1; yNotEmpty < height; yNotEmpty++)
                    {
                        if (grid.GetValue(x, yNotEmpty) != null && grid.GetValue(x, yNotEmpty) != default ) 
                        {
                            Debug.Log("(" + x + "," + yNotEmpty + ") to ("  + x + "," + yEmpty + ")");
                            MoveItemToPosition(grid.GetValue(x, yNotEmpty), x, yEmpty);
                            break;
                        }   
                    }
                }
            }
        }
    }

    private void MoveItemToPosition(GridObject<CellItem> gridObject, int x, int y)
    {
        // remove the Item from its original grid position
        CellItem cellItem = gridObject.GetValue();
        grid.SetValue(gridObject.x,gridObject.y,null);
        // place it the item at its new position
        grid.SetValue(x,y,gridObject);
        gridObject.SetValue(x,y);
        
        // update the matchable's internal grid position

        // start animation to move it on screen
        Debug.Log(grid.GetWorldPositionCenter(x, y));

        StartCoroutine(cellItem.MoveToPosition(grid.GetWorldPositionCenter(x, y)));
    }
    
}
