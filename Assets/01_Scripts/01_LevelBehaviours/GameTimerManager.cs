using System;
using System.Collections;
using ArthemyDev.ScriptsTools;
using Sirenix.OdinInspector;
using UnityEngine;

public class GameTimerManager : SingletonManager<GameTimerManager>
{

    [BoxGroup("DEBUG"), SerializeField, ReadOnly] private bool isTimerActive;
    [BoxGroup("DEBUG"), SerializeField, ReadOnly] private int totalSeconds;

    private Coroutine timerCoroutine;

    private void Start()
    {
        totalSeconds = 0;
        SetTimer(true);
    }


    public void SetTimer(bool status)
    {
        switch (status)
        {
            case true:
                isTimerActive = true;
                timerCoroutine = StartCoroutine(Timer());
                break;
            case false:
                StopCoroutine(Timer());
                isTimerActive = false;
                break;
        }
    }
    
    public int GetSeconds(){return totalSeconds;}

    IEnumerator Timer()
    {
        while (isTimerActive)
        {
            yield return ScriptsTools.GetWait(1);
            totalSeconds++;
        }
    }
    



}
