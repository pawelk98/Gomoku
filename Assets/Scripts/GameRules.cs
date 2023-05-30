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

    public enum Orientation
    {
        Horizontal,
        Vertical,
        DiagonalUp,
        DiagonalDown,
    }

    public static GameRules Instance { get; private set; }
    
    public Enemy CurrentEnemy { get; set; }
    public BoardHelper.Cell CurrentPlayer { get; private set; } = BoardHelper.Cell.PawnA; 
    BoardHelper boardHelper;
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
        boardHelper = BoardHelper.Instance;
        aI = AIController.Instance;
    }

    public void SwitchPlayer()
    {
        if(boardHelper.GameStopped) 
            return;

        if (CurrentPlayer == BoardHelper.Cell.PawnB)
        {
            CurrentPlayer = BoardHelper.Cell.PawnA;
            return;
        }

        CurrentPlayer = BoardHelper.Cell.PawnB;
        if (CurrentEnemy != Enemy.Human)
        {
            aI.MakeMove();
            CurrentPlayer = BoardHelper.Cell.PawnA;
        }
    }

    public void SetFirstPlayer()
    {
        CurrentPlayer = (BoardHelper.Cell)Random.Range(1, 3);
        if (CurrentPlayer == BoardHelper.Cell.PawnB && CurrentEnemy != Enemy.Human)
            AIController.Instance.MakeMove();
    }

    public List<Vector2Int> CheckWin(Vector2Int lastPlaced)
    {
        List<Vector2Int> winningPawns;
        List<Vector2Int> inARow;

        inARow = GetColumnData(lastPlaced, Orientation.Horizontal)[0];
        if (inARow.Count == 5)
        {
            winningPawns = new List<Vector2Int>(inARow);
            return winningPawns;
        }
        inARow = GetColumnData(lastPlaced, Orientation.Vertical)[0];
        if (inARow.Count == 5)
        {
            winningPawns = new List<Vector2Int>(inARow);
            return winningPawns;
        }
        inARow = GetColumnData(lastPlaced, Orientation.DiagonalUp)[0];
        if (inARow.Count == 5)
        {
            winningPawns = new List<Vector2Int>(inARow);
            return winningPawns;
        }
        inARow = GetColumnData(lastPlaced, Orientation.DiagonalDown)[0];
        if (inARow.Count == 5)
        {
            winningPawns = new List<Vector2Int>(inARow);
            return winningPawns;
        }

        return null;
    }

    public List<Vector2Int>[] GetColumnData (Vector2Int lastPlaced, Orientation orientation)
    {
        BoardHelper.Cell player = boardHelper.GetCell(lastPlaced);
        List<Vector2Int>[] columnData = new List<Vector2Int>[4];
        columnData[0] = new List<Vector2Int> { lastPlaced };    //current column length
        columnData[1] = new List<Vector2Int>(); //free or players side 1
        columnData[2] = new List<Vector2Int>(); //free or players side 2
        columnData[3] = new List<Vector2Int>(); //not connected pawns in range
        int posX, posY, cellCount;

        switch (orientation)
        {
            case Orientation.Horizontal:
                posX = lastPlaced.x - 1;
                cellCount = 0;
                while (GetCell(posX, lastPlaced.y) == player)
                {
                    columnData[0].Add(new Vector2Int(posX, lastPlaced.y));
                    posX--;
                    cellCount++;
                }
                while (GetCell(posX, lastPlaced.y) == BoardHelper.Cell.Empty || GetCell(posX, lastPlaced.y) == player)
                {
                    columnData[1].Add(new Vector2Int(posX, lastPlaced.y));
                    if (cellCount < 4 && GetCell(posX, lastPlaced.y) == player)
                        columnData[3].Add(new Vector2Int(posX, lastPlaced.y));
                    posX--;
                    cellCount++;
                }
                cellCount = 0;
                posX = lastPlaced.x + 1;
                while (GetCell(posX, lastPlaced.y) == player)
                {
                    columnData[0].Add(new Vector2Int(posX, lastPlaced.y));
                    posX++;
                    cellCount++;
                }
                while (cellCount < 4 && (GetCell(posX, lastPlaced.y) == BoardHelper.Cell.Empty || GetCell(posX, lastPlaced.y) == player))
                {
                    columnData[2].Add(new Vector2Int(posX, lastPlaced.y));
                    if (cellCount < 4 && GetCell(posX, lastPlaced.y) == player)
                        columnData[3].Add(new Vector2Int(posX, lastPlaced.y));
                    posX++;
                    cellCount++;
                }
                break;
            case Orientation.Vertical:
                posY = lastPlaced.y - 1;
                cellCount = 0;
                while (GetCell(lastPlaced.x, posY) == player)
                {
                    columnData[0].Add(new Vector2Int(lastPlaced.x, posY));
                    posY--;
                    cellCount++;
                }
                while (GetCell(lastPlaced.x, posY) == BoardHelper.Cell.Empty || GetCell(lastPlaced.x, posY) == player)
                {
                    columnData[1].Add(new Vector2Int(lastPlaced.x, posY));
                    if (cellCount < 4 && GetCell(lastPlaced.x, posY) == player)
                        columnData[3].Add(new Vector2Int(lastPlaced.x, posY));
                    posY--;
                    cellCount++;
                }
                posY = lastPlaced.y + 1;
                cellCount = 0;
                while (GetCell(lastPlaced.x, posY) == player)
                {
                    columnData[0].Add(new Vector2Int(lastPlaced.x, posY));
                    posY++;
                    cellCount++;
                }
                while (GetCell(lastPlaced.x, posY) == BoardHelper.Cell.Empty || GetCell(lastPlaced.x, posY) == player)
                {
                    columnData[2].Add(new Vector2Int(lastPlaced.x, posY));
                    if (cellCount < 4 && GetCell(lastPlaced.x, posY) == player)
                        columnData[3].Add(new Vector2Int(lastPlaced.x, posY));
                    posY++;
                    cellCount++;
                }
                break;
            case Orientation.DiagonalUp:
                posX = lastPlaced.x - 1;
                posY = lastPlaced.y - 1;
                cellCount = 0;
                while (GetCell(posX, posY) == player)
                {
                    columnData[0].Add(new Vector2Int(posX, posY));
                    posX--;
                    posY--;
                    cellCount++;
                }
                while (GetCell(posX, posY) == BoardHelper.Cell.Empty || GetCell(posX, posY) == player)
                {
                    columnData[1].Add(new Vector2Int(posX, posY));
                    if (cellCount < 4 && GetCell(posX, posY) == player)
                        columnData[3].Add(new Vector2Int(posX, posY));
                    posX--;
                    posY--;
                    cellCount++;
                }
                posX = lastPlaced.x + 1;
                posY = lastPlaced.y + 1;
                cellCount = 0;
                while (GetCell(posX, posY) == player)
                {
                    columnData[0].Add(new Vector2Int(posX, posY));
                    posX++;
                    posY++;
                    cellCount++;
                }
                while (GetCell(posX, posY) == BoardHelper.Cell.Empty || GetCell(posX, posY) == player)
                {
                    columnData[2].Add(new Vector2Int(posX, posY));
                    if (cellCount < 4 && GetCell(posX, posY) == player)
                        columnData[3].Add(new Vector2Int(posX, posY));
                    posX++;
                    posY++;
                    cellCount++;
                }
                break;
            case Orientation.DiagonalDown:
                posX = lastPlaced.x + 1;
                posY = lastPlaced.y - 1;
                cellCount = 0;
                while (GetCell(posX, posY) == player)
                {
                    columnData[0].Add(new Vector2Int(posX, posY));
                    posX++;
                    posY--;
                    cellCount++;
                }
                while (GetCell(posX, posY) == BoardHelper.Cell.Empty || GetCell(posX, posY) == player)
                {
                    columnData[1].Add(new Vector2Int(posX, posY));
                    if (cellCount < 4 && GetCell(posX, posY) == player)
                        columnData[3].Add(new Vector2Int(posX, posY));
                    posX++;
                    posY--;
                    cellCount++;
                }
                posX = lastPlaced.x - 1;
                posY = lastPlaced.y + 1;
                cellCount = 0;
                while (GetCell(posX, posY) == player)
                {
                    columnData[0].Add(new Vector2Int(posX, posY));
                    posX--;
                    posY++;
                    cellCount++;
                }
                while (GetCell(posX, posY) == BoardHelper.Cell.Empty || GetCell(posX, posY) == player)
                {
                    columnData[2].Add(new Vector2Int(posX, posY));
                    if (cellCount < 4 && GetCell(posX, posY) == player)
                        columnData[3].Add(new Vector2Int(posX, posY));
                    posX--;
                    posY++;
                    cellCount++;
                }
                break;
        }
        return columnData;
    }

    BoardHelper.Cell GetCell(int posX, int posY)
    {
        return boardHelper.GetCell(new Vector2Int(posX, posY));
    }
}
