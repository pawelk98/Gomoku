using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameRules : MonoBehaviour
{
    public enum Enemy
    {
        None,
        Human,
        AIEasy,
        AIMedium,
        AIHard
    }

    public static GameRules Instance { get; private set; }
    
    public Enemy CurrentEnemy { get; set; }
    public Board.Cell CurrentPlayer { get; private set; } = Board.Cell.PawnA; 
    Board board;
    AIController aI;

    void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;
    }

    void Start()
    {
        board = Board.Instance;
        aI = AIController.Instance;
    }

    public void SwitchPlayer()
    {
        if(board.GameStopped) 
            return;

        if (CurrentPlayer == Board.Cell.PawnA)
        {
            CurrentPlayer = Board.Cell.PawnB;
            if (CurrentEnemy != Enemy.Human)
            {
                aI.MakeMove();
                SwitchPlayer();
            }
        }
        else
            CurrentPlayer = Board.Cell.PawnA;
    }

    public void SetFirstPlayer()
    {
        CurrentPlayer = (Board.Cell)Random.Range(1, 3);
    }

    public List<Vector2Int> CheckWin(Vector2Int lastPlaced)
    {
        List<Vector2Int> winningPawns = new List<Vector2Int>();

        //horizontal
        List<Vector2Int> inARow = new List<Vector2Int>();
        for (int i = 0; i < 15; i++)
        {
            Vector2Int pawnPos = new Vector2Int(i, lastPlaced.y);
            if (checkCell(pawnPos))
                inARow.Add(pawnPos);
            else
                inARow.Clear();

            if (inARow.Count == 5)
                winningPawns = new List<Vector2Int>(inARow);
            else if (inARow.Count > 5)
                winningPawns.Clear();
        }

        //vertical
        inARow.Clear();
        for (int i = 0; i < 15; i++)
        {
            Vector2Int pawnPos = new Vector2Int(lastPlaced.x, i);
            if (checkCell(pawnPos))
                inARow.Add(pawnPos);
            else
                inARow.Clear();

            if (inARow.Count == 5)
                winningPawns = new List<Vector2Int>(inARow);
            else if (inARow.Count > 5)
                winningPawns.Clear();
        }

        //diagonal up
        inARow.Clear();
        for (int i = 0; i < 15; i++)
        {
            Vector2Int pawnPos = new Vector2Int(lastPlaced.x - lastPlaced.y + i, i);
            if (checkCell(pawnPos))
                inARow.Add(pawnPos);
            else
                inARow.Clear();

            if (inARow.Count == 5)
                winningPawns = new List<Vector2Int>(inARow);
            else if (inARow.Count > 5)
                winningPawns.Clear();
        }

        //diagonal down
        inARow.Clear();
        for (int i = 0; i < 15; i++)
        {
            Vector2Int pawnPos = new Vector2Int(lastPlaced.x + lastPlaced.y - 14 + i, 14 - i);
            if (checkCell(pawnPos))
                inARow.Add(pawnPos);
            else
                inARow.Clear();

            if (inARow.Count == 5)
                winningPawns = new List<Vector2Int>(inARow);
            else if (inARow.Count > 5)
                winningPawns.Clear();
        }


        if (winningPawns.Count == 5)
        {
            return winningPawns;
        }
        else
            return null;
    }

    bool checkCell(Vector2Int pos)
    {
        if (board.GetCell(pos) == CurrentPlayer)
            return true;
        else
            return false;
    }
}
