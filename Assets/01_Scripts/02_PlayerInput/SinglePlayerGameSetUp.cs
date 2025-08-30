using System;
using Sirenix.OdinInspector;
using UnityEngine;

public class SinglePlayerGameSetUp : MonoBehaviour
{
    [BoxGroup("References"), SerializeField] private TetrisInputHandler inputHandler;

    [BoxGroup("References"), SerializeField] private PieceController playerController;
    [BoxGroup("References"), SerializeField] private TetrisMapBoard boardController;


    private void Start()
    {
        playerController.SetUpPlayer(inputHandler);
        boardController.StartBoard();
    }
}
