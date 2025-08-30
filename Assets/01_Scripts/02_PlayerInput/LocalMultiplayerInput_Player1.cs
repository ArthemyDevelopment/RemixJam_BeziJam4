using UnityEngine;

public class LocalMultiplayerInput_Player1 : TetrisInputHandler
{
    protected override void OnEnable()
    {
        inputActions.Enable();
        inputActions.Multiplayer_Player1.Move.performed += OnMovePerformed;
        inputActions.Multiplayer_Player1.Move.canceled += OnMoveCanceled;
        inputActions.Multiplayer_Player1.SoftDrop.performed += OnSoftDropPerformed;
        inputActions.Multiplayer_Player1.SoftDrop.canceled += OnSoftDropCanceled;
        inputActions.Multiplayer_Player1.HoldPiece.performed += OnHoldPiecePerformed;
        inputActions.Multiplayer_Player1.Rotate.performed += OnRotatePerformed;
        inputActions.Multiplayer_Player1.EffectAction.performed += OnEffectActionPerformed;
    }

    protected override void OnDisable()
    {
        inputActions.Disable();
        inputActions.Multiplayer_Player1.Move.performed -= OnMovePerformed;
        inputActions.Multiplayer_Player1.Move.canceled -= OnMoveCanceled;
        inputActions.Multiplayer_Player1.SoftDrop.performed -= OnSoftDropPerformed;
        inputActions.Multiplayer_Player1.SoftDrop.canceled -= OnSoftDropCanceled;
        inputActions.Multiplayer_Player1.HoldPiece.performed -= OnHoldPiecePerformed;
        inputActions.Multiplayer_Player1.Rotate.performed -= OnRotatePerformed;
        inputActions.Multiplayer_Player1.EffectAction.performed -= OnEffectActionPerformed;
    }
}
