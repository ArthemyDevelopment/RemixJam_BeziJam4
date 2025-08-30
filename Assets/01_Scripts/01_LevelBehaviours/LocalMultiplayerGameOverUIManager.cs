using Sirenix.OdinInspector;
using UnityEngine;

public class LocalMultiplayerGameOverUIManager : GameOverManager
{
    [BoxGroup("References"), SerializeField] private TetrisMapBoard Player1Board;
    [BoxGroup("References"), SerializeField] private TetrisMapBoard Player2Board;

    private bool doesPlayer1Loose;
    private bool doesPlayer2Loose;


    public override void SetGameOver()
    {
        base.SetGameOver();
        if (!Player1Board.IsBoardActive)
        {
            //TODO: Set Player 1 loose canvas
            SetGameOverUI(1);
            doesPlayer1Loose = true;
        }

        if (!Player2Board.IsBoardActive)
        {
            //TODO: Set Player 2 loose canvas
            SetGameOverUI(2);
            doesPlayer2Loose = true;
        }
        
        if (doesPlayer1Loose && doesPlayer2Loose)
        {
            //Check and declaredWinner
        }
    }


    public void SetGameOverUI(int player)
    {
        switch (player)
        {
            case 1:
                
                break;
            
            case 2:
                break;
        }

        
            
    } 
    



}
