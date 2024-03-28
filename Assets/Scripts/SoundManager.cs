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
}
