using System;
using UnityEngine;

public class DEBUG_Inputs : MonoBehaviour
{
    [SerializeField] private TetrisInputHandler inputHandler;

    private void OnEnable()
    {
        if(inputHandler==null) inputHandler = TetrisInputHandler.current;
        
        inputHandler.OnMove+=HandleMove;
        inputHandler.OnRotate+=HandleRotate;
        inputHandler.OnInitSoftDrop+=HandleInitSoftDrop;
        inputHandler.OnEndSoftDrop+=HandleEndSoftDrop;
        inputHandler.OnHoldPiece+=HandleHoldPiece;
        inputHandler.OnEffectAction+=HandleCustomAction;
    }

    private void OnDisable()
    {
        inputHandler.OnMove-=HandleMove;
        inputHandler.OnRotate-=HandleRotate;
        inputHandler.OnInitSoftDrop-=HandleInitSoftDrop;
        inputHandler.OnEndSoftDrop-=HandleEndSoftDrop;
        inputHandler.OnHoldPiece-=HandleHoldPiece;
        inputHandler.OnEffectAction-=HandleCustomAction;
    }

    private void HandleMove(float direction)
    {
        Debug.Log($"Moving piece: {direction}");
    }
    
    private void HandleRotate()
    {
        Debug.Log("Rotating piece");
    }
    
    private void HandleInitSoftDrop()
    {
        Debug.Log("Soft drop started");
    }
    
    private void HandleEndSoftDrop()
    {
        Debug.Log("Soft drop started");
    }
    
    
    private void HandleHoldPiece()
    {
        Debug.Log("Hold piece");
    }
    
    private void HandleCustomAction()
    {
        Debug.Log("Custom action triggered");
    }
}
