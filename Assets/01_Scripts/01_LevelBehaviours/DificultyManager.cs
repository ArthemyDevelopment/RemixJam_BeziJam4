using ArthemyDev.ScriptsTools;
using UnityEngine;

public class DificultyManager : SingletonManager<DificultyManager>
{
    public int LinesToIncreaseDificutly;
    [SerializeField]private PieceController pieceController;

    private int curLineCheck;


    public void CheckDificultIncrease(int linesCleared)
    {
        curLineCheck+=linesCleared;

        if (curLineCheck >= LinesToIncreaseDificutly)
        {
            pieceController.ChangeSpeed();
        }

        curLineCheck = ScriptsTools.WrapInt(curLineCheck, 0, LinesToIncreaseDificutly);
    }

}
