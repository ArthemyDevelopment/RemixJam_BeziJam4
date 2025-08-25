using System;
using System.Collections;
using System.Collections.Generic;
using ArthemyDev.ScriptsTools;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public enum ChaosEffectType
{
    Material,
    Object,
}

[Serializable]
public class ChaosEffect
{
    [BoxGroup("Effect Settings")] public string effectName;

    [BoxGroup("Effect Settings")] public ChaosEffectType Type;

    [BoxGroup("Effect Settings")] public AudioClip VoiceSFX;
    
    [BoxGroup("Effect Settings"), ShowIf("@Type==ChaosEffectType.Material")]
    public Material effectMaterial;
    
    [BoxGroup("Effect Settings"), ShowIf("@Type==ChaosEffectType.Object")]
    public GameObject effectObject;
    
    [BoxGroup("Duration Settings")] public bool useButtonPresses = false;
    
    [BoxGroup("Duration Settings"),ShowIf("@!useButtonPresses")] 
    public Vector2Int durationRange = new Vector2Int(5, 15);
    
    [BoxGroup("Duration Settings"),ShowIf("useButtonPresses")]
    public int buttonPressesToEnd = 3;

    [BoxGroup("TriggerEvent")] public bool triggerCustomEvent = false;

    [BoxGroup("TriggerEvent"), ShowIf("@triggerCustomEvent")]
    public UnityEvent onStart= new UnityEvent(); 
    [BoxGroup("TriggerEvent"), ShowIf("@triggerCustomEvent")]
    public UnityEvent onEnd= new UnityEvent(); 
    
    [BoxGroup("Game Impact"),TextArea(2, 4)]
    public string description;
}


public class ChaosEffectsManager : SingletonManager<ChaosEffectsManager>
{
    [FoldoutGroup("References")] [SerializeField] private TilemapRenderer tilemapRenderer;
    [FoldoutGroup("References")] [SerializeField] private Animator DemonHand;
    [FoldoutGroup("Effect Configuration"),SerializeField] private Vector2Int ActiveEffectStepsRange = new Vector2Int(3, 8);
    [FoldoutGroup("Effect Configuration"),SerializeField] private Material defaultMaterial;
    [FoldoutGroup("Available Effects"),ListDrawerSettings(ShowIndexLabels = true, DraggableItems = true)]
    [SerializeField] private List<ChaosEffect> availableEffects = new List<ChaosEffect>();
    
    [FoldoutGroup("DEBUG"), ReadOnly, SerializeField] private ChaosEffect curEffect;
    [FoldoutGroup("DEBUG"), ReadOnly, SerializeField] private int stepsUntilNextEffect;
    [FoldoutGroup("DEBUG"), ReadOnly, SerializeField] private int currentEffectRemainingSteps;
    [FoldoutGroup("DEBUG"), ReadOnly, SerializeField] private int buttonPressesRemaining;
    [FoldoutGroup("DEBUG"), ReadOnly, SerializeField] private bool isEffectActive = false;
    [FoldoutGroup("DEBUG"), ReadOnly, SerializeField] private bool EffectOnQueue = false;
    [FoldoutGroup("DEBUG"), ReadOnly, SerializeField] private bool isSystemActive = true;
    [FoldoutGroup("DEBUG"), ReadOnly, SerializeField] private int cursesSurvived;
    
    
    private void Start()
    {
        InitializeEffectSystem();
    }
    

    private void InitializeEffectSystem()
    {
        GameTickManager.current.OnGameTick += OnGameTick;
        TetrisInputHandler.current.OnEffectAction += OnEffectActionPressed;
        
        stepsUntilNextEffect = GetStepsInTicks(Random.Range(ActiveEffectStepsRange.x, ActiveEffectStepsRange.y + 1));
    }

    private void StopEffectSystem()
    {
        GameTickManager.current.OnGameTick -= OnGameTick;
        TetrisInputHandler.current.OnEffectAction -= OnEffectActionPressed;
        EffectOnQueue = false;
        if (isEffectActive) EndCurrentEffect();
    }
    

    private int GetStepsInTicks(int steps)
    {
        if (DificultyManager.current != null)
        {
            return steps * DificultyManager.current.CurrentTicksToStep;
        }
        return steps * 20;
    }
    

