using System;
using ArthemyDev.ScriptsTools;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;

public class GameUIController : MonoBehaviour
{
    [FoldoutGroup("References Game UI"), SerializeField]private TMP_Text LevelDisplay;
    [FoldoutGroup("References Game UI"), SerializeField]private TMP_Text LinesDisplay;
    [FoldoutGroup("References Game UI"), SerializeField]private TMP_Text CursesDisplay;
    [FoldoutGroup("References Game UI"), SerializeField]private TMP_Text TimeDisplay;
    [FoldoutGroup("References Game UI"), SerializeField]private Tilemap UITilemap;
    
    
    [BoxGroup("Pieces UI"), SerializeField] private Vector2Int HoldedPiecesPosition;
    [BoxGroup("Pieces UI"), SerializeField] private Vector2Int NextPiecePosition;
    [FoldoutGroup("DEBUG"), SerializeField,ReadOnly]private Vector2Int[] PrevHoldedPieceCells;
    [FoldoutGroup("DEBUG"), SerializeField,ReadOnly]private Vector2Int[] PrevNextPieceCells;
    


    private void Start()
    {
        GameTickManager.current.OnUiGameTick += UpdateGameUI;
    }

    private void OnDestroy()
    {
        GameTickManager.current.OnUiGameTick -= UpdateGameUI;
    }


    private void UpdateGameUI()
    {
        LevelDisplay.text = DificultyManager.current.GetLevel().ToString();
        LinesDisplay.text = DificultyManager.current.GetLines().ToString();
        CursesDisplay.text = ChaosEffectsManager.current.GetCurses().ToString();
        TimeDisplay.text = ScriptsTools.FormatTime(GameTimerManager.current.GetSeconds());

    }

    public void UpdateHoldedPiece(TetrominoData piece)
    {
        if(PrevHoldedPieceCells.Length!=0) ClearPieceUI(PrevHoldedPieceCells, HoldedPiecesPosition);
        SetPieceUI(piece, HoldedPiecesPosition);
        PrevHoldedPieceCells = (Vector2Int[])piece.cells.Clone();
    }

    public void UpdateNextPiece(TetrominoData piece)
    {
        if(PrevNextPieceCells.Length!=0) ClearPieceUI(PrevNextPieceCells, NextPiecePosition);
        SetPieceUI(piece, NextPiecePosition);
        PrevNextPieceCells = (Vector2Int[])piece.cells.Clone();
    }

    void SetPieceUI(TetrominoData piece, Vector2Int pos)
    {
        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector2Int tilePosition = piece.cells[i] + pos;
            UITilemap.SetTile((Vector3Int)tilePosition, piece.tile);
        }
    }

    void ClearPieceUI(Vector2Int[] CellsToClean, Vector2Int pos)
    {
        for (int i = 0; i < CellsToClean.Length; i++)
        {
            Vector2Int tilePosition = CellsToClean[i] + pos;
            UITilemap.SetTile((Vector3Int)tilePosition, null);
        }
    }

    
}
