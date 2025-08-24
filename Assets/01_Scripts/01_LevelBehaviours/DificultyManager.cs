using ArthemyDev.ScriptsTools;
using Sirenix.OdinInspector;
using UnityEngine;

public class DificultyManager : SingletonManager<DificultyManager>
{
    [FoldoutGroup("Settings")]
    public int LinesToIncreaseDificutly = 10;
    
    [FoldoutGroup("Settings")]
    [SerializeField] private PieceController pieceController;
    
    [FoldoutGroup("Game Speed")]
    [SerializeField] private int startingTicksToStep = 20;
    
    [FoldoutGroup("Game Speed")]
    [SerializeField] private int minimumTicksToStep = 5;
    
    [FoldoutGroup("Game Speed")]
    [SerializeField] private int ticksReductionPerLevel = 2;

    [FoldoutGroup("DEBUG")]
    [ReadOnly] [SerializeField] private int curLineCheck;
    
    [FoldoutGroup("DEBUG")]
    [ReadOnly] [SerializeField] private int currentTicksToStep;
    
    [FoldoutGroup("DEBUG")]
    [ReadOnly] [SerializeField] private int currentLevel = 0;
    [FoldoutGroup("DEBUG")]
    [ReadOnly] [SerializeField] private int totalLines = 0;

    // Public property to access current ticks to step
    public int CurrentTicksToStep => currentTicksToStep;

    protected override void Awake()
    {
        base.Awake();
        currentTicksToStep = startingTicksToStep;
    }

    public void CheckDificultIncrease(int linesCleared)
    {
        curLineCheck += linesCleared;
        totalLines += linesCleared;

        if (curLineCheck >= LinesToIncreaseDificutly)
        {
            IncreaseDifficulty();
        }

        curLineCheck = ScriptsTools.WrapInt(curLineCheck, 0, LinesToIncreaseDificutly);
    }
    
    private void IncreaseDifficulty()
    {
        currentLevel++;
        
        // Reduce ticks to step (increase speed)
        int newTicksToStep = startingTicksToStep - (currentLevel * ticksReductionPerLevel);
        currentTicksToStep = Mathf.Max(minimumTicksToStep, newTicksToStep);
        
        // Update piece controller if using old method
        if (pieceController != null)
        {
            pieceController.ChangeSpeed();
        }
        
        Debug.Log($"Difficulty increased to level {currentLevel}! Ticks to step: {currentTicksToStep}");
    }

    public int GetLevel() { return currentLevel; }
    public int GetLines() { return totalLines; }

#if UNITY_EDITOR
    
    #region DEBUG
    
    [FoldoutGroup("DEBUG")][Button("Increase Difficulty")]
    private void DEBUG_IncreaseDifficulty()
    {
        IncreaseDifficulty();
    }
    
    [FoldoutGroup("DEBUG")][Button("Reset Difficulty")]
    private void DEBUG_ResetDifficulty()
    {
        currentLevel = 0;
        curLineCheck = 0;
        currentTicksToStep = startingTicksToStep;
        Debug.Log("Difficulty reset to starting level.");
    }
    
    #endregion
    
    #endif
}
