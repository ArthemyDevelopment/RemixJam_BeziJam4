using System;
using ArthemyDev.ScriptsTools;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class SinglePlayerGameOverUIManager : GameOverManager
{
    [FoldoutGroup("References Game Over UI"), SerializeField] private GameObject GameOverCanvas;
    [FoldoutGroup("References Game Over UI"), SerializeField] private TMP_Text GameOverLevel;
    [FoldoutGroup("References Game Over UI"), SerializeField] private TMP_Text GameOverLines;
    [FoldoutGroup("References Game Over UI"), SerializeField] private TMP_Text GameOverCurses;
    [FoldoutGroup("References Game Over UI"), SerializeField] private TMP_Text GameOverTime;
    [FoldoutGroup("References Game Over UI"), SerializeField]public GameObject FirstSelectedButton;


    private void OnEnable()
    {
        GameOverCanvas.SetActive(false);
    }

    public override void SetGameOver()
    {
        base.SetGameOver();
        GameTickManager.current.StopGameTicks();
        ChaosEffectsManager.current.SetSystemActive(false);
        GameTimerManager.current.SetTimer(false);
        SetGameOverUI();
    }
    
    public void SetGameOverUI()
    {
        GameOverLevel.text = DificultyManager.current.GetLevel().ToString();
        GameOverLines.text = DificultyManager.current.GetLines().ToString();
        GameOverCurses.text = ChaosEffectsManager.current.GetCurses().ToString();
        GameOverTime.text = ScriptsTools.FormatTime(GameTimerManager.current.GetSeconds());
        GameOverCanvas.SetActive(true);
        EventSystem.current.SetSelectedGameObject(FirstSelectedButton);
    }
}
