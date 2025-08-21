using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public class TetrisMapBoard : MonoBehaviour
{
    
    
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
        SetPiece(curPiece);
    }

    public void SetPiece(PieceController piece)
    {
        for (int i = 0; i < piece.Cells.Length; i++)
        {
            Vector2Int tilePosition = piece.Cells[i]+piece.Position;
            Tilemap.SetTile((Vector3Int)tilePosition, piece.Data.tile);
        }
    }
    
    public void ClearPiece(PieceController piece)
    {
        for (int i = 0; i < piece.Cells.Length; i++)
        {
            Vector2Int tilePosition = piece.Cells[i]+piece.Position;
            Tilemap.SetTile((Vector3Int)tilePosition, null);
        }
    }

    public bool IsValidPiecePosition(PieceController piece, Vector2Int position)
    {
        RectInt boundToCheck = Bounds;
        
        for (int i = 0; i < piece.Cells.Length; i++)
        {
            Vector2Int positionToCheck = piece.Cells[i] + position;

            if (!boundToCheck.Contains(positionToCheck)) return false;
            

            if (Tilemap.HasTile((Vector3Int)positionToCheck)) return false;
            

        }
        
        return true;
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

