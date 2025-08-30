using System;
using ArthemyDev.ScriptsTools;
using Sirenix.OdinInspector;
using Unity.AppUI.Redux;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Events;

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
    [FoldoutGroup("DEBUG"), SerializeField, ReadOnly]private TetrisInputHandler curInputHandler;

    [HideInInspector]public UnityEvent<bool> externalSetups;

    public void SetUpPlayer(TetrisInputHandler newInputHandler)
    {
        curInputHandler = newInputHandler;
        StartPlayer();
    }
    
    public TetrisInputHandler GetCurInput(){ return curInputHandler;}
    
    
    private void StartPlayer()
    {
        if (DificultyManager.current != null)
        {
            curTicksToStep = DificultyManager.current.CurrentTicksToStep;
        }
        else
        {
            curTicksToStep = 20;
        }
        
        SetInputs(true);
    }

    private void OnDestroy()
    {
        SetInputs(false);
    }

    public void SetInputs(bool status)
    {
        switch (status)
        {
            case true:
                curInputHandler.OnMove+=MovePiece;
                curInputHandler.OnInitSoftDrop+=InitSoftDrop;
                curInputHandler.OnEndSoftDrop+=StopSoftDrop;
                curInputHandler.OnRotate+=RotatePiece;
                curInputHandler.OnHoldPiece += HoldPiece;
                GameTickManager.current.OnGameTick += UpdateTicks;
                break;
            case false:
                curInputHandler.OnMove-=MovePiece;
                curInputHandler.OnInitSoftDrop-=InitSoftDrop;
                curInputHandler.OnEndSoftDrop-=StopSoftDrop;
                curInputHandler.OnRotate-=RotatePiece;
                curInputHandler.OnHoldPiece -= HoldPiece;
                GameTickManager.current.OnGameTick -= UpdateTicks;
                break;
        }
        
        externalSetups.Invoke(status);
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
            curTicksToStep = DificultyManager.current.CurrentTicksToStep;
            
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
        if (NewPosition == Position) return;
        if (Board.IsValidPiecePosition(Cells, NewPosition))
        {
            Position.y = NewPosition.y;
            Position.x = GetWrapedXPosition(NewPosition.x);
            SFXManager.current.TriggerMoveSound();
            NewPosition = Position;
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

        Vector2Int[] newRotation = new Vector2Int[Cells.Length];
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
            newRotation [i] = new Vector2Int(x, y); 
        }

        if (Board.IsValidPiecePosition(newRotation, Position))
        {
            for (int i = 0; i < newRotation.Length; i++)
            {
                Cells[i] = newRotation[i];
                SFXManager.current.TriggerRotateSound();
            }
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

        if (NewPosition == Position) return;
        
        if (Board.IsValidPiecePosition(Cells, NewPosition))
        {
            Position.y = NewPosition.y;
            Position.x = GetWrapedXPosition(NewPosition.x);
            SFXManager.current.TriggerMoveSound();
            NewPosition = Position;
        }
        else
        {
            LockPiece();
        }
    }


    void HoldPiece()
    {
        if (IsPlayerInputLocked) return;
        
        if (StoredPiece.TetrominoType == Tetromino.Null)
        {
            Board.ClearPiece(this);
            StoredPiece = Data;
            Board.SpawnPiece(Position);
            SFXManager.current.TriggerHoldSound();
            Board.PlayerUI.UpdateHoldedPiece(StoredPiece);
            return;
        }
        
        TetrominoData tempData = StoredPiece;
        Board.ClearPiece(this);
        if (Board.IsValidPiecePosition(tempData.cells, Position))
        {
            StoredPiece = Data;
            Board.SpawnPiece(Position, tempData);
            SFXManager.current.TriggerHoldSound();
            Board.PlayerUI.UpdateHoldedPiece(StoredPiece);
        }
        else Board.SetPiece(this);
    }

    void LockPiece()
    {
        SFXManager.current.TriggerLockSound();
        Board.SetPiece(this);
        Board.ClearLines();
        Board.SpawnPiece();
    }
    
    
    
    
    

}
