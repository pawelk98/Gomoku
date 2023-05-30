using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseController : MonoBehaviour
{
    Vector2 mousePos;
    BoardHelper board;
    GameRules gameRules;

    void Start()
    {
        board = BoardHelper.Instance;
        gameRules = GameRules.Instance;
    }

    void Update()
    {
        if (board.GameStopped)
            return;
        if (gameRules.CurrentEnemy == GameRules.Enemy.None)
            return;
        if (gameRules.CurrentEnemy != GameRules.Enemy.Human && gameRules.CurrentPlayer == BoardHelper.Cell.PawnB)
            return;

        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        board.MouseHover(gameRules.CurrentPlayer, board.ApproxBoardPos(mousePos));
        if(Input.GetMouseButtonDown(0))
            board.PlacePawn(gameRules.CurrentPlayer, board.ApproxBoardPos(mousePos));
    }
}
