using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Board : MonoBehaviour
{
    public int width;
    public int height;
    public GameObject tilePrefab;
    public GameObject[] cubes;
    public GameObject[,] allCubes;

    // Start is called before the first frame update
    void Start()
    {
        allCubes = new GameObject[width, height];
        SetUp();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    private void SetUp()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                Vector2 tempPosition = new Vector2(i, j);
                GameObject backgroundTile = Instantiate(tilePrefab,tempPosition, Quaternion.identity) as GameObject;
                backgroundTile.transform.parent = this.transform;
                backgroundTile.name = "( " + i + ", " + j + " )";
                
                int cubeToUse = Random.Range(0, cubes.Length);
                GameObject cube = Instantiate(cubes[cubeToUse], tempPosition, Quaternion.identity);
                cube.transform.parent = this.transform;
                cube.name =  "( " + i + ", " + j + " )";
                allCubes[i, j] = cube;
            }
        }
    }
}
