using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using ArthemyDev.ScriptsTools;


[DefaultExecutionOrder(-1)]
public class TetrisInputHandler : SingletonManager<TetrisInputHandler>
{
    [FoldoutGroup("Repeat input values")][SerializeField] private float moveRepeatDelay = 0.3f;
    [FoldoutGroup("Repeat input values")][SerializeField] private float moveRepeatRate = 0.1f;
    
    private PlayerInputs inputActions;
    [FoldoutGroup("DEBUG_DATA")][SerializeField]private float lastMoveInput;
    [FoldoutGroup("DEBUG_DATA")][SerializeField]private float moveTimer;
    [FoldoutGroup("DEBUG_DATA")][SerializeField]private bool isMoving;
    [FoldoutGroup("DEBUG_DATA")][SerializeField]private bool isSoftDropping;


    public delegate void InputButtonAction();
    public delegate void InputFloatAction(float f);
    
    
    [HideInInspector]public InputFloatAction OnMove;
    [HideInInspector]public InputButtonAction OnRotate;
    [HideInInspector]public InputButtonAction OnInitSoftDrop;
    [HideInInspector]public InputButtonAction OnEndSoftDrop;
    [HideInInspector]public InputButtonAction OnHoldPiece;
    [HideInInspector]public InputButtonAction OnTBD;
    protected override void Awake()
    {
        base.Awake();
        inputActions = new PlayerInputs();
    }
    
    private void OnEnable()
    {
        inputActions.Enable();
        inputActions.Player.Move.performed += OnMovePerformed;
        inputActions.Player.Move.canceled += OnMoveCanceled;
        inputActions.Player.SoftDrop.performed += OnSoftDropPerformed;
        inputActions.Player.SoftDrop.canceled += OnSoftDropCanceled;
        inputActions.Player.HoldPiece.performed += OnHoldPiecePerformed;
        inputActions.Player.Rotate.performed += OnRotatePerformed;
        inputActions.Player.TBD.performed += OnTBDPerformed;
    }
    
    private void OnDisable()
    {
        inputActions.Disable();
        inputActions.Player.Move.performed -= OnMovePerformed;
        inputActions.Player.Move.canceled -= OnMoveCanceled;
        inputActions.Player.SoftDrop.performed -= OnSoftDropPerformed;
        inputActions.Player.SoftDrop.canceled -= OnSoftDropCanceled;
        inputActions.Player.HoldPiece.performed -= OnHoldPiecePerformed;
        inputActions.Player.Rotate.performed -= OnRotatePerformed;
        inputActions.Player.TBD.performed -= OnTBDPerformed;
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
    
    
    
    private void OnMovePerformed(InputAction.CallbackContext context)
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
    
    private void OnMoveCanceled(InputAction.CallbackContext context)
    {
        isMoving = false;
        moveTimer = 0f;
    }
    
    private void OnSoftDropPerformed(InputAction.CallbackContext context)
    {
        if (!isSoftDropping)
        {
            isSoftDropping = true;
            OnInitSoftDrop?.Invoke();
        }
    }
    
    private void OnSoftDropCanceled(InputAction.CallbackContext context)
    {
        if (isSoftDropping)
        {
            isSoftDropping = false;
            OnEndSoftDrop?.Invoke();
        }
    }
    
    private void OnHoldPiecePerformed(InputAction.CallbackContext context)
    {
        OnHoldPiece?.Invoke();
    }
    
    private void OnRotatePerformed(InputAction.CallbackContext context)
    {
        OnRotate?.Invoke();
    }
    
    private void OnTBDPerformed(InputAction.CallbackContext context)
    {
        OnTBD?.Invoke();
    }
    
    public void SetInput(bool enabled)
    {
        if (enabled)
            inputActions.Enable();
        else
            inputActions.Disable();
    }
}

