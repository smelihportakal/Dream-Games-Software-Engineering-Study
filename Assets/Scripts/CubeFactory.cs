using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum CubeType
{
    Yellow,
    Red,
    Blue,
    Green,
    Any,
    Count
};

public class CubeFactory : MonoBehaviour
{
    [SerializeField] private CubeSprite[] _cubeSprites;

    [System.Serializable]
    public struct CubeSprite
    {
        public CubeType type;
        public Sprite sprite;
        public Sprite spriteTNT;
    }
    
    public CellItem CreateCube(string color)
    {
        switch (color)
        {
            case "r":
                return new Cube(type: ItemType.Cube, _cubeSprites[0].sprite, _cubeSprites[0].spriteTNT, _cubeSprites[0].type);
            case "g":
                return new Cube(type: ItemType.Cube, _cubeSprites[0].sprite, _cubeSprites[0].spriteTNT, _cubeSprites[0].type);
            case "b":
                return new Cube(type: ItemType.Cube, _cubeSprites[0].sprite, _cubeSprites[0].spriteTNT, _cubeSprites[0].type);
            case "y":
                return new Cube(type: ItemType.Cube, _cubeSprites[0].sprite, _cubeSprites[0].spriteTNT, _cubeSprites[0].type);
            default:
                return new Cube(type: ItemType.Cube, _cubeSprites[0].sprite, _cubeSprites[0].spriteTNT, _cubeSprites[0].type);
        }
    } 
}
