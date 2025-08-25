using ArthemyDev.ScriptsTools;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class GameOverManager : SingletonManager<GameOverManager>
{

    public void RestartGame()
    {
        SceneManager.LoadScene((int)GameScenes.GameScene);
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadScene((int)GameScenes.MainMenu);
    }
    
    
    [BoxGroup("DEBUG"), Button("Trigger Game Over")]
    public void SetGameOver()
    {
        GameTickManager.current.StopGameTicks();
        ChaosEffectsManager.current.SetSystemActive(false);
        GameTimerManager.current.SetTimer(false);
        GameUIManager.current.SetGameOverUI();
    }
    
    
}
