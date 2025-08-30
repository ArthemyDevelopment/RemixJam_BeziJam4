using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Tilemaps;

public class FreezeEffectController : InputEffects
{
    
    private int buttonPressesRemaining;

    [BoxGroup("Effect Settings"), SerializeField] private int TimesToPress;
    [BoxGroup("Effect Settings"),SerializeField]private UnityEvent OnEventActive;
    [BoxGroup("Effect Settings"),SerializeField]private UnityEvent OnEventFinish;
    [BoxGroup("Effect Settings"),SerializeField] private Material cleanMaterial;
    [BoxGroup("Effect Settings"), SerializeField] private TilemapRenderer tilemapRenderer;

    
    
    
    public void SetEvent()
    {
        isEffectActive = true;
        buttonPressesRemaining = TimesToPress;
        OnEventActive.Invoke();
    }
    
    
    protected override void OnEffectActionPressed()
    {
        if (!isEffectActive) return;
        
        buttonPressesRemaining--;
        
        if (buttonPressesRemaining <= 0)
        {
            isEffectActive = false;
            OnEventFinish.Invoke();
            tilemapRenderer.sharedMaterial = cleanMaterial;
        }
    }





}
