using UnityEngine;

public class LocalMultiplayerInput_Player2 : TetrisInputHandler
{
    protected override void OnEnable()
    {
        inputActions.Enable();
        inputActions.Multiplayer_Player2.Move.performed += OnMovePerformed;
        inputActions.Multiplayer_Player2.Move.canceled += OnMoveCanceled;
        inputActions.Multiplayer_Player2.SoftDrop.performed += OnSoftDropPerformed;
        inputActions.Multiplayer_Player2.SoftDrop.canceled += OnSoftDropCanceled;
        inputActions.Multiplayer_Player2.HoldPiece.performed += OnHoldPiecePerformed;
        inputActions.Multiplayer_Player2.Rotate.performed += OnRotatePerformed;
        inputActions.Multiplayer_Player2.EffectAction.performed += OnEffectActionPerformed;
    }

    protected override void OnDisable()
    {
        inputActions.Disable();
        inputActions.Multiplayer_Player2.Move.performed -= OnMovePerformed;
        inputActions.Multiplayer_Player2.Move.canceled -= OnMoveCanceled;
        inputActions.Multiplayer_Player2.SoftDrop.performed -= OnSoftDropPerformed;
        inputActions.Multiplayer_Player2.SoftDrop.canceled -= OnSoftDropCanceled;
        inputActions.Multiplayer_Player2.HoldPiece.performed -= OnHoldPiecePerformed;
        inputActions.Multiplayer_Player2.Rotate.performed -= OnRotatePerformed;
        inputActions.Multiplayer_Player2.EffectAction.performed -= OnEffectActionPerformed;
    }
}
