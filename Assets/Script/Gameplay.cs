using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEditor;
using System.Collections;

public class Gameplay : MonoBehaviour
{
    [Header("UI Elements")]
    public TextMeshProUGUI atttempsLeft;
    public TextMeshProUGUI currentPlayer;
    public TextMeshProUGUI gameState;
    public TextMeshProUGUI gameLog;

    [Header("Input")]
    public TMP_InputField guessInputField;
    public Button submitButton;
    public Button newgameButton;

    [Header("Game Settings")]
    public int minNumber = 1;
    public int maxNumber = 100;
    public int maxAttemps = 12;

    private int targetNumber;
    private int currentAttemps;
    private bool isPlayerTurn;
    private bool gameActive;

    void InitializeUI()
    {
        submitButton.onClick.AddListener(SubmitGuess);
        newgameButton.onClick.AddListener(StartNewGame);
        guessInputField.onSubmit.AddListener(delegate { SubmitGuess(); });
    }

    void SubmitGuess()
    {
        if (!gameActive || !isPlayerTurn) return;

        string input = guessInputField.text.Trim();
        if (string.IsNullOrEmpty(input)) return;

        int guess;
        if (!int.TryParse(input, out guess))
        {
            gameState.text += "Please enter a valid number.\n";
            return;
        }
        if (guess < minNumber || guess > maxNumber)
        {
            gameState.text += $"Please enter a number between {minNumber} - {maxNumber}";
            return;
        }
        ProcessGuess(guess, true);
        guessInputField.text = "";
    }
    void ProcessGuess(int guess, bool isPlayerTurn)
    {
        currentAttemps++;
        string PlayerName = isPlayerTurn ? "Player" : "Computer";

        gameLog.text += $"{PlayerName} guessed: {guess}\n";

        if (guess == targetNumber)
        {
            //Win
            gameLog.text += $"({PlayerName} got it right)\n";
            EndGame();
        }
        else if (currentAttemps >= maxAttemps)
        {
            //Lose
            gameLog.text += $"(Game Over! The correct number was {targetNumber}\n";
            EndGame();
        }
        else
        {
           //Wrong guess - give hint
           string hint = guess < targetNumber ? "Too Low" : "Too High";
           gameLog.text += $"{hint}\n";

           //switch players
           isPlayerTurn = !isPlayerTurn;
           currentPlayer.text = isPlayerTurn ? "Player" : "Computer";
           atttempsLeft.text = $"Attempts left: {maxAttemps - currentAttemps}";

           if (!isPlayerTurn)
           {
               guessInputField.interactable = false;
               submitButton.interactable = false;
               StartCoroutine(ComputerTurn(guess < targetNumber));
           }
           else
           {
               guessInputField.interactable = true;
               submitButton.interactable = true;
               guessInputField.Select();
               guessInputField.ActivateInputField();
            }
        }
    }

    IEnumerator ComputerTurn( bool targetIsHigher)
    {
        yield return new WaitForSeconds(2f); // Wait to thinking
        if (!gameActive) yield break;
        int computerGuess = Random.Range(minNumber, maxNumber + 1);
        ProcessGuess(computerGuess, false);
    }

    void EndGame()
    {
        gameActive = false;
        guessInputField.interactable = false;
        submitButton.interactable = false;
        currentPlayer.text = "";
        gameState.text = "Game Over! Click 'New Game' to start again.";
        Canvas.ForceUpdateCanvases();
    }

    void StartNewGame()
    {
        targetNumber = Random.Range(minNumber, maxNumber + 1);
        currentAttemps = 0;
        isPlayerTurn = true;
        gameActive = true;
        
        currentPlayer.text = "Player's turn";
        atttempsLeft .text = $"Attempts left: {maxAttemps}";
        gameLog.text = "=== Game Log ===\n";
        gameState.text = "Game In Progress";

        guessInputField.interactable = true;
        submitButton.interactable = true;
        guessInputField.text = "";

        guessInputField.Select();
        guessInputField.ActivateInputField();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InitializeUI();
        StartNewGame();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
