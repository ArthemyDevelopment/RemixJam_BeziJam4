using System;
using ArthemyDev.ScriptsTools;
using Sirenix.OdinInspector;
using UnityEngine;

public class PieceController : MonoBehaviour
{
    
    [FoldoutGroup("References")]public TetrisMapBoard Board;
    [FoldoutGroup("References")]public TetrominoData Data;
    [FoldoutGroup("PieceData")]public Vector2Int[] Cells;
    [FoldoutGroup("PieceData")]public Vector2Int Position;
    [FoldoutGroup("PieceData")]public int RotationIndex;
    [FoldoutGroup("PieceData")][SerializeField] private bool IsSoftDrop = false;


    private Vector2Int NewPosition;
    private bool rotationQueued;

    private void Start()
    {
        TetrisInputHandler.current.OnMove+=MovePiece;
        TetrisInputHandler.current.OnInitSoftDrop+=InitSoftDrop;
        TetrisInputHandler.current.OnEndSoftDrop+=StopSoftDrop;
        TetrisInputHandler.current.OnRotate+=RotatePiece;
        GameTickManager.current.OnGameTick += UpdateTicks;
    }

    private void OnDestroy()
    {
        TetrisInputHandler.current.OnMove-=MovePiece;
        TetrisInputHandler.current.OnInitSoftDrop-=InitSoftDrop;
        TetrisInputHandler.current.OnEndSoftDrop-=StopSoftDrop;
        TetrisInputHandler.current.OnRotate-=RotatePiece;
        GameTickManager.current.OnGameTick -= UpdateTicks;
    }

    public void Init(TetrisMapBoard board, Vector2Int pos, TetrominoData data)
    {
        Board = board;
        Position = pos;
        Data = data;
        RotationIndex = 0;

        if (Cells == null || Cells.Length==0) Cells = new Vector2Int[data.cells.Length];

        for (int i = 0; i < Data.cells.Length; i++) Cells[i] = Data.cells[i];

        NewPosition = Position;

    }


    private void UpdateTicks()
    {
        Board.ClearPiece(this);

        if (rotationQueued)
        {
            rotationQueued = false;
            ApplyRotationMatrix(1);
        }
        SetNewPosition();
        
        Board.SetPiece(this);
    }

   



    private void SetNewPosition()
    {
        if (Board.IsValidPiecePosition(this, NewPosition))
        {
            Position = NewPosition;
        }
        else NewPosition = Position;
    }
    

    private void MovePiece(float dir)
    {
        Vector2Int newPos = Position;
        newPos.x += (int)dir;
        NewPosition.x = newPos.x;
    }

    private void InitSoftDrop()
    {
        IsSoftDrop = true;
        DropPiece();
    }

    private void StopSoftDrop()
    {
        IsSoftDrop = false;
    }
    
    private void DropPiece()
    {
        Vector2Int newPos = Position;
        newPos.y -= 1;
        NewPosition.y = newPos.y;
    }
    
    

    void RotatePiece()
    {
        int originalRotation = RotationIndex;
        
        RotationIndex = ScriptsTools.WrapInt(RotationIndex + 1, 0, 4);

        rotationQueued = true;
        /*// Revert the rotation if the wall kick tests fail
        if (!TestWallKicks(rotationIndex, direction))
        {
            rotationIndex = originalRotation;
            ApplyRotationMatrix(-direction);
        }*/

    }

    private void ApplyRotationMatrix(int direction)
    {
        float[] matrix = TetrominoesGridData.RotationMatrix;
        
        for (int i = 0; i < Cells.Length; i++)
        {
            Vector2 cell = Cells[i];

            int x=0, y=0;

            switch (Data.TetrominoType)
            {
                case Tetromino.O:
                    x = (int)cell.x;
                    y = (int)cell.y;
                    break;
                case Tetromino.I:
                    cell.x -= 0.5f;
                    cell.y -= 0.5f;
                    x = Mathf.CeilToInt((cell.x * matrix[0] * direction) + (cell.y * matrix[1] * direction));
                    y = Mathf.CeilToInt((cell.x * matrix[2] * direction) + (cell.y * matrix[3] * direction));
                    break;
                default:
                    x = Mathf.RoundToInt((cell.x * matrix[0] * direction) + (cell.y * matrix[1] * direction));
                    y = Mathf.RoundToInt((cell.x * matrix[2] * direction) + (cell.y * matrix[3] * direction));
                    break;
            }

            Cells[i] = new Vector2Int(x, y);
        }
    }
    
    

}