    private void OnGameTick()
    {
        if (!isSystemActive) return;
        
        if (isEffectActive && curEffect != null && !curEffect.useButtonPresses)
        {
            currentEffectRemainingSteps--;
            
            if (currentEffectRemainingSteps <= 0)
            {
                cursesSurvived++;
                EndCurrentEffect();
            }
        }
        
        if (!isEffectActive && !EffectOnQueue)
        {
            stepsUntilNextEffect--;
            
            if (stepsUntilNextEffect <= 0)
                TriggerRandomEffect();
           
        }
    }
    
    private void TriggerRandomEffect()
    {
        if (availableEffects.Count == 0) return;
        
        int randomIndex = Random.Range(0, availableEffects.Count);
        ChaosEffect selectedEffect = availableEffects[randomIndex];
        
        StartEffect(selectedEffect);
    }
    

    public void StartEffect(ChaosEffect effect)
    {
        if (effect == null) return;
        

        switch (effect.Type)
        {
            case ChaosEffectType.Material:
                if (effect.effectMaterial == null) return;
                break;

            case ChaosEffectType.Object:
                if (effect.effectObject == null) return;
                break;
        }

        if (isEffectActive) EndCurrentEffect();
        
        curEffect = effect;
        EffectOnQueue = true;
        SFXManager.current.TriggerEffectVoice(curEffect.VoiceSFX);
        DemonHand.Play("SpawnEffect");
        
    }

    public void ApplyEffect()
    {
        if (!EffectOnQueue) return;

        EffectOnQueue = false;
        isEffectActive = true;
        
        switch (curEffect.Type)
        {
            case ChaosEffectType.Material:
                if (tilemapRenderer != null)
                {
                    tilemapRenderer.sharedMaterial = curEffect.effectMaterial;
                }
                break;
            case ChaosEffectType.Object:
                curEffect.effectObject.SetActive(true);
                break;
        }
        
        if (curEffect.useButtonPresses)
        {
            buttonPressesRemaining = curEffect.buttonPressesToEnd;
            currentEffectRemainingSteps = -1;
        }
        else
        {
            int effectSteps = Random.Range(curEffect.durationRange.x, curEffect.durationRange.y + 1);
            currentEffectRemainingSteps = GetStepsInTicks(effectSteps);
            buttonPressesRemaining = 0;
        }
        
        if(curEffect.triggerCustomEvent) curEffect.onStart.Invoke();
        
    }
    
    public void EndCurrentEffect()
    {
        if (!isEffectActive) return;

        if(curEffect.triggerCustomEvent) curEffect.onEnd.Invoke();
        
        switch (curEffect.Type)
        {
            case ChaosEffectType.Material:
                if (tilemapRenderer != null && defaultMaterial != null)
                    tilemapRenderer.sharedMaterial = defaultMaterial;
                
                break;
            case ChaosEffectType.Object:
                curEffect.effectObject.SetActive(false);
                break;
        }
        
        curEffect = null;
        isEffectActive = false;
        currentEffectRemainingSteps = 0;
        buttonPressesRemaining = 0;
        
        int nextEffectSteps = Random.Range(ActiveEffectStepsRange.x, ActiveEffectStepsRange.y + 1);
        stepsUntilNextEffect = GetStepsInTicks(nextEffectSteps);
        
    }
    

    private void OnEffectActionPressed()
    {
        if (!isEffectActive || curEffect == null || !curEffect.useButtonPresses) return;
        
        buttonPressesRemaining--;
        
        if (buttonPressesRemaining <= 0)
        {
            cursesSurvived++;
            EndCurrentEffect();
        }
    }
    
    
    public void SetSystemActive(bool active)
    {
        isSystemActive = active;
        
        if(active) InitializeEffectSystem();
        else StopEffectSystem();
        
    }
    
    public string GetCurrentEffectInfo()
    {
        if (!isEffectActive || curEffect == null) return "No effect active";
        
        if (curEffect.useButtonPresses)
        {
            return $"{curEffect.effectName}: {buttonPressesRemaining} button presses remaining";
        }
        else
        {
            return $"{curEffect.effectName}: {currentEffectRemainingSteps} ticks remaining";
        }
    }
    
