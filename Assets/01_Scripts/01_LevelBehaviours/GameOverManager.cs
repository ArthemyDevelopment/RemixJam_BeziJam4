using ArthemyDev.ScriptsTools;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;

public class GameOverManager : SingletonManager<GameOverManager>
{
    [BoxGroup("References")]public PieceController pieceController;
    [BoxGroup("References")]public TetrisMapBoard board;
    [BoxGroup("References")]public GameObject GameOverCanvas;
    [BoxGroup("References")]public GameObject FirstSelectedButton;


    protected override void Awake()
    {
        base.Awake();
        GameOverCanvas.SetActive(false);
    }

    
    [BoxGroup("DEBUG"), Button("Trigger Game Over")]
    public void SetGameOver()
    {
        GameTickManager.current.StopGameTicks();
        //PieceController.
        ChaosEffectsManager.current.SetSystemActive(false);
        GameOverCanvas.SetActive(true);
        EventSystem.current.SetSelectedGameObject(FirstSelectedButton);
    }
    
    
}
