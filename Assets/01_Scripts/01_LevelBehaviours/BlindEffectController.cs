using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;

public class BlindEffectController : MonoBehaviour
{

    [BoxGroup("Effect Settings"),SerializeField] private SpriteRenderer BlindEffectSprite;
    [BoxGroup("Effect Settings"),SerializeField]private int StartingFade;
    [BoxGroup("Effect Settings"),SerializeField] private int AmmountToReduce;
    [BoxGroup("DEBUG"), ReadOnly]private int curAlpha;

    public void SetEffect(bool state)
    {
        switch (state)
        {
            case true:
                GameTickManager.current.OnGameTick += IncreaseFade;
                TetrisInputHandler.current.OnTBD += ReduceFade;
                GameTickManager.current.OnUiGameTick += SetSpriteColor;
                curAlpha = StartingFade;
                SetSpriteColor();
                break;
            case false:
                GameTickManager.current.OnGameTick -= IncreaseFade;
                TetrisInputHandler.current.OnTBD -= ReduceFade;
                GameTickManager.current.OnUiGameTick -= SetSpriteColor;
                curAlpha = 0;
                SetSpriteColor();
                break;
        }
    }

    private void IncreaseFade()
    {
        curAlpha += 1;
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
