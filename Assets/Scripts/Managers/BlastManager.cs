using System;
using System.Collections;
using System.Collections.Generic;

public class BlastManager
{
    public GridBoard<GameItem> grid;
    public int width = 8;
    public int height = 8;

    void BlastConnectedCubes(GameItem tappedGameItem)
    {
        if (tappedGameItem == null || tappedGameItem.GetItemType() != ItemType.Cube)
            return;

        Cube tappedCube = (Cube)tappedGameItem;
        bool[,] visited = new bool[width, height];
        List<GameItem> connectedCubes = new List<GameItem>();

        FindConnectedCubes(tappedGameItem.x, tappedGameItem.y, tappedCube.cubeType, ref visited, ref connectedCubes);

        if (connectedCubes.Count <= 1)
        {
            return;
        }

        HashSet<GameItem> adjacentItems = new HashSet<GameItem>();
        getAdjacentItems(tappedGameItem.x, tappedGameItem.y, ref adjacentItems);

        if (connectedCubes.Count >= 5)
        {
            //TurnCubesIntoTnt(connectedCubes,tappedGameItem);
        }
        else
        {
            foreach (GameItem item in connectedCubes)
            {
                getAdjacentItems(item.x, item.y, ref adjacentItems);
                item.Clear();
            }
            
            /*
            foreach (GameItem item in connectedCubes)
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
            */

            foreach (var item in adjacentItems)
            {
                //Debug.Log(item);
                item.ClearOnNear();
            }
            GoalManager.Instance.decreaseMoveCount(1);

        }
        
    }
    
    /*
    void TurnCubesIntoTnt(List<GameItem> connectedCubes, GameItem tappedGameItem, ref HashSet<GameItem> adjacentItems)
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
    */
    
    void FindConnectedCubes(int x, int y, CubeType targetType, ref bool[,] visited, ref List<GameItem> connected)
    {
        if (x < 0 || y < 0 || x >= width || y >= height || grid.GetValue(x, y) == null)
        {
            return;
        }
        
        if (connected.Contains(grid.GetValue(x, y)) || visited[x,y])
        {
            return;
        }
        
        GameItem gameItem = grid.GetValue(x, y);

        if (gameItem.GetItemType() == ItemType.Cube)
        {
            Cube cube = (Cube)gameItem;
            if (cube.cubeType == targetType)
            {
                connected.Add(gameItem);
                visited[x, y] = true;
                
                FindConnectedCubes(x + 1, y, targetType, ref visited, ref connected);
                FindConnectedCubes(x - 1, y, targetType, ref visited, ref connected);
                FindConnectedCubes(x, y + 1, targetType, ref visited, ref connected);
                FindConnectedCubes(x, y - 1, targetType, ref visited, ref connected);
            }
        }
    }
    
    List<List<GameItem>> FindAllConnectedCubes()
    {
        List<List<GameItem>> connectedCubesLists = new List<List<GameItem>>();

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

                if (!visited[x, y] && currentGameItem != null && currentGameItem.GetItemType() == ItemType.Cube)
                {
                    List<GameItem> connectedCubes = new List<GameItem>();
                    FindConnectedCubes(x, y, ((Cube)currentGameItem).cubeType, ref visited, ref connectedCubes);
                    connectedCubesLists.Add(connectedCubes);
                }
            }
        }
        return connectedCubesLists;
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
}
