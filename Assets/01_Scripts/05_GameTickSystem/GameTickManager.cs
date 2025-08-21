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


    private void Start()
    {
        StartCoroutine(StartGameTicks());
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
