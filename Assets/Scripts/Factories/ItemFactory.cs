using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneTemplate;
using UnityEngine;

public abstract class ItemFactory 
{
    protected GameObject prefab;

    public virtual void setGoal()
    {
        
    }
    
    public virtual GameItem CreateItem(string key, Vector3 position)
    {
        GameObject itemObject = GameObject.Instantiate(prefab, position, Quaternion.identity);
        GameItem item = itemObject.GetComponent<GameItem>();
        if (item != null)
        {
            setGoal();
            return item;
        }
        else
        {
            return null;
        }
    }

}

public class BoxFactory : ItemFactory 
{
    public BoxFactory(GameObject prefab)
    {
        this.prefab = prefab;
    }

    public override void setGoal()
    {
        EndGameManager.Instance.SetupGoal("bo", prefab.GetComponent<Box>().defaultSprite);
    }
    
    public override GameItem CreateItem(string key, Vector3 position)
    {
        setGoal();
        GameObject itemObject = ObjectPooler.Instance.SpawnFromPool("bo", position, Quaternion.identity);
        Box item = itemObject.GetComponent<Box>();
        return item;
    }
}

public class StoneFactory : ItemFactory
{
    public StoneFactory(GameObject prefab)
    {
        this.prefab = prefab;
    }

    public override void setGoal()
    {
        EndGameManager.Instance.SetupGoal("s", prefab.GetComponent<Stone>().defaultSprite);
    }
    
    public override GameItem CreateItem(string key, Vector3 position)
    {
        setGoal();
        GameObject itemObject = ObjectPooler.Instance.SpawnFromPool("s", position, Quaternion.identity);
        Stone item = itemObject.GetComponent<Stone>();
        return item;
    }
}

public class VaseFactory : ItemFactory 
{
    public VaseFactory(GameObject prefab)
    {
        this.prefab = prefab;
    }
    
    public override void setGoal()
    {
        EndGameManager.Instance.SetupGoal("v", prefab.GetComponent<Vase>().defaultSprite);
    }
    
    public override GameItem CreateItem(string key, Vector3 position)
    {
        setGoal();
        GameObject itemObject = ObjectPooler.Instance.SpawnFromPool("v", position, Quaternion.identity);
        Vase item = itemObject.GetComponent<Vase>();
        return item;
    }
    

}

public class TntFactory : ItemFactory 
{
    public TntFactory(GameObject prefab)
    {
        this.prefab = prefab;
    }
    
    public override GameItem CreateItem(string key, Vector3 position)
    {
        GameObject itemObject = ObjectPooler.Instance.SpawnFromPool("tnt", position + new Vector3(0,0,-3), Quaternion.identity);
        TNT item = itemObject.GetComponent<TNT>();
        return item;
    }
}

public class CubeFactory : ItemFactory 
{
    public Dictionary<string, CubeColor> colorDictionary;

    public CubeFactory(GameObject prefab, Dictionary<string, CubeColor> colorDictionary)
    {
        this.prefab = prefab;
        this.colorDictionary = colorDictionary;
    }

    public override GameItem CreateItem(string key, Vector3 position)
    {
        GameObject itemObject = ObjectPooler.Instance.SpawnFromPool("cube", position, Quaternion.identity);
        Cube item = itemObject.GetComponent<Cube>();
        switch (key)
        {
            case "rand":
                int rn = Random.Range(0, 4);
                item.ChangeColor(colorDictionary[new string[] { "b", "g", "r", "y" }[rn]]);
                return item;
            default:
                item.ChangeColor(colorDictionary[key]);
                return item;
        }
    }
}