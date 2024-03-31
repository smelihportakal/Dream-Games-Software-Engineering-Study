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

public class CubesFactory : MonoBehaviour
{
    [SerializeField] private CubeSprite[] _cubeSprites;

    [System.Serializable]
    public struct CubeSprite
    {
        public CubeType type;
        public Sprite sprite;
        public Sprite spriteTNT;
    }
}
