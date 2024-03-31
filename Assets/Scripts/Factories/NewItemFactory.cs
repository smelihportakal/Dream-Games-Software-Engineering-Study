using System.Collections.Generic;
using UnityEditor.SceneTemplate;
using UnityEngine;

public class NewItemFactory 
{
    public Dictionary<string, CubeColor> colorDictionary;

    public NewItemFactory( Dictionary<string, CubeColor> colorDictionary)
    {
        this.colorDictionary = colorDictionary;
    }

    public GameItem CreateGameItem(string key, Vector3 position)
    {
        switch (key)
        {
            case "b":
                return CreateCube(key, position);
            //GameObject itemObject = ObjectPooler.Instance.SpawnFromPool("cube", position, Quaternion.identity);
            //Cube item = itemObject.GetComponent<Cube>();
            case "g":
                return CreateCube(key, position);
            case "y":
                return CreateCube(key, position);
            case "r":
                return CreateCube(key, position);
            case "rand":
                return CreateCube(key, position);
            case "t":
                return ObjectPooler.Instance.SpawnFromPool("tnt", position, Quaternion.identity).GetComponent<TNT>();
            case "bo":
                Box boxItem = ObjectPooler.Instance.SpawnFromPool(key, position, Quaternion.identity).GetComponent<Box>();
                EndGameManager.Instance.SetupGoal("bo", boxItem.defaultSprite);
                return boxItem;
            case "s":
                Stone stoneItem = ObjectPooler.Instance.SpawnFromPool(key, position, Quaternion.identity).GetComponent<Stone>();
                EndGameManager.Instance.SetupGoal("s", stoneItem.defaultSprite);
                return stoneItem;
            case "v":
                Vase vaseItem = ObjectPooler.Instance.SpawnFromPool(key, position, Quaternion.identity).GetComponent<Vase>();
                EndGameManager.Instance.SetupGoal("v", vaseItem.defaultSprite);
                return vaseItem;
            default:
                return ObjectPooler.Instance.SpawnFromPool(key, position, Quaternion.identity).GetComponent<GameItem>();
        }
    }
    
    public Cube CreateCube(string key, Vector3 position)
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