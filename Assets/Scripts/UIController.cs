using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [SerializeField]
    Button replayBtn;
    [SerializeField]
    Button humanBtn;
    [SerializeField]
    Button aIEasyBtn;
    [SerializeField]
    Button aIMediumBtn;
    [SerializeField]
    Button aIHardBtn;
    [SerializeField]
    TextMeshProUGUI gameState;
    [SerializeField]
    GameObject startScreen;
    [SerializeField]
    GameObject endScreen;

    BoardHelper board;
    GameRules gameRules;
    public static UIController Instance { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;
    }

    void Start()
    {
        board = BoardHelper.Instance;
        gameRules = GameRules.Instance;
        replayBtn.onClick.AddListener(ReplayCallback);
        humanBtn.onClick.AddListener(() => EnemyCallback(GameRules.Enemy.Human));
        aIEasyBtn.onClick.AddListener(() => EnemyCallback(GameRules.Enemy.AIEasy));
        aIMediumBtn.onClick.AddListener(() => EnemyCallback(GameRules.Enemy.AIMedium));
        aIHardBtn.onClick.AddListener(() => EnemyCallback(GameRules.Enemy.AIHard));

        startScreen.SetActive(true);
        endScreen.SetActive(false);
    }

    void ReplayCallback()
    {
        board.ResetBoard();
        startScreen.SetActive(true);
        endScreen.SetActive(false);
    }

    void EnemyCallback(GameRules.Enemy enemy)
    {
        switch(enemy)
        {
            case GameRules.Enemy.Human:
                gameRules.CurrentEnemy = GameRules.Enemy.Human;
                break;
            case GameRules.Enemy.AIEasy:
                gameRules.CurrentEnemy = GameRules.Enemy.AIEasy;
                break;
            case GameRules.Enemy.AIMedium:
                gameRules.CurrentEnemy = GameRules.Enemy.AIMedium;
                break;
            case GameRules.Enemy.AIHard:
                gameRules.CurrentEnemy = GameRules.Enemy.AIHard;
                break;
        }

        startScreen.SetActive(false);
        gameRules.SetFirstPlayer();
    }

    public void ToggleEndScreen()
    {
        if (gameRules.CurrentPlayer == BoardHelper.Cell.PawnA)
            gameState.text = "Player A wins!";
        else
            gameState.text = "Player B wins!";

        endScreen.SetActive(true);
    }
}
