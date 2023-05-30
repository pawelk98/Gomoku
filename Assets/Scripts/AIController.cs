using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AIController : MonoBehaviour
{
    public static AIController Instance { get; private set; }
    public static int MinMaxCount { get; private set; } = 0;
    public static int MinMaxAlpha { get; private set; } = 0;
    public static int MinMaxBeta { get; private set; } = 0;
    BoardHelper boardHelper;
    GameRules gameRules;
    [SerializeField]
    int easyDepth;
    [SerializeField]
    int mediumDepth;
    [SerializeField]
    int hardDepth;
    [SerializeField]
    int proximity;
    GameRules.Orientation[] orientations;
    int CurrentDepth { get; set; }
    bool winningMove;


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
        gameRules = GameRules.Instance;
        orientations = new GameRules.Orientation[4];
        orientations[0] = GameRules.Orientation.Horizontal;
        orientations[1] = GameRules.Orientation.Vertical;
        orientations[2] = GameRules.Orientation.DiagonalUp;
        orientations[3] = GameRules.Orientation.DiagonalDown;
    }

    public void MakeMove()
    {
        switch(gameRules.CurrentEnemy)
        {
            case GameRules.Enemy.AIEasy:
                CurrentDepth = easyDepth;
                break;
            case GameRules.Enemy.AIMedium:
                CurrentDepth = mediumDepth;
                break;
            case GameRules.Enemy.AIHard:
                CurrentDepth = hardDepth;
                break;
            default:
                CurrentDepth = 1;
                break;
        }


        winningMove = false;
        MinMaxCount = 0;
        MinMaxAlpha = 0;
        MinMaxBeta = 0;
        var minMaxResult = MinMax(boardHelper.LastMove, CurrentDepth, int.MinValue, int.MaxValue);
        Vector2Int move = minMaxResult.Item2;
        boardHelper.PlacePawn(BoardHelper.Cell.PawnB, move);
        Debug.Log($"MinMax wywo³ano: {MinMaxCount} razy");
        Debug.Log($"Alpha wywo³ano: {MinMaxAlpha} razy");
        Debug.Log($"Beta wywo³ano: {MinMaxBeta} razy");
        Debug.Log($"AIEval: {minMaxResult.Item1}");
    }

    (int, Vector2Int) MinMax(Vector2Int lastMove, int depth, int alpha, int beta)
    {
        MinMaxCount++;
        if (depth == 0 || winningMove)
            return (EvaluateBoard(boardHelper.GetCell(lastMove)), lastMove);
        List<Vector2Int> possibleMoves = boardHelper.GetPossibleMoves(proximity);
        if (possibleMoves.Count == 0)
            possibleMoves.Add(new Vector2Int(UnityEngine.Random.Range(4,10), UnityEngine.Random.Range(4, 10)));

        if (boardHelper.GetCell(lastMove) == BoardHelper.Cell.PawnA ||
            boardHelper.GetCell(lastMove) == BoardHelper.Cell.Empty) //maximizing
        {
            var bestVal = (int.MinValue, Vector2Int.zero);
            foreach (var move in possibleMoves)
            {
                MinMaxAlpha++;
                boardHelper.TempAIMove(move, BoardHelper.Cell.PawnB);
                var tempVal = MinMax(move, depth - 1, alpha, beta);
                if (tempVal.Item1 > bestVal.Item1)
                    bestVal = (tempVal.Item1, move);
                if (bestVal.Item1 > alpha)
                    alpha = bestVal.Item1;
                boardHelper.UndoAIMove(move);
                if (alpha >= beta)
                    break;
            }
            return bestVal;
        }
        else //minimizing
        {
            var bestVal = (int.MaxValue, Vector2Int.zero);
            foreach (var move in possibleMoves)
            {
                MinMaxBeta++;
                boardHelper.TempAIMove(move, BoardHelper.Cell.PawnA);
                var tempVal = MinMax(move, depth - 1, alpha, beta);
                if (tempVal.Item1 < bestVal.Item1)
                    bestVal = (tempVal.Item1, move);
                if (bestVal.Item1 < beta)
                    beta = bestVal.Item1;
                boardHelper.UndoAIMove(move);
                if (alpha >= beta)
                    break;
            }
            return bestVal;
        }
    }

    int EvaluateBoard(BoardHelper.Cell currentPlayer)
    {
        int eval = 0;

        for (int x = 0; x < 15; x++)
            for (int y = 0; y < 15; y++)
                eval += EvaluateCell(new Vector2Int(x, y), currentPlayer);

        return eval;
    }

    int EvaluateCell(Vector2Int pos, BoardHelper.Cell currentPlayer)
    {
        int eval = 0;
        if (boardHelper.GetCell(pos) == BoardHelper.Cell.Empty)
            return eval;
        bool pawnAndRound = currentPlayer == boardHelper.GetCell(pos) ? true : false;

        foreach (var orientation in orientations)
        {
            var columnData = gameRules.GetColumnData(pos, orientation);

            switch (columnData[0].Count)
            {
                case 5:     //won
                    eval += 1000000;
                    break;
                case 4:
                    if(pawnAndRound)
                    { 
                        if (columnData[1].Count >= 1 && columnData[2].Count >= 1)   //win in next 2 rounds
                            eval += 10000;
                        else if (columnData[1].Count + columnData[2].Count >= 1)
                            eval += 4;
                    }
                    else
                    {
                        if (columnData[1].Count + columnData[2].Count >= 1)     //lose in next round
                            eval += 100000;
                    }
                    break;
                case 3:
                    if (pawnAndRound)
                    {
                        if (columnData[1].Count >= 2 && columnData[2].Count >= 2)
                            eval += 6;
                        else if (columnData[1].Count + columnData[2].Count >= 2)
                            eval += 3;
                    }
                    else
                    {
                        if (columnData[1].Count >= 1 && columnData[2].Count >= 1)   //lose in next 2 rounds
                            eval += 1000;
                        else if (columnData[1].Count + columnData[2].Count >= 2)
                            eval += 3;
                    }
                    break;
                case 2:
                    if (columnData[1].Count >= 3 && columnData[2].Count >= 3)
                        eval += 4;
                    else if (columnData[1].Count + columnData[2].Count >= 3)
                        eval += 2;
                    break;
                case 1:
                    if (columnData[1].Count >= 3 && columnData[2].Count >= 4)
                        eval += 2;
                    else if (columnData[1].Count + columnData[2].Count >= 4)
                        eval += 1;
                    break;
                default:
                    eval += 0;
                    break;
            }

        }
        if (boardHelper.GetCell(pos) == BoardHelper.Cell.PawnA)
            eval = -eval;

        return eval;
    }
}