    public int GetCurses(){ return cursesSurvived;}
    
    #if UNITY_EDITOR
    
    #region DEBUG
    
    [FoldoutGroup("DEBUG"),Button("Trigger Effect by Name")]
    public void DEBUG_TriggerEffectByName(string effectName)
    {
        ChaosEffect effect = availableEffects.Find(e => e.effectName.Equals(effectName, StringComparison.OrdinalIgnoreCase));
        
        if (effect != null)
        {
            StartEffect(effect);
        }
        else
        {
            Debug.LogWarning($"Effect '{effectName}' not found!");
        }
    }
    
    [FoldoutGroup("DEBUG"),Button("Trigger Random Effect")]
    private void DEBUG_TriggerRandomEffect()
    {
        TriggerRandomEffect();
    }
    
    [FoldoutGroup("DEBUG"),Button("End Current Effect")]
    private void DEBUG_EndCurrentEffect()
    {
        EndCurrentEffect();
    }
    
    [FoldoutGroup("DEBUG"),Button("Setup Default Effects")]
    private void DEBUG_SetupDefaultEffects()
    {
        // Load materials from your 00_SpriteMaterials folder
        Material cleanMat = Resources.Load<Material>("02_Art/02_Materials/00_SpriteMaterials/CleanMaterial");
        Material clipMat = Resources.Load<Material>("02_Art/02_Materials/00_SpriteMaterials/ClipingEffect");
        Material fadeMat = Resources.Load<Material>("02_Art/02_Materials/00_SpriteMaterials/FadeEffect");
        Material flickerMat = Resources.Load<Material>("02_Art/02_Materials/00_SpriteMaterials/FlickerEffect");
        Material frozeMat = Resources.Load<Material>("02_Art/02_Materials/00_SpriteMaterials/FrozeEffect");
        Material greyMat = Resources.Load<Material>("02_Art/02_Materials/00_SpriteMaterials/GreyBlocksEffect");
        
        availableEffects.Clear();
        
        // Add default effects
        availableEffects.Add(new ChaosEffect
        {
            effectName = "Clipping Chaos",
            effectMaterial = clipMat,
            durationRange = new Vector2Int(8, 15),
            description = "Parts of blocks appear to clip, making placement confusing."
        });
        
        availableEffects.Add(new ChaosEffect
        {
            effectName = "Fade Madness",
            effectMaterial = fadeMat,
            durationRange = new Vector2Int(6, 12),
            description = "Blocks fade in and out, making the grid harder to read."
        });
        
        availableEffects.Add(new ChaosEffect
        {
            effectName = "Flicker Storm",
            effectMaterial = flickerMat,
            durationRange = new Vector2Int(5, 10),
            description = "Rapid flickering effect disrupts visual clarity."
        });
        
        availableEffects.Add(new ChaosEffect
        {
            effectName = "Frozen Vision",
            effectMaterial = frozeMat,
            useButtonPresses = true,
            buttonPressesToEnd = 3,
            description = "Freezing effect that requires button presses to clear."
        });
        
        availableEffects.Add(new ChaosEffect
        {
            effectName = "Grey Scale",
            effectMaterial = greyMat,
            durationRange = new Vector2Int(10, 18),
            description = "All blocks appear in grey scale, removing color cues."
        });
        
        Debug.Log("Default chaos effects setup complete!");
    }

    [FoldoutGroup("DEBUG"), Button("Add new MaterialEffect")]
    private void DEBUG_AddMaterialEffectToList()
    {
        availableEffects.Add(new ChaosEffect
        {
            effectName = "New MaterialEffect",
            effectMaterial = null,
            durationRange = new Vector2Int(8, 15),
            description = "Add description of the effect"
        });
    }
    
    [FoldoutGroup("DEBUG"), Button("Add new ObjectEffect")]
    private void DEBUG_AddObjectEffectToList()
    {
        availableEffects.Add(new ChaosEffect
        {
            effectName = "New ObjectEffect",
            effectObject = null,
            durationRange = new Vector2Int(8, 15),
            description = "Add description of the effect"
        });
    }
    
    #endregion
    
    #endif
}

