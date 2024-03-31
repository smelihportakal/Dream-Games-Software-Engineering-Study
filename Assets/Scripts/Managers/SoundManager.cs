using System.Collections;
using System.Collections.Generic;using System.Security.Claims;
using UnityEngine;

[System.Serializable]
public class AudioClass
{
    public string tag;
    public AudioSource audio;
}

public class SoundManager : MonoBehaviour
{
    public AudioSource boxDestroy;
    public AudioSource stoneDestroy;
    public AudioClass[] audios;
    public Dictionary<string, AudioSource> audioDictionary;

    // Start is called before the first frame update
    void Start()
    {
        audioDictionary = new Dictionary<string, AudioSource>();


        foreach (AudioClass audio in audios)
        {
            audioDictionary.Add(audio.tag, audio.audio);
        }
    }
    public static SoundManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    public void BoxDestroy()
    {
        boxDestroy.Play();
    }
    
    public void StoneDestroy()
    {
        stoneDestroy.Play();
    }

    public void PlayAudio(string tag)
    {
        audioDictionary[tag].Play();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    /*
     *     public GameItem CreateGameItem(string key, Vector3 position)
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
                   return ObjectPooler.Instance.SpawnFromPool(key, position, Quaternion.identity).GetComponent<Box>();
               case "s":
                   return ObjectPooler.Instance.SpawnFromPool(key, position, Quaternion.identity).GetComponent<Stone>();
               case "v":
                   return ObjectPooler.Instance.SpawnFromPool(key, position, Quaternion.identity).GetComponent<Vase>();
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

     */
}
