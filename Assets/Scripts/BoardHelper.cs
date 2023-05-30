using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class BoardHelper : MonoBehaviour
{
    public enum Cell : byte
    {
        Empty,
        PawnA,
        PawnB,
        PawnAVis,
        PawnBVis,
        OutOfBoard
    }

    public static BoardHelper Instance { get; private set; }
    List<GameObject> pawnGameObjects = new List<GameObject>();
    GameObject visualisation;
    Vector2Int visualisationPos = new Vector2Int(-1, -1);
    GameRules gameRules;
    UIController uI;
    public bool GameStopped { get; private set; } = false;
    public Vector2Int LastMove { get; private set; }
    public Cell[][] Board { get; private set; }

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

        Board = new Cell[15][];
        for (int i = 0; i < 15; i++)
        {
            Board[i] = new Cell[15];
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

        if (realPos.x < -7)
            realPos.x = -7;
        else if (realPos.x > 7)
            realPos.x = 7;

        if (realPos.y < -7)
            realPos.y = -7;
        else if (realPos.y > 7)
            realPos.y = 7;

        return realPos;
    }

    public Vector2 BoardToRealPos(Vector2Int pos)
    {
        Vector2 realPos = new Vector2(
            (pos.x - 7) * cellSize,
            (pos.y - 7) * cellSize);
        return realPos;
    }

    public Cell GetCell(Vector2Int pos)
    {
        if (pos.x >= 0 && pos.x < 15 && pos.y >= 0 && pos.y < 15)
            return Board[pos.x][pos.y];
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
            visualisation = Instantiate(pawnPrefabs[(int)pawn + 1], BoardToRealPos(pos), Quaternion.identity);
        }
    }

    public void PlacePawn(Cell pawn, Vector2Int pos)
    {
        if (Board[pos.x][pos.y] != Cell.Empty || pawn == Cell.Empty)
            return;

        if (pawn == Cell.PawnA || pawn == Cell.PawnB)
        {
            Board[pos.x][pos.y] = pawn;
            LastMove = pos;
            pawnGameObjects.Add(Instantiate(pawnPrefabs[(int)pawn - 1], BoardToRealPos(pos), Quaternion.identity));

            if (visualisation != null)
                Destroy(visualisation);
            visualisationPos.Set(-1, -1);
            List<Vector2Int> winningPositions;
            winningPositions = gameRules.CheckWin(pos);
            if (winningPositions != null)
            {
                GameStopped = true;
                foreach (Vector2Int winningPos in winningPositions)
                    pawnGameObjects.Add(Instantiate(pawnPrefabs[(int)pawn + 3], BoardToRealPos(winningPos), Quaternion.identity));
                uI.ToggleEndScreen();
            }
            gameRules.SwitchPlayer();
        }
    }

    public List<Vector2Int> GetPossibleMoves(int proximity)
    {
        List<Vector2Int> possibleMoves = new List<Vector2Int>();

        if (GameStopped)
            return possibleMoves;

        for (int x = 0; x < 15; x++)
            for (int y = 0; y < 15; y++)
                if (Board[x][y] == Cell.Empty)
                {
                    if(CheckNeighbors(new Vector2Int(x,y), proximity))
                        possibleMoves.Add(new Vector2Int(x, y));
                }

        Debug.Log($"Iloœæ ruchów: {possibleMoves.Count}");
        return possibleMoves;
    }

    bool CheckNeighbors(Vector2Int pos, int range)
    {
        for (int x = pos.x - range; x <= pos.x + range; x++)
        {
            for (int y = pos.y - range; y <= pos.y + range; y++)
            {
                if (x < 0 || y < 0 || x >= 15 || y >= 15)
                    continue;

                if (Board[x][y] == Cell.PawnA || Board[x][y] == Cell.PawnB)
                    return true;
            }
        }
        return false;
    }


    public void ResetBoard()
    {
        foreach (GameObject pawn in pawnGameObjects)
            Destroy(pawn);

        for (int x = 0; x < 15; x++)
        {
            for (int y = 0; y < 15; y++)
                Board[x][y] = Cell.Empty;
        }

        GameStopped = false;
        gameRules.CurrentEnemy = GameRules.Enemy.None;
    }

    public void UndoAIMove(Vector2Int tempMove)
    {
        Board[tempMove.x][tempMove.y] = Cell.Empty;
    }

    public void TempAIMove(Vector2Int tempMove, Cell player)
    {
        Board[tempMove.x][tempMove.y] = player;
    }
}
