using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneTemplate;
using UnityEngine;

public abstract class ItemFactory 
{
    protected GameObject prefab;
    //public abstract GameItem CreateItem(string key, Vector3 position);
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
        GoalManager.Instance.SetupGoal("bo", prefab.GetComponent<Box>().defaultSprite);
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
        GoalManager.Instance.SetupGoal("s", prefab.GetComponent<Stone>().defaultSprite);
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
        GoalManager.Instance.SetupGoal("v", prefab.GetComponent<Vase>().defaultSprite);
    }

}

public class TntFactory : ItemFactory 
{
    public TntFactory(GameObject prefab)
    {
        this.prefab = prefab;
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