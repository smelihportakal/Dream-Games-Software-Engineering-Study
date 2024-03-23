using System;
using UnityEngine;

public class GridSystem2D<T> {
    readonly int width;
    readonly int height;
    readonly float cellSizeX;
    readonly float cellSizeY;
    readonly Vector3 origin;
    readonly T[,] gridArray;

    readonly CoordinateConverter coordinateConverter;

    public event Action<int, int, T> OnValueChangeEvent;
    
    public static GridSystem2D<T> VerticalGrid(int width, int height, float cellSizeX, float cellSizeY , Vector3 origin, bool debug = false) {
        return new GridSystem2D<T>(width, height, cellSizeX, cellSizeY, origin, new VerticalConverter(), debug);
    }
    
    public static GridSystem2D<T> HorizontalGrid(int width, int height, float cellSizeX, float cellSizeY , Vector3 origin, bool debug = false) {
        return new GridSystem2D<T>(width, height, cellSizeX ,cellSizeY, origin, new HorizontalConverter(), debug);
    }

    public GridSystem2D(int width, int height, float cellSizeX, float cellSizeY, Vector3 origin, CoordinateConverter coordinateConverter, bool debug) {
        this.width = width;
        this.height = height;
        this.cellSizeX = cellSizeX;
        this.cellSizeY = cellSizeY;
        this.origin = origin;
        this.coordinateConverter = coordinateConverter ?? new VerticalConverter();

        gridArray = new T[width, height];
        
        if (debug) {
            DrawDebugLines();
        }
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
    
    void DrawDebugLines() {
        const float duration = 100f;
        var parent = new GameObject("Debugging");

        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                //CreateWorldText(parent, x + "," + y, GetWorldPositionCenter(x, y), coordinateConverter.Forward);
                Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x, y + 1), Color.white, duration);
                Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x + 1, y), Color.white, duration);
            }
        }

        Debug.DrawLine(GetWorldPosition(0, height), GetWorldPosition(width, height), Color.white, duration);
        Debug.DrawLine(GetWorldPosition(width, 0), GetWorldPosition(width, height), Color.white, duration);
    }
    
    public abstract class CoordinateConverter {
        public abstract Vector3 GridToWorld(int x, int y, float cellSizeX, float cellSizeY, Vector3 origin);
        
        public abstract Vector3 GridToWorldCenter(int x, int y, float cellSizeX, float cellSizeY, Vector3 origin);

        public abstract Vector2Int WorldToGrid(Vector3 worldPosition, float cellSizeX, float cellSizeY, Vector3 origin);

        public abstract Vector3 Forward { get; }
    }
    
    /// <summary>
    /// A coordinate converter for vertical grids, where the grid lies on the X-Y plane.
    /// </summary>
    public class VerticalConverter : CoordinateConverter {
        public override Vector3 GridToWorld(int x, int y, float cellSizeX, float cellSizeY, Vector3 origin) {
            return new Vector3(x * cellSizeX + origin.x, y * cellSizeY + origin.y, origin.z);
        }
        
        public override Vector3 GridToWorldCenter(int x, int y, float cellSizeX, float cellSizeY, Vector3 origin) {
            return new Vector3(x * cellSizeX + cellSizeX * 0.5f, y * cellSizeY + cellSizeY * 0.5f, 0) + origin;
        }

        public override Vector2Int WorldToGrid(Vector3 worldPosition, float cellSizeX, float cellSizeY, Vector3 origin) {
            Vector3 gridPosition =  new Vector3((worldPosition.x - origin.x)/cellSizeX , (worldPosition.y - origin.y)/cellSizeY, 0 );
            var x = Mathf.FloorToInt(gridPosition.x);
            var y = Mathf.FloorToInt(gridPosition.y);
            return new Vector2Int(x, y);
        }
    
        public override Vector3 Forward => Vector3.forward;
    }
    
    /// <summary>
    /// A coordinate converter for horizontal grids, where the grid lies on the X-Z plane.
    /// </summary>
    public class HorizontalConverter : CoordinateConverter {
        public override Vector3 GridToWorldCenter(int x, int y, float cellSizeX, float cellSizeY, Vector3 origin) {
            return new Vector3(x * cellSizeX + cellSizeX * 0.5f, 0, y * cellSizeY + cellSizeY * 0.5f) + origin;
        }

        public override Vector3 GridToWorld(int x, int y, float cellSizeX, float cellSizeY, Vector3 origin) {
            return new Vector3(x * cellSizeX + origin.x, origin.y, y * cellSizeY + origin.z);
        }

        public override Vector2Int WorldToGrid(Vector3 worldPosition, float cellSizeX, float cellSizeY, Vector3 origin) {
            Vector3 gridPosition = new Vector3((worldPosition.x - origin.x)/cellSizeX , (worldPosition.y - origin.y)/cellSizeY, 0 );
            var x = Mathf.FloorToInt(gridPosition.x);
            var y = Mathf.FloorToInt(gridPosition.z);
            return new Vector2Int(x, y);
        }
    
        public override Vector3 Forward => -Vector3.up;
    }
}
