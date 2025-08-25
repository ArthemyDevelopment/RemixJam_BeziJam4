using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;

public class BlindEffectController : MonoBehaviour
{

    [BoxGroup("Effect Settings"),SerializeField] private SpriteRenderer BlindEffectSprite;
    [BoxGroup("Effect Settings"),SerializeField]private float StartingFade;
    [BoxGroup("Effect Settings"),SerializeField] private float AmmountToReduce;
    [BoxGroup("DEBUG"), ReadOnly]private float curAlpha;

    public void SetEffect(bool state)
    {
        switch (state)
        {
            case true:
                GameTickManager.current.OnGameTick += IncreaseFade;
                TetrisInputHandler.current.OnEffectAction += ReduceFade;
                GameTickManager.current.OnUiGameTick += SetSpriteColor;
                curAlpha = StartingFade;
                SetSpriteColor();
                break;
            case false:
                GameTickManager.current.OnGameTick -= IncreaseFade;
                TetrisInputHandler.current.OnEffectAction -= ReduceFade;
                GameTickManager.current.OnUiGameTick -= SetSpriteColor;
                curAlpha = 0;
                SetSpriteColor();
                break;
        }
    }

    private void IncreaseFade()
    {
        curAlpha += 0.01f;
    }

    private void ReduceFade()
    {
        curAlpha -= AmmountToReduce;
        if (curAlpha < StartingFade) curAlpha = StartingFade;
    }

    private void SetSpriteColor()
    {
        BlindEffectSprite.color = new Color(BlindEffectSprite.color.r,
                                            BlindEffectSprite.color.g,
                                            BlindEffectSprite.color.b,
                                            curAlpha);
    }
    
    
    
    
    
    
    
    
    
    
    


}
