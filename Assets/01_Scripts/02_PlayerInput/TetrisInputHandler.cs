using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using ArthemyDev.ScriptsTools;


[DefaultExecutionOrder(-1)]
public class TetrisInputHandler : MonoBehaviour
{
    [FoldoutGroup("Repeat input values")][SerializeField] private float moveRepeatDelay = 0.3f;
    [FoldoutGroup("Repeat input values")][SerializeField] private float moveRepeatRate = 0.1f;
    
    protected PlayerInputs inputActions;
    [FoldoutGroup("DEBUG"),SerializeField, ReadOnly]private float lastMoveInput;
    [FoldoutGroup("DEBUG"),SerializeField, ReadOnly]private float moveTimer;
    [FoldoutGroup("DEBUG"),SerializeField, ReadOnly]private bool isMoving;
    [FoldoutGroup("DEBUG"),SerializeField, ReadOnly]private bool isSoftDropping;


    public delegate void InputButtonAction();
    public delegate void InputFloatAction(float f);
    
    
    [HideInInspector]public InputFloatAction OnMove;
    [HideInInspector]public InputButtonAction OnRotate;
    [HideInInspector]public InputButtonAction OnInitSoftDrop;
    [HideInInspector]public InputButtonAction OnEndSoftDrop;
    [HideInInspector]public InputButtonAction OnHoldPiece;
    [HideInInspector]public InputButtonAction OnEffectAction;
    protected void Awake()
    {
        inputActions = new PlayerInputs();
    }
    
    protected virtual void OnEnable()
    {
        inputActions.Enable();
        inputActions.Player.Move.performed += OnMovePerformed;
        inputActions.Player.Move.canceled += OnMoveCanceled;
        inputActions.Player.SoftDrop.performed += OnSoftDropPerformed;
        inputActions.Player.SoftDrop.canceled += OnSoftDropCanceled;
        inputActions.Player.HoldPiece.performed += OnHoldPiecePerformed;
        inputActions.Player.Rotate.performed += OnRotatePerformed;
        inputActions.Player.EffectAction.performed += OnEffectActionPerformed;
    }
    
    protected virtual void OnDisable()
    {
        inputActions.Disable();
        inputActions.Player.Move.performed -= OnMovePerformed;
        inputActions.Player.Move.canceled -= OnMoveCanceled;
        inputActions.Player.SoftDrop.performed -= OnSoftDropPerformed;
        inputActions.Player.SoftDrop.canceled -= OnSoftDropCanceled;
        inputActions.Player.HoldPiece.performed -= OnHoldPiecePerformed;
        inputActions.Player.Rotate.performed -= OnRotatePerformed;
        inputActions.Player.EffectAction.performed -= OnEffectActionPerformed;
    }
    
    private void Update()
    {
        HandleMoveRepeating();
    }
    
    /// <summary>
    /// Handles continuous movement when direction is held down
    /// </summary>
    private void HandleMoveRepeating()
    {
        if (!isMoving) return;
        
        moveTimer += Time.deltaTime;
        
        // Initial delay before repeating
        float currentDelay = moveTimer < moveRepeatDelay ? moveRepeatDelay : moveRepeatRate;
        
        if (moveTimer >= currentDelay)
        {
            OnMove?.Invoke(lastMoveInput);
            moveTimer = moveRepeatDelay; // Reset to repeat rate after first repeat
        }
    }
    
    
    
    protected void OnMovePerformed(InputAction.CallbackContext context)
    {
        float moveInput = context.ReadValue<float>();

        /*if (moveInput > 0) moveInput = 1;
        if (moveInput < 0) moveInput = -1;*/
        
        lastMoveInput = moveInput;
        
        // Immediate first move
        OnMove?.Invoke(lastMoveInput);
        
        isMoving = true;
        moveTimer = 0f;
    }
    
    protected void OnMoveCanceled(InputAction.CallbackContext context)
    {
        isMoving = false;
        moveTimer = 0f;
    }
    
    protected void OnSoftDropPerformed(InputAction.CallbackContext context)
    {
        if (!isSoftDropping)
        {
            isSoftDropping = true;
            OnInitSoftDrop?.Invoke();
        }
    }
    
    protected void OnSoftDropCanceled(InputAction.CallbackContext context)
    {
        if (isSoftDropping)
        {
            isSoftDropping = false;
            OnEndSoftDrop?.Invoke();
        }
    }
    
    protected void OnHoldPiecePerformed(InputAction.CallbackContext context)
    {
        OnHoldPiece?.Invoke();
    }
    
    protected void OnRotatePerformed(InputAction.CallbackContext context)
    {
        OnRotate?.Invoke();
    }
    
    protected void OnEffectActionPerformed(InputAction.CallbackContext context)
    {
        OnEffectAction?.Invoke();
    }
    
    public void SetInput(bool enabled)
    {
        if (enabled)
            inputActions.Enable();
        else
            inputActions.Disable();
    }
}

