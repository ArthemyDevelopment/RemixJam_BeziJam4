using ArthemyDev.ScriptsTools;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class GameOverManager : SingletonManager<GameOverManager>
{

    [SerializeField] private UnityEvent OnGameOver;
    
    public void RestartGame()
    {
        SceneManager.LoadScene((int)GameScenes.GameScene);
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadScene((int)GameScenes.MainMenu);
    }
    
    
    [BoxGroup("DEBUG"), Button("Trigger Game Over")]
    public virtual void SetGameOver()
    {
        OnGameOver.Invoke();
    }
    
    
}
