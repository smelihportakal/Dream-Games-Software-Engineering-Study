using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyManager : MonoBehaviour
{
    public static DestroyManager Instance {get; private set; }
    public int width;
    public int height;
    
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
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public List<Vector2> calculateAdjacentLocations(int x, int y)
    {
        List<Vector2> adjacentLocations = new List<Vector2>();
        if (y > 0) adjacentLocations.Add(new Vector2(x,y-1));
        if (x > 0) adjacentLocations.Add(new Vector2(x-1,y));
        if (y < GameManager.Instance.grid.height -1) adjacentLocations.Add(new Vector2(x,y+1));
        if (x < GameManager.Instance.grid.width -1) adjacentLocations.Add(new Vector2(x+1,y-1));

        return adjacentLocations;
    }

    public void DestroyConnectedCubes(int x, int y)
    {
        
    }
    
}
