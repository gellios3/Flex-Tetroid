using UnityEngine;
using UnityEngine.Tilemaps;

public class GridManager : MonoBehaviour
{
    public const int GridWidth = 10;
    private const int GridHeight = 20;

    /// <summary>
    /// Get transform at grid position
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    public TileBase GetTransformAtGridPosition(Vector2 pos)
    {
        var tileMap = transform.GetComponent<Tilemap>();
        return pos.y > GridHeight - 1 ? null : tileMap.GetTile(new Vector3Int((int) pos.x, (int) pos.y, 0));
    }

    /// <summary>
    /// Delete row
    /// </summary>
    public void DeleteRow()
    {
        for (var y = 0; y < GridHeight; ++y)
        {
            if (!IsFullRowAt(y)) continue;
            DeleteMinoAt(y);
            MoveAllRowsDown(y + 1);
            --y;
        }
    }

    /// <summary>
    /// Check is above grid 
    /// </summary>
    /// <param name="tetrominoManager"></param>
    /// <returns></returns>
    public static bool CheckIsAboveGrid(TetrominoManager tetrominoManager)
    {
        for (var x = 0; x < GridWidth; ++x)
        {
            foreach (Transform mino in tetrominoManager.transform)
            {
                var pos = Game.RoundPosition(mino.position);

                if (pos.y > GridHeight - 1)
                {
                    return true;
                }
            }
        }

        return false;
    }


    public void UpdateTileMap(Transform tetrominoTransform, Tile terominoTile)
    {
        // get parent tilemap
        var tileMap = transform.GetComponent<Tilemap>();

        foreach (Transform mino in tetrominoTransform)
        {
            var originCell = tileMap.WorldToCell(mino.position);
            tileMap.SetTile(originCell, terominoTile);
        }
    }

    /// <summary>
    /// Is full row at
    /// </summary>
    /// <param name="y"></param>
    /// <returns></returns>
    private bool IsFullRowAt(int y)
    {
        var tileMap = transform.GetComponent<Tilemap>();
        for (var x = 0; x < GridWidth; ++x)
        {
            if (null == tileMap.GetTile(new Vector3Int(x, y, 0)))
            {
                return false;
            }
        }

        // Increment rows count if it full row
        FindObjectOfType<ScoreManager>().RowsCount++;
        return true;
    }

    /// <summary>
    /// Delete mino at
    /// </summary>
    /// <param name="y"></param>
    private void DeleteMinoAt(int y)
    {
        var tileMap = transform.GetComponent<Tilemap>();
        for (var x = 0; x < GridWidth; ++x)
        {
            tileMap.SetTile(new Vector3Int(x, y, 0), null);
        }
    }

    /// <summary>
    /// Move row down
    /// </summary>
    /// <param name="y"></param>
    private void MoveRowDown(int y)
    {
        var tileMap = transform.GetComponent<Tilemap>();
        for (var x = 0; x < GridWidth; ++x)
        {
            var tile = tileMap.GetTile(new Vector3Int(x, y, 0));
            if (null == tile) continue;
            tileMap.SetTile(new Vector3Int(x, y - 1, 0), tile);
            tileMap.SetTile(new Vector3Int(x, y, 0), null);
        }
    }

    /// <summary>
    /// Move all rows down
    /// </summary>
    /// <param name="y"></param>
    private void MoveAllRowsDown(int y)
    {
        for (var i = y; i < GridHeight; ++i)
        {
            MoveRowDown(i);
        }
    }
}