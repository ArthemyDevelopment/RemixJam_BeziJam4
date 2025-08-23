using System;
using ArthemyDev.ScriptsTools;
using Sirenix.OdinInspector;
using Unity.AppUI.Redux;
using UnityEngine;
using UnityEngine.Animations;

public class PieceController : MonoBehaviour
{
    
    [FoldoutGroup("References")]public TetrisMapBoard Board;
    [FoldoutGroup("References")]public TetrominoData Data;
    [FoldoutGroup("PieceData")]public Vector2Int[] Cells;
    [FoldoutGroup("PieceData")]public Vector2Int Position;
    [FoldoutGroup("PieceData")]public int RotationIndex;
    [FoldoutGroup("PieceData")][SerializeField] private bool IsSoftDrop = false;
    [FoldoutGroup("PieceData")] [SerializeField] private int StartTicksToStep = 20;


    [FoldoutGroup("DEBUG"), SerializeField, ReadOnly]private TetrominoData StoredPiece;
    [FoldoutGroup("DEBUG"), SerializeField, ReadOnly]private bool IsPlayerInputLocked;
    [FoldoutGroup("DEBUG"), SerializeField, ReadOnly]private Vector2Int NewPosition;
    [FoldoutGroup("DEBUG"), SerializeField, ReadOnly]private bool rotationQueued;
    [FoldoutGroup("DEBUG"), SerializeField, ReadOnly]private int curTicksToStep;
    [FoldoutGroup("DEBUG"), SerializeField, ReadOnly]private int curTick;

    private void Start()
    {
        if (DificultyManager.current != null)
        {
            curTicksToStep = DificultyManager.current.CurrentTicksToStep;
        }
        else
        {
            curTicksToStep = 20;
        }
        
        TetrisInputHandler.current.OnMove+=MovePiece;
        TetrisInputHandler.current.OnInitSoftDrop+=InitSoftDrop;
        TetrisInputHandler.current.OnEndSoftDrop+=StopSoftDrop;
        TetrisInputHandler.current.OnRotate+=RotatePiece;
        TetrisInputHandler.current.OnHoldPiece += HoldPiece;
        GameTickManager.current.OnGameTick += UpdateTicks;
    }

    private void OnDestroy()
    {
        TetrisInputHandler.current.OnMove-=MovePiece;
        TetrisInputHandler.current.OnInitSoftDrop-=InitSoftDrop;
        TetrisInputHandler.current.OnEndSoftDrop-=StopSoftDrop;
        TetrisInputHandler.current.OnRotate-=RotatePiece;
        TetrisInputHandler.current.OnHoldPiece -= HoldPiece;
        GameTickManager.current.OnGameTick -= UpdateTicks;
    }

    public void Init(Vector2Int pos, TetrominoData data)
    {
        Position = pos;
        Data = data;
        RotationIndex = 0;

        if (Cells == null || Cells.Length==0) Cells = new Vector2Int[data.cells.Length];

        for (int i = 0; i < Data.cells.Length; i++) Cells[i] = Data.cells[i];

        NewPosition = Position;

    }

    public void ChangeSpeed()
    {
        if (DificultyManager.current != null)
        {
            curTicksToStep = DificultyManager.current.CurrentTicksToStep;
            Debug.Log($"Piece speed updated to {curTicksToStep} ticks per step");
        }
    }


    private void UpdateTicks()
    {
        Board.ClearPiece(this);

        if (!IsPlayerInputLocked)
        {
            if (rotationQueued)
            {
                rotationQueued = false;
                ApplyRotationMatrix(1);
            }
            SetNewPosition();
        }
        else
        {
            rotationQueued = false;
            NewPosition = Position;
        }
        CheckStepDown();
        Board.SetPiece(this);
    }

    public void LockPlayerInput(bool state)
    {
        IsPlayerInputLocked = state;
        IsSoftDrop = false;
    }

    private void SetNewPosition()
    {
        if (Board.IsValidPiecePosition(this, NewPosition))
        {
            Position.y = NewPosition.y;
            Position.x = GetWrapedXPosition(NewPosition.x);
        }
        else
        {
            NewPosition = Position;
        } 
    }
    

    private void MovePiece(float dir)
    {
        Vector2Int newPos = Position;
        newPos.x += (int)dir;
        NewPosition.x = newPos.x;
    }

    private void InitSoftDrop()
    {
        if (IsPlayerInputLocked) return;
        IsSoftDrop = true;
        DropPiece();
    }

    private void StopSoftDrop()
    {
        if (IsPlayerInputLocked) return;
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

            x = GetWrapedXPosition(x);
            Cells[i] = new Vector2Int(x, y);
        }
    }


    int GetWrapedXPosition(int input)
    {
        return ScriptsTools.WrapInt(input, TetrisMapBoard.BOARD_X_BOUND_NEGATIVE, TetrisMapBoard.BOARD_X_BOUND_POSITIVE);
    }

    void CheckStepDown()
    {
        if (IsSoftDrop)
        {
            StepDownPiece();
        }

        curTick++;
        if (curTick >= curTicksToStep)
        {
            StepDownPiece();
        }

    }

    void StepDownPiece()
    {
        curTick = 0;
        DropPiece();
        if (Board.IsValidPiecePosition(this, NewPosition))
        {
            Position.y = NewPosition.y;
            Position.x = GetWrapedXPosition(NewPosition.x);
        }
        else
        {
            LockPiece();
        }
    }


    void HoldPiece()
    {
        if (IsPlayerInputLocked) return;
        
        Board.ClearPiece(this);
        if (StoredPiece.TetrominoType == Tetromino.Null)
        {
            StoredPiece = Data;
            Board.SpawnPiece(Position);
        }
        else
        {
            TetrominoData tempData = StoredPiece;
            StoredPiece = Data;
            Board.SpawnPiece(Position,tempData);
        }
        
        //TODO:Set in UI the stored Piece
    }

    void LockPiece()
    {
        Board.SetPiece(this);
        Board.ClearLines();
        Board.SpawnPiece();
    }
    
    
    
    
    

}
