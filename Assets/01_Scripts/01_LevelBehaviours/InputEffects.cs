using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UIElements;

public class InputEffects : MonoBehaviour
{
    [BoxGroup("References"),SerializeField] protected PieceController targetPlayer;
    [FoldoutGroup("DEBUG")]protected bool isEffectActive;
    
    private void Awake()
    {
        if (targetPlayer == null) return;
        targetPlayer.externalSetups.AddListener(SetUpInput);
    }

    protected virtual void SetUpInput(bool state)
    {
        Debug.Log("Set input: " + state);
        switch (state)
        {
            case true:
                targetPlayer.GetCurInput().OnEffectAction += OnEffectActionPressed;
                break;
            case false:
                targetPlayer.GetCurInput().OnEffectAction -= OnEffectActionPressed;
                break;
        }
    }

    protected virtual void OnEffectActionPressed()
    {
        
        
    }
}
