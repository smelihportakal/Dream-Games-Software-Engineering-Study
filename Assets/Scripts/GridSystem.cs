using System;
using System.Drawing;
using UnityEngine;
using Color = UnityEngine.Color;

public class GridBoard<T> {
    public int width;
    public int height;
    readonly float cellSizeX;
    readonly float cellSizeY;
    readonly Vector3 origin;
    readonly T[,] gridArray;

    readonly CoordinateConverter coordinateConverter;

    public event Action<int, int, T> OnValueChangeEvent;
    
    public static GridBoard<T> Grid(int width, int height, float cellSizeX, float cellSizeY , Vector3 origin, bool debug = false) {
        return new GridBoard<T>(width, height, cellSizeX, cellSizeY, origin, new CoordinateConverter());
    }
    
    public GridBoard(int width, int height, float cellSizeX, float cellSizeY, Vector3 origin, CoordinateConverter coordinateConverter) {
        this.width = width;
        this.height = height;
        this.cellSizeX = cellSizeX;
        this.cellSizeY = cellSizeY;
        this.origin = origin;
        this.coordinateConverter = coordinateConverter ?? new CoordinateConverter();

        gridArray = new T[width, height];
    }
    
    // Set a value from a grid position
    public void SetValue(Vector3 worldPosition, T value) {
        Vector2Int pos = coordinateConverter.WorldToGrid(worldPosition, cellSizeX, cellSizeY,origin);
        SetValue(pos.x, pos.y, value);
    }

    public void SetValue(int x, int y, T value) {
        if (IsValid(x, y)) {
            gridArray[x, y] = value;
            OnValueChangeEvent?.Invoke(x, y, value);
        }
    }
    
    // Get a value from a grid position
    public T GetValue(Vector3 worldPosition) {
        Vector2Int pos = GetXY(worldPosition);
        return GetValue(pos.x, pos.y);
    }

    public T GetValue(int x, int y) {
        return IsValid(x, y) ? gridArray[x, y] : default;
    }

    bool IsValid(int x, int y) => x >= 0 && y >= 0 && x < width && y < height;
    
    public Vector2Int GetXY(Vector3 worldPosition) => coordinateConverter.WorldToGrid(worldPosition, cellSizeX, cellSizeY, origin);
    
    public Vector3 GetWorldPositionCenter(int x, int y) => coordinateConverter.GridToWorldCenter(x, y, cellSizeX, cellSizeY, origin);

    Vector3 GetWorldPosition(int x, int y) => coordinateConverter.GridToWorld(x, y, cellSizeX, cellSizeY, origin);
    
    public Vector2 GetSizeOfGrid()
    {
        return new Vector2(width * cellSizeX + 0.35f, height * cellSizeY + 0.35f);
    }
    
    public abstract class CoordinateConverter2 {
        public abstract Vector3 GridToWorld(int x, int y, float cellSizeX, float cellSizeY, Vector3 origin);
        
        public abstract Vector3 GridToWorldCenter(int x, int y, float cellSizeX, float cellSizeY, Vector3 origin);

        public abstract Vector2Int WorldToGrid(Vector3 worldPosition, float cellSizeX, float cellSizeY, Vector3 origin);

        public abstract Vector3 Forward { get; }
    }
    
    /// <summary>
    /// A coordinate converter for vertical grids, where the grid lies on the X-Y plane.
    /// </summary>
    public class CoordinateConverter  {
        public  Vector3 GridToWorld(int x, int y, float cellSizeX, float cellSizeY, Vector3 origin) {
            return new Vector3(x * cellSizeX + origin.x, y * cellSizeY + origin.y, origin.z);
        }
        
        public  Vector3 GridToWorldCenter(int x, int y, float cellSizeX, float cellSizeY, Vector3 origin) {
            return new Vector3(x * cellSizeX + cellSizeX * 0.5f, y * cellSizeY + cellSizeY * 0.5f, 0) + origin;
        }

        public  Vector2Int WorldToGrid(Vector3 worldPosition, float cellSizeX, float cellSizeY, Vector3 origin) {
            Vector3 gridPosition =  new Vector3((worldPosition.x - origin.x)/cellSizeX , (worldPosition.y - origin.y)/cellSizeY, 0 );
            var x = Mathf.FloorToInt(gridPosition.x);
            var y = Mathf.FloorToInt(gridPosition.y);
            return new Vector2Int(x, y);
        }
    
        public  Vector3 Forward => Vector3.forward;
    }
}
