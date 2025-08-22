using System;
using System.Collections;
using System.Collections.Generic;
using ArthemyDev.ScriptsTools;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public class TetrisMapBoard : MonoBehaviour
{
    public const int BOARD_X_BOUND_NEGATIVE= -5;
    public const int BOARD_X_BOUND_POSITIVE= 5;
    
    [FoldoutGroup("References")]public Tilemap Tilemap;
    [FoldoutGroup("References")]public PieceController curPiece;
    
    [FoldoutGroup("Values")]public Vector2Int SpawnPos;
    [FoldoutGroup("Values")]public Vector2Int MapBoardSize = new Vector2Int(10,20);
    
    public TetrominoData[] Tetrominoes;

    private int _indexToSpawn;
    private TetrominoData _pieceToSpawn;
    public RectInt Bounds{
        get
        {
            Vector2Int position = new Vector2Int(-MapBoardSize.x / 2, -MapBoardSize.y / 2);
            return new RectInt(position, MapBoardSize);

        }}

    private void Awake()
    {
        if (Tilemap == null) Tilemap = GetComponentInChildren<Tilemap>();
    }

    private void Start()
    {
        SpawnPiece();
    }

    public void SpawnPiece()
    {
        _indexToSpawn = Random.Range(0, Tetrominoes.Length);
        _pieceToSpawn = Tetrominoes[_indexToSpawn];
        curPiece.Init(this, SpawnPos, _pieceToSpawn);
        
        if(IsValidPiecePosition(curPiece, SpawnPos)) SetPiece(curPiece);
        else GameOver();
    }

    public void SetPiece(PieceController piece)
    {
        for (int i = 0; i < piece.Cells.Length; i++)
        {
            Vector2Int tilePosition = piece.Cells[i]+piece.Position;
            tilePosition.x = ScriptsTools.WrapInt(tilePosition.x, BOARD_X_BOUND_NEGATIVE, BOARD_X_BOUND_POSITIVE);
            Tilemap.SetTile((Vector3Int)tilePosition, piece.Data.tile);
        }
    }
    
    public void ClearPiece(PieceController piece)
    {
        for (int i = 0; i < piece.Cells.Length; i++)
        {
            Vector2Int tilePosition = piece.Cells[i]+piece.Position;
            tilePosition.x = ScriptsTools.WrapInt(tilePosition.x, BOARD_X_BOUND_NEGATIVE, BOARD_X_BOUND_POSITIVE);
            Tilemap.SetTile((Vector3Int)tilePosition, null);
        }
    }

    public bool IsValidPiecePosition(PieceController piece, Vector2Int position)
    {
        RectInt boundToCheck = Bounds;
        
        for (int i = 0; i < piece.Cells.Length; i++)
        {
            Vector2Int positionToCheck = piece.Cells[i] + position;
            positionToCheck.x = ScriptsTools.WrapInt(positionToCheck.x, BOARD_X_BOUND_NEGATIVE,BOARD_X_BOUND_POSITIVE);

            if (!boundToCheck.Contains(positionToCheck))
            {
                //Debug.Log("FAILED: Out of bounds");
                return false;
            }


            if (Tilemap.HasTile((Vector3Int)positionToCheck))
            {
                //Debug.Log("FAILED: Tile in position");
                return false;
            }
            

        }
        //Debug.Log("SUCCESS: Valid position");
        return true;
    }

    
    public void ClearLines() 
    {
        RectInt bounds = Bounds;
        int gridHeight = bounds.height; 
        
        Span<bool> fullLines = stackalloc bool[gridHeight];
        int linesCleared = 0;
        
        for (int row = bounds.yMin; row < bounds.yMax; row++)
        {
            if (IsLineFull(row))
            {
                int relativeRow = row - bounds.yMin;
                fullLines[relativeRow] = true;
                linesCleared++;
            }
        }

        if (linesCleared == 0) return;
        
        DificultyManager.current.CheckDificultIncrease(linesCleared);
        
        int writeRow = bounds.yMin;
        
        
        for (int readRow = bounds.yMin; readRow < bounds.yMax; readRow++)
        {
            int relativeReadRow = readRow - bounds.yMin;
            
            if (!fullLines[relativeReadRow])
            {
                if (writeRow != readRow)
                {
                    MoveRowBatch(readRow, writeRow);
                }
                writeRow++;
            }
        }
        for (int row = writeRow; row < bounds.yMax; row++) 
        {
            ClearLine(row);
        }
    }
    
    private bool IsLineFull(int row)
    {
        RectInt bounds = Bounds;
        
        for (int col = bounds.xMin; col < bounds.xMax; col++)
        {
            Vector3Int pos = new Vector3Int(col, row, 0);
            if (!Tilemap.HasTile(pos)) return false;
        }

        return true;
    }

    private void ClearLine(int row)
    {
        RectInt bounds = Bounds;
        
        for (int col = bounds.xMin; col < bounds.xMax; col++)
        {
            Vector3Int pos = new Vector3Int(col, row, 0);
            Tilemap.SetTile(pos, null);
        }
    }

    private void MoveRowBatch(int fromRow, int toRow)
    {
        RectInt bounds = Bounds;
        int gridWidth = bounds.width;
        Vector3Int[] sourcePositions = new Vector3Int[gridWidth];
        Vector3Int[] targetPositions = new Vector3Int[gridWidth];
        TileBase[] tiles = new TileBase[gridWidth];
        
        int index = 0;
        for (int col = bounds.xMin; col < bounds.xMax; col++)
        {
            sourcePositions[index] = new Vector3Int(col, fromRow, 0);
            targetPositions[index] = new Vector3Int(col, toRow, 0);
            tiles[index] = Tilemap.GetTile(sourcePositions[index]);
            index++;
        }
        
        Tilemap.SetTiles(sourcePositions, new TileBase[gridWidth]); 
        Tilemap.SetTiles(targetPositions, tiles); 
    }


    private void GameOver()
    {
        GameTickManager.current.StopGameTicks();
    }
    
    #if UNITY_EDITOR

    #region DEBUG

    [FoldoutGroup("DEBUG")][Button]
    public void DEBUG_InitTetrominoesData()
    {
        for (int i = 0; i < Tetrominoes.Length; i++)
        {
            Tetrominoes[i].Init();
        }
    }

    #endregion
    
    
    
    
    #endif
}

