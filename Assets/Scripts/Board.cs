using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class Board : MonoBehaviour
{
    public enum Cell
    {
        Empty,
        PawnA,
        PawnB,
        PawnAVis,
        PawnBVis,
        OutOfBoard
    }

    public static Board Instance { get; private set; }
    Cell[][] board;
    List<GameObject> pawnGameObjects = new List<GameObject>();
    GameObject visualisation;
    Vector2Int visualisationPos = new Vector2Int(-1,-1);
    GameRules gameRules;
    UIController uI;
    public bool GameStopped { get; private set; } = false;


    [SerializeField]
    float cellSize;
    [SerializeField]
    GameObject[] pawnPrefabs;

    void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;

        board = new Cell[15][];
        for (int i = 0; i < 15; i++)
        {
            board[i] = new Cell[15];
        }
    }

    private void Start()
    {
        gameRules = GameRules.Instance;
        uI = UIController.Instance;
    }

    public Vector2Int ApproxBoardPos(Vector2 pos)
    {
        Vector2Int newPos = ApproxRealPos(pos);
        newPos.x += 7; 
        newPos.y += 7;
        return newPos;
    }

    Vector2Int ApproxRealPos(Vector2 pos)
    {
        Vector2Int realPos = new Vector2Int(
            Mathf.RoundToInt(pos.x / cellSize),
            Mathf.RoundToInt(pos.y / cellSize));

        if(realPos.x < -7)
            realPos.x = -7;
        else if(realPos.x > 7)
            realPos.x = 7;

        if (realPos.y < -7)
            realPos.y = -7;
        else if (realPos.y > 7)
            realPos.y = 7;

        return realPos;
    }

    public Vector2 BoardToRealPos(int x, int y)
    {
        Vector2 pos = new Vector2(
            (x - 7) * cellSize,
            (y - 7) * cellSize);
        return pos;
    }

    public Cell GetCell(Vector2Int pos)
    {
        if (pos.x >= 0 && pos.x < 15 && pos.y >= 0 && pos.y < 15)
            return board[pos.x][pos.y];
        else
            return Cell.OutOfBoard;
    }
    
    public void MouseHover(Cell pawn, Vector2Int pos)
    {
        if (GetCell(pos) != Cell.Empty)
        {
            Destroy(visualisation);
            visualisationPos.Set(-1, -1);
            return;
        }

        if (visualisationPos.x != pos.x || visualisationPos.y != pos.y)
        {
            if (visualisation != null)
                Destroy(visualisation);
            visualisationPos.Set(pos.x, pos.y);
            visualisation = Instantiate(pawnPrefabs[(int)pawn + 1], BoardToRealPos(pos.x, pos.y), Quaternion.identity);
        }
    }

    public void PlacePawn(Cell pawn, Vector2Int pos, bool temp = false)
    {
        if (board[pos.x][pos.y] != Cell.Empty || pawn == Cell.Empty)
            return;

        if (pawn == Cell.PawnA || pawn == Cell.PawnB)
        {
            board[pos.x][pos.y] = pawn;
            if(!temp)
                pawnGameObjects.Add(Instantiate(pawnPrefabs[(int)pawn - 1], BoardToRealPos(pos.x, pos.y), Quaternion.identity));
            if(visualisation != null)
                Destroy(visualisation);
            visualisationPos.Set(-1, -1);
            List<Vector2Int> winningPositions;
            winningPositions = gameRules.CheckWin(pos);
            if (winningPositions != null)
            {
                GameStopped = true;
                foreach (Vector2Int winningPos in winningPositions)
                    if (!temp)
                    {
                        pawnGameObjects.Add(Instantiate(pawnPrefabs[(int)pawn + 3], BoardToRealPos(winningPos.x, winningPos.y), Quaternion.identity));
                        uI.ToggleEndScreen();
                    }
            }    
            gameRules.SwitchPlayer();
        }
    }

    public void UndoTempMove(Vector2Int pos)
    {
        board[pos.x][pos.y] = Cell.Empty;
        GameStopped = false;
    }

    public List<Vector2Int> GetPossibleMoves()
    {
        List<Vector2Int> possibleMoves = new List<Vector2Int>();

        if (GameStopped)
            return possibleMoves;

        for (int x = 0; x < 15; x++)
            for (int y = 0; y < 15; y++)
                if (board[x][y] == Cell.Empty)
                    possibleMoves.Add(new Vector2Int(x, y));

        return possibleMoves;
    }

    public void ResetBoard()
    {
        foreach (GameObject pawn in pawnGameObjects)
            Destroy(pawn);

        for (int x = 0; x < 15; x++)
        {
            for(int y = 0; y < 15; y++)
                board[x][y] = Cell.Empty;
        }

        GameStopped = false;
        gameRules.CurrentEnemy = GameRules.Enemy.None;
        gameRules.SetFirstPlayer();
    }
}
