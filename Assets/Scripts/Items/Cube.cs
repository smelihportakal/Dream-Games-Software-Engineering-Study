using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cube : GameItem
{
    public Sprite normalState;
    public Sprite bombState;
    public CubeType cubeType;
    private int state = 0;
    public GameObject particle;
    public CubeColor currentCubeColor;
    
    public List<CubeColor> cubeColors;
    public Dictionary<string, CubeColor> colorDictionary;
    

    public override void OnTap()
    {
        bool[,] visited = new bool[GameManager.Instance.grid.width, GameManager.Instance.grid.height];
        List<Cube> connectedCubes = new List<Cube>();
        getConnectedCubes(cubeType,ref visited,ref connectedCubes);
        HashSet<GameItem> adjacentItems = new HashSet<GameItem>(); 
        
        if (connectedCubes.Count <= 1)
        {
            return;
        }
        if (connectedCubes.Count >= 5)
        {
            foreach (Cube item in connectedCubes)
            {
                item.getAdjacentItems(ref adjacentItems);
                GameManager.Instance.clearGridCell(item.x,item.y);
                GameManager.Instance.StartSafeCoroutine(item.MoveToPositionAndDestroy(GameManager.Instance.grid.GetWorldPositionCenter(x, y), 3));
            }
            GameObject cellItem = ObjectPooler.Instance.SpawnFromPool("tnt", GameManager.Instance.grid.GetWorldPositionCenter(x, y), Quaternion.identity);
                //Instantiate(GameManager.Instance.tntPrefab, GameManager.Instance.grid.GetWorldPositionCenter(x, y), Quaternion.identity, transform);
            //cellItem.SetType(cellItemTypes[rn]);
            GameManager.Instance.grid.SetValue(x, y, cellItem.GetComponent<TNT>());
            GameManager.Instance.grid.GetValue(x, y).setCoordinate(x, y);
            
            //ObjectPooler.Instance.ReturnObjectToPool("cube", gameObject);
        }
        else
        {
            foreach (Cube item in connectedCubes)
            {
                item.getAdjacentItems(ref adjacentItems);
                item.Clear();
            }

        }
        
        foreach (var item in adjacentItems)
        {
            item.ClearOnNear();
        }
        
        GoalManager.Instance.decreaseMoveCount(1);
    }

    public void getConnectedCubes(CubeType targetType, ref bool[,] visited, ref List<Cube> connected)
    {
        if (connected.Contains(this) || visited[x,y] || cubeType != targetType)
        {
            return;
        }
        connected.Add(this);
        visited[x, y] = true;

        HashSet<GameItem> adjacentItems = new HashSet<GameItem>(); 
        getAdjacentItems(ref adjacentItems);
        foreach (GameItem item in adjacentItems)
        {
            if (item.GetItemType() == ItemType.Cube)
            {
                Cube connectedCube = (Cube)item;
                connectedCube.getConnectedCubes(targetType,ref visited,ref connected);
            }
        }

    }

    public void getAdjacentItems(ref HashSet<GameItem> adjacentItems)
    {
        GridBoard<GameItem> grid = GameManager.Instance.grid;
        //HashSet<GameItem> adjacentItems = new HashSet<GameItem>();
        if (x >= 0 && y - 1 >= 0)
        {
            if (grid.GetValue(x, y - 1) != null )
            {
                adjacentItems.Add(grid.GetValue(x, y - 1));
            }
        }

        if (x >= 0 && y + 1 < grid.height)
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

        if (x + 1 < grid.width && y < grid.height)
        {
            if (grid.GetValue(x + 1, y) != null)
            {
                adjacentItems.Add(grid.GetValue(x + 1, y));
            }
        }
    }

    public void changeState(int state)
    {
        this.state = state;
        if (this.state == 0)
        {
            GetComponent<SpriteRenderer>().sprite = currentCubeColor.normalState;
        }
        else
        {
            GetComponent<SpriteRenderer>().sprite = currentCubeColor.bombState;
        }
    }
    void Update()
    {
  
    }

    public void ChangeColor(CubeColor cubeColor)
    {
        currentCubeColor = cubeColor;
        GetComponent<SpriteRenderer>().sprite = currentCubeColor.normalState;
        cubeType = currentCubeColor.cubeType;
    }

    public override void Clear()
    {
        IsBeingCleared = true;
        GameManager.Instance.grid.SetValue(x,y, null);
        ParticleManager.Instance.StartCubeParticle(x, y, currentCubeColor.particleMaterial);
        //GameManager.Instance.StartCubeParticle(x,y, currentCubeColor.particleMaterial);
        //StartCoroutine(GridBoard.Instance.CubeParticleCoroutine(x,y));
        ObjectPooler.Instance.ReturnObjectToPool("cube", gameObject);
        SoundManager.Instance.BoxDestroy();
        //Debug.Log("are you working");
    }
    
    private IEnumerator ClearCoroutine()
    {

        GameObject particles = ObjectPooler.Instance.SpawnFromPool("cube_particle",
            GameManager.Instance.grid.GetWorldPositionCenter(x, y), Quaternion.identity);
        yield return new WaitForSecondsRealtime(1f);
        //Debug.Log("Particle deleted");
        ObjectPooler.Instance.ReturnObjectToPool("cube",gameObject);

        ObjectPooler.Instance.ReturnObjectToPool("cube_particle", particles);

    }
    
    public override IEnumerator  MoveToPositionAndDestroy(Vector3 targetPosition, float speed)
    {
        Vector3 from = transform.position;
        Vector3 to = targetPosition;
        float howfar = 0;
        do
        {
            howfar += speed * Time.deltaTime;
            if (howfar > 1)
                howfar = 1;
            
            transform.position = Vector3.LerpUnclamped(from, to, Easing(howfar));
            yield return null;
        } while (howfar != 1);

        ObjectPooler.Instance.ReturnObjectToPool("cube",gameObject);
        //Debug.Log("is it working");
        //gameObject.SetActive(false);
        //Destroy(gameObject);
    }

}
