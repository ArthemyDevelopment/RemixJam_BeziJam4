using System;
using System.Collections;
using ArthemyDev.ScriptsTools;
using UnityEngine;

public class GameTickManager : SingletonManager<GameTickManager>
{
    public bool GameisPlaying=true;
    public float GameTickTime;
    public delegate void GameTick();

    public GameTick OnGameTick;

    private Coroutine gameTickCoroutine;
    

    private void Start()
    {
        gameTickCoroutine=StartCoroutine(StartGameTicks());
    }

    public void StopGameTicks()
    {
        StopCoroutine(gameTickCoroutine);
    }


    IEnumerator StartGameTicks()
    {
        while (GameisPlaying)
        {
            OnGameTick?.Invoke();
            yield return ScriptsTools.GetWait(GameTickTime);
        }
    }
    
    



}
