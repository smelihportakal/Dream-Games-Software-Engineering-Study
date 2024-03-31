using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class CubeColor
{
    public string colorCode;
    public CubeType cubeType;
    public Sprite normalState;
    public Sprite bombState;
    public Material particleMaterial;
}

public enum CubeType
{
    Yellow,
    Red,
    Blue,
    Green
};

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
        FindConnectedCubes(cubeType,ref visited,ref connectedCubes);
        HashSet<GameItem> adjacentItems = new HashSet<GameItem>(); 
        
        if (connectedCubes.Count <= 1)
        {
            GetComponent<Animator>().Play("Cube Shake",0);
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
            SoundManager.Instance.PlayAudio("pop");
            GameObject cellItem = ObjectPooler.Instance.SpawnFromPool("tnt", GameManager.Instance.grid.GetWorldPositionCenter(x, y) + new Vector3(0,0,-3), Quaternion.identity);
            GameManager.Instance.grid.SetValue(x, y, cellItem.GetComponent<TNT>());
            GameManager.Instance.grid.GetValue(x, y).setCoordinate(x, y);
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
        
        GameManager.Instance.CollapseGrid();
        EndGameManager.Instance.decreaseMoveCount(1);
    }

    public void FindConnectedCubes(CubeType targetType, ref bool[,] visited, ref List<Cube> connected)
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
                connectedCube.FindConnectedCubes(targetType,ref visited,ref connected);
            }
        }
    }
    
    public void ChangeState(int state)
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

    public void ChangeColor(CubeColor cubeColor)
    {
        currentCubeColor = cubeColor;
        GetComponent<SpriteRenderer>().sprite = currentCubeColor.normalState;
        cubeType = currentCubeColor.cubeType;
    }

    public override void Clear()
    {
        GameManager.Instance.grid.SetValue(x,y, null);
        ParticleManager.Instance.StartCubeParticle(x, y, currentCubeColor.particleMaterial);
        ObjectPooler.Instance.ReturnObjectToPool("cube", gameObject);
        SoundManager.Instance.PlayAudio("cube");
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
    }

    public override void Recycle()
    {
        ObjectPooler.Instance.ReturnObjectToPool("cube", gameObject);
    }
}
