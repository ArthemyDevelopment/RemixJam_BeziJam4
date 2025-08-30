using System;
using System.Collections;
using ArthemyDev.ScriptsTools;
using Sirenix.OdinInspector;
using UnityEngine;


[DefaultExecutionOrder(-5)]
public class GameTickManager : SingletonManager<GameTickManager>
{
    [BoxGroup("GameTicks Values")]public bool GameIsPlaying=true;
    [BoxGroup("GameTicks Values")]public float GameTickTime;
    public delegate void GameTick();
    [HideInInspector]public GameTick OnGameTick;
    [HideInInspector]public GameTick OnUiGameTick;

    private Coroutine gameTickCoroutine;
    

    private void Start()
    {
        gameTickCoroutine=StartCoroutine(StartGameTicks());
    }

    public void StopGameTicks()
    {
        GameIsPlaying = false;
        if(gameTickCoroutine!=null)StopCoroutine(gameTickCoroutine);
    }


    IEnumerator StartGameTicks()
    {
        while (GameIsPlaying)
        {
            OnGameTick?.Invoke();
            OnUiGameTick?.Invoke();
            yield return ScriptsTools.GetWait(GameTickTime);
        }
    }
    
    



}
